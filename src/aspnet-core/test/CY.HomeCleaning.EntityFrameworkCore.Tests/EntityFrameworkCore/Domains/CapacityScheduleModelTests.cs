using System;
using System.Threading.Tasks;
using CY.HomeCleaning.Business;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace CY.HomeCleaning.EntityFrameworkCore.Domains;

[Collection(HomeCleaningTestConsts.CollectionDefinitionName)]
public class CapacityScheduleModelTests : HomeCleaningEntityFrameworkCoreTestBase
{
    private readonly IRepository<CapacitySchedule, Guid> _capacityScheduleRepository;

    public CapacityScheduleModelTests()
    {
        _capacityScheduleRepository = GetRequiredService<IRepository<CapacitySchedule, Guid>>();
    }

    [Fact]
    public async Task Should_Configure_RowVersion_As_Concurrency_Token()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var dbContext = await _capacityScheduleRepository.GetDbContextAsync();
            var entityType = dbContext.Model.FindEntityType(typeof(CapacitySchedule));

            entityType.ShouldNotBeNull();

            var rowVersionProperty = entityType!.FindProperty(nameof(CapacitySchedule.RowVersion));
            rowVersionProperty.ShouldNotBeNull();
            rowVersionProperty!.IsConcurrencyToken.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Should_Configure_Unique_Index_For_Date_And_TimeSlot()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var dbContext = await _capacityScheduleRepository.GetDbContextAsync();
            var entityType = dbContext.Model.FindEntityType(typeof(CapacitySchedule));

            entityType.ShouldNotBeNull();
            var index = entityType!.FindIndex(new[]
            {
                entityType.FindProperty(nameof(CapacitySchedule.ServiceDate))!,
                entityType.FindProperty(nameof(CapacitySchedule.TimeSlot))!
            });

            index.ShouldNotBeNull();
            index!.IsUnique.ShouldBeTrue();
        });
   }
}