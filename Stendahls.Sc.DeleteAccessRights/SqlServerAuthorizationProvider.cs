namespace Stendahls.Sc.DeleteAccessRights
{
    public class SqlServerAuthorizationProvider : Sitecore.Security.AccessControl.SqlServerAuthorizationProvider
    {
        protected override Sitecore.Security.AccessControl.ItemAuthorizationHelper ItemHelper { get; set; } = new ItemAuthorizationHelper();
    }
}