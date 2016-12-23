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

            ulong @ulong = BitConverter.ToUInt64(bytes, 0) / (1 << 11);

            double value = @ulong / (double)(1UL << 53);

            return value;
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
