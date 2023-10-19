using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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

            ChannelFactory<ILoadBalanser> kanal = new ChannelFactory<ILoadBalanser>(binding, new EndpointAddress(adresa));
            ILoadBalanser proksi = kanal.CreateChannel();

            proksi.DelegirajZahtev();
            Console.WriteLine("Zahtev je prosleđen balanseru opterećenja.");
        }
    }
}
