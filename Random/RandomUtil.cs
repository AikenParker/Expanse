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
        public static bool Bool()
        {
            lock (@lock)
            {
                return instance.Bool();
            }
        }

        /// <summary>
        /// Returns a random bool. Chance determines true percentage.
        /// </summary>
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
        public static Color Color(float alpha)
        {
            lock (@lock)
            {
                return instance.Color(alpha);
            }
        }

        /// <summary>
        /// Returns a random element in an array.
        /// </summary>
        public static T Element<T>(T[] array)
        {
            lock (@lock)
            {
                return instance.Element(array);
            }
        }

        /// <summary>
        /// Returns a random element in a list.
        /// </summary>
        public static T Element<T>(IList<T> list)
        {
            lock (@lock)
            {
                return instance.Element(list);
            }
        }

        /// <summary>
        /// Shuffles the element order of an array.
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            lock (@lock)
            {
                instance.Shuffle(array);
            }
        }

        /// <summary>
        /// Suffles the element order of a list.
        /// </summary>
        public static void Shuffle<T>(IList<T> list)
        {
            lock (@lock)
            {
                instance.Shuffle(list);
            }
        }
    }
}
