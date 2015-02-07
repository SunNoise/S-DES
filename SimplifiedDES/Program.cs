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
            try
            {
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
        }

        private void ReadFile(string fileName, out BitArray[] plainText, out BitArray[] cipherText)
        {
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
        }

        private void DecryptionBruteforce(BitArray[] plainTextArray, BitArray[] cipherTextArray)
        {
            //Decryption bruteforce is faster than encryption
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
        }
    }
}
