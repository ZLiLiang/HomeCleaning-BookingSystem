using System;
using CY.HomeCleaning.Business;
using Volo.Abp.Domain.Entities.Auditing;

namespace CY.HomeCleaning.Business;

public class UserCoupon : FullAuditedAggregateRoot<Guid>
{
    public Guid CouponTemplateId { get; set; }

    public Guid UserId { get; set; }

    public UserCouponStatus Status { get; set; }

    public Guid? LockedOrderId { get; set; }

    public DateTime ClaimedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    protected UserCoupon()
    {
    }

    public UserCoupon(Guid id, Guid couponTemplateId, Guid userId, DateTime claimedAt, DateTime expireAt)
        : base(id)
    {
        CouponTemplateId = couponTemplateId;
        UserId = userId;
        ClaimedAt = claimedAt;
        ExpireAt = expireAt;
        Status = UserCouponStatus.Unused;
    }
}