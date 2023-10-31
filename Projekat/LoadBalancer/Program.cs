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
            #region SERVER ZA LOADBALANSER
            NetTcpBinding binding1 = new NetTcpBinding();
            string adresa1 = "net.tcp://localhost:9998/LoadBalanser";

            binding1.Security.Mode = SecurityMode.Transport;
            binding1.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            // Pokretanje hosta za LB
            ServiceHost host1 = new ServiceHost(typeof(LoadBalanser));
            host1.AddServiceEndpoint(typeof(ILoadBalanser), binding1, adresa1);
            host1.Open();
            #endregion

            #region SERVER ZA PRIJAVU RADNIKA
            NetTcpBinding binding2 = new NetTcpBinding();
            string adresa2 = "net.tcp://localhost:9997/PrijavaRadnika";

            // TODO - promeni na sertifikate
            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            ServiceHost host2 = new ServiceHost(typeof(PrijavaRadnika));
            host2.AddServiceEndpoint(typeof(IPrijavaRadnika), binding2, adresa2);

            // Pokretanje hosta za Radnike
            try
            {
                host2.Open();
                Console.WriteLine("LoadBalancer je pokrenut. Pritisni bilo koji taster za gašenje.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("[GREŠKA] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
                Console.WriteLine("Pritisni bilo koji taster za izlaz.");
                Console.ReadKey();
            }
            finally
            {
                host2.Close();
                host1.Close();
            }
            #endregion
        }
    }
}
