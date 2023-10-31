using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Klijent : ChannelFactory<IServer>, IServer, IDisposable
    {
        // Kanal za konekciju
        public IServer kanal;

        // Konstruktori
        public Klijent(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            kanal = this.CreateChannel();
        }

        public Klijent(NetTcpBinding binding, string address) : base(binding, address)
        {
            kanal = this.CreateChannel();
        }

        #region ZAHTEVI
        public string DobaviPotrosnju(string id, string ime, string prezime)
        {
            try
            {
                return kanal.DobaviPotrosnju(id, ime, prezime);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string IzmeniPotrosnju(string id, string novaPotrosnja)
        {
            try
            {
                return kanal.IzmeniPotrosnju(id, novaPotrosnja);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string IzmeniID(string stariID, string noviID)
        {
            try
            {
                return kanal.IzmeniID(stariID, noviID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string DodajBrojilo(string id, string ime, string prezime, string potrosnja)
        {
            try
            {
                return kanal.DodajBrojilo(id, ime, prezime, potrosnja);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string ObrisiBrojilo(string id)
        {
            try
            {
                return kanal.ObrisiBrojilo(id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string ObrisiBazu()
        {
            try
            {
                return kanal.ObrisiBazu();
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string ArhivirajBazu()
        {
            try
            {
                return kanal.ArhivirajBazu();
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        // GetSecretKey ??
        #endregion

        #region IDISPOSABLE
        // IDisposable interfejs
        public void Dispose()
        {
            if (kanal != null)
            {
                kanal = null;
            }
            this.Close();
        }
        #endregion
    }
}
