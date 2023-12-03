using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
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

            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate =
                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

        }

        #region ZAHTEVI
        public string DobaviPotrosnju(string id, string ime, string prezime)
        {
            string idEnc = string.Empty;
            string imeEnc = string.Empty;
            string prezimeEnc = string.Empty;

            AES.DecryptFile(id, idEnc, SecretKey.sKey);
            AES.DecryptFile(ime, imeEnc, SecretKey.sKey);
            AES.DecryptFile(prezime, prezimeEnc, SecretKey.sKey);

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
            string idEnc = string.Empty;
            string novaPotrosnjaEnc = string.Empty;

            AES.DecryptFile(id, idEnc, SecretKey.sKey);
            AES.DecryptFile(novaPotrosnja, novaPotrosnjaEnc, SecretKey.sKey);


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
            string stariIDEnc = string.Empty;
            string noviIDEnc = string.Empty;

            AES.DecryptFile(stariID, stariIDEnc, SecretKey.sKey);
            AES.DecryptFile(noviID, noviIDEnc, SecretKey.sKey);
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
            string idEnc = string.Empty;
            string imeEnc = string.Empty;
            string prezimeEnc = string.Empty;
            string potrosnjaEnc = string.Empty;

            AES.DecryptFile(id, idEnc, SecretKey.sKey);
            AES.DecryptFile(ime, imeEnc, SecretKey.sKey);
            AES.DecryptFile(prezime, prezimeEnc, SecretKey.sKey);
            AES.DecryptFile(potrosnja, potrosnjaEnc, SecretKey.sKey);


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

            string idEnc = string.Empty;

            AES.DecryptFile(id, idEnc, SecretKey.sKey);

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
