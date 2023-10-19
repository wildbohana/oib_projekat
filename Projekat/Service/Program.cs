using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
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

            // Ostvara se host
            ServiceHost host = new ServiceHost(typeof(CentralniServer));
            host.AddServiceEndpoint(typeof(IServer), binding, adresa);
            host.Open();

            Console.WriteLine("Korisnik koji je pokrenuo server: " + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("Servis je pokrenut. Pritisnite bilo koji taster za gasenje.");
            Console.ReadLine();
            host.Close();
        }
    }
}
