using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Audit
{
    public enum AuditEventTypes
    {
        AuthenticationSuccess = 0,
        AuthorizationSuccess = 1,
        AuthorizationFailed = 2,
        
    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        // permissionString = (string)RolesConfigFile.ResourceManager.GetObject(rolename)

        private static ResourceManager ResourceManager
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null) { }
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string AuthenticationSuccess
        {
            get
            {
                //return ResourceManager.GetString(AuditEventTypes.AuthenticationSuccess.ToString());
                //return (string)AuditEventFile.ResourceManager.GetObject(AuthenticationSuccess);
                return "ti nikada neces biti gas";
            }
        }

        public static string AuthorizationSuccess
        {
            get
            {
                //return ResourceManager.GetString(AuditEventTypes.AuthorizationSuccess.ToString());
                //return (string)AuditEventFile.ResourceManager.GetObject(AuthorizationSuccess);
                return "Vi cete mozda nekada biti gas.";
            }
        }

        public static string AuthorizationFailed
        {
            get
            {
                //return ResourceManager.GetString(AuditEventTypes.AuthorizationFailed.ToString());
                //return (string)AuditEventFile.ResourceManager.GetObject(AuthorizationFailed);
                return "E nes dalje vala";
            }
        }


    }
}
