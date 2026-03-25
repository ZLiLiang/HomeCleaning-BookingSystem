using CY.HomeCleaning.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace CY.HomeCleaning.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(HomeCleaningEntityFrameworkCoreModule),
    typeof(HomeCleaningApplicationContractsModule)
    )]
public class HomeCleaningDbMigratorModule : AbpModule
{
}
