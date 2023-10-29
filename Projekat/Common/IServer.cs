using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IServer
    {
        [OperationContract]
        string DobaviPotrosnju(string id, string ime, string prezime);

        // Samo operateri
        [OperationContract]
        string IzmeniPotrosnju(string id, string novaPotrosnja);

        // Samo operateri
        [OperationContract]
        string IzmeniID(string stariID, string noviID);

        // Samo admini
        [OperationContract]
        string DodajBrojilo(string id, string ime, string prezime, string potrosnja);

        // Samo admini
        [OperationContract]
        string ObrisiBrojilo(string id);

        // Samo supet-admini
        [OperationContract]
        string ObrisiBazu();

        // Samo super-admini
        [OperationContract]
        string ArhivirajBazu();
    }
}
