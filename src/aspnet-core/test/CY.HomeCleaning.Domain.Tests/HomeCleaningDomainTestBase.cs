using Volo.Abp.Modularity;

namespace CY.HomeCleaning;

/* Inherit from this class for your domain layer tests. */
public abstract class HomeCleaningDomainTestBase<TStartupModule> : HomeCleaningTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
