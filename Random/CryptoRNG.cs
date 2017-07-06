using System;
using Expanse.Extensions;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using System.Security.Cryptography.RNGCryptoServiceProvider.
    /// </summary>
    public class CryptoRNG : IRandomNumberGenerator
    {
        protected readonly byte[] byteCache32 = new byte[4];
        protected readonly byte[] byteCache64 = new byte[8];

        /// <summary>
        /// Creates a new Random wrapper using CryptoRNG.
        /// </summary>
        public static Random CreateNew()
        {
            return new Random(new CryptoRNG());
        }

        protected System.Security.Cryptography.RNGCryptoServiceProvider rng;

        public CryptoRNG()
        {
            rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        }

        double IRandomNumberGenerator.NextDouble()
        {
            rng.GetBytes(byteCache64);

            ulong @ulong = BitConverter.ToUInt64(byteCache64, 0) / (1 << 11);

            double value = @ulong / (double)(1UL << 0x35);

            return value;
        }

        int IRandomNumberGenerator.NextInt()
        {
            rng.GetBytes(byteCache32);

            byteCache32.SetBit(31, false);

            return BitConverter.ToInt32(byteCache32, 0);
        }

        int IRandomNumberGenerator.NextInt(int max)
        {
            if (max == 0)
                return 0;

            rng.GetBytes(byteCache32);

            byteCache32.SetBit(31, max < 0);

            int value = BitConverter.ToInt32(byteCache32, 0) % max;

            return value;
        }

        int IRandomNumberGenerator.NextInt(int min, int max)
        {
            int diff = max - min;
            int mask = diff >> 31;
            int range = (mask ^ diff) - mask;

            if (range == 0)
                return min;

            rng.GetBytes(byteCache32);

            byteCache32.SetBit(31, diff < 0);

            int value = BitConverter.ToInt32(byteCache32, 0) % range;

            return value + min;
        }

        void IRandomNumberGenerator.NextBytes(byte[] data)
        {
            rng.GetBytes(data);
        }
    }
}
