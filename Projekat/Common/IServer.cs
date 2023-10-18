using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// Samo za serverske komponente se pravi interfejs
// Klijenti su klijenti, ne treba ništa

namespace Common
{
    [ServiceContract]
    public interface IServer
    {
        // Mock funkcija
        [OperationContract]
        void Autentifikuj();

        [OperationContract]
        void Autorizuj();
    }
}
