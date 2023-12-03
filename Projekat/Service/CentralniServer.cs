using Common;
using Manager;
using System;
using System.Collections.Generic;
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

        public string GetSecretKey()
        {
            return SecretKey.sKey;
        }

        #region ISERVER
        public string DobaviPotrosnju(string id, string ime, string prezime)
        {
            // TODO izmeni interfejse, dodaj secret keyy kao argument
            

            string idDec = null;
            string imeDec = null ;
            string prezimeDec = null;

            try
            {
                AES.DecryptFile(id, idDec, SecretKey.sKey);
                Console.WriteLine("The file on location {0} is successfully decrypted.", id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
            }

            try
            {
                AES.DecryptFile(ime, imeDec, SecretKey.sKey);
                Console.WriteLine("The file on location {0} is successfully decrypted.", ime);
            }
            catch (Exception e)
            {
                Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
            }

            try
            {
                AES.DecryptFile(prezime, prezimeDec, SecretKey.sKey);
                Console.WriteLine("The file on location {0} is successfully decrypted.", prezime);
            }
            catch (Exception e)
            {
                Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
            }


            Console.WriteLine("\n[ZAHTEV] Dobavi potrosnju");

            // Autentifikacija
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);




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
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = RbacFormatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Modifikuj"))
            {
                Console.WriteLine("\n[ZAHTEV] Dodaj novo brojilo");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                string idDec = null;
                string novaPotrosnjaDec = null;

                try
                {
                    AES.DecryptFile(id, idDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", id);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }
                try
                {
                    AES.DecryptFile(novaPotrosnja, novaPotrosnjaDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", novaPotrosnja);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }


                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniPotrosnju", idDec, novaPotrosnjaDec });
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

        //Operator
        public string IzmeniID(string stariID, string noviID)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = RbacFormatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Modifikuj"))
            {
                Console.WriteLine("\n[ZAHTEV] Izmeni ID brojila");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                string stariIDDec = string.Empty;
                string noviIDDEc = string.Empty;

                try
                {
                    AES.DecryptFile(stariID, stariIDDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", stariID);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }

                try
                {
                    AES.DecryptFile(noviID, noviIDDEc, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", noviID);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }

                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "IzmeniID", stariIDDec, noviIDDec });
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

        //Administrator
        public string DodajBrojilo(string id, string ime, string prezime, string potrosnja)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = RbacFormatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("DodajEntitet"))
            {
                Console.WriteLine("\n[ZAHTEV] Dodaj novo brojilo");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                string idDec = string.Empty;
                string imeDec = string.Empty;
                string prezimeDec = string.Empty;
                string potrosnjaDec = string.Empty;

                try
                {
                    AES.DecryptFile(id, idDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", id);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }
                try
                {
                    AES.DecryptFile(ime, imeDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", ime);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }
                try
                {
                    AES.DecryptFile(prezime, prezimeDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", ime);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }
                try
                {
                    AES.DecryptFile(potrosnja, potrosnjaDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", ime);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }




                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "DodajBrojilo", idDec, imeDec, prezimeDec, potrosnjaDec });
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

        //Administrator
        public string ObrisiBrojilo(string id)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = RbacFormatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ObrisiEntitet"))
            {
                Console.WriteLine("\n[ZAHTEV] Obrisi brojilo");

                // Autentifikacija
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

                WindowsIdentity windowsIdentity = identity as WindowsIdentity;
                Console.WriteLine("Ime klijenta: " + windowsIdentity.Name);

                string idDec = string.Empty;

                try
                {
                    AES.DecryptFile(id, idDec, SecretKey.sKey);
                    Console.WriteLine("The file on location {0} is successfully decrypted.", id);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Decryption failed. Reason: {0}", e.Message);
                }


                // Prosleđivanje zahteva LB-u
                List<string> rezultat = ProslediZahtevLB(new List<string> { "ObrisiBrojilo", idDec });
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

        //Superadministrator
        public string ObrisiBazu()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = RbacFormatter.ParseName(principal.Identity.Name);

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

        //Superadministrator
        public string ArhivirajBazu()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = RbacFormatter.ParseName(principal.Identity.Name);

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
