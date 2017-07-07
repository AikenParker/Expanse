using System;
using System.Collections.Generic;
using Expanse.Extensions;

namespace Expanse.Random
{
    /// <summary>
    /// Generates random numbers using the XorShift algorithm.
    /// </summary>
    /// <remarks>
    /// XorShift is extrememly fast at filling large arrays of random bytes.
    /// Only recommened if you use NextBytes() and require large byte arrays to be filled.
    /// Highly recommended to use another RNG to seed this RNG through the constructor.
    /// </remarks>
    /// <see cref=">https://bitbucket.org/rstarkov/demoxorshift/src/002604952fec024be698f8167050b03c85af2f81/Xorshift.cs?at=default&fileviewer=file-view-default"/>
    public class XorShiftRNG : IRNG
    {
        /// <summary>
        /// Creates a new Random wrapper using XorShiftRNG.
        /// </summary>
        /// <param name="seed1">First seed value.</param>
        /// <param name="seed2">Second seed value.</param>
        /// <param name="seed3">Third seed value.</param>
        /// <param name="seed4">Fourth seed value.</param>
        /// <returns>Returns a new Random wrapper using XorShiftRNG.</returns>
        public static RNG CreateNew(uint seed1, uint seed2, uint seed3, uint seed4)
        {
            return new RNG(new XorShiftRNG(seed1, seed2, seed3, seed4));
        }

        /// <summary>
        /// Creates a new Random wrapper using XorShiftRNG.
        /// </summary>
        /// <param name="seeder">RNG used to generate the seed for this RNG.</param>
        /// <returns>Returns a new Random wrapper using XorShiftRNG.</returns>
        public static RNG CreateNew(IRNG seeder)
        {
            return new RNG(new XorShiftRNG(seeder));
        }

        protected readonly byte[] byteCache32 = new byte[4];
        protected readonly byte[] byteCache64 = new byte[8];

        // FillBufferMultipleRequired
        private const int BUFFER_REQ = 16;

        private uint X = 123456789;
        private uint Y = 362436069;
        private uint Z = 521288629;
        private uint W = 88675123;

        private Queue<byte> bytes = new Queue<byte>();
        private int byteCount = 0;

        /// <summary>
        /// Creates a specifically seeded XorShiftRNG instance.
        /// </summary>
        /// <param name="seed1">First seed value.</param>
        /// <param name="seed2">Second seed value.</param>
        /// <param name="seed3">Third seed value.</param>
        /// <param name="seed4">Fourth seed value.</param>
        public XorShiftRNG(uint seed1, uint seed2, uint seed3, uint seed4)
        {
            X = seed1; Y = seed2; Z = seed3; W = seed4;
        }

        /// <summary>
        /// Creates a randomly seeded XorShiftRNG instance.
        /// </summary>
        /// <param name="seeder">RNG used to generate the seed for this RNG.</param>
        public XorShiftRNG(IRNG seeder)
        {
            if (seeder == null)
                throw new ArgumentNullException("seeder");

            seeder.NextBytes(byteCache64);

#if UNSAFE
            unsafe
            {
                fixed (byte* pbytes = byteCache64)
                {
                    X = *(uint*)pbytes;
                    Y = *(uint*)&pbytes[sizeof(uint) * 1];
                }
            }
#else
            X = BitConverter.ToUInt32(byteCache64, 0);
            Y = BitConverter.ToUInt32(byteCache64, 4);
#endif

            seeder.NextBytes(byteCache64);

#if UNSAFE
            unsafe
            {
                fixed (byte* pbytes = byteCache64)
                {
                    Z = *(uint*)pbytes;
                    W = *(uint*)&pbytes[sizeof(uint) * 1];
                }
            }
#else
            Z = BitConverter.ToUInt32(byteCache64, 0);
            W = BitConverter.ToUInt32(byteCache64, 4);
#endif
        }

        /// <summary>
        /// Generates the next byte values from the random number generator.
        /// </summary>
        /// <param name="data">Byte array to set the random bytes into.</param>
        public void NextBytes(byte[] data)
        {
            int offset = 0;

            while (byteCount > 0 && offset < data.Length)
            {
                data[offset++] = bytes.Dequeue();
                byteCount--;
            }

            int length = ((data.Length - offset) / BUFFER_REQ) * BUFFER_REQ;

            if (length > 0)
                FillBuffer(data, offset, offset + length);

            offset += length;

            while (offset < data.Length)
            {
                if (byteCount == 0)
                {
                    uint t = X ^ (X << 11);
                    X = Y; Y = Z; Z = W;
                    W = W ^ (W >> 19) ^ (t ^ (t >> 8));

                    bytes.Enqueue((byte)(W & 0xFF));
                    bytes.Enqueue((byte)((W >> 8) & 0xFF));
                    bytes.Enqueue((byte)((W >> 16) & 0xFF));
                    bytes.Enqueue((byte)((W >> 24) & 0xFF));

                    byteCount += 4;
                }

                data[offset++] = bytes.Dequeue();
                byteCount--;
            }
        }

        /// <summary>
        /// Generates the next random double value between 0 and 1.
        /// </summary>
        /// <returns>Returns the next random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            NextBytes(byteCache64);

            ulong @ulong = BitConverter.ToUInt64(byteCache64, 0) / (1 << 11);

            double value = @ulong / (double)(1UL << 0x35);

            return value;
        }

        /// <summary>
        /// Generates the next random int value between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns the next random int value between 0 and Int32.MaxValue.</returns>
        public int NextInt()
        {
            NextBytes(byteCache32);

            byteCache32.SetBit(31, false);

            return BitConverter.ToInt32(byteCache32, 0);
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

            NextBytes(byteCache32);

            byteCache32.SetBit(31, max < 0);

            int value = BitConverter.ToInt32(byteCache32, 0) % max;

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
            int diff = max - min;
            int mask = diff >> 31;
            int range = (mask ^ diff) - mask;

            if (range == 0)
                return min;

            NextBytes(byteCache32);

            byteCache32.SetBit(31, diff < 0);

            int value = BitConverter.ToInt32(byteCache32, 0) % range;

            return value + min;
        }

        // Fills the byte array with random data
        private void FillBuffer(byte[] buffer, int offset, int offsetEnd)
        {
#if UNSAFE
            unsafe
            {
                fixed (byte* pbytes = buffer)
                {
                    uint* pbuf = (uint*)(pbytes + offset);
                    uint* pend = (uint*)(pbytes + offsetEnd);

                    while (pbuf < pend)
                    {
                        uint tx = X ^ (X << 11);
                        uint ty = Y ^ (Y << 11);
                        uint tz = Z ^ (Z << 11);
                        uint tw = W ^ (W << 11);

                        *(pbuf++) = X = W ^ (W >> 19) ^ (tx ^ (tx >> 8));
                        *(pbuf++) = Y = X ^ (X >> 19) ^ (ty ^ (ty >> 8));
                        *(pbuf++) = Z = Y ^ (Y >> 19) ^ (tz ^ (tz >> 8));
                        *(pbuf++) = W = Z ^ (Z >> 19) ^ (tw ^ (tw >> 8));
                    }
                }
            }
#else
            while (offset < offsetEnd)
            {
                uint t = X ^ (X << 11);
                X= Y; Y = Z; Z = W;
                W = W ^ (W >> 19) ^ (t ^ (t >> 8));

                buffer[offset++] = (byte)(W & 0xFF);
                buffer[offset++] = (byte)((W >> 8) & 0xFF);
                buffer[offset++] = (byte)((W >> 16) & 0xFF);
                buffer[offset++] = (byte)((W >> 24) & 0xFF);
            }
#endif
        }
    }
}
