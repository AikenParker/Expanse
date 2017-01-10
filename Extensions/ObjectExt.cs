using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Expanse
{
    public static class ObjectExt
    {
        /// <summary>
        /// Determines if target object is null OR empty
        /// </summary>
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
            else if ((source as ICollection<T>) != null)
            {
                return (source as ICollection<T>).Count <= 0;
            }
            else if ((source as ICollection) != null)
            {
                return (source as ICollection).Count <= 0;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the source object equals any of the objects given.
        /// </summary>
        public static bool EqualsAny<T>(this T source, T a, T b)
        {
            if (source.Equals(a))
                return true;

            if (source.Equals(b))
                return true;

            return false;
        }

        /// <summary>
        /// Returns true if the source object equals any of the objects given.
        /// </summary>
        public static bool EqualsAny<T>(this T source, T a, T b, T c)
        {
            if (source.Equals(a))
                return true;

            if (source.Equals(b))
                return true;

            if (source.Equals(c))
                return true;

            return false;
        }

        /// <summary>
        /// Returns true if the source object equals any of the objects given.
        /// </summary>
        public static bool EqualsAny<T>(this T source, params T[] comparisons)
        {
            foreach (T item in comparisons)
                if (source.Equals(item))
                    return true;

            return false;
        }
    }
}