using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Klijent : ChannelFactory<IServer>, IServer, IDisposable
    {
        // Kanal za konekciju
        public IServer kanal;

        // Konstruktori
        public Klijent(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            kanal = this.CreateChannel();
        }

        public Klijent(NetTcpBinding binding, string address) : base(binding, address)
        {
            kanal = this.CreateChannel();
        }

        // Mock funkcija
        public void PosaljiZahtev()
        {
            kanal.PosaljiZahtev();
        }

        // TODO dodaj Disposable()
    }
}
