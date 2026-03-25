using CY.HomeCleaning.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CY.HomeCleaning.Permissions;

public class HomeCleaningPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(HomeCleaningPermissions.GroupName);

        var backoffice = group.AddPermission(HomeCleaningPermissions.Backoffice.Default, L("Permission:Backoffice"));
        backoffice.AddChild(HomeCleaningPermissions.Backoffice.Dashboard, L("Permission:Backoffice.Dashboard"));
        backoffice.AddChild(HomeCleaningPermissions.Backoffice.OrderManagement, L("Permission:Backoffice.OrderManagement"));
        backoffice.AddChild(HomeCleaningPermissions.Backoffice.DispatchManagement, L("Permission:Backoffice.DispatchManagement"));
        backoffice.AddChild(HomeCleaningPermissions.Backoffice.CouponManagement, L("Permission:Backoffice.CouponManagement"));

        var customer = group.AddPermission(HomeCleaningPermissions.Customer.Default, L("Permission:Customer"));
        customer.AddChild(HomeCleaningPermissions.Customer.PlaceOrder, L("Permission:Customer.PlaceOrder"));
        customer.AddChild(HomeCleaningPermissions.Customer.CancelOrder, L("Permission:Customer.CancelOrder"));
        customer.AddChild(HomeCleaningPermissions.Customer.ViewOwnOrders, L("Permission:Customer.ViewOwnOrders"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HomeCleaningResource>(name);
    }
}
