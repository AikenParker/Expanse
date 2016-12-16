using System;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using System.Random.
    /// </summary>
    public class SystemRNG : IRandomNumberGenerator
    {
        protected System.Random rng;

        public SystemRNG() : this(Environment.TickCount) { }

        public SystemRNG(int seed)
        {
            rng = new System.Random(seed);
        }

        double IRandomNumberGenerator.NextDouble()
        {
            return rng.NextDouble();
        }

        int IRandomNumberGenerator.NextInt(int min, int max)
        {
            return rng.Next(min, max);
        }

        void IRandomNumberGenerator.NextBytes(byte[] data)
        {
            rng.NextBytes(data);
        }
    }
}
