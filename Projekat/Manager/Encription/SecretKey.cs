using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Uvek AES enkripcija

namespace Manager
{
    public class SecretKey
    {
        #region GENERATE/GET
        public static string GenerateKey()
        {
            string aesKey;
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.KeySize = 128;
                aesAlgorithm.GenerateKey();

                // 128/6 = 5,33 chunks -> poslednja dva znaka će biti == da popune do 6 chunks (144 bytes)
                // 6 chunks == jedan karakter (24 * 6 chunks == 24 karaktera)

                // byte[] -> string (6-bit) (24 karaktera)
                aesKey = Convert.ToBase64String(aesAlgorithm.Key);
            }

            return aesKey;
        }

        public static string GetKey(string folder, string outFile)
        {
            string fullPath = folder + outFile;
            string key;
            if (!File.Exists(fullPath))
            {
                key = GenerateKey();
                StoreKey(key, folder, outFile);
            }
            else
            {
                key = LoadKey(fullPath);
            }

            return key;
        }
        #endregion

        #region STORING/LOADING
        public static void StoreKey(string secretKey, string folder, string outFile)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            FileStream fOutput = new FileStream(folder + outFile, FileMode.OpenOrCreate, FileAccess.Write);
            // Ključ se čuva u UTF-8 formatu jer je to podrazumevan format Windows OS fajl sistema
            byte[] buffer = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                fOutput.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("SecretKeys.StoreKey:: ERROR {0}", e.Message);
            }
            finally
            {
                fOutput.Close();
            }
        }

        public static string LoadKey(string inFile)
        {
            // Ključ se čita iz UTF-8 formata jer je tako i sačuvan iz gore navedenog razloga
            FileStream fInput = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[(int)fInput.Length];

            try
            {
                fInput.Read(buffer, 0, (int)fInput.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("SecretKeys.LoadKey:: ERROR {0}", e.Message);
            }
            finally
            {
                fInput.Close();
            }

            return Encoding.UTF8.GetString(buffer);
        }
        #endregion
    }
}
