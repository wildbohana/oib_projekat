using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Osnovni port + broj aktivnih radnika = broj porta novog radnika
            int osnovniPort = 9900;

            // Server za prijavu radnika na sistem
            NetTcpBinding binding1 = new NetTcpBinding();
            string adresa1 = "net.tcp://localhost:9997/PrijavaRadnika";

            // TODO - izmeni u sertifikate
            binding1.Security.Mode = SecurityMode.Transport;
            binding1.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            //EndpointAddress epadresa = new EndpointAddress(new Uri(adresa1));
            //ChannelFactory<IPrijavaRadnika> kanal = new ChannelFactory<IPrijavaRadnika>(binding1, epadresa);
            ChannelFactory<IPrijavaRadnika> kanal = new ChannelFactory<IPrijavaRadnika>(binding1, adresa1);

            // Radnik traži ID
            IPrijavaRadnika proksi = null;
            int id = -1;

            try
            {
                proksi = kanal.CreateChannel();
                id = proksi.DodeliID();
            }
            catch (Exception e)
            {
                Console.WriteLine("[GREŠKA] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
                Console.WriteLine("Pritisni bilo koji taster za izlaz.");
                Console.ReadKey();
            }

            // Kada dobije ID, radnik otvara svoj host
            NetTcpBinding binding2 = new NetTcpBinding();
            int noviPort = osnovniPort + id;
            string adresa2 = "net.tcp://localhost:" + noviPort + "/Radnik";

            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            ServiceHost host = new ServiceHost(typeof(Radnik));
            host.AddServiceEndpoint(typeof(IRadnik), binding2, adresa2);
            host.Open();

            // Nakon što otvori host, prijavljuje se na LB
            proksi.Prijava(id);
            Console.WriteLine("Radnik je pokrenut. ID radnika je: " + id + ". Pritisni bilo koji taster za gašenje.");
            Console.ReadKey();

            // Kada se ugasi, odjavljuje se sa LB
            proksi.Odjava(id);
            host.Close();
        }
    }
}
