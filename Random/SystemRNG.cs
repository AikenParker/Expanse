using System;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using System.Random.
    /// </summary>
    public class SystemRNG : IRandomNumberGenerator
    {
        /// <summary>
        /// Creates a new Random wrapper using SystemRNG.
        /// </summary>
        public static Random CreateNew()
        {
            return new Random(new SystemRNG());
        }

        /// <summary>
        /// Creates a new Random wrapper using SystemRNG.
        /// </summary>
        public static Random CreateNew(int seed)
        {
            return new Random(new SystemRNG(seed));
        }

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

        int IRandomNumberGenerator.NextInt()
        {
            return rng.Next();
        }

        int IRandomNumberGenerator.NextInt(int max)
        {
            return rng.Next(max);
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
