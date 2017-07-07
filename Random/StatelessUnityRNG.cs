using Expanse.Extensions;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers using UnityEngine.Random without saving states.
    /// </summary>
    public class StatelessUnityRNG : IRNG
    {
        /// <summary>
        /// Creates a new Random wrapper using StatelessUnityRNG.
        /// </summary>
        /// <returns>Returns a new Random wrapper using StatelessUnityRNG.</returns>
        public static RNG CreateNew()
        {
            return new RNG(new StatelessUnityRNG());
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            return UnityEngine.Random.value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            return UnityEngine.Random.Range(0, int.MaxValue);
        }

        /// <summary>
        /// Generates the next random int value between 0 and max.
        /// </summary>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Returns the next random int value between 0 and max.</returns>
        public int NextInt(int max)
        {
            return UnityEngine.Random.Range(0, max);
        }

        /// <summary>
        /// Generates the next random int value between min and max.
        /// </summary>
        /// <param name="min">Maximum specified int value. (Inclusive)</param>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Generates the next random int value between min and max.</returns>
        public int NextInt(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// Generates the next byte values from the random number generator.
        /// </summary>
        /// <param name="data">Byte array to set the random bytes into.</param>
        public void NextBytes(byte[] data)
        {
            for (int i = 0; i < data.Length * 8; i++)
            {
                data.SetBit(i, UnityEngine.Random.value < 0.5f ? false : true);
            }
        }
    }
}
