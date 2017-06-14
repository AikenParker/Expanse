using System;
using System.Collections;
using System.Collections.Generic;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Object related extension methods.
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// Determines if target object is null OR empty.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="source">Target object.</param>
        /// <returns>Returns true if the the source target object is null or empty.</returns>
        public static bool IsNullOrEmpty<T>(this T source)
        {
            if (source == null || source.Equals(null))
            {
                return true;
            }
            else if (typeof(T).IsValueType)
            {
                return false;
            }
            else if (source is ICollection<T>)
            {
                return (source as ICollection<T>).Count <= 0;
            }
            else if (source is ICollection)
            {
                return (source as ICollection).Count <= 0;
            }
            else if (source is IEnumerable<T>)
            {
                foreach (var elem in source as IEnumerable)
                {
                    return false;
                }
            }
            else if (source is IEnumerable)
            {
                foreach (var elem in source as IEnumerable)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="a">First object to compare equality with.</param>
        /// <param name="b">Second object to compare equality with.</param>
        /// <returns>Returns true the source object is equal with either a or b.</returns>
        public static bool EqualsAny<T>(this T source, T a, T b)
            where T : IEquatable<T>
        {
            return source.Equals(a) || source.Equals(b);
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="a">First object to compare equality with.</param>
        /// <param name="b">Second object to compare equality with.</param>
        /// <param name="c">Third object to compare equality with.</param>
        /// <returns>Returns true the source object is equal with either a, b or c.</returns>
        public static bool EqualsAny<T>(this T source, T a, T b, T c)
            where T : IEquatable<T>
        {
            return source.Equals(a) || source.Equals(b) || source.Equals(c);
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="a">First object to compare equality with.</param>
        /// <param name="b">Second object to compare equality with.</param>
        /// <param name="c">Third object to compare equality with.</param>
        /// <param name="d">Fourth object to compare equality with.</param>
        /// <returns>Returns true the source object is equal with either a, b or c.</returns>
        public static bool EqualsAny<T>(this T source, T a, T b, T c, T d)
            where T : IEquatable<T>
        {
            return source.Equals(a) || source.Equals(b) || source.Equals(c) || source.Equals(d);
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="comparisons">Collection of comparison objects to check equality with.</param>
        /// <returns>Returns true the source object is equal with either a, b or c.</returns>
        public static bool EqualsAny<T>(this T source, params T[] comparisons)
            where T : IEquatable<T>
        {
            for (int i = 0; i < comparisons.Length; i++)
            {
                if (source.Equals(comparisons[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="a">First object to compare equality with.</param>
        /// <param name="b">Second object to compare equality with.</param>
        /// <returns>Returns true the source object is equal with either a or b.</returns>
        public static bool EqualsAnyObject<T>(this T source, T a, T b)
            where T : class
        {
            return source.Equals(a) || source.Equals(b);
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="a">First object to compare equality with.</param>
        /// <param name="b">Second object to compare equality with.</param>
        /// <param name="c">Third object to compare equality with.</param>
        /// <returns>Returns true the source object is equal with either a, b or c.</returns>
        public static bool EqualsAnyObject<T>(this T source, T a, T b, T c)
            where T : class
        {
            return source.Equals(a) || source.Equals(b) || source.Equals(c);
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="a">First object to compare equality with.</param>
        /// <param name="b">Second object to compare equality with.</param>
        /// <param name="c">Third object to compare equality with.</param>
        /// <param name="d">Fourth object to compare equality with.</param>
        /// <returns>Returns true the source object is equal with either a, b or c.</returns>
        public static bool EqualsAnyObject<T>(this T source, T a, T b, T c, T d)
            where T : class
        {
            return source.Equals(a) || source.Equals(b) || source.Equals(c) || source.Equals(d);
        }

        /// <summary>
        /// Determines if the source object equals any of the objects given.
        /// </summary>
        /// <typeparam name="T">Type of the object to compare.</typeparam>
        /// <param name="source">Source object to compare.</param>
        /// <param name="comparisons">Collection of comparison objects to check equality with.</param>
        /// <returns>Returns true the source object is equal with either a, b or c.</returns>
        public static bool EqualsAnyObject<T>(this T source, params T[] comparisons)
            where T : class
        {
            for (int i = 0; i < comparisons.Length; i++)
            {
                if (source.Equals(comparisons[i]))
                    return true;
            }

            return false;
        }
    }
}