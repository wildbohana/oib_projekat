using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class CentralniServer : IServer
    {
        // Prijem zahteva na server, prosleđivanje na LB
        public void PosaljiZahtev()
        {
            Console.WriteLine("Centralni server je primio zahtev");
            ProslediZahtevLB();
        }

        // Spajanje na LB i prosleđivanje zahteva
        static void ProslediZahtevLB()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string adresa = "net.tcp://localhost:9998/LoadBalanser";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            // Ovo ovako može samo ako smo sigurni da je WinAuth korišćena
            // U suptotnom as castovanje vraća null i onda imamo problem
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta koji je pozvao metodu : " + windowsIdentity.Name);
            Console.WriteLine("Jedinstveni identifikator : " + windowsIdentity.User);
            Console.WriteLine("Grupe korisnika:");
            foreach (IdentityReference group in windowsIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                string name = (sid.Translate(typeof(NTAccount))).ToString();
                Console.WriteLine(name);
            }

            ChannelFactory<ILoadBalanser> kanal = new ChannelFactory<ILoadBalanser>(binding, new EndpointAddress(adresa));
            ILoadBalanser proksi = kanal.CreateChannel();

            proksi.DelegirajZahtev();
            Console.WriteLine("Zahtev je prosleđen balanseru opterećenja.");
        }
    }
}
