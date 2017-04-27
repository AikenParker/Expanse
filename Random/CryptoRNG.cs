using System;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using System.Security.Cryptography.RNGCryptoServiceProvider.
    /// </summary>
    public class CryptoRNG : IRandomNumberGenerator
    {
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
            byte[] bytes = new byte[8];

            rng.GetBytes(bytes);

            ulong @ulong = BitConverter.ToUInt64(bytes, 0) / (1 << 11);

            double value = @ulong / (double)(1UL << 0x35);

            return value;
        }

        int IRandomNumberGenerator.NextInt()
        {
            byte[] bytes = new byte[4];

            rng.GetBytes(bytes);

            bytes.SetBit(31, false);

            return BitConverter.ToInt32(bytes, 0);
        }

        int IRandomNumberGenerator.NextInt(int max)
        {
            if (max == 0)
                return 0;

            byte[] bytes = new byte[4];

            rng.GetBytes(bytes);

            bytes.SetBit(31, max < 0);

            int value = BitConverter.ToInt32(bytes, 0) % max;

            return value;
        }

        int IRandomNumberGenerator.NextInt(int min, int max)
        {
            int diff = max - min;
            int mask = diff >> 31;
            int range = (mask ^ diff) - mask;

            if (range == 0)
                return min;

            byte[] bytes = new byte[4];

            rng.GetBytes(bytes);

            bytes.SetBit(31, diff < 0);

            int value = BitConverter.ToInt32(bytes, 0) % range;

            return value + min;
        }

        void IRandomNumberGenerator.NextBytes(byte[] data)
        {
            rng.GetBytes(data);
        }
    }
}
