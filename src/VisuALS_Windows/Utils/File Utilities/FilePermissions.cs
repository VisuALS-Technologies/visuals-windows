using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace VisuALS_WPF_App
{
    public static class FilePermissions
    {
        private static WindowsIdentity _currentUser = WindowsIdentity.GetCurrent();
        private static WindowsPrincipal _currentPrincipal = new WindowsPrincipal(_currentUser);

        public static bool HasAccess(string path, FileSystemRights right)
        {
            if (File.Exists(path))
            {
                FileInfo fileinfo = new FileInfo(path);
                // Get the collection of authorization rules that apply to the file.
                AuthorizationRuleCollection acl = fileinfo.GetAccessControl()
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));
                return HasFileOrDirectoryAccess(right, acl);
            }
            else if (Directory.Exists(path))
            {
                DirectoryInfo dirinfo = new DirectoryInfo(path);
                // Get the collection of authorization rules that apply to the directory.
                AuthorizationRuleCollection acl = dirinfo.GetAccessControl()
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));
                return HasFileOrDirectoryAccess(right, acl);
            }
            throw new FileNotFoundException(path);
        }

        private static bool HasFileOrDirectoryAccess(FileSystemRights right,
                                              AuthorizationRuleCollection acl)
        {
            bool allow = false;
            bool inheritedAllow = false;
            bool inheritedDeny = false;

            for (int i = 0; i < acl.Count; i++)
            {
                var currentRule = (FileSystemAccessRule)acl[i];
                // If the current rule applies to the current user.
                if (_currentUser.User.Equals(currentRule.IdentityReference) ||
                    _currentPrincipal.IsInRole(
                                    (SecurityIdentifier)currentRule.IdentityReference))
                {

                    if (currentRule.AccessControlType.Equals(AccessControlType.Deny))
                    {
                        if ((currentRule.FileSystemRights & right) == right)
                        {
                            if (currentRule.IsInherited)
                            {
                                inheritedDeny = true;
                            }
                            else
                            { // Non inherited "deny" takes overall precedence.
                                return false;
                            }
                        }
                    }
                    else if (currentRule.AccessControlType
                                                      .Equals(AccessControlType.Allow))
                    {
                        if ((currentRule.FileSystemRights & right) == right)
                        {
                            if (currentRule.IsInherited)
                            {
                                inheritedAllow = true;
                            }
                            else
                            {
                                allow = true;
                            }
                        }
                    }
                }
            }

            if (allow)
            { // Non inherited "allow" takes precedence over inherited rules.
                return true;
            }
            return inheritedAllow && !inheritedDeny;
        }
    }
}
