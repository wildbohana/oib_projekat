using Common;
using Manager;
using Manager.Audit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service
{
    public class CentralniServer : IServer
    {
        #region SKEY
        private string NapraviSKey(string path, string keyFile)
        {
            string sKey = SecretKey.GetKey(path, keyFile);
            return sKey;
        }

        // Za server (bez enkripcije)
        private string UcitajSKey(string lhkorisnika, string kime)
        {
            string path = "..\\..\\SecretKeys\\";
            string keyFile = lhkorisnika + "_" + kime + ".txt";

            string skey = NapraviSKey(path, keyFile);
            return skey;
        }

        // Za klijenta (sa enkripcijom preko javnog ključa)
        public string DobaviSKey(string lhkorisnika, string kime)
        {
            string path = "..\\..\\SecretKeys\\";
            string keyFile = lhkorisnika + "_" + kime + ".txt";
            string skey = NapraviSKey(path, keyFile);

            string clientName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);
            string publicKey = certificate.GetRSAPublicKey().ToXmlString(false);

            string enkriptovanSKey = Manager.RSA.EncryptSKey(skey, publicKey);
            return enkriptovanSKey;
        }
        #endregion

        #region ISERVER
        public string DobaviPotrosnju(string id, string ime, string prezime)
        {
            Console.WriteLine("\n[ZAHTEV] Dobavi potrosnju");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

            string fullname = windowsIdentity.Name.ToString();
            string lhkorisnika = fullname.Split('\\')[0];
            string kime = fullname.Split('\\')[1];
            string sKey = UcitajSKey(lhkorisnika, kime);

            string idDec = null;
            string imeDec = null;
            string prezimeDec = null;

            try
            {
                AES.DecryptString(id, out idDec, sKey);
                Console.WriteLine("Parametar ID je uspesno dekriptovan.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
            }

            try
            {
                AES.DecryptString(ime, out imeDec, sKey);
                Console.WriteLine("Parametar IME je uspesno dekriptovan.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
            }

            try
            {
                AES.DecryptString(prezime, out prezimeDec, sKey);
                Console.WriteLine("Parametar PREZIME je uspesno dekriptovan.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
            }

            // Prosleđivanje zahteva LB-u
            List<string> rezultat = ProslediZahtevLB(new List<string> { "DobaviPotrosnju", idDec, imeDec, prezimeDec });
            string odgovor = "\n";

            foreach (string str in rezultat)
            {
                odgovor = odgovor + '\t' + str;
            }

            Console.WriteLine("Dobavljanje potrosnje izvrseno!");
            return odgovor;
        }

        // Operator
        public string IzmeniPotrosnju(string id, string novaPotrosnja)
        {
            //audit
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

                string fullname = windowsIdentity.Name.ToString();
                string lhkorisnika = fullname.Split('\\')[0];
                string kime = fullname.Split('\\')[1];
                string sKey = UcitajSKey(lhkorisnika, kime);

                string idDec = null;
                string novaPotrosnjaDec = null;

                try
                {
                    AES.DecryptString(id, out idDec, sKey);
                    Console.WriteLine("Parametar ID je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                try
                {
                    AES.DecryptString(novaPotrosnja, out novaPotrosnjaDec, sKey);
                    Console.WriteLine("Parametar NOVAPOTROSNJA je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }

                try
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniPotrosnju", idDec, novaPotrosnjaDec });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                Console.WriteLine("Izmena potrosnje izvrsena!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "IzmeniPotrosnju metoda zahteva Modifikuj permisiju.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new FaultException("Korisnik " + userName + " je pokusao da pozove IzmeniPotrosnju. Za nju je potrebna Modifikuj permisija.");
            }
        }

        // Operator
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

                string fullname = windowsIdentity.Name.ToString();
                string lhkorisnika = fullname.Split('\\')[0];
                string kime = fullname.Split('\\')[1];
                string sKey = UcitajSKey(lhkorisnika, kime);

                string stariIDDec = string.Empty;
                string noviIDDec = string.Empty;

                try
                {
                    AES.DecryptString(stariID, out stariIDDec, sKey);
                    Console.WriteLine("Parametar STARIID je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }

                try
                {
                    AES.DecryptString(noviID, out noviIDDec, sKey);
                    Console.WriteLine("Parametar NOVIID je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                
                try
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniID", stariIDDec, noviIDDec });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                Console.WriteLine("Izmena ID izvrsena!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "IzmeniID metoda zahteva Modifikuj permisiju.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("Korisnik " + userName + " je pokusao da pozove IzmeniID metodu. Za nju je neophodna Modifikuj permisija.");
            }
        }

        // Administrator
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

                string fullname = windowsIdentity.Name.ToString();
                string lhkorisnika = fullname.Split('\\')[0];
                string kime = fullname.Split('\\')[1];
                string sKey = UcitajSKey(lhkorisnika, kime);

                string idDec = string.Empty;
                string imeDec = string.Empty;
                string prezimeDec = string.Empty;
                string potrosnjaDec = string.Empty;

                try
                {
                    AES.DecryptString(id, out idDec, sKey);
                    Console.WriteLine("Parametar ID je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                try
                {
                    AES.DecryptString(ime, out imeDec, sKey);
                    Console.WriteLine("Parametar IME je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                try
                {
                    AES.DecryptString(prezime, out prezimeDec, sKey);
                    Console.WriteLine("Parametar PREZIME je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                try
                {
                    AES.DecryptString(potrosnja, out potrosnjaDec, sKey);
                    Console.WriteLine("Parametar POTROSNJA je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                
                try
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "DodajBrojilo", idDec, imeDec, prezimeDec, potrosnjaDec });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                Console.WriteLine("Dodavanje brojila izvresno!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "DodajBrojilo metoda zahteva DodajEntitet permisiju.");
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new FaultException("Korisnik " + userName + " je pokusao da pozove DodajBrojilo metodu. Ona zahteva DodajEntitet permisiju.");
            }
        }

        // Administrator
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

                string fullname = windowsIdentity.Name.ToString();
                string lhkorisnika = fullname.Split('\\')[0];
                string kime = fullname.Split('\\')[1];
                string sKey = UcitajSKey(lhkorisnika, kime);

                string idDec = string.Empty;

                try
                {
                    AES.DecryptString(id, out idDec, sKey);
                    Console.WriteLine("Parametar ID je uspesno dekriptovan.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Dekripcija neuspesna. Razlog: {0}", e.Message);
                }
                
                try
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBrojilo", idDec });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                Console.WriteLine("Brisanje brojila izvrseno!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "ObrisiBrojilo metoda zahteva ObrisiEntitet permisiju.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new FaultException("Korisnik " + userName + " je pokusao da pozove ObrisiBrojilo metodu. Ona zahteva ObrisiEntitet permisiju.");
            }
        }

        // Superadministrator
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

                try
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBazu" });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                Console.WriteLine("Brisanje baze izvrseno!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "ObrisiBazu metoda zahteva ObrisiBazu permisiju.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new FaultException("Korisnik " + userName + " je pokusao da pozove ObrisiBazu metodu. Ona zahteva ObrisiBazu permisiju.");
            }
        }

        // Superadministrator
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

                try
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ArhivirajBazu" });
                string odgovor = "\n";

                foreach (string str in rezultat)
                {
                    odgovor = odgovor + '\t' + str;
                }

                Console.WriteLine("Arhiviranje baze izvrseno!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "ArhivirajBazu metoda zahteva ArhivirajBazu permisiju.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new FaultException("Korisnik " + userName + " je pokusao da pozove ArhivirajBazu metodu. Ona zahteva ArhivirajBazu permisiju.");
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
