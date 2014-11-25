using System;

namespace PEG.Utils
{
    public static class BitShift
    {
        public static long ShiftLeft(int shift)
        {
            return (long)Math.Pow(2, shift);
        }

        public static bool ShiftRight(this long value)
        {
            return Math.Log(value, 2) == 1;
        }

        public static readonly ulong[] ByteMasks = new ulong[8];

        static BitShift()
        {
            for (int i = 0; i < 8; i++)
            {
                ulong value = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (i != j)
                    {
                        value |= (ulong)Byte.MaxValue << (j * 8);
                    }
                }
                ByteMasks[i] = value;
            }
        }
    }
}