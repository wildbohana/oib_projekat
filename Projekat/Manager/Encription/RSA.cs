using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Asimetrična enkripcija deljenog AES ključa
// Koristi se Base64 string jer je SKey pretvaran iz byte[] u string preko toga

namespace Manager
{
    public class RSA
    {
        public static string EncryptSKey(string plainText, string publicKey)
        {
            // Looks for the certificate's public key to encript message
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.FromXmlString(publicKey);

            byte[] byteText = Convert.FromBase64String(plainText);
            byte[] byteMsg = csp.Encrypt(byteText, false);

            return Convert.ToBase64String(byteMsg);
        }

        public static string DecryptSKey(string cipherText, string privateKey)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.FromXmlString(privateKey);

            byte[] byteMsg = Convert.FromBase64String(cipherText);
            byte[] byteText = csp.Decrypt(byteMsg, false);

            return Convert.ToBase64String(byteText);
        }
    }
}
