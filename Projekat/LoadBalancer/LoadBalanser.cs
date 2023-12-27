using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalanser : ILoadBalanser
    {
        static int poslednjiRadnik = -1;

        // Prosleđivanje zahteva ka nekom od Radnika
        public List<string> DelegirajZahtev(List<string> zahtev)
        {
            List<string> rezultat = new List<string>();
            if (PrijavaRadnika.Radnici.Count > 0)
            {
                // Po principu kružnog bafera
                poslednjiRadnik = (poslednjiRadnik + 1) % (PrijavaRadnika.Radnici.Count);
            }
            else
            {
                poslednjiRadnik = -1;
            }

            // Ako postoje radnici, proveri da li je neki od njih slobodan
            if (PrijavaRadnika.Radnici.Count > 0)
            {
                int i = 0;
                foreach (IRadnik radnik in PrijavaRadnika.Radnici.Values)
                {
                    if (i == poslednjiRadnik)
                    {
                        Console.WriteLine("[ZAHTEV] Zahtev je prosleđen radniku sa ID=" + i);
                        rezultat = radnik.ObradaZahteva(zahtev);
                    }
                    i++;
                }
            }
            else
            {
                Console.WriteLine("[PROBLEM] Trenutno nema slobodnih radnika!");
            }

            return rezultat;
        }
    }
}
