using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.IO;

namespace Manager
{
	public class CertManager
	{		
		public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

			// Proverava da li je subjectName sertifikata isto kao i prosleđeni parametar
			foreach (X509Certificate2 c in certCollection)
			{
				if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
				{
					return c;
				}
			}

			return null;
		}

		// Za klijentske sertifikate, oni moraju imati Exportable flag zbog privatnih ključeva
        public static X509Certificate2 GetPfxCertificateFromStorage(string subjectName)
        {
			// Projekat\Client\Certs\
            string path = "..\\..\\Certs\\";
            string filename = path + subjectName + ".pfx";
            X509Certificate2 cert = new X509Certificate2(filename, "12345", X509KeyStorageFlags.Exportable);
            return cert;
        }
    }
}
