using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class IListExt
    {
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
        /// Retrieves an element safely from a list.
        /// Checks for null and appropriate index.
        /// </summary>
        public static T SafeGet<T>(this IList<T> source, int index) where T : class
        {
            if (source.IsNullOrEmpty()) return null;
            if (!source.IndexExists(index)) return null;
            return source[index];
        }

        public static int IndexOf<T>(this IList<T> source, T item)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.IndexOf(item);
        }

        public static bool IndexExists<T>(this IList<T> source, int index)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int listLength = source.Count;

            if (index >= 0 && index < listLength)
                return source[index] != null;
            else return false;
        }
    }
}