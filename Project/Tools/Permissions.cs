using System.Collections.Generic;

namespace Project.Tools
{
    public class UserPermissions
    {
        public List<TabPermission> Tabs { get; set; }
        public List<DirectoryPermission> Directoryes { get; set; }
    }

    public class TabPermission
    {
        public string Name { get; set; }
        public string RusName { get; set; }
        public Permission Permissions { get; set; }
    }

    public class DirectoryPermission
    {
        public string Name { get; set; }
        public string RusName { get; set; }
        public Permission Permissions { get; set; }
    }

    public class Permission
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
    }
}