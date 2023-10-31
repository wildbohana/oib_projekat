using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class PrijavaRadnika : IPrijavaRadnika
    {
        // Aktivni radnici se čuvaju u rečniku
        public static ConcurrentDictionary<int, IRadnik> Radnici = new ConcurrentDictionary<int, IRadnik>();
        private int port = 9900;

        #region PRIJAVA
        // Prijava radnika
        public void Prijava(int id)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            int portRadnika = this.port + id;

            ChannelFactory<IRadnik> kanal = new ChannelFactory<IRadnik>(binding, new EndpointAddress("net.tcp://localhost:" + portRadnika + "/Radnik"));
            IRadnik proksi = kanal.CreateChannel();
            Radnici.TryAdd(id, proksi);
        }
        #endregion

        #region ODJAVA
        // Odjava radnika
        public void Odjava(int id)
        {
            Radnici.TryRemove(id, out _);
        }
        #endregion

        #region IDs
        // Pomoćna metoda za generisanje ID radnika
        private int GenerisiID()
        {
            for (int i = 0; i < Radnici.Count; i++)
            {
                if (!Radnici.ContainsKey(i))
                {
                    return i;
                }
            }
            return Radnici.Count();
        }

        public int DodeliID()
        {
            return GenerisiID();
        }
        #endregion
    }
}
