using System;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using System.Security.Cryptography.RNGCryptoServiceProvider.
    /// </summary>
    public class CryptoRNG : IRandomNumberGenerator
    {
        protected System.Security.Cryptography.RNGCryptoServiceProvider rng;

        public CryptoRNG()
        {
            rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        }

        double IRandomNumberGenerator.NextDouble()
        {
            byte[] bytes = new byte[8];

            rng.GetBytes(bytes);

            System.Collections.BitArray bitArray = new System.Collections.BitArray(bytes);

            // Set all exponent bits to 0
            for (int i = 52; i < 62; i++)
                bitArray.Set(i, false);

            // Except for the 1's
            bitArray.Set(62, true);

            // Set sign bit to positive
            bitArray.Set(63, false);

            bitArray.CopyTo(bytes, 0);

            double value = BitConverter.ToDouble(bytes, 0);

            return value - (int)value;
        }

        int IRandomNumberGenerator.NextInt(int min, int max)
        {
            if (min == max)
                return min;

            return (int)Math.Round(((IRandomNumberGenerator)this).NextDouble() * (max - min - 1)) + min;
        }

        void IRandomNumberGenerator.NextBytes(byte[] data)
        {
            rng.GetBytes(data);
        }
    }
}
