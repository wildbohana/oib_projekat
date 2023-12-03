using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class AES
    {
        // Uvek u ECB modu
		public static void EncryptFile(string inFile, string outFile, string secretKey)
        {
            byte[] header = null;   //image header (54 byte) should not be encrypted
            byte[] body = null;     //image body to be encrypted
            byte[] encryptedBody = null;

            AesFormatter.Decompose(File.ReadAllBytes(inFile), out header, out body);

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(body, 0, body.Length);
                    encryptedBody = memoryStream.ToArray();
                }
            }
            
            int outputLenght = header.Length + encryptedBody.Length;              //header.Length + body.Length
            AesFormatter.Compose(header, encryptedBody, outputLenght, outFile);
        }

        // Uvek u ECB
        public static void DecryptFile(string inFile, string outFile, string secretKey)
        {
            byte[] header = null;       //image header (54 byte) should not be decrypted
            byte[] body = null;         //image body to be decrypted
            byte[] decryptedBody = null;

            AesFormatter.Decompose(File.ReadAllBytes(inFile), out header, out body);
            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();
            using (MemoryStream memoryStream = new MemoryStream(body))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedBody = new byte[body.Length];     //decrypted image body - the same lenght as encrypted part
                    cryptoStream.Read(decryptedBody, 0, decryptedBody.Length);
                }
            }            

            int outputLenght = header.Length + decryptedBody.Length;
            AesFormatter.Compose(header, decryptedBody, outputLenght, outFile);
        }
    }
}
