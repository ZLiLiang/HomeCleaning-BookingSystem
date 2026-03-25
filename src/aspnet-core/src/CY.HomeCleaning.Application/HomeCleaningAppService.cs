using System;
using System.Collections.Generic;
using System.Text;
using CY.HomeCleaning.Localization;
using Volo.Abp.Application.Services;

namespace CY.HomeCleaning;

/* Inherit your application services from this class.
 */
public abstract class HomeCleaningAppService : ApplicationService
{
    protected HomeCleaningAppService()
    {
        LocalizationResource = typeof(HomeCleaningResource);
    }
}
