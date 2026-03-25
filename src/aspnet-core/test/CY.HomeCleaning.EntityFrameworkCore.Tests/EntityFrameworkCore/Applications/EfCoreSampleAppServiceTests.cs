using CY.HomeCleaning.Samples;
using Xunit;

namespace CY.HomeCleaning.EntityFrameworkCore.Applications;

[Collection(HomeCleaningTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<HomeCleaningEntityFrameworkCoreTestModule>
{

}
