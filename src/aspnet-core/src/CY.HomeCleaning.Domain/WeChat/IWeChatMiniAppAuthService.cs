using System.Threading.Tasks;

namespace CY.HomeCleaning.WeChat;

public interface IWeChatMiniAppAuthService
{
    Task<WeChatMiniAppLoginResult> LoginByCodeAsync(string code);
}
