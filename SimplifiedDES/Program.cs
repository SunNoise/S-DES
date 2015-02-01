using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var contents = prog.ReadFile(args[0]);
                KeyGeneration keygen = new KeyGeneration("1011011010");
                var encryptedContents = new List<BitArray>();
                foreach(var content in contents)
                {
                    Console.Write(Tools.BitArrayToString(content));
                    var encrypted = Decryption.Decrypt(content, keygen.K1, keygen.K2);
                    encryptedContents.Add(encrypted);
                    Console.WriteLine(String.Concat(",", Tools.BitArrayToString(encrypted)));
                }
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
            //wait for key press to end.
            Console.ReadKey();
        }

        private List<BitArray> ReadFile(string fileName)
        {
            var fileContent = new List<BitArray>();

            using (var reader = File.OpenText(fileName)) {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    //foreach value in values loop
                    var bitArray = Tools.StringToBitArray(values[1]);
                    fileContent.Add(bitArray);
                }
            }

            return fileContent;
        }
    }
}
