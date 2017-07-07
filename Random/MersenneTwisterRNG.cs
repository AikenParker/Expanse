using System;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers using the Mersenne Twister algorithm.
    /// </summary>
    /// <see cref=">https://en.wikipedia.org/wiki/Mersenne_Twister"/>
    public class MersenneTwisterRNG : IRNG
    {
        /// <summary>
        /// Creates a new Random wrapper using MersenneTwisterRNG.
        /// </summary>
        /// <returns>Returns a new Random wrapper using MersenneTwisterRNG.</returns>
        public static RNG CreateNew()
        {
            return new RNG(new MersenneTwisterRNG());
        }

        /// <summary>
        /// Creates a new Random wrapper using MersenneTwisterRNG.
        /// </summary>
        /// <returns>Returns a new Random wrapper using MersenneTwisterRNG.</returns>
        public static RNG CreateNew(ulong seed)
        {
            return new RNG(new MersenneTwisterRNG(seed));
        }

        private const int W = 64;
        private const ulong N = 312;
        private const ulong M = 156;
        private const ulong R = 31;
        private const ulong A = 0xB5026F5AA96619E9;
        private const int U = 29;
        private const ulong D = 0x5555555555555555;
        private const int S = 17;
        private const ulong B = 0x71D67FFFEDA60000;
        private const int T = 37;
        private const ulong C = 0xFFF7EEE000000000;
        private const int L = 43;
        private const ulong F = 6364136223846793005;

        private const ulong LOWER_MASK = 0x7FFFFFFF;
        private const ulong UPPER_MASK = ~LOWER_MASK;

        private ulong[] mersenneTwister = new ulong[N];
        private ulong index = N + 1;

        public MersenneTwisterRNG() : this((ulong)Environment.TickCount) { }

        public MersenneTwisterRNG(ulong seed)
        {
            index = N;
            mersenneTwister[0] = seed;

            for (ulong i = 1; i < N; ++i)
            {
                mersenneTwister[i] = (F * (mersenneTwister[i - 1] ^ (mersenneTwister[i - 1] >> (W - 2))) + i);
            }
        }

        /// <summary>
        /// Generates the next unsigned long value from the random number generator.
        /// </summary>
        /// <returns>Returns the next unsigned long value from the random number generator.</returns>
        public ulong NextUlong()
        {
            if (index >= N)
            {
                if (index > N)
                {
                    throw new Exception("Generator was never seeded");
                }

                Twist();
            }

            ulong y = mersenneTwister[index];

            y = y ^ ((y >> U) & D);
            y = y ^ ((y << S) & B);
            y = y ^ ((y << T) & C);
            y = y ^ (y >> L);

            ++index;

            return y;
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

            for (int i = 0; i < dataLength;)
            {
                ulong value = NextUlong();

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 0)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 1)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 2)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 3)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 4)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 5)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 6)) & 0xFF);

                if (i < dataLength)
                    data[i++] = (byte)((value >> (8 * 7)) & 0xFF);
            }
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            ulong value = NextUlong();

            return (value / (1 << 11)) / (double)(1UL << 0x35);
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            unchecked
            {
                int value = (int)NextUlong();

                if (value < 0)
                    value *= -1;

                return value;
            }
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

            unchecked
            {
                int value = (int)NextUlong();

                if (value < 0 && max > 0 || value > 0 && max < 0)
                    value *= -1;

                return value % max;
            }
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

            unchecked
            {
                int value = (int)NextUlong();

                if (value < 0 && diff > 0 || value > 0 && diff < 0)
                    value *= -1;

                return (value % range) + min;
            }
        }

        // Resets the state of the random number generator.
        private void Twist()
        {
            for (ulong i = 0; i < N; ++i)
            {
                ulong x = (mersenneTwister[i] & UPPER_MASK) + (mersenneTwister[(i + 1) % N] & LOWER_MASK);
                ulong xA = x >> 1;

                if (x % 2 != 0)
                {
                    xA = xA ^ A;
                }

                mersenneTwister[i] = mersenneTwister[(i + M) % N] ^ xA;
            }

            index = 0;
        }
    }
}
