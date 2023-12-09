using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// AES uvek radi u ECB modu

// string se interno u programu čuva u UTF-16 formatu (UTF-16 == Unicode)
// string se u fajlovima čuva u UTF-8

namespace Manager
{
    public class AES
    {
        #region ENKRIPCIJA
        public static void EncryptString(string inMessage, out string outMessage, string secretKey)
        {
            byte[] forEncryption = Encoding.Default.GetBytes(inMessage);        // UTF-16 string
            byte[] key = Convert.FromBase64String(secretKey);                   // 8-bit  string
            byte[] encryptedMessage = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };

            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor(key, null);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(forEncryption, 0, forEncryption.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();

                    encryptedMessage = memoryStream.ToArray();
                    outMessage = Encoding.Default.GetString(encryptedMessage);
                }
            }
        }
        #endregion

        #region DEKRIPCIJA
        public static void DecryptString(string inMessage, out string outMessage, string secretKey)
        {
            byte[] message = Encoding.Default.GetBytes(inMessage);
            byte[] key = Convert.FromBase64String(secretKey);
            byte[] decryptedData = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor(key, null);

            using (MemoryStream memoryStream = new MemoryStream(message))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedData = new byte[message.Length];
                    cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                    cryptoStream.Close();
                }
            }

            outMessage = Encoding.Default.GetString(decryptedData);
        }
        #endregion
    }
}
