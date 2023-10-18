using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CentralniServer : IServer
    {
        // TODO prosledi zahtev na LB
        public void PosaljiZahtev()
        {
            Console.WriteLine("Centralni server je primio zahtev");
        }
    }
}
