using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Klijent : ChannelFactory<IServer>, IServer, IDisposable
    {
        // Kanal za konekciju
        public IServer kanal;

        public Klijent(NetTcpBinding binding, string address) : base(binding, address)
        {
            kanal = this.CreateChannel();
        }

        #region SKEY
        // Pomoćna funkcija za dobavljanje ključa za enkripciju
        public string DobaviSKey(string lhkorisnika, string kime)
        {
            string sKey = kanal.DobaviSKey(lhkorisnika, kime);
            return sKey;
        }
        #endregion

        #region ZAHTEVI
        public string DobaviPotrosnju(string id, string ime, string prezime)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string fullname = wi.Name.ToString();
            string lhkorisnika = fullname.Split('\\')[0];
            string kime = fullname.Split('\\')[1];
            string sKey = DobaviSKey(lhkorisnika, kime);

            string idEnc = null;
            string imeEnc = null;
            string prezimeEnc = null;

            AES.EncryptString(id, out idEnc, sKey);
            AES.EncryptString(ime, out imeEnc, sKey);
            AES.EncryptString(prezime, out prezimeEnc, sKey);

            try
            {
                return kanal.DobaviPotrosnju(idEnc, imeEnc, prezimeEnc);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string IzmeniPotrosnju(string id, string novaPotrosnja)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string fullname = wi.Name.ToString();
            string lhkorisnika = fullname.Split('\\')[0];
            string kime = fullname.Split('\\')[1];
            string sKey = DobaviSKey(lhkorisnika, kime);

            string idEnc = null;
            string novaPotrosnjaEnc = null;

            AES.EncryptString(id, out idEnc, sKey);
            AES.EncryptString(novaPotrosnja, out novaPotrosnjaEnc, sKey);

            try
            {
                return kanal.IzmeniPotrosnju(idEnc, novaPotrosnjaEnc);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string IzmeniID(string stariID, string noviID)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string fullname = wi.Name.ToString();
            string lhkorisnika = fullname.Split('\\')[0];
            string kime = fullname.Split('\\')[1];
            string sKey = DobaviSKey(lhkorisnika, kime);

            string stariIDEnc = null;
            string noviIDEnc = null;

            AES.EncryptString(stariID, out stariIDEnc, sKey);
            AES.EncryptString(noviID, out noviIDEnc, sKey);

            try
            {
                return kanal.IzmeniID(stariIDEnc, noviIDEnc);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string DodajBrojilo(string id, string ime, string prezime, string potrosnja)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string fullname = wi.Name.ToString();
            string lhkorisnika = fullname.Split('\\')[0];
            string kime = fullname.Split('\\')[1];
            string sKey = DobaviSKey(lhkorisnika, kime);

            string idEnc = null;
            string imeEnc = null;
            string prezimeEnc = null;
            string potrosnjaEnc = null;

            AES.EncryptString(id, out idEnc, sKey);
            AES.EncryptString(ime, out imeEnc, sKey);
            AES.EncryptString(prezime, out prezimeEnc, sKey);
            AES.EncryptString(potrosnja, out potrosnjaEnc, sKey);

            try
            {
                return kanal.DodajBrojilo(idEnc, imeEnc, prezimeEnc, potrosnjaEnc);
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }
            return "";
        }

        public string ObrisiBrojilo(string id)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string fullname = wi.Name.ToString();
            string lhkorisnika = fullname.Split('\\')[0];
            string kime = fullname.Split('\\')[1];
            string sKey = DobaviSKey(lhkorisnika, kime);

            string idEnc = null;
            AES.EncryptString(id, out idEnc, sKey);

            try
            {
                return kanal.ObrisiBrojilo(idEnc);
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
