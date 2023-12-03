using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class AesFormatter
    {
        const int headerLenght = 54;

        public static void Decompose(byte[] bytePicture, out byte[] header, out byte[] body)
        {
            header = new byte[headerLenght];
            body = new byte[bytePicture.Length - headerLenght];

            for (int i = 0; i < bytePicture.Length; i++)
            {
                if (i < headerLenght)
                {
                    header[i] = bytePicture[i];
                }
                else
                {
                    body[i - headerLenght] = bytePicture[i];
                }
            }
        }

        public static void Compose(byte[] header, byte[] body, int outputLenght, string outFile)
        {
            byte[] output = new byte[outputLenght];

            for (int i = 0; i < outputLenght; i++)
            {
                if (i < headerLenght)
                {
                    output[i] = header[i];
                }
                else
                {
                    output[i] = body[i - headerLenght];
                }
            }

            BinaryWriter Writer = new BinaryWriter(File.OpenWrite(outFile));
            Writer.Write(output); 
            Writer.Flush();
            Writer.Close();
        }
    }
}
