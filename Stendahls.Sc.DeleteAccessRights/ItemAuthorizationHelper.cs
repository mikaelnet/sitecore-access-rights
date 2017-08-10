using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;

namespace Stendahls.Sc.DeleteAccessRights
{
    public class ItemAuthorizationHelper : Sitecore.Security.AccessControl.ItemAuthorizationHelper
    {
        public override AccessResult GetAccess(Item item, Account account, AccessRight accessRight)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(account, "account");
            Assert.ArgumentNotNull(accessRight, "accessRight");

            AccessResult itemAccess;

            // Check state access - (Copied from original implementation)
            AccessResult stateAccess = GetStateAccess(item, account, accessRight);
            if (stateAccess != null)
                return stateAccess;

            // Check workflow access - (Copied from original implementation)
            AccessResult workflowAccess = GetWorkflowAccess(item, accessRight, account);
            if (workflowAccess.Permission == AccessPermission.Deny)
                return workflowAccess;

            if (accessRight == AccessRight.ItemRemoveVersion)
            {
                // Ensure the user can read/write the current language. (Copied from original implementation)
                if (!item.Access.CanWriteLanguage())
                    return new AccessResult(AccessPermission.Deny, new AccessExplanation(account, accessRight, "You cannot remove version of this item because you do not have write access to the current language."));
                if (!item.Access.CanReadLanguage())
                    return new AccessResult(AccessPermission.Deny, new AccessExplanation(account, accessRight, "You cannot remove version of this item because you do not have read access to the current language."));

                // Check explicit item:removeVersion access instead of item:write+item:delete.
                itemAccess = GetItemAccess(item, account, accessRight, PropagationType.Entity);
                if (itemAccess.Permission != AccessPermission.NotSet)
                    return itemAccess;

                // Fallback on default behavior if the item:removeversion access right isn't configured. (Copied from original implementation)
                if (GetItemAccess(item, account, AccessRight.ItemWrite, PropagationType.Entity).Permission != AccessPermission.Allow)
                    return new AccessResult(AccessPermission.Deny, new AccessExplanation(account, accessRight, "You cannot remove version of this item because you do not have write access to the item."));
                if (GetItemAccess(item, account, AccessRight.ItemDelete, PropagationType.Entity).Permission != AccessPermission.Allow)
                    return new AccessResult(AccessPermission.Deny, new AccessExplanation(account, accessRight, "You cannot remove version of this item because you do not have delete access right to the item."));

                itemAccess = GetItemAccess(item, account, AccessRight.ItemRead, PropagationType.Entity);
                if (itemAccess.Permission != AccessPermission.Allow)
                    return new AccessResult(AccessPermission.Deny, new AccessExplanation(account, accessRight, "You cannot remove version of this item because you do not have read access to the item."));

                return itemAccess;
            }

            // Evaluate item access rights (Copied from original implementation)
            itemAccess = GetItemAccess(item, account, accessRight, PropagationType.Entity);

            if (accessRight == AccessRight.ItemDelete && itemAccess.Permission == AccessPermission.NotSet)
            {
                // Allow the original creator of an item to delete it, if delete permission isn't defined for the item
                // Test if this action is performed by the original creator of the item.
                // (The CreatedBy field is stored per language version)
                var originalItem = item.Versions.GetVersions(true).OrderBy(i => i.Statistics.Created).FirstOrDefault();
                if (string.Equals(originalItem?.Statistics.CreatedBy, account.Name))
                {
                    var text = $"{account.Name} has been granted the 'item:delete' access right for the {item.Paths.Path} item since {account.Name} owns it.";
                    return new AccessResult(AccessPermission.Allow, new AccessExplanation(item, account, accessRight, text));
                }
            }

            return itemAccess;
        }
    }
}