using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Program prog = new Program();
                var plainTextList = new List<BitArray>();
                var cipherTextList = new List<BitArray>();
                prog.ReadFile(args[0], out plainTextList, out cipherTextList);
                prog.EncryptAndDecrypt(plainTextList, cipherTextList);
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private void ReadFile(string fileName, out List<BitArray> plainText, out List<BitArray> cipherText)
        {
            plainText = new List<BitArray>();
            cipherText = new List<BitArray>();

            using (var reader = File.OpenText(fileName)) {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    var plain = Tools.StringToBitArray(values[0]);
                    var cipher = Tools.StringToBitArray(values[1]);
                    plainText.Add(plain);
                    cipherText.Add(cipher);
                }
            }
        }

        private void EncryptAndDecrypt(List<BitArray> plainTextList, List<BitArray> cipherTextList)
        {
            Stopwatch completeWatch = new Stopwatch();
            Stopwatch encryptionWatch = new Stopwatch();
            Stopwatch decryptionWatch = new Stopwatch();
            completeWatch.Start();
            KeyGeneration keygen = new KeyGeneration("1011011010");
            encryptionWatch.Start();
            foreach (var content in plainTextList)
            {
                var encrypted = Encryption.Encrypt(content, keygen.K1, keygen.K2);
            }
            encryptionWatch.Stop();
            decryptionWatch.Start();
            foreach (var content in cipherTextList)
            {
                var decrypted = Decryption.Decrypt(content, keygen.K1, keygen.K2);
            }
            decryptionWatch.Stop();
            completeWatch.Stop();
            Console.WriteLine("Encryption Time: " + encryptionWatch.Elapsed.TotalMilliseconds + " ms.");
            Console.WriteLine("Decryption Time: " + decryptionWatch.Elapsed.TotalMilliseconds + " ms.");
            Console.WriteLine("Encryption & Decryption Time: " + completeWatch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
