﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Audit
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "Manager.Audit";
        const string LogName = "testLog";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);

            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("\nError while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void AuthenticationSuccess(string username)
        {
            if (customLog != null)
            {
                string UserAuthenticationSuccess = AuditEvents.AuthenticationSuccess;
                string message = String.Format(UserAuthenticationSuccess, username);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("\nError while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.AuthenticationSuccess));
            }
        }

        public static void AuthorizationSuccess(string username, string serviceName)
        {
            if (customLog != null)
            {
                string AuthorizationSuccess = AuditEvents.AuthorizationSuccess;
                string message = String.Format(AuthorizationSuccess, username, serviceName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.AuthorizationSuccess));
            }
        }

        public static void AuthorizationFailed(string username, string serviceName, string reason)
        {
            if (customLog != null)
            {
                string AuthorizationFailed = AuditEvents.AuthorizationFailed;
                string message = String.Format(AuthorizationFailed, username, serviceName, reason);
                customLog.WriteEntry(message);
            }
            else
            {
                string msg = "Error while trying to write event: EVENT ID " + AuditEventTypes.AuthorizationFailed.ToString() + "to event log.";
                throw new ArgumentException(msg);
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
