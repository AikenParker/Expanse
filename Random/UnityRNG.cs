using System;
using System.Collections;

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

            // Convert to string first because implicit float to double conversion
            // introduces some unexpected inaccuracies (e.g 0.1f + 0.1f = 0.20000000123f)
            // TODO: Create a bit util to zero the specific bits when need off
            double value = Convert.ToDouble(UnityEngine.Random.value.ToString());

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

            BitArray bitArray = new BitArray(data);

            // TODO: Benchmark this. Consider cloning the bits from UnityEngine.Random.value
            for (int i = 0; i < bitArray.Count; i++)
            {
                bitArray.Set(i, UnityEngine.Random.value < 0.5f ? false : true);
            }

            bitArray.CopyTo(data, 0);

            rngState = UnityEngine.Random.state;
            UnityEngine.Random.state = prevState;
        }
    }
}
