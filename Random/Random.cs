using System;
using System.Collections.Generic;
using Expanse.Utilities;
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
        /// <returns>Returns a random integer between 0 and Int32.MaxValue.</returns>
        public int Int()
        {
            return rng.NextInt();
        }

        /// <summary>
        /// Returns a random integer between 0 and max. (Exclusive upper bound)
        /// </summary>
        /// <param name="max">Maximum range of the int.</param>
        /// <returns>Returns a random integer between 0 and max. (Exclusive upper bound)</returns>
        public int Int(int max)
        {
            return rng.NextInt(max);
        }

        /// <summary>
        /// Returns a random integer between min and max. (Exclusive upper bound)
        /// </summary>
        /// <param name="a">Min/Max range of the int.</param>
        /// <param name="b">Min/Max range of the int.</param>
        /// <returns>Returns a random integer between min and max. (Exclusive upper bound)</returns>
        public int Int(int a, int b)
        {
            int min = Mathf.Min(a, b);
            int max = Mathf.Max(a, b);

            return rng.NextInt(min, max);
        }

        /// <summary>
        /// Returns a random double between 0 and 1.
        /// </summary>
        /// <returns>Returns a random double between 0 and 1.</returns>
        public double Double()
        {
            return rng.NextDouble();
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        /// <param name="max">Maximum range of the double.</param>
        /// <returns>Returns a random double between 0 and max.</returns>
        public double Double(double max)
        {
            return (Double() * max);
        }

        /// <summary>
        /// Returns a random double between a and b.
        /// </summary>
        /// <param name="a">Min/Max range of the double.</param>
        /// <param name="b">Min/Max range of the double.</param>
        /// <returns>Returns a random double between a and b.</returns>
        public double Double(double a, double b)
        {
            double min = Math.Min(a, b);
            double max = Math.Max(a, b);

            return Double(max - min) + min;
        }

        /// <summary>
        /// Returns a byte with randomly set bits.
        /// </summary>
        /// <returns>Returns a byte with randomly set bits.</returns>
        public byte Byte()
        {
            return (byte)Int(0x00, 0xFF);
        }

        /// <summary>
        /// Returns a byte array with randomly set bits.
        /// </summary>
        /// <param name="length">Length of the byte array to create.</param>
        /// <returns>Returns a byte array with randomly set bits.</returns>
        public byte[] Bytes(int length = 1)
        {
            byte[] data = new byte[length];

            rng.NextBytes(data);

            return data;
        }

        /// <summary>
        /// Randomly sets the bits in a given byte array.
        /// </summary>
        /// <param name="data">Source byte array to set.</param>
        public void Bytes(byte[] data)
        {
            rng.NextBytes(data);
        }

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <returns>Returns a random bool.</returns>
        public bool Bool()
        {
            return Double() < 0.5;
        }

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <param name="chance">Chance between 0 and 1 for a True.</param>
        /// <returns>Returns a random bool.</returns>
        public bool Bool(float chance)
        {
            return Double() < chance;
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        /// <returns>Returns a random float between 0 and 1.</returns>
        public float Float()
        {
            return (float)Double();
        }

        /// <summary>
        /// Returns a random float between 0 and max.
        /// </summary>
        /// <param name="max">Maximum range of the float.</param>
        /// <returns>Returns a random float between 0 and max.</returns>
        public float Float(float max)
        {
            return (float)Double(max);
        }

        /// <summary>
        /// Returns a random float between a and b.
        /// </summary>
        /// <param name="a">Min/Max range of the float.</param>
        /// <param name="b">Min/Max range of the float.</param>
        /// <returns>Returns a random float between a and b.</returns>
        public float Float(float a, float b)
        {
            return (float)Double(a, b);
        }

        /// <summary>
        /// Returns either 1 or -1.
        /// </summary>
        /// <returns>Returns either 1 or -1.</returns>
        public int Sign()
        {
            return Bool() ? 1 : -1;
        }

        /// <summary>
        /// Returns a random normalized Vector2.
        /// </summary>
        /// <returns>Returns a random normalized Vector2.</returns>
        public Vector2 Vector2()
        {
            return new Vector2(Float(-1, 1), Float(-1, 1)).normalized;
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        /// <param name="maxMagnitude">Maximum magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between 0 and maxMagnitude.</returns>
        public Vector2 Vector2(float maxMagnitude)
        {
            return new Vector2(Float(-1, 1), Float(-1, 1)).normalized * Float(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        /// <param name="aMagnitude">Min/Max magnitude for the Vector.</param>
        /// <param name="bMagnitude">Min/Max magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between aMagnitude and bMagnitude.</returns>
        public Vector2 Vector2(float aMagnitude, float bMagnitude)
        {
            float minMagnitude = Mathf.Min(aMagnitude, bMagnitude);
            float maxMagnitude = Mathf.Max(aMagnitude, bMagnitude);

            return new Vector2(Float(-1, 1), Float(-1, 1)).normalized * Float(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Returns a random normalized Vector3.
        /// </summary>
        /// <returns>Returns a random normalized Vector3.</returns>
        public Vector3 Vector3()
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized;
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        /// <param name="maxMagnitude">Maximum magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between 0 and maxMagnitude.</returns>
        public Vector3 Vector3(float maxMagnitude)
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector3 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        /// <param name="aMagnitude">Min/Max magnitude for the Vector.</param>
        /// <param name="bMagnitude">Min/Max magnitude for the Vector.</param>
        /// <returns>Returns a random Vector3 with a magnitude between aMagnitude and bMagnitude.</returns>
        public Vector3 Vector3(float aMagnitude, float bMagnitude)
        {
            float minMagnitude = Mathf.Min(aMagnitude, bMagnitude);
            float maxMagnitude = Mathf.Max(aMagnitude, bMagnitude);

            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Returns a random normalized Vector4.
        /// </summary>
        /// <returns>Returns a random normalized Vector4.</returns>
        public Vector4 Vector4()
        {
            return new Vector4(Float(-1, 1), Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized;
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        /// <param name="maxMagnitude">Maximum magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between 0 and maxMagnitude.</returns>
        public Vector4 Vector4(float maxMagnitude)
        {
            return new Vector4(Float(-1, 1), Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector4 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        /// <param name="aMagnitude">Min/Max magnitude for the Vector.</param>
        /// <param name="bMagnitude">Min/Max magnitude for the Vector.</param>
        /// <returns>Returns a random Vector4 with a magnitude between aMagnitude and bMagnitude.</returns>
        public Vector4 Vector4(float aMagnitude, float bMagnitude)
        {
            float minMagnitude = Mathf.Min(aMagnitude, bMagnitude);
            float maxMagnitude = Mathf.Max(aMagnitude, bMagnitude);

            return new Vector4(Float(-1, 1), Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized * Float(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Returns a random color.
        /// </summary>
        /// <returns>Returns a random color.</returns>
        public Color Color()
        {
            int data = Int();

            byte r = (byte)((data >> 24) & 0xFF);
            byte g = (byte)((data >> 16) & 0xFF);
            byte b = (byte)((data >> 08) & 0xFF);
            byte a = (byte)((data >> 00) & 0xFF);

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Returns a random color with an alpha.
        /// </summary>
        /// <param name="alpha">Alpha value to apply to the color.</param>
        /// <returns>Returns a random color with an alpha.</returns>
        public Color Color(float alpha)
        {
            int data = Int();

            byte r = (byte)((data >> 24) & 0xFF);
            byte g = (byte)((data >> 16) & 0xFF);
            byte b = (byte)((data >> 08) & 0xFF);

            return new Color32(r, g, b, (byte)(alpha * 0xFF));
        }

        /// <summary>
        /// Returns a random element in a list.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="list">Source list of elements.</param>
        /// <returns>Returns a random element in a list.</returns>
        public T Element<T>(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int listCount = list.Count;

            if (listCount == 0)
                throw new InvalidArgumentException("list must have at least 1 element");

            return list[Int(listCount)];
        }

        /// <summary>
        /// Returns a weighted random element in a list.
        /// </summary>
        /// <remarks>Weights below 0 are counted as 0.</remarks>
        /// <remarks>If total weight of the list is 0 a random element is returned.</remarks>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="list">Source list of elements.</param>
        /// <param name="weightSelector">Gets a weight value from an element.</param>
        /// <returns>Returns a weighted random element in a list.</returns>
        public T WeightedElement<T>(IList<T> list, Func<T, float> weightSelector)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int listCount = list.Count;

            if (listCount == 0)
                throw new InvalidArgumentException("list must have at least 1 element");

#if UNSAFE
            unsafe
            {
                float* weights = stackalloc float[listCount];
#else
            {
                float[] weights = new float[listCount];
#endif
                float totalWeight = 0;

                for (int i = 0; i < listCount; i++)
                {
                    T item = list[i];
                    float itemWeight = weightSelector(item);

                    if (itemWeight <= 0)
                        itemWeight = 0f;

                    weights[i] = itemWeight;
                    totalWeight += itemWeight;
                }

                if (totalWeight == 0)
                    return Element(list);

                float randomVal = Float(totalWeight);

                float currentWeight = 0f;

                for (int i = 0; i < listCount; i++)
                {
                    currentWeight += weights[i];

                    if (currentWeight >= randomVal)
                        return list[i];
                }

                throw new UnexpectedException("An element should have been returned by now");
            }
        }

        /// <summary>
        /// Suffles the element order of a list.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="list">Source list of elements.</param>
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
