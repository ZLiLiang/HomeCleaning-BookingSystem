using System;
using CY.HomeCleaning.Business;
using Volo.Abp.Domain.Entities.Auditing;

namespace CY.HomeCleaning.Business;

public class Order : FullAuditedAggregateRoot<Guid>
{
    public string OrderNo { get; set; } = string.Empty;

    public Guid CustomerUserId { get; set; }

    public Guid ServiceItemId { get; set; }

    public DateTime ServiceDate { get; set; }

    public string TimeSlot { get; set; } = string.Empty;

    public decimal OriginalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public OrderStatus Status { get; set; }

    public string SnapshotData { get; set; } = string.Empty;

    protected Order()
    {
    }

    public Order(Guid id, string orderNo, Guid customerUserId, Guid serviceItemId)
        : base(id)
    {
        OrderNo = orderNo;
        CustomerUserId = customerUserId;
        ServiceItemId = serviceItemId;
        Status = OrderStatus.PendingPayment;
    }
}