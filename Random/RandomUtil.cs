using System;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Thread-safe static utility methods for Expanse.Random.
    /// </summary>
    public static class RandomUtil
    {
        private static Random instance = new Random();

        private readonly static object @lock = new object();

        /// <summary>
        /// Global static instance of Random.
        /// </summary>
        public static Random Instance
        {
            get { return instance; }
            set
            {
                lock (@lock)
                {
                    instance = value;
                }
            }
        }

        /// <summary>
        /// Returns a random integer between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>Returns a random integer between 0 and Int32.MaxValue.</returns>
        public static int Int()
        {
            lock (@lock)
            {
                return instance.Int();
            }
        }

        /// <summary>
        /// Returns a random integer between 0 and max. (Exclusive upper bound)
        /// </summary>
        /// <param name="max">Maximum range of the int.</param>
        /// <returns>Returns a random integer between 0 and max. (Exclusive upper bound)</returns>
        public static int Int(int max)
        {
            lock (@lock)
            {
                return instance.Int(max);
            }
        }

        /// <summary>
        /// Returns a random integer between min and max. (Exclusive upper bound)
        /// </summary>
        /// <param name="a">Min/Max range of the int.</param>
        /// <param name="b">Min/Max range of the int.</param>
        /// <returns>Returns a random integer between min and max. (Exclusive upper bound)</returns>
        public static int Int(int min, int max)
        {
            lock (@lock)
            {
                return instance.Int(min, max);
            }
        }

        /// <summary>
        /// Returns a random double between 0 and 1.
        /// </summary>
        /// <returns>Returns a random double between 0 and 1.</returns>
        public static double Double()
        {
            lock (@lock)
            {
                return instance.Double();
            }
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        /// <param name="max">Maximum range of the double.</param>
        /// <returns>Returns a random double between 0 and max.</returns>
        public static double Double(double max)
        {
            lock (@lock)
            {
                return instance.Double(max);
            }
        }

        /// <summary>
        /// Returns a random double between a and b.
        /// </summary>
        /// <param name="a">Min/Max range of the double.</param>
        /// <param name="b">Min/Max range of the double.</param>
        /// <returns>Returns a random double between a and b.</returns>
        public static double Double(double a, double b)
        {
            lock (@lock)
            {
                return instance.Double(a, b);
            }
        }

        /// <summary>
        /// Returns a byte with randomly set bits.
        /// </summary>
        /// <returns>Returns a byte with randomly set bits.</returns>
        public static byte Byte()
        {
            lock (@lock)
            {
                return instance.Byte();
            }
        }

        /// <summary>
        /// Returns a byte array with randomly set bits.
        /// </summary>
        /// <param name="length">Length of the byte array to create.</param>
        /// <returns>Returns a byte array with randomly set bits.</returns>
        public static byte[] Bytes(int length = 1)
        {
            lock (@lock)
            {
                return instance.Bytes(length);
            }
        }

        /// <summary>
        /// Randomly sets the bits in a given byte array.
        /// </summary>
        /// <param name="data">Source byte array to set.</param>
        public static void Bytes(byte[] data)
        {
            lock (@lock)
            {
                instance.Bytes(data);
            }
        }

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <returns>Returns a random bool.</returns>
        public static bool Bool()
        {
            lock (@lock)
            {
                return instance.Bool();
            }
        }

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <param name="chance">Chance between 0 and 1 for a True.</param>
        /// <returns>Returns a random bool.</returns>
        public static bool Bool(float chance)
        {
            lock (@lock)
            {
                return instance.Bool(chance);
            }
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        /// <returns>Returns a random float between 0 and 1.</returns>
        public static float Float()
        {
            lock (@lock)
            {
                return instance.Float();
            }
        }

        /// <summary>
        /// Returns a random float between 0 and max.
        /// </summary>
        /// <param name="max">Maximum range of the float.</param>
        /// <returns>Returns a random float between 0 and max.</returns>
        public static float Float(float max)
        {
            lock (@lock)
            {
                return instance.Float(max);
            }
        }

        /// <summary>
        /// Returns a random float between a and b.
        /// </summary>
        /// <param name="a">Min/Max range of the float.</param>
        /// <param name="b">Min/Max range of the float.</param>
        /// <returns>Returns a random float between a and b.</returns>
        public static float Float(float a, float b)
        {
            lock (@lock)
            {
                return instance.Float(a, b);
            }
        }

        /// <summary>
        /// Returns either 1 or -1.
        /// </summary>
        /// <returns>Returns either 1 or -1.</returns>
        public static int Sign()
        {
            lock (@lock)
            {
                return instance.Sign();
            }
        }

        /// <summary>
        /// Returns a random normalized Vector2.
        /// </summary>
        /// <returns>Returns a random normalized Vector2.</returns>
        public static Vector2 Vector2()
        {
            lock (@lock)
            {
                return instance.Vector2();
            }
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        /// <param name="maxMagnitude">Maximum magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between 0 and maxMagnitude.</returns>
        public static Vector2 Vector2(float maxMagnitude)
        {
            lock (@lock)
            {
                return instance.Vector2(maxMagnitude);
            }
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        /// <param name="aMagnitude">Min/Max magnitude for the Vector.</param>
        /// <param name="bMagnitude">Min/Max magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between aMagnitude and bMagnitude.</returns>
        public static Vector2 Vector2(float aMagnitude, float bMagnitude)
        {
            lock (@lock)
            {
                return instance.Vector2(aMagnitude, bMagnitude);
            }
        }

        /// <summary>
        /// Returns a random normalized Vector3.
        /// </summary>
        /// <returns>Returns a random normalized Vector3.</returns>
        public static Vector3 Vector3()
        {
            lock (@lock)
            {
                return instance.Vector3();
            }
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        /// <param name="maxMagnitude">Maximum magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between 0 and maxMagnitude.</returns>
        public static Vector3 Vector3(float maxMagnitude)
        {
            lock (@lock)
            {
                return instance.Vector3(maxMagnitude);
            }
        }

        /// <summary>
        /// Returns a random Vector3 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        /// <param name="aMagnitude">Min/Max magnitude for the Vector.</param>
        /// <param name="bMagnitude">Min/Max magnitude for the Vector.</param>
        /// <returns>Returns a random Vector3 with a magnitude between aMagnitude and bMagnitude.</returns>
        public static Vector3 Vector3(float aMagnitude, float bMagnitude)
        {
            lock (@lock)
            {
                return instance.Vector3(aMagnitude, bMagnitude);
            }
        }

        /// <summary>
        /// Returns a random normalized Vector4.
        /// </summary>
        /// <returns>Returns a random normalized Vector4.</returns>
        public static Vector4 Vector4()
        {
            lock (@lock)
            {
                return instance.Vector4();
            }
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        /// <param name="maxMagnitude">Maximum magnitude for the Vector.</param>
        /// <returns>Returns a random Vector2 with a magnitude between 0 and maxMagnitude.</returns>
        public static Vector4 Vector4(float maxMagnitude)
        {
            lock (@lock)
            {
                return instance.Vector4(maxMagnitude);
            }
        }

        /// <summary>
        /// Returns a random Vector4 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        /// <param name="aMagnitude">Min/Max magnitude for the Vector.</param>
        /// <param name="bMagnitude">Min/Max magnitude for the Vector.</param>
        /// <returns>Returns a random Vector4 with a magnitude between aMagnitude and bMagnitude.</returns>
        public static Vector4 Vector4(float aMagnitude, float bMagnitude)
        {
            lock (@lock)
            {
                return instance.Vector4(aMagnitude, bMagnitude);
            }
        }

        /// <summary>
        /// Returns a random color.
        /// </summary>
        /// <returns>Returns a random color.</returns>
        public static Color Color()
        {
            lock (@lock)
            {
                return instance.Color();
            }
        }

        /// <summary>
        /// Returns a random color with an alpha.
        /// </summary>
        /// <param name="alpha">Alpha value to apply to the color.</param>
        /// <returns>Returns a random color with an alpha.</returns>
        public static Color Color(float alpha)
        {
            lock (@lock)
            {
                return instance.Color(alpha);
            }
        }

        /// <summary>
        /// Returns a random element in a list.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="list">Source list of elements.</param>
        /// <returns>Returns a random element in a list.</returns>
        public static T Element<T>(IList<T> list)
        {
            lock (@lock)
            {
                return instance.Element(list);
            }
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
        public static T WeightedElement<T>(IList<T> list, Func<T, float> weightSelector)
        {
            lock (@lock)
            {
                return instance.WeightedElement(list, weightSelector);
            }
        }

        /// <summary>
        /// Suffles the element order of a list.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="list">Source list of elements.</param>
        public static void Shuffle<T>(IList<T> list)
        {
            lock (@lock)
            {
                instance.Shuffle(list);
            }
        }
    }
}
