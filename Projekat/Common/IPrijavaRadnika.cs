using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IPrijavaRadnika
    {

        // Mock funkcija
        [OperationContract]
        void Prijava();

        // Mock funkcija
        [OperationContract]
        void Odjava();
    }
}
