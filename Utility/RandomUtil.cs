using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    public static class RandomUtil
    {
        public static bool GetChance(float chance)
        {
            return UnityEngine.Random.value < chance;
        }

        public static int GetRange(int startNum, int endNum)
        {
            return UnityEngine.Random.Range(startNum, endNum);
        }

        public static float GetRange(float startNum, float endNum)
        {
            return UnityEngine.Random.Range(startNum, endNum);
        }

        public static int GetValue(int range)
        {
            return UnityEngine.Random.Range(0, range);
        }

        public static float GetValue(float range = 1f)
        {
            return UnityEngine.Random.Range(0f, range);
        }

        public static int GetDirection()
        {
            return (UnityEngine.Random.value < 0.5f) ? -1 : 1;
        }

        public static float GetDirection(float magnitude)
        {
            return (float)RandomUtil.GetDirection() * magnitude;
        }

        public static Vector2 GetDirectionV2(float magnitude = 1f)
        {
            return Random.insideUnitCircle.normalized * magnitude;
        }

        public static Vector3 GetDirectionV3(float magnitude = 1f)
        {
            return Random.onUnitSphere * magnitude;
        }

        public static T GetValue<T>(T[] values)
        {
            int index = RandomUtil.GetValue(values.Length);
            return values[index];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();

            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static void Shuffle<T>(this T[] array)
        {
            System.Random rng = new System.Random();

            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
