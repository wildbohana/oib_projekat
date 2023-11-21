﻿using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string adresa = "net.tcp://localhost:9999/CentralniServer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            // Podešavanje hosta
            ServiceHost host = new ServiceHost(typeof(CentralniServer));
            host.AddServiceEndpoint(typeof(IServer), binding, adresa);

            // Dodavanje custom sigurnosne polise
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>
            {
                new CustomAuthorizationPolicy()
            };
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            // Otvara se host
            host.Open();

            Console.WriteLine("Korisnik koji je pokrenuo server: " + WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("Tip autentifikacije: " + WindowsIdentity.GetCurrent().AuthenticationType);
            Console.WriteLine("ID korisnika: " + WindowsIdentity.GetCurrent().User);

            Console.WriteLine("\nServis je pokrenut. Pritisnite bilo koji taster za gasenje.");
            Console.ReadLine();
            host.Close();
        }
    }
}
