using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES.Attacks
{
    class Bruteforce
    {
        internal static string EncryptionAttack(BitArray plain, BitArray cipher, List<string> cache)
        {
            foreach (var c in cache)
            {
                string key = c;
                string result;
                if ((result = EAttack(plain, cipher, key)) != null)
                    return result;
            }
            for (int x = 0; x < 1024; x++)
            {
                string key = Convert.ToString(x, 2).PadLeft(10, '0');
                if (!cache.Contains(key))
                {
                    string result;
                    if ((result = EAttack(plain, cipher, key)) != null)
                        return result;
                }
            }

            throw new KeyNotFoundException();
        }

        internal static string DecryptionAttack(BitArray plain, BitArray cipher, List<string> cache)
        {
            foreach (var c in cache)
            {
                string key = c;
                string result;
                if ((result = DAttack(plain, cipher, key)) != null)
                    return result;
            }
            for (int x = 0; x < 1024; x++)
            {
                string key = Convert.ToString(x, 2).PadLeft(10, '0');
                if (!cache.Contains(key))
                {
                    string result;
                    if ((result = DAttack(plain, cipher, key)) != null)
                        return result;
                }
            }
            throw new KeyNotFoundException();
        }

        internal static IEnumerable<string> KeysForAll(List<string> keys, BitArray[] plainTextArray, BitArray[] cipherTextArray)
        {
            var uselessKeys = new List<string>();
            foreach (var key in keys)
            {
                for (int x = 0; x < plainTextArray.Length; x++)
                {
                    if (DAttack(plainTextArray[x], cipherTextArray[x], key) == null)
                    {
                        uselessKeys.Add(key);
                        break;
                    }
                }
            }
            return keys.Except(uselessKeys);
        }

        private static string EAttack(BitArray plain, BitArray cipher, string key)
        {
            KeyGeneration keygen = new KeyGeneration(key);
            var encrypted = Encryption.Encrypt(plain, keygen.K1, keygen.K2);
            if (Tools.BitArrayEquals(encrypted, cipher))
                return key;
            return null;
        }

        private static string DAttack(BitArray plain, BitArray cipher, string key)
        {
            KeyGeneration keygen = new KeyGeneration(key);
            var decrypted = Decryption.Decrypt(cipher, keygen.K1, keygen.K2);
            if (Tools.BitArrayEquals(decrypted, plain))
                return key;
            return null;
        }
    }
}
