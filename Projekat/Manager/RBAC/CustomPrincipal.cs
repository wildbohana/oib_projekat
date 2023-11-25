﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class CustomPrincipal : IPrincipal
    {
        WindowsIdentity identity = null;

        // Geter
        public IIdentity Identity
        {
            get { return identity; }
        }

        // Konstruktor
        public CustomPrincipal(WindowsIdentity windowsIdentity)
        {
            identity = windowsIdentity;
        }

        // Provera permisija
        public bool IsInRole(string permission)
        {
            foreach (IdentityReference group in this.identity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                string groupName = Formatter.ParseName(name.ToString());
                string[] permissions;

                if (RolesConfig.GetPermissions(groupName, out permissions))
                {
                    if (!permissions.Contains(permission))
                    {
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
