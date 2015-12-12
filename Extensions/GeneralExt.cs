using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Expanse
{
    namespace Ext
    {
        /// <summary>
        /// General purpose extension methods.
        /// </summary>
        public static class GeneralExt
        {
            /// <summary>
            /// Determines if target object is null OR empty (as an ICollection)
            /// </summary>
            public static bool IsNullOrEmpty<T>(this T source)
            {
                if (source == null || source.Equals(null))
                    return true;
                else if (typeof(T).IsValueType)
                    return false;
                else if ((source as ICollection) != null)
                    return (source as ICollection).Count <= 0;
#pragma warning disable 168
                else if ((source as IEnumerable) != null)
                {
                    int count = 0;
                    foreach (var elem in (source as IEnumerable))
                        count++;
                    return count <= 0;
                }
#pragma warning restore 168
                else
                    return false;
            }

            /// <summary>
            /// Times how long it takes to perform an action in a specified amount of iterations.
            /// </summary>
            /// <returns>Length in milliseconds it took.</returns>
            public static float Benchmark(Action action, int iterations)
            {
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                for (int i = 0; i < iterations; i++)
                {
                    action.Invoke();
                }
                Timer.Stop();
                return Timer.ElapsedMilliseconds;
            }

            /// <summary>
            /// If a particular element in the source IEnumerable meets the specified condition then perform a specified action.
            /// </summary>
            public static void ConditionalAction<T>(this IEnumerable<T> source, Predicate<T> condition, Action<T> action)
            {
                if (source.IsNullOrEmpty())
                    throw new ArgumentNullException();

                foreach (T item in source)
                    if (condition(item))
                        action(item);
            }

            /// <summary>
            /// Retrieves an element safely from a list.
            /// Checks for null and appropriate index.
            /// </summary>
            public static T SafeGet<T>(this IList<T> source, int index) where T : class
            {
                if (source.IsNullOrEmpty()) return null;
                if (index < 0 || index >= source.Count) return null;
                return source[index];
            }

            /// <summary>
            /// Retrieves an element safely from a list.
            /// Checks for null and appropriate index.
            /// If requested index is above or below range of elements the element returned will be the first or last respectively.
            /// </summary>
            public static T SafeGet<T>(this IList<T> source, int index, bool clamp) where T : class
            {
                if (source.IsNullOrEmpty()) return null;
                if (index < 0) return clamp ? source[0] : null;
                else if (index >= source.Count) return clamp ? source[source.Count - 1] : null;
                else return source[index];
            }

            /// <summary>
            /// Retrieves an element safely from an IEnumerable.
            /// Unconstrained but slightly less safe.
            /// </summary>
            public static T SafeGet<T>(this IEnumerable<T> source, int index, T fallback = default(T))
            {
                int iterator = 0;
                foreach(var item in source)
                {
                    if (iterator == index)
                        return item;
                    iterator++;
                }
                return fallback;
            }

            /// <summary>
            /// Resizes a target list.
            /// </summary>
            public static void Resize<T>(this List<T> list, int newSize, T newObjects = default(T))
            {
                int curSize = list.Count;

                if (curSize > newSize)
                    list.RemoveRange(newSize, curSize - newSize);
                else
                    list.AddRange(Enumerable.Repeat(newObjects, newSize - curSize));
            }

            /// <summary>
            /// If it can it will convert a target IEnumerable into another type.
            /// </summary>
            public static IEnumerable<TOutput> ConvertValid<TInput, TOutput>(this IEnumerable<TInput> source, Converter<TInput, TOutput> converter)
            {
                foreach (TInput Obj in source)
                {
                    TOutput temp = converter(Obj);
                    if (!(temp as object).IsNullOrEmpty())
                        yield return temp;
                }
            }

            /// <summary>
            /// Returns a random element of a list. If empty it returns the default value of specified type.
            /// </summary>
            public static T Random<T>(this List<T> source)
            {
                if (source.IsNullOrEmpty())
                    return default(T);
                else
                    return source[UnityEngine.Random.Range(0, source.Count)];
            }

            /// <summary>
            /// Normalizes a float between a new float range.
            /// </summary>
            public static float Normalize(this float source, float curMin, float curMax, float newMin, float newMax)
            {
                return Mathf.Lerp(newMin, newMax, (source - curMin) / (curMax - curMin));
            }

            /// <summary>
            /// Returns true if the source object equals any of the objects given.
            /// </summary>
            public static bool EqualToAny<T>(this T source, params T[] comparisons)
            {
                foreach (T item in comparisons)
                    if (source.Equals(item))
                        return true;

                return false;
            }

            /// <summary>
            /// Similar to .NET FirstOrDefault but allows specification of default value.
            /// </summary>
            public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate, T defaultValue)
            {
                if (source.IsNullOrEmpty()) return defaultValue;

                foreach (T obj in source)
                    if (predicate(obj))
                        return obj;

                return defaultValue;
            }

            /// <summary>
            /// Similar to .NET FirstOrDefault except this includes a max number of iterations. Returns default if it reaches that point.
            /// </summary>
            public static T FirstOrDefaultWithMax<T>(this List<T> source, Func<T, bool> predicate, int max)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    if (i >= max) return default(T);
                    if (predicate(source[i])) return source[i];
                }

                return default(T);
            }

            /// <summary>
            /// Similar to .NET FirstOrDefault except this includes a max number of iterations. Returns the last valid value or default if it reaches that point.
            /// </summary>
            public static T LastOrDefaultWithMax<T>(this List<T> source, Func<T, bool> predicate, int max)
            {
                for (int i = source.Count - 1; i >= 0; i--)
                {
                    if (i < source.Count - max) return default(T);
                    if (predicate(source[i])) return source[i];
                }

                return default(T);
            }

            /// <summary>
            /// Similar to FirstOrDefault except this only includes values in a range. Returns default if it reaches the end.
            /// </summary>
            public static T FirstOrDefaultInRange<T>(this List<T> source, Func<T, bool> predicate, int from, int to)
            {
                for (int i = Math.Max(0, from - 1); i < source.Count; i++)
                {
                    if (i >= to) return default(T);
                    if (predicate(source[i])) return source[i];
                }

                return default(T);
            }

            /// <summary>
            /// Similar to LastOrDefault except this only includes values in a range. Returns default if it reaches the start.
            /// </summary>
            public static T LastOrDefaultInRange<T>(this List<T> source, Func<T, bool> predicate, int from, int to)
            {
                for (int i = to; i >= 0; i--)
                {
                    if (i < from - 1) return default(T);
                    if (predicate(source[i])) return source[i];
                }

                return default(T);
            }

            /// <summary>
            /// Adds spaces before capital letter characters in a string.
            /// </summary>
            public static string AddSpaces(this string source, bool preserveAcronyms = true)
            {
                if (string.IsNullOrEmpty(source))
                    return string.Empty;

                StringBuilder newText = new StringBuilder(source.Length * 2);
                newText.Append(source[0]);

                for (int i = 1; i < source.Length; i++)
                {
                    if (char.IsUpper(source[i]))
                        if ((source[i - 1] != ' ' && !char.IsUpper(source[i - 1])) ||
                            (preserveAcronyms && char.IsUpper(source[i - 1]) &&
                             i < source.Length - 1 && !char.IsUpper(source[i + 1])))
                            newText.Append(' ');
                    newText.Append(source[i]);
                }

                return newText.ToString();
            }

            /// <summary>
            /// Reverses the digit order of an integer.
            /// </summary>
            public static int Reverse(this int num)
            {
                int result = 0;
                while (num > 0)
                {
                    result = result * 10 + num % 10;
                    num /= 10;
                }
                return result;
            }
        }
    }
}
