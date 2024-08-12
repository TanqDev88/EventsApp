using Ticketera.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ticketera.Permissions;

public class TicketeraPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TicketeraPermissions.GroupName);

        var categories = context.AddGroup("Categories",L("Categories"));
        var categoryManagement = categories.AddPermission(TicketeraPermissions.Permission_Categories, L("Categories"));
        categoryManagement.AddChild(TicketeraPermissions.Permission_Categories_See, L("SeeCategories"));
        categoryManagement.AddChild(TicketeraPermissions.Permission_Categories_CreateOrUpdate, L("CreateOrUpdateCategories"));
        categoryManagement.AddChild(TicketeraPermissions.Permission_Categories_Delete, L("DeleteCategories"));

        var sectors = context.AddGroup("Sectors", L("Sectors"));
        var sectorsManagement = sectors.AddPermission(TicketeraPermissions.Permission_Sectors, L("Sectors"));
        sectorsManagement.AddChild(TicketeraPermissions.Permission_Sectors_See, L("SeeSectors"));
        sectorsManagement.AddChild(TicketeraPermissions.Permission_Sectors_CreateOrUpdate, L("CreateOrUpdateSectors"));
        sectorsManagement.AddChild(TicketeraPermissions.Permission_Sectors_Delete, L("DeleteSectors"));

        var events = context.AddGroup("Events", L("Events"));
        var eventsManagement = events.AddPermission(TicketeraPermissions.Permission_Events, L("Events"));        
        eventsManagement.AddChild(TicketeraPermissions.Permission_Events_CreateOrUpdate, L("CreateOrUpdateEvents"));
        eventsManagement.AddChild(TicketeraPermissions.Permission_Events_Delete, L("DeleteEvents"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TicketeraResource>(name);
    }
}
