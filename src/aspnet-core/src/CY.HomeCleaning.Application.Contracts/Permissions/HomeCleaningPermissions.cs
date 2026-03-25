namespace CY.HomeCleaning.Permissions;

public static class HomeCleaningPermissions
{
    public const string GroupName = "HomeCleaning";

    public static class Backoffice
    {
        public const string Default = GroupName + ".Backoffice";
        public const string Dashboard = Default + ".Dashboard";
        public const string OrderManagement = Default + ".OrderManagement";
        public const string DispatchManagement = Default + ".DispatchManagement";
        public const string CouponManagement = Default + ".CouponManagement";
    }

    public static class Customer
    {
        public const string Default = GroupName + ".Customer";
        public const string PlaceOrder = Default + ".PlaceOrder";
        public const string CancelOrder = Default + ".CancelOrder";
        public const string ViewOwnOrders = Default + ".ViewOwnOrders";
    }
}
