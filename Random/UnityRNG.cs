using System;

namespace Expanse
{
    /// <summary>
    /// Generates random numbers using UnityEngine.Random.
    /// </summary>
    public class UnityRNG : IRandomNumberGenerator
    {
        protected UnityEngine.Random.State rngState;

        public UnityRNG() : this(Environment.TickCount) { }

        public UnityRNG(int seed)
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;
        }

        double IRandomNumberGenerator.NextDouble()
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            double value = (double)(decimal)UnityEngine.Random.value;

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;

            return value;
        }

        int IRandomNumberGenerator.NextInt(int min, int max)
        {
            UnityEngine.Random.State prevState = UnityEngine.Random.state;
            UnityEngine.Random.state = rngState;

            int value = UnityEngine.Random.Range(min, max);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;

            return value;
        }

        void IRandomNumberGenerator.NextBytes(byte[] data)
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
