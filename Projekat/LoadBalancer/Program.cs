using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string adresa = "net.tcp://localhost:9998/LoadBalanser";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            // Pokretanje hosta za LB
            ServiceHost host = new ServiceHost(typeof(LoadBalanser));
            host.AddServiceEndpoint(typeof(ILoadBalanser), binding, adresa);
            host.Open();

            // Za gašenje
            Console.WriteLine("LoadBalancer je pokrenut. Pritisni bilo koji taster za gašenje.");
            Console.ReadKey();
            host.Close();
        }
    }
}
