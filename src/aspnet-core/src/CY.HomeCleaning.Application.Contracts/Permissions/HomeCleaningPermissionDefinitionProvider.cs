using CY.HomeCleaning.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CY.HomeCleaning.Permissions;

public class HomeCleaningPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(HomeCleaningPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(HomeCleaningPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HomeCleaningResource>(name);
    }
}
