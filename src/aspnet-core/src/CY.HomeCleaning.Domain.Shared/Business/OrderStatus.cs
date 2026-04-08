namespace CY.HomeCleaning.Business;

public enum OrderStatus
{
    PendingPayment = 1,
    Effective = 2,
    PendingReview = 3,
    Refunding = 4,
    Closed = 5
}