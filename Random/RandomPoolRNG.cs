using System;
using Expanse.Utilities;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers from a pre-filled pool using another RNG.
    /// </summary>
    public class RandomPoolRNG : IRNG
    {
        /// <summary>
        /// Creates a new Random wrapper using RandomPoolRNG.
        /// </summary>
        /// <param name="poolSize">Size in bytes the pool should be.</param>
        /// <returns>Returns a new Random wrapper using RandomPoolRNG.</returns>
        public static RNG CreateNew(int poolSize)
        {
            return new RNG(new RandomPoolRNG(poolSize));
        }

        /// <summary>
        /// Creates a new Random wrapper using RandomPoolRNG.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="poolSize">Size in bytes the pool should be.</param>
        /// <returns>Returns a new Random wrapper using RandomPoolRNG.</returns>
        public static RNG CreateNew(IRNG rng, int poolSize)
        {
            return new RNG(new RandomPoolRNG(rng, poolSize));
        }

        private IRNG rng;
        private int poolSize;
        private byte[] pool;
        private int index;

        private RandomPoolRNG() { }

        /// <summary>
        /// Creates a RandomPoolRNG instance using CryptoRNG-seed-supplied-XorShiftRNG as the pool filler.
        /// </summary>
        /// <param name="poolSize">Size in bytes the pool should be.</param>
        public RandomPoolRNG(int poolSize)
        {
            CryptoRNG cryptoRNG = new CryptoRNG();

            this.rng = new XorShiftRNG(cryptoRNG);
            this.poolSize = poolSize;
            this.pool = new byte[poolSize];

            FillPool();
        }

        /// <summary>
        /// Creates a RandomPoolRNG instance.
        /// </summary>
        /// <param name="rng">RNG used to fill the pool.</param>
        /// <param name="poolSize">Size in bytes the pool should be.</param>
        public RandomPoolRNG(IRNG rng, int poolSize)
        {
            if (rng == null)
                throw new ArgumentNullException("rng");

            if (poolSize < sizeof(double))
                throw new InvalidArgumentException("poolSize must be greater than " + sizeof(double));

            this.rng = rng;
            this.poolSize = poolSize;
            this.pool = new byte[poolSize];

            FillPool();
        }

        // Refills the pool with random byte data from RNG
        private void FillPool()
        {
            rng.NextBytes(pool);
        }

        /// <summary>
        /// Generates the next byte values from the random number generator.
        /// </summary>
        /// <param name="data">Byte array to set the random bytes into.</param>
        public void NextBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int dataLength = data.Length;

            // TODO: Allow data length of any size to work (Potential refill mid-way)

            if (dataLength > poolSize)
                throw new InvalidArgumentException("data Length must be less than poolSize");

            if (index + dataLength > poolSize)
            {
                FillPool();
                index = 0;
            }
#if UNSAFE
            unsafe
            {
                fixed (byte* poolPtr = pool)
                {
                    for (int i = 0; i < dataLength; i++)
                    {
                        data[i] = poolPtr[index + i];
                        index++;
                    }
                }
            }
#else
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = pool[index + i];
                index++;
            }
#endif
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            if (index + sizeof(double) > poolSize)
            {
                FillPool();
                index = 0;
            }

            ulong @ulong;

#if UNSAFE
            unsafe
            {
                fixed (byte* poolPtr = pool)
                    @ulong = *(ulong*)&poolPtr[index];
            }
#else
            @ulong = BitConverter.ToUInt64(pool, index);
#endif

            index += sizeof(double);

            double value = @ulong / (double)ulong.MaxValue;

            return value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            if (index + sizeof(int) > poolSize)
            {
                FillPool();
                index = 0;
            }

            int value;

#if UNSAFE
            unsafe
            {
                fixed (byte* poolPtr = pool)
                    value = *(int*)&poolPtr[index];
            }
#else
            value = BitConverter.ToInt32(pool, index);
#endif

            if (value < 0)
                value *= -1;

            index += sizeof(int);

            return value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and max.
        /// </summary>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Returns the next random int value between 0 and max.</returns>
        public int NextInt(int max)
        {
            if (max == 0)
                return 0;

            return NextInt() % max;
        }

        /// <summary>
        /// Generates the next random int value between min and max.
        /// </summary>
        /// <param name="min">Maximum specified int value. (Inclusive)</param>
        /// <param name="max">Maximum specified int value. (Exclusive)</param>
        /// <returns>Generates the next random int value between min and max.</returns>
        public int NextInt(int min, int max)
        {
            int diff = max - min;
            int mask = diff >> 31;
            int range = (mask ^ diff) - mask;

            if (range == 0)
                return min;

            int value = (NextInt(diff) % range) + min;

            return value;
        }
    }
}
