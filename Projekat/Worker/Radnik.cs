using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class Radnik : IRadnik
    {
        public void ObradaZahteva()
        {
            Console.WriteLine("Radnik je primio zahtev!");
        }


        ///////////////////// UNDER CONSTRUCTION /////////////////////


        #region RAD SA TXT BAZOM
        // Folder u kom sa nalazi .sln
        static string direktorijumProjekta = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        // Folder u kom se nalazi baza
        static string direktorijumBaza = direktorijumProjekta.ToString() + "\\BazaPodataka\\";

        // Sinhronizacija pristupa (slično kao mutex i lock)
        static EventWaitHandle kontrolaPristupa = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");

        // Čitanje iz fajla
        static List<string> ProcitajIzFajla()
        {
            List<string> procitano = new List<string>();

            if (kontrolaPristupa.WaitOne())
            {
                try
                {
                    StreamReader sr = new StreamReader(direktorijumBaza + "baza.txt");
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
            if (kontrolaPristupa.WaitOne())
            {
                try
                {
                    StreamWriter sw = new StreamWriter(direktorijumBaza + "baza.txt");
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
