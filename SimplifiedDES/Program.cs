using SimplifiedDES.Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

        private void PrecomputedAttack(BitArray[] plainTextArray, BitArray[] cipherTextArray)
        {
            var KeyList = new List<string>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SimplifiedDES.Properties.Settings.PrecomputedConnectionString"].ConnectionString;
                conn.Open();

                for (int x = 0; x < cipherTextArray.Length; x++)
                {
                    SqlCommand command = new SqlCommand("SELECT [key] FROM Combinations WHERE plain = @0 AND cipher = @1", conn);
                    command.Parameters.Add(new SqlParameter("0", Tools.BitArrayToString(plainTextArray[x])));
                    command.Parameters.Add(new SqlParameter("1", Tools.BitArrayToString(cipherTextArray[x])));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!KeyList.Contains(reader[0]))
                                KeyList.Add(String.Format("{0}", reader[0]));
                        }
                    }
                }
            }
            StringBuilder sB = new StringBuilder("Keys found: ");
            foreach (var key in KeyList)
            {
                sB.Append(key);
                sB.Append(", ");
            }
            sB.Remove(sB.Length - 2, 2);
            Console.WriteLine(sB.ToString());
            var goodKeys = Bruteforce.KeysForAll(KeyList, plainTextArray, cipherTextArray);
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

        private void FillDB()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SimplifiedDES.Properties.Settings.PrecomputedConnectionString"].ConnectionString; 
                conn.Open();

                int z = 0;
                for (int x = 0; x < 256; x++)
                {
                    string input = Convert.ToString(x, 2).PadLeft(8, '0');
                    for (int y = 0; y < 1024; y++)
                    {
                        string key = Convert.ToString(y, 2).PadLeft(10, '0');
                        KeyGeneration keygen = new KeyGeneration(key);
                        string output = Tools.BitArrayToString(Encryption.Encrypt(Tools.StringToBitArray(input), keygen.K1, keygen.K2));

                        SqlCommand insertCommand = new SqlCommand("INSERT INTO Combinations ([plain], [cipher], [key], [Id]) VALUES (@0, @1, @2, @3)", conn);
                        insertCommand.Parameters.Add(new SqlParameter("0", input));
                        insertCommand.Parameters.Add(new SqlParameter("1", output));
                        insertCommand.Parameters.Add(new SqlParameter("2", key));
                        insertCommand.Parameters.Add(new SqlParameter("3", z));
                        insertCommand.ExecuteNonQuery();

                        z++;
                        Console.WriteLine(z);
                    }
                }
            }
        }
    }
}
