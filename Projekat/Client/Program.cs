﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/CentralniServer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je: " + WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("Tip autentifikacije: " + WindowsIdentity.GetCurrent().AuthenticationType);
            Console.WriteLine("ID korisnika: " + WindowsIdentity.GetCurrent().User);

            using (Klijent proxy = new Klijent(binding, address))
            {
                Console.WriteLine("Klijent šalje zahtev serveru");
                proxy.PosaljiZahtev();
            }

            Console.ReadLine();
        }
    }
}
