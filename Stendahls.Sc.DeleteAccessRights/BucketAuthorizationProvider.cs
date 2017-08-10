namespace Stendahls.Sc.DeleteAccessRights
{
    public class BucketAuthorizationProvider : Sitecore.Buckets.Security.BucketAuthorizationProvider
    {
        protected override Sitecore.Security.AccessControl.ItemAuthorizationHelper ItemHelper { get; set; } = new ItemAuthorizationHelper();
    }
}