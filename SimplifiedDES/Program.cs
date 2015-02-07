using SimplifiedDES.Attacks;
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
            Stopwatch completeWatch = new Stopwatch();
            try
            {
                completeWatch.Start();
                Program prog = new Program();
                BitArray[] plainTextArray;
                BitArray[] cipherTextArray;
                prog.ReadFile(args[0], out plainTextArray, out cipherTextArray);
                prog.DecryptionBruteforce(plainTextArray, cipherTextArray);
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                completeWatch.Stop();
            }
            Console.WriteLine("Total Time: " + completeWatch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void ReadFile(string fileName, out BitArray[] plainText, out BitArray[] cipherText)
        {
            Stopwatch readingWatch = new Stopwatch();
            readingWatch.Start();
            var plainTextList = new List<BitArray>();
            var cipherTextList = new List<BitArray>();

            using (var reader = File.OpenText(fileName)) {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    var plain = Tools.StringToBitArray(values[0]);
                    var cipher = Tools.StringToBitArray(values[1]);
                    plainTextList.Add(plain);
                    cipherTextList.Add(cipher);
                }
            }
            plainText = plainTextList.ToArray();
            cipherText = cipherTextList.ToArray();
            readingWatch.Stop();
            Console.WriteLine("Reading From File Time: " + readingWatch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void EncryptAndDecrypt(BitArray[] plainTextArray, BitArray[] cipherTextArray, string key)
        {
            Stopwatch completeWatch = new Stopwatch();
            Stopwatch encryptionWatch = new Stopwatch();
            Stopwatch decryptionWatch = new Stopwatch();
            completeWatch.Start();
            KeyGeneration keygen = new KeyGeneration(key);
            encryptionWatch.Start();
            foreach (var content in plainTextArray)
            {
                var encrypted = Encryption.Encrypt(content, keygen.K1, keygen.K2);
            }
            encryptionWatch.Stop();
            decryptionWatch.Start();
            foreach (var content in cipherTextArray)
            {
                var decrypted = Decryption.Decrypt(content, keygen.K1, keygen.K2);
            }
            decryptionWatch.Stop();
            completeWatch.Stop();
            Console.WriteLine("Encryption Time: " + encryptionWatch.Elapsed.TotalMilliseconds + " ms.");
            Console.WriteLine("Decryption Time: " + decryptionWatch.Elapsed.TotalMilliseconds + " ms.");
            Console.WriteLine("Encryption & Decryption Time: " + completeWatch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void BruteForceAttacks(BitArray[] plainTextArray, BitArray[] cipherTextArray)
        {
            Stopwatch BFEncryptionWatch = new Stopwatch();
            Stopwatch BFDecryptionWatch = new Stopwatch();
            BFEncryptionWatch.Start();
            var EncKeyList = new List<string>();
            for (int x = 0; x < plainTextArray.Length; x++)
            {
                var key = Bruteforce.EncryptionAttack(plainTextArray[x], cipherTextArray[x], EncKeyList);
                if (!EncKeyList.Contains(key))
                    EncKeyList.Add(key);
            }
            BFEncryptionWatch.Stop();
            BFDecryptionWatch.Start();
            var DecKeyList = new List<string>();
            for (int x = 0; x < cipherTextArray.Length; x++)
            {
                var key = Bruteforce.DecryptionAttack(plainTextArray[x], cipherTextArray[x], DecKeyList);
                if (!DecKeyList.Contains(key))
                    DecKeyList.Add(key);
            }
            BFDecryptionWatch.Stop();
            Console.WriteLine("Bruteforce Encryption Time: " + BFEncryptionWatch.Elapsed.TotalMilliseconds + " ms.");
            Console.WriteLine("Bruteforce Decryption Time: " + BFDecryptionWatch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void DecryptionBruteforce(BitArray[] plainTextArray, BitArray[] cipherTextArray)
        {
            //Decryption bruteforce is faster than encryption
            Stopwatch DBFWatch = new Stopwatch();
            DBFWatch.Start();
            var DecKeyList = new List<string>();
            for (int x = 0; x < cipherTextArray.Length; x++)
            {
                var key = Bruteforce.DecryptionAttack(plainTextArray[x], cipherTextArray[x], DecKeyList);
                if (!DecKeyList.Contains(key))
                    DecKeyList.Add(key);
            }
            StringBuilder sB = new StringBuilder("Keys found: ");
            foreach (var key in DecKeyList)
            {
                sB.Append(key);
                sB.Append(", ");
            }
            sB.Remove(sB.Length - 2, 2);
            Console.WriteLine(sB.ToString());
            var goodKeys = Bruteforce.KeysForAll(DecKeyList, plainTextArray, cipherTextArray);
            sB.Clear();
            sB.Append("Keys that work for all the pairs: ");
            foreach (var key in goodKeys)
            {
                sB.Append(key);
                sB.Append(", ");
            }
            sB.Remove(sB.Length - 2, 2);
            Console.WriteLine(sB.ToString());
            DBFWatch.Stop();
            Console.WriteLine("Decryption Bruteforce Time: " + DBFWatch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
