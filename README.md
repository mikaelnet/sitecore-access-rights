# Sitecore Delete Access Rights
This package enables the "item:removeVersion" access right, 
allowing authors to remove individual item versions without 
allowing authors to delete the entire item. It also allows 
the initial creator of an item to delete his/her own item, 
unless an explicit deny delete access right is applied to 
the item.

# Build and install
Update the nuget package sitecore references if the version
doesn't match your Sitecore version. Compile the solution 
and copy the output dll and the config file to target
instance.

The config file updates the two default authorization providers, 
SqlServerAuthorizationProvider and BucketAuthorizationProvider.
If you've made customization to these providers, this module
needs to be updated accordingly.
