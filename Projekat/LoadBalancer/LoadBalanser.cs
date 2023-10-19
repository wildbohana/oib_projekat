using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalanser : ILoadBalanser
    {
        // Prosleđivanje zahteva ka nekoj od Worker komponenti
        // Za sad je samo jedan Worker, kasnije se proširuje za n komada
        // TODO menjaj kasnije
        public void DelegirajZahtev()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string adresa = "net.tcp://localhost:9997/Radnik";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            ChannelFactory<IRadnik> kanal = new ChannelFactory<IRadnik>(binding, new EndpointAddress(adresa));
            IRadnik proksi = kanal.CreateChannel();

            proksi.ObradaZahteva();

            Console.WriteLine("Zahtev je prosleđen radniku.");
        }

        // TODO kasnije
        public void WorkerOdjava()
        {
            throw new NotImplementedException();
        }

        // TODO kasnije
        public void WorkerPrijava()
        {
            throw new NotImplementedException();
        }
    }
}
