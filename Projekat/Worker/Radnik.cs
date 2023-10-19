using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class Radnik : IRadnik
    {
        public void ObradaZahteva()
        {
            Console.WriteLine("Radnik je primio zahtev!");
        }
    }
}
