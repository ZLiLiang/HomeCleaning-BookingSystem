using CY.HomeCleaning.Samples;
using Xunit;

namespace CY.HomeCleaning.EntityFrameworkCore.Domains;

[Collection(HomeCleaningTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<HomeCleaningEntityFrameworkCoreTestModule>
{

}
