using Volo.Abp.Modularity;

namespace CY.HomeCleaning;

public abstract class HomeCleaningApplicationTestBase<TStartupModule> : HomeCleaningTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
