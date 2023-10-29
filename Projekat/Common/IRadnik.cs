using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IRadnik
    {
        [OperationContract]
        List<string> ObradaZahteva(List<string> zahtev);
    }
}
