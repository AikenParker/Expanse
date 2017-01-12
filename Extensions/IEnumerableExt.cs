using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    /// <summary>
    /// A collection of IEnumerable (LINQ-like) related extension methods.
    /// </summary>
    public static class IEnumerableExt
    {
        /// <summary>
        /// Logs a collection of objects.
        /// TODO: Place in DLL and allow custom string formatting.
        /// </summary>
        public static void Log<Input, Output>(this IEnumerable<Input> source, string prefix = null, Func<Input, Output> selector = null, LogType logType = LogType.Log)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int index = 0;

            bool hasSelector = selector != null;

            foreach (Input elem in source)
            {
                string elemName = hasSelector ? selector(elem).ToString() : elem.ToString();

                string message;

                if (!string.IsNullOrEmpty(prefix))
                    message = string.Format("{0} | {1} {2}", ++index, prefix, elemName);
                else
                    message = string.Format("{0} | {1}", ++index, elemName);

                switch (logType)
                {
                    case LogType.Log:
                        Debug.Log(message, elem as UnityEngine.Object);
                        break;

                    case LogType.Warning:
                        Debug.LogWarning(message, elem as UnityEngine.Object);
                        break;

                    case LogType.Error:
                        Debug.LogError(message, elem as UnityEngine.Object);
                        break;

                    case LogType.Exception:
                        Debug.LogException(new UnityException(message), elem as UnityEngine.Object);
                        break;

                    case LogType.Assert:
                        Debug.LogAssertion(message, elem as UnityEngine.Object);
                        break;
                }
            }
        }

        /// <summary>
        /// Determines if all objects in souce are not equal to one another.
        /// </summary>
        public static bool IsUnique<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Distinct().Count() != source.Count();
        }

        /// <summary>
        /// Returns true if all items do not equal null.
        /// </summary>
        public static bool All<T>(this IEnumerable<T> source)
        {
            return All(source, (x) => x != null);
        }

        /// <summary>
        /// Returns true if all items meet a condition.
        /// </summary>
        public static bool All<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (T elem in source)
            {
                if (!predicate(elem))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values. 
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }
    }
}
