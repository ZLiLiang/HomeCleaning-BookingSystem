using Xunit;

namespace CY.HomeCleaning.EntityFrameworkCore;

[CollectionDefinition(HomeCleaningTestConsts.CollectionDefinitionName)]
public class HomeCleaningEntityFrameworkCoreCollection : ICollectionFixture<HomeCleaningEntityFrameworkCoreFixture>
{

}
