using System;
using CY.HomeCleaning.Business;
using Volo.Abp.Domain.Entities.Auditing;

namespace CY.HomeCleaning.Business;

public class ServiceItem : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public BillingUnitType BillingUnitType { get; set; }

    public string? IntroductionResourceUrl { get; set; }

    public Guid? RefundRuleId { get; set; }

    protected ServiceItem()
    {
    }

    public ServiceItem(Guid id, string name, decimal basePrice, BillingUnitType billingUnitType)
        : base(id)
    {
        Name = name;
        BasePrice = basePrice;
        BillingUnitType = billingUnitType;
    }
}