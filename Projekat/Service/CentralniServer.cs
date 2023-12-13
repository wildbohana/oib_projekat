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

        ClientActionLogger cLog = new ClientActionLogger();
 

        #region SKEY
        private string NapraviSKey(string path, string keyFile)

        {
            string sKey = SecretKey.GetKey(path, keyFile);
            return sKey;
        }

        public string DobaviSKey(string lhkorisnika, string kime)
        {
            string path = "..\\..\\SecretKeys\\";
            string keyFile = lhkorisnika + "_" + kime + ".txt";

            string skey = NapraviSKey(path, keyFile);
            return skey;
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
            string sKey = DobaviSKey(lhkorisnika, kime);

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

            return odgovor;
        }

        //Operator
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
                string sKey = DobaviSKey(lhkorisnika, kime);

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
                //audit

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
                cLog.LogAction(userName, "Izmeni potrosnju", true);
                Console.WriteLine("Izmeni potrosnju executed!");

                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "IzmeniPotrosnju method need Modifikuj permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                cLog.LogAction(userName, "IzmeniPotrosnju", false);
                throw new FaultException("User " + userName + " try to call IzmeniPotrosnju method. IzmeniPotrosnju method need  Modifikuj permission.");
            }
        }

        //Operator
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
                string sKey = DobaviSKey(lhkorisnika, kime);

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
                //
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
                cLog.LogAction(userName, "IzmeniID", true);
                Console.WriteLine("IzmeniID executed!");

                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "IzmeniID method need Modifikuj permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                cLog.LogAction(userName, "IzmeniID", false);
                throw new FaultException("User " + userName + " try to call IzmeniID method. IzmeniID method need  Modifikuj permission.");
            }
        }

        //Administrator
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
                string sKey = DobaviSKey(lhkorisnika, kime);

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
                //
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
                cLog.LogAction(userName, "DodajBrojilo", true);
                Console.WriteLine("DodajBrojilo executed!");

                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "DodajBrojilo method need DodajEntitet permission.");
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                cLog.LogAction(userName, "DodajBrojilo", false);
                throw new FaultException("User " + userName + " try to call DodajBrojilo method. DodajBrojilo method need DodajEntitet permission.");
            }
        }

        //Administrator
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
                string sKey = DobaviSKey(lhkorisnika, kime);

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
                //
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
                cLog.LogAction(userName, "ObrisiBrojilo", true);
                Console.WriteLine("ObrisiBrojilo executed!");
                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "ObrisiBrojilo method need ObrisiEntitet permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                cLog.LogAction(userName, "ObrisiBrojilo", false);
                throw new FaultException("User " + userName + " try to call ObrisiBrojilo method. ObrisiBrojilo method need ObrisiEntitet permission.");
            }
        }

        //Superadministrator
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
                cLog.LogAction(userName, "ObrisiBazu", true);
                Console.WriteLine("ObrisiBazu executed!");

                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "ObrisiBazu method need ObrisiBazu permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                cLog.LogAction(userName, "ObrisiBazu", false);
                throw new FaultException("User " + userName + " try to call ObrisiBazu method. ObrisiBazu method need ObrisiBazu permission.");
            }
        }

        //Superadministrator
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
                cLog.LogAction(userName, "ArhivirajBazu", true);
                Console.WriteLine("ArhivirajBazu executed!");

                return odgovor;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName, OperationContext.Current.IncomingMessageHeaders.Action, "ArhivirajBazu method need ArhivirajBazu permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                cLog.LogAction(userName, "ArhivirajBazu", false);
                throw new FaultException("User " + userName + " try to call ArhivirajBazu method. ArhivirajBazu method need ArhivirajBazu permission.");
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
