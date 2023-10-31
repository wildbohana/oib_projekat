using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IPrijavaRadnika
    {
        [OperationContract]
        void Prijava(int id);

        [OperationContract]
        void Odjava(int id);

        [OperationContract]
        int DodeliID();
    }
}
