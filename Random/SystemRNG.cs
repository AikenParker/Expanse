using System;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers using System.Random.
    /// </summary>
    public class SystemRNG : IRNG
    {
        /// <summary>
        /// Creates a new Random wrapper using SystemRNG.
        /// </summary>
        /// <returns>Returns a new Random wrapper using SystemRNG.</returns>
        public static RNG CreateNew()
        {
            return new RNG(new SystemRNG());
        }

        /// <summary>
        /// Creates a new Random wrapper using SystemRNG.
        /// </summary>
        /// <param name="seed">Seed value to set the RNG to.</param>
        /// <returns>Returns a new Random wrapper using SystemRNG.</returns>
        public static RNG CreateNew(int seed)
        {
            return new RNG(new SystemRNG(seed));
        }

        protected System.Random rng;

        public SystemRNG() : this(Environment.TickCount) { }

        public SystemRNG(int seed)
        {
            rng = new System.Random(seed);
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            return rng.NextDouble();
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            return rng.Next();
        }

        /// <summary>
        /// Generates the next random int value between 0 and max.
        /// </summary>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Returns the next random int value between 0 and max.</returns>
        public int NextInt(int max)
        {
            return rng.Next(max);
        }

        /// <summary>
        /// Generates the next random int value between min and max.
        /// </summary>
        /// <param name="min">Maximum specified int value. (Inclusive)</param>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Generates the next random int value between min and max.</returns>
        public int NextInt(int min, int max)
        {
            return rng.Next(min, max);
        }

        /// <summary>
        /// Generates the next byte values from the random number generator.
        /// </summary>
        /// <param name="data">Byte array to set the random bytes into.</param>
        public void NextBytes(byte[] data)
        {
            rng.NextBytes(data);
        }
    }
}
