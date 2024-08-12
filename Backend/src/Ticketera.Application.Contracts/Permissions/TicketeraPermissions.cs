using System.Text.RegularExpressions;

namespace Ticketera.Permissions;

public static class TicketeraPermissions
{
    public const string GroupName = "Ticketera";

    public const string Permission_Categories = "Pages.Categories";
    public const string Permission_Categories_See = "Pages.Categories.See";
    public const string Permission_Categories_CreateOrUpdate = "Pages.Categories.CreateOrUpdate";
    public const string Permission_Categories_Delete = "Pages.Categories.Delete";

    public const string Permission_Sectors = "Pages.Sectors";
    public const string Permission_Sectors_See = "Pages.Sectors.See";
    public const string Permission_Sectors_CreateOrUpdate = "Pages.Sectors.CreateOrUpdate";
    public const string Permission_Sectors_Delete = "Pages.Sectors.Delete";

    public const string Permission_Events = "Pages.Events";    
    public const string Permission_Events_CreateOrUpdate = "Pages.Events.CreateOrUpdate";
    public const string Permission_Events_Delete = "Pages.Events.Delete";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
