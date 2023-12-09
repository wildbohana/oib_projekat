using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class Radnik : IRadnik
    {
        #region KONFIGURACIJA
        // Vrednosti iz config fajla - cene po zonama
        static int zelenaCena = Int32.Parse(ConfigurationManager.AppSettings["zelenaCena"]);
        static int plavaCena = Int32.Parse(ConfigurationManager.AppSettings["plavaCena"]);
        static int crvenaCena = Int32.Parse(ConfigurationManager.AppSettings["crvenaCena"]);

        // Vrednosti iz config fajla - cene po zonama
        static int zelenaGranica = Int32.Parse(ConfigurationManager.AppSettings["zelenaGornjaGranica"]);
        static int plavaGranica = Int32.Parse(ConfigurationManager.AppSettings["plavaGornjaGranica"]);
        #endregion

        #region IRADNIK
        public List<string> ObradaZahteva(List<string> zahtev)
        {
            Console.WriteLine("Radnik je primio zahtev!");
            Console.WriteLine("Zahtev: " + zahtev[0] + "\n");

            if (zahtev[0] == "DobaviPotrosnju")
            {
                return DobaviPotrosnju(zahtev);
            }
            else if (zahtev[0] == "DodajBrojilo")
            {
                return DodajBrojilo(zahtev[1], zahtev[2], zahtev[3], zahtev[4]);
            }
            else if (zahtev[0] == "ObrisiBrojilo")
            {
                return ObrisiBrojilo(zahtev[1]);
            }
            else if (zahtev[0] == "IzmeniPotrosnju")
            {
                return IzmeniPotrosnju(zahtev[1], zahtev[2]);
            }
            else if (zahtev[0] == "IzmeniID")
            {
                return IzmeniID(zahtev[1], zahtev[2]);
            }
            else if (zahtev[0] == "ObrisiBazu")
            {
                return ObrisiBazu();
            }
            else if (zahtev[0] == "ArhivirajBazu")
            {
                return ArhivirajBazu();
            }
            else
            {
                return new List<string>();
            }
        }
        #endregion

        #region ZAHTEVI
        static List<string> DobaviPotrosnju(List<string> zahtev)
        {
            // zahtev[0] - string koji predstavlja zahtev (u ovom slučaju "DobaviPotrosnju"
            string ime = zahtev[2];
            string prezime = zahtev[3];
            int id = -1;
            Int32.TryParse(zahtev[1], out id);

            List<string> baza = ProcitajIzFajla();

            foreach (string red in baza)
            {
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                // Porede se stringovi - onda je najbolje izvlačiti podatke iz zahteva
                if (bazaID == zahtev[1] && bazaIme == zahtev[2] && bazaPrezime == zahtev[3])
                {
                    int cena = 0;
                    int potrosnja = 0;
                    Int32.TryParse(bazaPotrosnja, out potrosnja);
                    string imeZone = "";
                    int cenaZone = 0;

                    // Zelena zona
                    if (potrosnja < zelenaGranica)
                    {
                        cena = potrosnja * zelenaCena;
                        imeZone += "ZELENA";
                        cenaZone = zelenaCena;
                    }
                    // Plava zona
                    else if (potrosnja < plavaGranica)
                    {
                        cena = potrosnja * plavaCena;
                        imeZone += "PLAVA";
                        cenaZone = plavaCena;
                    }
                    // Crvena zona
                    else
                    {
                        cena = potrosnja * crvenaCena;
                        imeZone += "CRVENA";
                        cenaZone = crvenaCena;
                    }

                    // Poruka koja se vraća korisniku
                    string poruka = "";
                    poruka += "Potrosnja za ID: " + bazaID + ", ime i prezime: " + bazaIme + " " + bazaPrezime + " je:\n";
                    poruka += "\tZona: " + imeZone + " [" + cenaZone + " din/kWh]" + "\n";
                    poruka += "\tPotrosnja: " + potrosnja.ToString() + "\n";
                    poruka += "\tCena: " + cena + ".\n";

                    return new List<string> { poruka };
                }
            }

            return new List<string> { "Greska!", "Podaci za trazenog korisnika se ne nalaze u bazi." };
        }

        // Dodavanje novog brojila u bazu podataka, posle dodavanja se vrši upis u fajl
        static List<string> DodajBrojilo(string id, string ime, string prezime, string potrosnja)
        {
            List<string> baza = ProcitajIzFajla();

            foreach (string red in baza)
            {
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                if (bazaID == id)
                {
                    return new List<string> { "Greska!", "U bazi se vec nalazi brojilo sa datim ID-jem." };
                }
            }

            baza.Add(id + "," + ime + "," + prezime + "," + potrosnja);
            UpisiUFajl(baza);

            return new List<string> { "OK", "Uspesno dodato novo brojilo." };
        }

        // Brisanje brojila iz baze
        static List<string> ObrisiBrojilo(string id)
        {
            List<string> baza = ProcitajIzFajla();
            int i = 0;
            bool postoji = false;

            foreach (string red in baza)
            {
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                if (id == bazaID)
                {
                    postoji = true;
                    break;
                }

                i++;
            }

            if (postoji)
            {
                string red = baza[i];
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                baza.RemoveAt(i);
                UpisiUFajl(baza);

                return new List<string> { "OK", "Brojilo: " + bazaID + ", ime i prezime: " + bazaIme + " " + bazaPrezime + " - obrisano." };
            }
            else
            {
                return new List<string> { "Greska!", "Trazeni podaci se ne nalaze u bazi podataka." };
            }
        }

        // Izmena potrosnje za brojilo
        static List<string> IzmeniPotrosnju(string id, string novaVrednost)
        {
            List<string> baza = ProcitajIzFajla();
            int i = 0;

            foreach (string red in baza)
            {
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                if (bazaID == id)
                {
                    baza[i] = bazaID + "," + bazaIme + "," + bazaPrezime + "," + novaVrednost;
                    UpisiUFajl(baza);

                    return new List<string> { "OK", "Vrednost izmenjena. Stara potrosnja: " + bazaPotrosnja + ". Nova potrosnja: " + novaVrednost + "." };
                }

                i++;
            }

            return new List<string> { "Greska!", "Trazeni podaci se ne nalaze u bazi podataka." };
        }

        // Izmena ID-ja za brojilo
        static List<string> IzmeniID(string stariID, string noviID)
        {
            List<string> baza = ProcitajIzFajla();

            foreach (string red in baza)
            {
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                if (bazaID == noviID)
                {
                    return new List<string> { "Greska!", "Brojilo sa tim ID-jem vec postoji u bazi podataka." };
                }
            }

            int i = 0;

            foreach (string red in baza)
            {
                string bazaID, bazaIme, bazaPrezime, bazaPotrosnja;
                ObradaRedaIzBaze(red, out bazaID, out bazaIme, out bazaPrezime, out bazaPotrosnja);

                if (bazaID == stariID)
                {
                    baza[i] = noviID + "," + bazaIme + "," + bazaPrezime + "," + bazaPotrosnja;
                    UpisiUFajl(baza);
                    return new List<string> { "OK", "ID izmenjen. Stari ID: " + stariID + ". Novi ID: " + noviID + "." };
                }

                i++;
            }

            return new List<string> { "Greska!", "Stari ID ne postoji u bazi podataka!." };
        }

        // Brisanje podataka iz baze
        public List<string> ObrisiBazu()
        {
            List<string> baza = ProcitajIzFajla();
            baza.Clear();
            UpisiUFajl(baza);

            return new List<string> { "OK", "Baza podataka je uspesno obrisana!" };
        }

        // Arhiviranje baze - pravi arhivu i briše sve iz baze
        public List<string> ArhivirajBazu()
        {
            // Arhiviranje baze
            List<string> baza = ProcitajIzFajla();
            string imeArhive = SacuvajArhivu(baza);

            // Brisanje baze
            baza.Clear();
            UpisiUFajl(baza);

            return new List<string> { "OK", "Baza podataka uspesno arhivirana. Ime arhive: " + imeArhive };
        }
        #endregion

        #region BAZA PODATAKA
        // Folder u kom sa nalazi .sln
        static string direktorijumProjekta = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        // Folder u kom se nalazi baza
        static string direktorijumBaza = direktorijumProjekta + "\\BazaPodataka\\";

        // Sinhronizacija pristupa (slično kao mutex i lock)
        static EventWaitHandle kontrolaPristupa = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");

        // Čitanje iz fajla
        static List<string> ProcitajIzFajla()
        {
            List<string> procitano = new List<string>();

            // Ako ne postoji fajl, kreiraj ga
            string bazatxt = direktorijumBaza + "baza.txt";
            if (!File.Exists(bazatxt))
            {
                File.CreateText(bazatxt);
            }

            if (kontrolaPristupa.WaitOne())
            {
                try
                {
                    StreamReader sr = new StreamReader(bazatxt);
                    string red = "";

                    while ((red = sr.ReadLine()) != null)
                    {
                        procitano.Add(red);
                    }

                    sr.Close();
                }
                finally
                {
                    kontrolaPristupa.Set();
                }
            }

            return procitano;
        }

        // Upis u fajl
        static void UpisiUFajl(List<string> upis)
        {
            // Ako ne postoji fajl, kreiraj ga
            string bazatxt = direktorijumBaza + "baza.txt";
            if (!File.Exists(bazatxt))
            {
                File.CreateText(bazatxt);
            }

            if (kontrolaPristupa.WaitOne())
            {
                try
                {
                    StreamWriter sw = new StreamWriter(bazatxt);
                    foreach (string red in upis)
                    {
                        sw.WriteLine(red);
                    }

                    sw.Close();
                }
                finally
                {
                    kontrolaPristupa.Set();
                }
            }
        }

        // Arhiviranje
        static string SacuvajArhivu(List<string> arhiva)
        {
            string imeArhive = direktorijumBaza + "Arhiva\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";

            try
            {
                StreamWriter sw = new StreamWriter(imeArhive);        
                foreach (string red in arhiva)
                {
                    sw.WriteLine(red);
                }

                sw.Close();
            }
            finally { }

            return imeArhive;
        }

        // Obrada reda iz baze
        // FORMAT STRINGA U BAZI: id,ime,prezime,potrosnja
        static void ObradaRedaIzBaze(string red, out string id, out string ime, out string prezime, out string potrosnja)
        {
            string[] values = red.Split(',');
            id = values[0];
            ime = values[1];
            prezime = values[2];
            potrosnja = values[3];
        }
        #endregion
    }
}
