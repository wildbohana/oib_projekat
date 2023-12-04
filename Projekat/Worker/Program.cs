using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.Security.Principal;
using System.ServiceModel.Security;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region PRIJAVA RADNIKA NA LB
            // Sertifikat
            string srvCertCN = "loadbalancer";
            // Osnovni port + broj aktivnih radnika = broj porta novog radnika
            int osnovniPort = 9900;

            // Server za prijavu radnika na sistem
            NetTcpBinding binding1 = new NetTcpBinding();
            binding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // Podešavanje kanala ka LB (sertifikati)
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress adresa1 = new EndpointAddress(new Uri("net.tcp://localhost:9997/PrijavaRadnika"), new X509CertificateEndpointIdentity(srvCert));
            ChannelFactory<IPrijavaRadnika> kanal = new ChannelFactory<IPrijavaRadnika>(binding1, adresa1);

            // Radnik traži ID
            IPrijavaRadnika proksi = null;
            int id = -1;

            try
            {
                string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                
                kanal.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
                kanal.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                // Podešavanje klijentskih sertifikata na kanalu
                kanal.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
                
                proksi = kanal.CreateChannel();
                id = proksi.DodeliID();

                Console.WriteLine("Radnik je prijavljen na LoadBalancer sa ID: " + id);

            }
            catch (Exception e)
            {
                Console.WriteLine("[GRESKA] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
                Console.WriteLine("Pritisni bilo koji taster za izlaz.");
                Console.ReadKey();
            }
            #endregion

            #region POKRETANJE RADNIKA
            // Kada dobije ID, radnik pokreće svoj host
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
            Console.WriteLine("Radnik je pokrenut. Pritisni bilo koji taster za gasenje.");
            Console.ReadKey();

            // Kada se ugasi, odjavljuje se sa LB
            proksi.Odjava(id);
            host.Close();
            #endregion
        }
    }
}
