using Microsoft.Extensions.Localization;
using CY.HomeCleaning.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CY.HomeCleaning;

[Dependency(ReplaceServices = true)]
public class HomeCleaningBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<HomeCleaningResource> _localizer;

    public HomeCleaningBrandingProvider(IStringLocalizer<HomeCleaningResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
