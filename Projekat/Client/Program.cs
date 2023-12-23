using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string adresa = "net.tcp://localhost:9999/CentralniServer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            try
            {
                Klijent proksi = new Klijent(binding, adresa);

                // Autentifikacija
                Console.WriteLine("Klijenta je pokrenuo korisnik: " + WindowsIdentity.GetCurrent().Name);
                Console.WriteLine("Tip autentifikacije: " + WindowsIdentity.GetCurrent().AuthenticationType);
                Console.WriteLine("ID korisnika: " + WindowsIdentity.GetCurrent().User + "\n");

                Meni(proksi);

                Console.WriteLine("\nPritisnite bilo koji taster za izlaz.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nDesila se greska!\n" + e.Message);
                Console.ReadKey();
            }
        }

        #region MENI
        public static void Meni(Klijent proksi)
        {
            while (true)
            {
                Console.WriteLine("Izbor operacije: ");
                Console.WriteLine("-----------------------");
                Console.WriteLine("1. [SVI] Dobavi potrosnju struje.");
                Console.WriteLine("2. [OPERATOR] Izmena ID brojila.");
                Console.WriteLine("3. [OPERATOR] Izmena potrosnje.");
                Console.WriteLine("4. [ADMIN] Dodaj novo brojilo.");
                Console.WriteLine("5. [ADMIN] Obrisi postojece brojilo.");
                Console.WriteLine("6. [SUPER-ADMIN] Obrisi bazu podataka.");
                Console.WriteLine("7. [SUPER-ADMIN] Arhiviraj bazu podataka.");
                Console.WriteLine("8. Izlaz.");

                int i = 0;
                if (!int.TryParse(Console.ReadLine(), out i))
                {
                    Console.WriteLine("\nPogresan izbor!");
                    continue;
                }

                switch (i)
                {
                    case 1:
                        ZahtevPotrosnja(proksi);
                        break;
                    case 2:
                        ZahtevIzmenaID(proksi);
                        break;
                    case 3:
                        ZahtevIzmenaPotrosnje(proksi);
                        break;
                    case 4:
                        ZahtevNovoBrojilo(proksi);
                        break;
                    case 5:
                        ZahtevBrisanjeBrojila(proksi);
                        break;
                    case 6:
                        ZahtevObrisiBazu(proksi);
                        break;
                    case 7:
                        ZahtevArhivirajBazu(proksi);
                        break;
                    case 8:
                        return;
                    default:
                        break;
                }

                Console.Write("Pritisni enter za dalje...");
                Console.ReadLine();
            }
        }
        #endregion

        #region ZAHTEVI
        // Pomoćna funkcija za prebrojavanje dužine unetog ID
        private static int brojCifara(Int32 broj)
        {
            int brojac = 0;
            while (broj > 0)
            {
                broj = broj / 10;
                brojac++;
            }

            return brojac;
        }

        private static void ZahtevPotrosnja(IServer proksi)
        {
            Console.Write("Unesi ime: ");
            String ime = Console.ReadLine();
            Console.Write("Unesi prezime: ");
            String prezime = Console.ReadLine();

            while (true)
            {
                int id = -1;
                Console.Write("Unesi ID (8-cifara): ");

                if (Int32.TryParse(Console.ReadLine(), out id) && brojCifara(id) == 8)
                {
                    Console.WriteLine(proksi.DobaviPotrosnju(id.ToString(), ime, prezime));
                    break;
                }
                else
                {
                    Console.WriteLine("Neispravan unos!");
                }
            }
        }

        private static void ZahtevIzmenaID(IServer proksi)
        {
            Console.Write("Unesi stari ID: ");
            int stariID = 0;

            if (Int32.TryParse(Console.ReadLine(), out stariID) && brojCifara(stariID) == 8)
            {
                Console.Write("Unesi novi ID: ");
                int noviID = 0;

                if (Int32.TryParse(Console.ReadLine(), out noviID))
                {
                    Console.WriteLine(proksi.IzmeniID(stariID.ToString(), noviID.ToString()));
                }
                else
                {
                    Console.WriteLine("Neispravan unos!");
                }
            }
            else
            {
                Console.WriteLine("Neispravan unos!");
            }
        }

        private static void ZahtevIzmenaPotrosnje(IServer proksi)
        {
            Console.Write("Unesi ID brojila: ");
            int id = -1;
            double novaPotrosnja;

            if (Int32.TryParse(Console.ReadLine(), out id) && brojCifara(id) == 8)
            {
                Console.Write("Unesi vrednost za novu potrosnju: ");
                if (double.TryParse(Console.ReadLine(), out novaPotrosnja))
                {
                    Console.WriteLine(proksi.IzmeniPotrosnju(id.ToString(), novaPotrosnja.ToString()));
                }
                else
                {
                    Console.WriteLine("Neispravna vrednost!");
                }
            }
            else
            {
                Console.WriteLine("Neispravan unos!");
            }
        }

        private static void ZahtevNovoBrojilo(IServer proksi)
        {
            Console.Write("Unesi ID za novo brojilo: ");

            int id;
            string ime;
            string prezime;
            int potrosnja;

            if (Int32.TryParse(Console.ReadLine(), out id) && brojCifara(id) == 8)
            {
                Console.Write("Unesi potrosnju: ");
                if (Int32.TryParse(Console.ReadLine(), out potrosnja))
                {
                    Console.Write("Unesi ime: ");
                    ime = Console.ReadLine();
                    Console.Write("Unesi prezime: ");
                    prezime = Console.ReadLine();

                    Console.WriteLine(proksi.DodajBrojilo(id.ToString(), ime, prezime, potrosnja.ToString()));
                }
                else
                {
                    Console.WriteLine("Neispravna vrednost!");
                }
            }
            else
            {
                Console.WriteLine("Neispravan unos!");
            }
        }

        private static void ZahtevBrisanjeBrojila(IServer proksi)
        {
            Console.Write("Unesi ID brojila za brisanje: ");
            int id;

            if (Int32.TryParse(Console.ReadLine(), out id) && brojCifara(id) == 8)
            {
                Console.WriteLine(proksi.ObrisiBrojilo(id.ToString()));
            }
            else
            {
                Console.WriteLine("Neispravan unos!");
            }
        }

        private static void ZahtevObrisiBazu(IServer proksi)
        {
            Console.WriteLine(proksi.ObrisiBazu());
        }

        private static void ZahtevArhivirajBazu(IServer proksi)
        {
            Console.WriteLine(proksi.ArhivirajBazu());
        }
        #endregion
    }
}
