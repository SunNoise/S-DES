using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES
{
    class KeyGeneration
    {
        internal BitArray K1 { get { return k1; } }
        internal BitArray K2 { get { return k2; } }

        private BitArray k1, k2;
        public KeyGeneration(string key)
        {
            BitArray keyArray = Tools.StringToBitArray(key);
            var P10 = Tools.Permute(keyArray, "3,5,2,7,4,10,1,9,8,6");
            BitArray P10_LS_1, P10_LS_2;
            k1 = GetKey(P10, out P10_LS_1, 1);
            k2 = GetKey(P10_LS_1, out P10_LS_2, 2);
        }

        private BitArray GetKey(BitArray input, out BitArray output, int shifts)
        {
            var inputSplit = Tools.SplitInHalf(input);
            for (int x = 0; x < inputSplit.Length; x++)
            {
                inputSplit[x] = Tools.LeftShift(shifts, inputSplit[x]);
            }
            output = Tools.Join(inputSplit, 10);

            return Tools.Permute(output, "6,3,7,4,8,5,10,9");
        }
    }
}
