using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CY.HomeCleaning.Business;

public class CapacitySchedule : FullAuditedAggregateRoot<Guid>
{
    public DateTime ServiceDate { get; set; }

    public string TimeSlot { get; set; } = string.Empty;

    public int MaxCapacity { get; set; }

    public int UsedCapacity { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    protected CapacitySchedule()
    {
    }

    public CapacitySchedule(Guid id, DateTime serviceDate, string timeSlot, int maxCapacity)
        : base(id)
    {
        ServiceDate = serviceDate;
        TimeSlot = timeSlot;
        MaxCapacity = maxCapacity;
        UsedCapacity = 0;
    }
}