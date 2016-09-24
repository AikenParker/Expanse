using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class IEnumerableExt
    {
        public static void DestroyGameObjects<T>(this IEnumerable<T> source, bool immediate = false) where T: Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (T elem in source)
            {
                if (immediate)
                    GameObject.DestroyImmediate(elem.gameObject);
                else
                    GameObject.Destroy(elem.gameObject);
            }
        }

        public static void DestroyGameObjects<T>(this IEnumerable<T> source, Predicate<T> predicate, bool immediate = false) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (T elem in source)
            {
                if (predicate(elem))
                    if (immediate)
                        GameObject.DestroyImmediate(elem.gameObject);
                    else
                        GameObject.Destroy(elem.gameObject);
            }
        }

        public static void Log<T>(this IEnumerable<T> source, string prefix)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int index = 0;

            foreach (T elem in source)
                Debug.Log(string.Format("{0} | {1} {2}", ++index, prefix, elem));
        }

        public static void Log<Input, Output>(this IEnumerable<Input> source, string prefix, Func<Input, Output> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int index = 0;

            foreach (Input elem in source)
                Debug.Log(string.Format("{0} | {1} {2}", ++index, prefix, selector(elem)));
        }
    }
}
