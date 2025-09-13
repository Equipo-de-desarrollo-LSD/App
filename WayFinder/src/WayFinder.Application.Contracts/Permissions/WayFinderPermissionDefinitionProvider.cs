using WayFinder.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace WayFinder.Permissions;

public class WayFinderPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WayFinderPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(WayFinderPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WayFinderResource>(name);
    }
}
