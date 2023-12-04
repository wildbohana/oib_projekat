﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// AES uvek radi u ECB modu

namespace Manager
{
    public class AES
    {
        #region ENKRIPCIJA
        public static void EncryptString(string inMessage, out string outMessage, string secretKey)
        {
            byte[] forEncryption = Encoding.Unicode.GetBytes(inMessage);
            byte[] key = Encoding.ASCII.GetBytes(secretKey);
            byte[] encryptedMessage = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            //ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor(key, null);
            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(forEncryption, 0, forEncryption.Length);
                    cryptoStream.FlushFinalBlock();
                    //cryptoStream.Close();

                    encryptedMessage = memoryStream.ToArray();
                    outMessage = Encoding.Unicode.GetString(encryptedMessage);
                }
            }
        }
        #endregion

        #region DEKRIPCIJA
        public static void DecryptString(string inMessage, out string outMessage, string secretKey)
        {
            byte[] message = Encoding.Unicode.GetBytes(inMessage);
            byte[] key = Encoding.ASCII.GetBytes(secretKey);
            byte[] decryptedData = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();

            using (MemoryStream memoryStream = new MemoryStream(message))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedData = new byte[message.Length];
                    cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                }
            }

            outMessage = Encoding.Unicode.GetString(decryptedData);
        }
        #endregion
    }
}
