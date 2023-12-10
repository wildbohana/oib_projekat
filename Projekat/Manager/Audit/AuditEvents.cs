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

    public enum CriticalLevel
    {
        ProcessInformation = 0,
        ProcessWarning = 1,
        ProcessCritical = 2

    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

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
                return ResourceManager.GetString(AuditEventTypes.AuthenticationSuccess.ToString());
            }
        }

        public static string AuthorizationSuccess
        {
            get
            { 
                return ResourceManager.GetString(AuditEventTypes.AuthorizationSuccess.ToString());
            }
        }

        public static string AuthorizationFailed
        {
            get
            {
                return ResourceManager.GetString(AuditEventTypes.AuthorizationFailed.ToString());
            }
        }

        public static string AuditInformation
        {
            get
            {
                return ResourceManager.GetString(CriticalLevel.ProcessInformation.ToString());
            }
        }

        public static string AuditWarning
        {
            get
            {
                return ResourceManager.GetString(CriticalLevel.ProcessWarning.ToString());
            }
        }

        public static string AuditCritical
        {
            get
            {
                return ResourceManager.GetString(CriticalLevel.ProcessCritical.ToString());
            }
        }

    }
}
