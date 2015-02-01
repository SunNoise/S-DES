using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES
{
    static class Decryption
    {
        //same as encryption, but you switch the keys
        internal static BitArray Decrypt(BitArray input, BitArray k1, BitArray k2)
        {
            var IP = Tools.Permute(input, "2,6,3,1,4,8,5,7");
            var inputSplit = Tools.SplitInHalf(IP);
            Encryption.FunctionK(inputSplit, k2);
            var swappedInput = Tools.Swap(inputSplit);
            Encryption.FunctionK(swappedInput, k1);
            var FkResult = Tools.Join(swappedInput, 8);
            var IP_1 = Tools.Permute(FkResult, "4,1,3,5,7,2,8,6");
            return IP_1;
        }
    }
}
