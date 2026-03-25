namespace CY.HomeCleaning.WeChat;

public class WeChatMiniAppOptions
{
    public const string SectionName = "WeChat:MiniApp";

    public string AppId { get; set; } = string.Empty;

    public string AppSecret { get; set; } = string.Empty;

    public bool EnableMockMode { get; set; } = true;
}
