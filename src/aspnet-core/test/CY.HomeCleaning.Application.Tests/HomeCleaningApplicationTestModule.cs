using Volo.Abp.Modularity;

namespace CY.HomeCleaning;

[DependsOn(
    typeof(HomeCleaningApplicationModule),
    typeof(HomeCleaningDomainTestModule)
)]
public class HomeCleaningApplicationTestModule : AbpModule
{

}
