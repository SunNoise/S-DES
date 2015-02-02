using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES
{
    static class Tools
    {
        internal static BitArray Permute(BitArray input, string pos)
        {
            var positions = pos.Split(',');
            BitArray temp = new BitArray(positions.Length, false);
            
            for (int x = 0; x < positions.Length; x++)
            {
                int position = Convert.ToInt32(positions[x]) - 1;
                if (input[position])
                    temp[x] = true;
            }
            return temp;
        }

        internal static BitArray[] SplitInHalf(BitArray input)
        {
            int half = input.Length / 2;
            BitArray leftSide = new BitArray(half, false);
            BitArray rightSide = new BitArray(half, false);

            for (int x = 0; x < input.Length; x++)
            {
                if (x < half) //left side
                {
                    if (input[x])
                        leftSide[x] = true;
                }
                else //right side
                {
                    if (input[x])
                        rightSide[x - half] = true;
                }
            }
            BitArray[] split = new BitArray[2] { leftSide, rightSide };
            return split;
        }

        internal static BitArray LeftShift(int times, BitArray input)
        {
            BitArray temp = new BitArray(input.Length, false);
            times %= temp.Length;
            for (int x = 0; x < temp.Length; x++)
            {
                if (input[x])
                {
                    if (times > x)
                        temp[temp.Length - times + x] = true;
                    else
                        temp[x - times] = true;
                }
            }
            return temp;
        }

        internal static BitArray Join(BitArray[] input, int size)
        {
            BitArray temp = new BitArray(size, false);
            int tracker = 0;
            for (int x = 0; x < input.Length; x++)
            {
                var part = input[x];
                for (int y = 0; y < part.Length; y++)
                {
                    if (part[y])
                        temp[tracker] = true;
                    tracker++;
                }
            }
            return temp;
        }

        internal static BitArray[] Swap(BitArray[] input)
        {
            return new BitArray[2] { input[1], input[0] };
        }

        internal static BitArray StringToBitArray(string input)
        {
            int length = input.Length;
            BitArray bitArray = new BitArray(length, false);
            for (int x = 0; x < length; x++)
            {
                switch (input[x])
                {
                    case '0':
                        bitArray[x] = false;
                        break;
                    case '1':
                        bitArray[x] = true;
                        break;
                }
            }
            return bitArray;
        }

        internal static string BitArrayToString(BitArray input)
        {
            int length = input.Length;
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < length; x++)
            {
                sb.Append(input[x] ? 1 : 0);
            }
            return sb.ToString();
        }

        internal static int BitArrayToInt(BitArray input)
        {
            int value = 0;
            int count = 0;
            for (int i = input.Count-1; i >= 0; i--)
            {
                if (input[i])
                    value += Convert.ToInt16(Math.Pow(2, count));
                count++;
            }

            return value;
        }

        internal static bool BitArrayEquals(BitArray BA1, BitArray BA2)
        {
            if (BA1.Length != BA2.Length)
            {
                return false;
            }

            for (int i = 0; i < BA1.Length; i++)
            {
                if (BA1[i] != BA2[i])
                {
                    // since padding bits are forced to zero in the constructor,
                    //  we can test those for equality just as well and the valid
                    //  bits
                    return false;
                }
            }
            return true;
        }
    }
}
