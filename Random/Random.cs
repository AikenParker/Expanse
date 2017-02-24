using System;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Interface supported RNG wrapper that provides a collection of random related functionality.
    /// </summary>
    public class Random
    {
        protected IRandomNumberGenerator rng;

        /// <summary>
        /// Random utility instance that wraps a UnityRNG instance.
        /// </summary>
        public Random()
        {
            this.rng = new UnityRNG();
        }

        /// <summary>
        /// Random utility instance that wraps an RNG.
        /// </summary>
        /// <param name="rng">UnityRNG (default), SystemRNG, CryptoRNG</param>
        public Random(IRandomNumberGenerator rng)
        {
            this.rng = rng;
        }

        /// <summary>
        /// Returns a random integer between 0 and Int32.MaxValue.
        /// </summary>
        public int Int()
        {
            return rng.NextInt();
        }

        /// <summary>
        /// Returns a random integer between 0 and max. (Exclusive upper bound)
        /// </summary>
        public int Int(int max)
        {
            return rng.NextInt(max);
        }

        /// <summary>
        /// Returns a random integer between min and max. (Exclusive upper bound)
        /// </summary>
        public int Int(int a, int b)
        {
            int min = Mathf.Min(a, b);
            int max = Mathf.Max(a, b);

            return rng.NextInt(min, max);
        }

        /// <summary>
        /// Returns a random double between 0 and 1.
        /// </summary>
        public double Double()
        {
            return rng.NextDouble();
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        public double Double(double max)
        {
            return (Double() * max);
        }

        /// <summary>
        /// Returns a random double between a and b.
        /// </summary>
        public double Double(double a, double b)
        {
            double min = Math.Min(a, b);
            double max = Math.Max(a, b);

            return Double(max - min) + min;
        }

        /// <summary>
        /// Returns a byte array with randomly set bits.
        /// </summary>
        public byte[] Bytes(int length = 1)
        {
            byte[] data = new byte[length];

            rng.NextBytes(data);

            return data;
        }

        /// <summary>
        /// Randomly sets the bits in a given byte array.
        /// </summary>
        public void Bytes(byte[] data)
        {
            rng.NextBytes(data);
        }

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        public bool Bool()
        {
            return Double() < 0.5;
        }

        /// <summary>
        /// Returns a random bool. Chance determines true percentage.
        /// </summary>
        public bool Bool(float chance)
        {
            return Double() < chance;
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        public float Float()
        {
            return (float)Double();
        }

        /// <summary>
        /// Returns a random float between 0 and max.
        /// </summary>
        public float Float(float max)
        {
            return (float)Double(max);
        }

        /// <summary>
        /// Returns a random float between a and b.
        /// </summary>
        public float Float(float a, float b)
        {
            return (float)Double(a, b);
        }

        /// <summary>
        /// Returns either 1 or -1.
        /// </summary>
        public int Sign()
        {
            return Bool() ? 1 : -1;
        }

        /// <summary>
        /// Returns a random normalized Vector2.
        /// </summary>
        public Vector2 Vector2()
        {
            return new Vector2(Float(-1, 1), Float(-1, 1)).normalized;
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        public Vector2 Vector2(float maxMagnitude)
        {
            return new Vector2(Float(-1, 1), Float(-1, 1)).normalized * Float(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        public Vector2 Vector2(float aMagnitude, float bMagnitude)
        {
            float minMagnitude = Mathf.Min(aMagnitude, bMagnitude);
            float maxMagnitude = Mathf.Max(aMagnitude, bMagnitude);

            return new Vector2(Float(-1, 1), Float(-1, 1)).normalized * Float(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Returns a random normalized Vector3.
        /// </summary>
        public Vector3 Vector3()
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized;
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        public Vector3 Vector3(float maxMagnitude)
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector3 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        public Vector3 Vector3(float aMagnitude, float bMagnitude)
        {
            float minMagnitude = Mathf.Min(aMagnitude, bMagnitude);
            float maxMagnitude = Mathf.Max(aMagnitude, bMagnitude);

            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Returns a random normalized Vector4.
        /// </summary>
        public Vector4 Vector4()
        {
            return new Vector4(Float(-1, 1), Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized;
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        public Vector4 Vector4(float maxMagnitude)
        {
            return new Vector4(Float(-1, 1), Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector4 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        public Vector4 Vector4(float aMagnitude, float bMagnitude)
        {
            float minMagnitude = Mathf.Min(aMagnitude, bMagnitude);
            float maxMagnitude = Mathf.Max(aMagnitude, bMagnitude);

            return new Vector4(Float(-1, 1), Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Returns a random color.
        /// </summary>
        public Color Color()
        {
            return new Color(Float(), Float(), Float(), Float());
        }

        /// <summary>
        /// Returns a random color with an alpha.
        /// </summary>
        public Color Color(float alpha)
        {
            return new Color(Float(), Float(), Float(), alpha);
        }

        /// <summary>
        /// Returns a random element in an array.
        /// </summary>
        public T Element<T>(T[] array)
        {
            return array[Int(array.Length)];
        }

        /// <summary>
        /// Returns a random element in a list.
        /// </summary>
        public T Element<T>(IList<T> list)
        {
            return list[Int(list.Count)];
        }

        /// <summary>
        /// Shuffles the element order of an array.
        /// </summary>
        public void Shuffle<T>(T[] array)
        {
            int n = array.Length;

            while (n > 1)
            {
                int k = Int(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        /// <summary>
        /// Suffles the element order of a list.
        /// </summary>
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                int k = Int(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
    }
}
