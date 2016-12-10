using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    public class RandomUtil
    {
        public readonly int seed;

        public RandomUtil()
        {
            this.seed = Environment.TickCount;
            this.rng = new System.Random();
        }

        public RandomUtil(int seed)
        {
            this.seed = seed;
            this.rng = new System.Random(seed);
        }

        #region INSTANCE

        protected System.Random rng;

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        public bool Bool()
        {
            return rng.NextDouble() < 0.5;
        }

        /// <summary>
        /// Returns a random bool. Chance determines true percentage.
        /// </summary>
        public bool Bool(float chance)
        {
            return rng.NextDouble() < chance;
        }

        /// <summary>
        /// Returns a random integer between 0 and INT_MAX.
        /// </summary>
        public int Int()
        {
            return rng.Next();
        }

        /// <summary>
        /// Returns a random integer between 0 and max. (Exclusive upper bound)
        /// </summary>
        public int Int(int max)
        {
            return rng.Next(max);
        }

        /// <summary>
        /// Returns a random integer between min and max. (Exclusive upper bound)
        /// </summary>
        public int Int(int min, int max)
        {
            return rng.Next(min, max);
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        public float Float()
        {
            return (float)rng.NextDouble();
        }

        /// <summary>
        /// Returns a random float between 0 and max.
        /// </summary>
        public float Float(float max)
        {
            return (float)(rng.NextDouble() * max);
        }

        /// <summary>
        /// Returns a random float between a and b.
        /// </summary>
        public float Float(float a, float b)
        {
            float min = Mathf.Min(a, b);
            float max = Mathf.Max(a, b);

            return Float(max - min) + min;
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

        #endregion

        #region STATIC

        static RandomUtil instance = new RandomUtil();

        /// <summary>
        /// Returns the global static instance of RandomUtil.
        /// </summary>
        public static RandomUtil Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        public static bool GetBool()
        {
            return instance.Bool();
        }

        /// <summary>
        /// Returns a random bool. Chance determines true percentage.
        /// </summary>
        public static bool GetBool(float chance)
        {
            return instance.Bool(chance);
        }

        /// <summary>
        /// Returns a random integer between 0 and INT_MAX.
        /// </summary>
        public static int GetInt()
        {
            return instance.Int();
        }

        /// <summary>
        /// Returns a random integer between 0 and max. (Exclusive upper bound)
        /// </summary>
        public static int GetInt(int max)
        {
            return instance.Int(max);
        }

        /// <summary>
        /// Returns a random integer between min and max. (Exclusive upper bound)
        /// </summary>
        public static int GetInt(int min, int max)
        {
            return instance.Int(min, max);
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        public static float GetFloat()
        {
            return instance.Float();
        }

        /// <summary>
        /// Returns a random float between 0 and max.
        /// </summary>
        public static float GetFloat(float max)
        {
            return instance.Float(max);
        }

        /// <summary>
        /// Returns a random float between a and b.
        /// </summary>
        public static float GetFloat(float a, float b)
        {
            return instance.Float(a, b);
        }

        /// <summary>
        /// Returns either 1 or -1.
        /// </summary>
        public static int GetSign()
        {
            return instance.Sign();
        }

        /// <summary>
        /// Returns a random normalized Vector2.
        /// </summary>
        public static Vector2 GetVector2()
        {
            return instance.Vector2();
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        public static Vector2 GetVector2(float maxMagnitude)
        {
            return instance.Vector2(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        public static Vector2 GetVector2(float aMagnitude, float bMagnitude)
        {
            return instance.Vector2(aMagnitude, bMagnitude);
        }

        /// <summary>
        /// Returns a random normalized Vector3.
        /// </summary>
        public static Vector3 GetVector3()
        {
            return instance.Vector3();
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        public static Vector3 GetVector3(float maxMagnitude)
        {
            return instance.Vector3(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector3 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        public static Vector3 GetVector3(float aMagnitude, float bMagnitude)
        {
            return instance.Vector3(aMagnitude, bMagnitude);
        }

        /// <summary>
        /// Returns a random normalized Vector4.
        /// </summary>
        public static Vector4 GetVector4()
        {
            return instance.Vector4();
        }

        /// <summary>
        /// Returns a random Vector2 with a magnitude between 0 and maxMagnitude.
        /// </summary>
        public static Vector4 GetVector4(float maxMagnitude)
        {
            return instance.Vector4(maxMagnitude);
        }

        /// <summary>
        /// Returns a random Vector4 with a magnitude between aMagnitude and bMagnitude.
        /// </summary>
        public static Vector4 GetVector4(float aMagnitude, float bMagnitude)
        {
            return instance.Vector4(aMagnitude, bMagnitude);
        }

        /// <summary>
        /// Returns a random color.
        /// </summary>
        public static Color GetColor()
        {
            return instance.Color();
        }

        /// <summary>
        /// Returns a random color with an alpha.
        /// </summary>
        public static Color GetColor(float alpha)
        {
            return instance.Color(alpha);
        }

        /// <summary>
        /// Returns a random element in an array.
        /// </summary>
        public static T GetElement<T>(T[] array)
        {
            return instance.Element(array);
        }

        /// <summary>
        /// Returns a random element in a list.
        /// </summary>
        public static T GetElement<T>(IList<T> list)
        {
            return instance.Element(list);
        }

        /// <summary>
        /// Shuffles the element order of an array.
        /// </summary>
        public static void ApplyShuffle<T>(T[] array)
        {
            instance.Shuffle(array);
        }

        /// <summary>
        /// Suffles the element order of a list.
        /// </summary>
        public static void ApplyShuffle<T>(IList<T> list)
        {
            instance.Shuffle(list);
        }

        #endregion
    }
}
