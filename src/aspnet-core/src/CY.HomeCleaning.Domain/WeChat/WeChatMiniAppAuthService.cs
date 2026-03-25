using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace CY.HomeCleaning.WeChat;

public class WeChatMiniAppAuthService : IWeChatMiniAppAuthService, ITransientDependency
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<WeChatMiniAppOptions> _options;

    public WeChatMiniAppAuthService(
        IHttpClientFactory httpClientFactory,
        IOptions<WeChatMiniAppOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task<WeChatMiniAppLoginResult> LoginByCodeAsync(string code)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new BusinessException("HomeCleaning:InvalidWeChatCode");
        }

        var options = _options.Value;
        if (options.EnableMockMode)
        {
            return new WeChatMiniAppLoginResult(
                OpenId: $"mock_openid_{code}",
                UnionId: $"mock_unionid_{code}",
                SessionKey: "mock_session_key"
            );
        }

        if (options.AppId.IsNullOrWhiteSpace() || options.AppSecret.IsNullOrWhiteSpace())
        {
            throw new BusinessException("HomeCleaning:WeChatMiniAppNotConfigured");
        }

        var client = _httpClientFactory.CreateClient();
        var requestUrl =
            $"https://api.weixin.qq.com/sns/jscode2session?appid={Uri.EscapeDataString(options.AppId)}&secret={Uri.EscapeDataString(options.AppSecret)}&js_code={Uri.EscapeDataString(code)}&grant_type=authorization_code";

        using var response = await client.GetAsync(requestUrl);
        var payload = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new BusinessException("HomeCleaning:WeChatApiError").WithData("payload", payload);
        }

        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        if (root.TryGetProperty("errcode", out var errCodeElement) && errCodeElement.GetInt32() != 0)
        {
            var errCode = errCodeElement.GetInt32();
            var errMsg = root.TryGetProperty("errmsg", out var errMsgElement)
                ? errMsgElement.GetString()
                : "unknown";
            throw new BusinessException("HomeCleaning:WeChatCodeExchangeFailed")
                .WithData("errcode", errCode)
                .WithData("errmsg", errMsg ?? string.Empty);
        }

        var openId = root.GetProperty("openid").GetString();
        if (openId.IsNullOrWhiteSpace())
        {
            throw new BusinessException("HomeCleaning:WeChatOpenIdMissing");
        }

        var unionId = root.TryGetProperty("unionid", out var unionIdElement)
            ? unionIdElement.GetString()
            : null;
        var sessionKey = root.TryGetProperty("session_key", out var sessionKeyElement)
            ? sessionKeyElement.GetString()
            : null;

        return new WeChatMiniAppLoginResult(openId!, unionId, sessionKey);
    }
}
