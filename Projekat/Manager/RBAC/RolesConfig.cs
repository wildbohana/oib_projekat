using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class RolesConfig
    {
        static string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        static string path = projectPath + "\\Manager\\RBAC\\RolesConfigFile.resx";

        // Dobavljanje permisija za ulogu iz fajla
        public static bool GetPermissions(string rolename, out string[] permissions)
        {
            permissions = new string[10];
            string permissionString = string.Empty;

            permissionString = (string)RolesConfigFile.ResourceManager.GetObject(rolename);
            if (permissionString != null)
            {
                permissions = permissionString.Split(',');
                return true;
            }

            return false;
        }
    }
}
