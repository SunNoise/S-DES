using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES
{
    static class Encryption
    {
        internal static BitArray Encrypt(BitArray input, BitArray k1, BitArray k2)
        {
            var IP = Tools.Permute(input, "2,6,3,1,4,8,5,7");
            var inputSplit = Tools.SplitInHalf(IP);
            FunctionK(inputSplit, k1);
            var swappedInput = Tools.Swap(inputSplit);
            FunctionK(swappedInput, k2);
            var FkResult = Tools.Join(swappedInput, 8);
            var IP_1 = Tools.Permute(FkResult, "4,1,3,5,7,2,8,6");
            return IP_1;
        }

        internal static BitArray SubBoxes(BitArray input, char[,] matrix)
        {
            var permuted = Tools.Permute(input, "1,4,2,3");
            var inputSplit = Tools.SplitInHalf(permuted);
            int[] sides = new int[2];
            for(int x = 0; x < sides.Length; x++)
            {
                sides[x] = Tools.BitArrayToInt(inputSplit[x]);
            }
            int res = (int)Char.GetNumericValue(matrix[sides[0], sides[1]]);
            string result = Convert.ToString(res, 2).PadLeft(2, '0');
            return Tools.StringToBitArray(result);
        }

        internal static void FunctionK(BitArray[] input, BitArray key)
        {
            var EP = Tools.Permute(input[1], "4,1,2,3,2,3,4,1");
            EP.Xor(key);
            var xOR_Split = Tools.SplitInHalf(EP);
            var s0 = SubBoxes(xOR_Split[0], new char[,] {  {'1', '0', '3', '2'},
                                                           {'3', '2', '1', '0'},
                                                           {'0', '2', '1', '3'},
                                                           {'3', '1', '3', '2'}});

            var s1 = SubBoxes(xOR_Split[1], new char[,] {  {'0', '1', '2', '3'},
                                                           {'2', '0', '1', '3'},
                                                           {'3', '0', '1', '0'},
                                                           {'2', '1', '0', '3'}});
            var subBoxRes = Tools.Join(new BitArray[] { s0, s1 }, 4);
            var P4 = Tools.Permute(subBoxRes, "2,4,3,1");
            input[0].Xor(P4);
        }
    }
}
