using System;
using Expanse.Extensions;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers using System.Security.Cryptography.RNGCryptoServiceProvider.
    /// </summary>
    public class CryptoRNG : IRNG
    {
        protected readonly byte[] byteCache32 = new byte[4];
        protected readonly byte[] byteCache64 = new byte[8];

        /// <summary>
        /// Creates a new Random wrapper using CryptoRNG.
        /// </summary>
        public static RNG CreateNew()
        {
            return new RNG(new CryptoRNG());
        }

        protected System.Security.Cryptography.RNGCryptoServiceProvider rng;

        public CryptoRNG()
        {
            rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            rng.GetBytes(byteCache64);

            ulong @ulong = BitConverter.ToUInt64(byteCache64, 0) / (1 << 11);

            double value = @ulong / (double)(1UL << 0x35);

            return value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            rng.GetBytes(byteCache32);

            byteCache32.SetBit(31, false);

            return BitConverter.ToInt32(byteCache32, 0);
        }

        /// <summary>
        /// Generates the next random int value between 0 and max.
        /// </summary>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Returns the next random int value between 0 and max.</returns>
        public int NextInt(int max)
        {
            if (max == 0)
                return 0;

            rng.GetBytes(byteCache32);

            byteCache32.SetBit(31, max < 0);

            int value = BitConverter.ToInt32(byteCache32, 0) % max;

            return value;
        }

        /// <summary>
        /// Generates the next random int value between min and max.
        /// </summary>
        /// <param name="min">Maximum specified int value. (Inclusive)</param>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Generates the next random int value between min and max.</returns>
        public int NextInt(int min, int max)
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

        /// <summary>
        /// Generates the next byte values from the random number generator.
        /// </summary>
        /// <param name="data">Byte array to set the random bytes into.</param>
        public void NextBytes(byte[] data)
        {
            rng.GetBytes(data);
        }
    }
}
