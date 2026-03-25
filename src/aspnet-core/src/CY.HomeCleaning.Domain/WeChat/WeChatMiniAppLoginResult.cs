namespace CY.HomeCleaning.WeChat;

public record WeChatMiniAppLoginResult(
    string OpenId,
    string? UnionId,
    string? SessionKey
);
