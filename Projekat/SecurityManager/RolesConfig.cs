using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class RolesConfig
    {
        static string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        static string path = projectPath + "\\SecurityManager\\RolesConfigFile.resx";

        #region PERMISIJE
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

        // Dodavanje permisija
        public static void AddPermissions(string rolename, string[] permissions)
        {
            string permissionString = string.Empty;
            permissionString = (string)RolesConfigFile.ResourceManager.GetObject(rolename);

            if (permissionString != null)
            {
                var reader = new ResXResourceReader(path);
                var node = reader.GetEnumerator();
                var writer = new ResXResourceWriter(path);

                while (node.MoveNext())
                {
                    if (node.Key.ToString().Equals(rolename))
                    {
                        string value = node.Value.ToString();
                        foreach (string prms in permissions)
                        {
                            value += "," + prms;
                        }
                        writer.AddResource(node.Key.ToString(), value);
                    }
                    else
                    {
                        writer.AddResource(node.Key.ToString(), node.Value.ToString());
                    }
                }

                writer.Generate();
                writer.Close();
            }
        }

        // Uklanjanje permisija
        public static void RemovePermissions(string rolename, string[] permissions)
        {
            var reader = new ResXResourceReader(path);
            var node = reader.GetEnumerator();
            var writer = new ResXResourceWriter(path);

            while (node.MoveNext())
            {
                // Ako uloga nije ona koju menjamo, preskoči je
                if (!node.Key.ToString().Equals(rolename))
                {
                    writer.AddResource(node.Key.ToString(), node.Value.ToString());
                }
                // Ako jeste, obriši neke od permisija
                else
                {
                    List<string> currentPermisions = (node.Value.ToString().Split(',')).ToList();

                    foreach (string permForDelete in permissions)
                    {
                        for (int i = 0; i < currentPermisions.Count(); i++)
                        {
                            if (currentPermisions[i].Equals(permForDelete))
                            {
                                currentPermisions.RemoveAt(i);
                                break;
                            }
                        }
                    }

                    string value = currentPermisions[0];
                    for (int i = 1; i < currentPermisions.Count(); i++)
                    {
                        value += "," + currentPermisions[i];
                    }
                    writer.AddResource(node.Key.ToString(), value);
                }
            }

            writer.Generate();
            writer.Close();
        }
        #endregion

        #region ULOGE
        // Dodavanje uloga
        public static void AddRole(string rolename)
        {
            var reader = new ResXResourceReader(path);
            var node = reader.GetEnumerator();
            var writer = new ResXResourceWriter(path);

            while (node.MoveNext())
            {
                writer.AddResource(node.Key.ToString(), node.Value.ToString());
            }

            var newNode = new ResXDataNode(rolename, "");
            writer.AddResource(newNode);
            writer.Generate();
            writer.Close();
        }

        // Uklanjanje uloga
        public static void RemoveRole(string rolename)
        {
            var reader = new ResXResourceReader(path);
            var node = reader.GetEnumerator();
            var writer = new ResXResourceWriter(path);

            // Pročita sve, i ponovo u fajl upiše sve osim obrisane uloge ??? 
            // Ingenious
            while (node.MoveNext())
            {
                if (!node.Key.ToString().Equals(rolename))
                    writer.AddResource(node.Key.ToString(), node.Value.ToString());
            }

            writer.Generate();
            writer.Close();
        }
        #endregion
    }
}
