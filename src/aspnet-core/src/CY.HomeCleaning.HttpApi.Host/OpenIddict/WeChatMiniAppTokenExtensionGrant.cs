using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CY.HomeCleaning.Authorization;
using CY.HomeCleaning.WeChat;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

namespace CY.HomeCleaning.OpenIddict;

public class WeChatMiniAppTokenExtensionGrant : ITokenExtensionGrant, ITransientDependency
{
    public string Name => HomeCleaningGrantTypes.WeChatMiniApp;

    public async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        var requestServices = context.HttpContext.RequestServices;
        var weChatMiniAppAuthService = requestServices.GetRequiredService<IWeChatMiniAppAuthService>();
        var identityUserRepository = requestServices.GetRequiredService<IIdentityUserRepository>();
        var identityUserManager = requestServices.GetRequiredService<IdentityUserManager>();
        var guidGenerator = requestServices.GetRequiredService<IGuidGenerator>();

        var code = context.Request.GetParameter("code")?.ToString();
        if (code.IsNullOrWhiteSpace())
        {
            throw new BusinessException("HomeCleaning:WeChatCodeRequired");
        }

        var loginResult = await weChatMiniAppAuthService.LoginByCodeAsync(code!);
        var user = await FindOrCreateUserAsync(loginResult.OpenId, identityUserRepository, identityUserManager, guidGenerator);
        var roles = await identityUserManager.GetRolesAsync(user);
        var scopeArray = context.Request.GetScopes().ToImmutableArray();

        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role
        );

        identity.SetClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
        identity.SetClaim(OpenIddictConstants.Claims.Username, user.UserName ?? string.Empty);
        identity.SetClaim(OpenIddictConstants.Claims.Name, user.Name ?? user.UserName ?? string.Empty);
        identity.SetClaims(OpenIddictConstants.Claims.Role, roles.ToImmutableArray());

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(scopeArray);
        principal.SetResources(await GetResourcesAsync(context, scopeArray));

        return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
    }

    private async Task<IdentityUser> FindOrCreateUserAsync(
        string openId,
        IIdentityUserRepository identityUserRepository,
        IdentityUserManager identityUserManager,
        IGuidGenerator guidGenerator)
    {
        var username = BuildUsername(openId);
        var normalizedUserName = identityUserManager.NormalizeName(username);

        var user = await identityUserRepository.FindByNormalizedUserNameAsync(normalizedUserName);
        if (user != null)
        {
            return user;
        }

        user = new IdentityUser(guidGenerator.Create(), username, $"{username}@wechat.local");
        user.SetIsActive(true);
        user.Name = "WeChat";
        user.Surname = openId;

        var createResult = await identityUserManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            throw new AbpException(string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }

        var roleResult = await identityUserManager.AddToRoleAsync(user, HomeCleaningRoles.Customer);
        if (!roleResult.Succeeded)
        {
            throw new AbpException(string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }

        return user;
    }

    [NotNull]
    private static string BuildUsername(string openId)
    {
        var sanitized = new string(openId.Where(char.IsLetterOrDigit).ToArray());
        if (sanitized.Length > 20)
        {
            sanitized = sanitized[..20];
        }

        return $"wx_{sanitized}";
    }

    private static async Task<List<string>> GetResourcesAsync(ExtensionGrantContext context, ImmutableArray<string> scopes)
    {
        var scopeManager = context.HttpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>();
        var resources = new List<string>();

        await foreach (var resource in scopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }

        return resources;
    }
}
