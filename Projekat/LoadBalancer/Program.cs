using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;

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
            // Sertifikat za server
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            // Podešavanje binding-a
            NetTcpBinding binding2 = new NetTcpBinding();
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string adresa2 = "net.tcp://localhost:9997/PrijavaRadnika";

            ServiceHost host2 = new ServiceHost(typeof(PrijavaRadnika));
            host2.AddServiceEndpoint(typeof(IPrijavaRadnika), binding2, adresa2);

            // Podešavanje kredencijala (klijentskih i serverskih)
            host2.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host2.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host2.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            // Pokretanje hosta za Radnike
            try
            {
                host2.Open();
                Console.WriteLine("LoadBalancer je pokrenut. Pritisni bilo koji taster za gasenje.\n");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("[GRESKA] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
                Console.WriteLine("Pritisni bilo koji taster za izlaz.");
                Console.ReadKey();
            }
            finally
            {
                host2.Close();
                host1.Close();
            }

            host2.Close();
            host1.Close();
            #endregion
        }
    }
}
