using Volo.Abp.Settings;

namespace CY.HomeCleaning.Settings;

public class HomeCleaningSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(HomeCleaningSettings.MySetting1));
    }
}
