using CY.HomeCleaning.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CY.HomeCleaning.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class HomeCleaningController : AbpControllerBase
{
    protected HomeCleaningController()
    {
        LocalizationResource = typeof(HomeCleaningResource);
    }
}
