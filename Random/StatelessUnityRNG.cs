using Expanse.Extensions;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using UnityEngine.Random without saving states.
    /// </summary>
    public class StatelessUnityRNG : IRandomNumberGenerator
    {
        /// <summary>
        /// Creates a new Random wrapper using StatelessUnityRNG.
        /// </summary>
        public static Random CreateNew()
        {
            return new Random(new StatelessUnityRNG());
        }

        double IRandomNumberGenerator.NextDouble()
        {
            return UnityEngine.Random.value;
        }

        int IRandomNumberGenerator.NextInt()
        {
            return UnityEngine.Random.Range(0, int.MaxValue);
        }

        int IRandomNumberGenerator.NextInt(int max)
        {
            return UnityEngine.Random.Range(0, max);
        }

        int IRandomNumberGenerator.NextInt(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        void IRandomNumberGenerator.NextBytes(byte[] data)
        {
            for (int i = 0; i < data.Length * 8; i++)
            {
                data.SetBit(i, UnityEngine.Random.value < 0.5f ? false : true);
            }
        }
    }
}
