using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CY.HomeCleaning.Authorization;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace CY.HomeCleaning.Data;

public class HomeCleaningRolePermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private const string RolePermissionProviderName = "R";

    private static readonly IReadOnlyList<string> AdminPermissions = new List<string>
    {
        "HomeCleaning.Backoffice",
        "HomeCleaning.Backoffice.Dashboard",
        "HomeCleaning.Backoffice.OrderManagement",
        "HomeCleaning.Backoffice.DispatchManagement",
        "HomeCleaning.Backoffice.CouponManagement",
        "HomeCleaning.Customer",
        "HomeCleaning.Customer.PlaceOrder",
        "HomeCleaning.Customer.CancelOrder",
        "HomeCleaning.Customer.ViewOwnOrders"
    };

    private static readonly IReadOnlyList<string> OperatorPermissions = new List<string>
    {
        "HomeCleaning.Backoffice",
        "HomeCleaning.Backoffice.Dashboard",
        "HomeCleaning.Backoffice.OrderManagement",
        "HomeCleaning.Backoffice.DispatchManagement"
    };

    private static readonly IReadOnlyList<string> CustomerPermissions = new List<string>
    {
        "HomeCleaning.Customer",
        "HomeCleaning.Customer.PlaceOrder",
        "HomeCleaning.Customer.CancelOrder",
        "HomeCleaning.Customer.ViewOwnOrders"
    };

    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public HomeCleaningRolePermissionDataSeedContributor(
        IIdentityRoleRepository roleRepository,
        IdentityRoleManager roleManager,
        IPermissionDataSeeder permissionDataSeeder,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _roleRepository = roleRepository;
        _roleManager = roleManager;
        _permissionDataSeeder = permissionDataSeeder;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await EnsureRoleAsync(HomeCleaningRoles.Admin);
        await EnsureRoleAsync(HomeCleaningRoles.Operator);
        await EnsureRoleAsync(HomeCleaningRoles.Customer);

        await _permissionDataSeeder.SeedAsync(
            RolePermissionProviderName,
            HomeCleaningRoles.Admin,
            AdminPermissions,
            context.TenantId
        );

        await _permissionDataSeeder.SeedAsync(
            RolePermissionProviderName,
            HomeCleaningRoles.Operator,
            OperatorPermissions,
            context.TenantId
        );

        await _permissionDataSeeder.SeedAsync(
            RolePermissionProviderName,
            HomeCleaningRoles.Customer,
            CustomerPermissions,
            context.TenantId
        );
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        var role = await _roleRepository.FindByNormalizedNameAsync(roleName.ToUpperInvariant());
        if (role != null)
        {
            return;
        }

        role = new IdentityRole(
            _guidGenerator.Create(),
            roleName,
            _currentTenant.Id
        );

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new AbpException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
