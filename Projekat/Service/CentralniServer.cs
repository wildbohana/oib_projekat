using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
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
            Console.WriteLine("\n[ZAHTEV] Dobavi potrosnju");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "DobaviPotrosnju", id, ime, prezime });
            string odgovor = "\n";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            return odgovor;
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Operator")]
        public string IzmeniPotrosnju(string id, string novaPotrosnja)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Modifikuj"))
            {
                Console.WriteLine("\n[ZAHTEV] Dodaj novo brojilo");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniPotrosnju", id, novaPotrosnja });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                return odgovor;
            }
            else
            {
                return "\tNevalidne permisije za korisnika!";
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Operator")]
        public string IzmeniID(string stariID, string noviID)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Modifikuj"))
            {
                Console.WriteLine("\n[ZAHTEV] Izmeni ID brojila");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniID", stariID, noviID });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                return odgovor;
            }
            else
            {
                return "\tNevalidne permisije za korisnika!";
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public string DodajBrojilo(string id, string ime, string prezime, string potrosnja)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("DodajEntitet"))
            {
                Console.WriteLine("\n[ZAHTEV] Dodaj novo brojilo");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "DodajBrojilo", id, ime, prezime, potrosnja });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                return odgovor;
            }
            else
            {
                return "\tNevalidne permisije za korisnika!";
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public string ObrisiBrojilo(string id)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ObrisiEntitet"))
            {
                Console.WriteLine("\n[ZAHTEV] Obrisi brojilo");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBrojilo", id });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                return odgovor;
            }
            else
            {
                return "\tNevalidne permisije za korisnika!";
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "SuperAdministrator")]
        public string ObrisiBazu()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ObrisiBazu"))
            {
                Console.WriteLine("\n[ZAHTEV] Obrisi bazu podataka");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBazu" });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                return odgovor;
            }
            else
            {
                return "\tNevalidne permisije za korisnika!";
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "SuperAdministrator")]
        public string ArhivirajBazu()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ArhivirajBazu"))
            {
                Console.WriteLine("\n[ZAHTEV] Arhiviraj bazu podataka");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ArhivirajBazu" });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                return odgovor;
            }
            else
            {
                return "\tNevalidne permisije za korisnika!";
            }
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

            Console.WriteLine("Zahtev je prosledjen balanseru opterecenja.");
            List<string> rezultat = proksi.DelegirajZahtev(zahtev);

            return rezultat;
        }
        #endregion
    }
}
