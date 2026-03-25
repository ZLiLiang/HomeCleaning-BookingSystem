using Volo.Abp.Modularity;

namespace CY.HomeCleaning;

[DependsOn(
    typeof(HomeCleaningDomainModule),
    typeof(HomeCleaningTestBaseModule)
)]
public class HomeCleaningDomainTestModule : AbpModule
{

}
