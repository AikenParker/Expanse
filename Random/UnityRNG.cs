using System;
using Expanse.Extensions;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers using UnityEngine.Random.
    /// </summary>
    public class UnityRNG : IRNG
    {
        /// <summary>
        /// Creates a new Random wrapper using UnityRNG.
        /// </summary>
        /// <returns>Returns a new Random wrapper using UnityRNG.</returns>
        public static RNG CreateNew()
        {
            return new RNG(new UnityRNG());
        }

        /// <summary>
        /// Creates a new Random wrapper using UnityRNG.
        /// </summary>
        public static RNG CreateNew(int seed)
        {
            return new RNG(new UnityRNG(seed));
        }

        protected UnityEngine.Random.State rngState;

        public UnityRNG() : this(Environment.TickCount) { }

        public UnityRNG(int seed)
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            double value = UnityEngine.Random.value;

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;

            return value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            int value = UnityEngine.Random.Range(0, int.MaxValue);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;

            return value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and max.
        /// </summary>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Returns the next random int value between 0 and max.</returns>
        public int NextInt(int max)
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            int value = UnityEngine.Random.Range(0, max);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;

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
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            int value = UnityEngine.Random.Range(min, max);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;

            return value;
        }

        /// <summary>
        /// Generates the next byte values from the random number generator.
        /// </summary>
        /// <param name="data">Byte array to set the random bytes into.</param>
        public void NextBytes(byte[] data)
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            for (int i = 0; i < data.Length * 8; i++)
            {
                data.SetBit(i, UnityEngine.Random.value < 0.5f ? false : true);
            }

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;
        }
    }
}
