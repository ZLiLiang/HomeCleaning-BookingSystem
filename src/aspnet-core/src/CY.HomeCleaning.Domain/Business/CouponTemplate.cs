using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CY.HomeCleaning.Business;

public class CouponTemplate : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;

    public decimal FaceValue { get; set; }

    public decimal MinimumSpend { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public Guid? ApplicableServiceItemId { get; set; }

    public int TotalCount { get; set; }

    protected CouponTemplate()
    {
    }

    public CouponTemplate(Guid id, string name, decimal faceValue, DateTime validFrom, DateTime validTo)
        : base(id)
    {
        Name = name;
        FaceValue = faceValue;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}