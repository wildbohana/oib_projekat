using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service
{
    public class CentralniServer : IServer
    {
        #region ISERVER
        public string DobaviPotrosnju(string id, string ime, string prezime)
        {
            Console.WriteLine("[ZAHTEV] Dobavi potrosnju");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "DobaviPotrosnju", id, ime, prezime });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        public string IzmeniPotrosnju(string id, string novaPotrosnja)
        {
            Console.WriteLine("[ZAHTEV] Dodaj novo brojilo");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniPotrosnju", id, novaPotrosnja });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        public string IzmeniID(string stariID, string noviID)
        {
            Console.WriteLine("[ZAHTEV] Izmeni ID brojila");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniID", stariID, noviID });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        public string DodajBrojilo(string id, string ime, string prezime, string potrosnja)
        {
            Console.WriteLine("[ZAHTEV] Dodaj novo brojilo");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "DodajBrojilo", id, ime, prezime, potrosnja });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        public string ObrisiBrojilo(string id)
        {
            Console.WriteLine("[ZAHTEV] Obrisi brojilo");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBrojilo", id });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        public string ObrisiBazu()
        {
            Console.WriteLine("[ZAHTEV] Obrisi bazu podataka");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBazu" });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        public string ArhivirajBazu()
        {
            Console.WriteLine("[ZAHTEV] Arhiviraj bazu podataka");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "ArhivirajBazu" });
            string odgovor = "";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }
        #endregion

        #region PROSLEĐIVANJE
        // Pomoćna funkcija - pozivaju je sve ostale metode iz interfejsa
        // Spajanje na LB i prosleđivanje zahteva
        private static List<string> ProslediZahtevLB(List<string> zahtev)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string adresa = "net.tcp://localhost:9998/LoadBalanser";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            ChannelFactory<ILoadBalanser> kanal = new ChannelFactory<ILoadBalanser>(binding, new EndpointAddress(adresa));
            ILoadBalanser proksi = kanal.CreateChannel();

            Console.WriteLine("Zahtev je prosleđen balanseru opterećenja.");
            List<string> rezultat = proksi.DelegirajZahtev(zahtev);

            return rezultat;
        }
        #endregion
    }
}
