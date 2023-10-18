using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ILoadBalanser
    {
        // Mock funkcija
        [OperationContract]
        void DelegirajZahtev();

        // Mock funkcija
        [OperationContract]
        void WorkerPrijava();

        // Mock funkcija
        [OperationContract]
        void WorkerOdjava();
    }
}
