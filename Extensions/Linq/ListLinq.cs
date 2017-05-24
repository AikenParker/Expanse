using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of List<T> related LINQ-like extension methods.
    /// </summary>
    public static class ListLinq
    {
        /// <summary>
        /// Returns true if a list contains an object.
        /// </summary>
        public static bool ContainsObject<T>(this IList<T> list, T obj) where T : class
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T item = list[i];

                if (item == obj)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a new list where items are casted from one type to another. Faster than OfType<>().ToList().
        /// </summary>
        public static List<TOutput> OfTypeToList<TInput, TOutput>(this IList<TInput> list)
            where TInput : class
            where TOutput : class
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TOutput item = list[i] as TOutput;

                if (item != null)
                {
                    output.Add(item);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items are casted from one type to another. Faster than Cast<>().ToList().
        /// </summary>
        public static List<TOutput> CastToList<TInput, TOutput>(this IList<TInput> list)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                output.Add((TOutput)(object)list[i]);
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items from another list meet some criteria. Faster than Where<>().ToList().
        /// </summary>
        public static List<T> WhereToList<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = list.Count;

            List<T> output = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                T item = list[i];

                if (predicate(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items are selected from another list. Faster than Select<>().ToList().
        /// </summary>
        public static List<TOutput> SelectToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, TOutput> selector)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (selector == null)
                throw new ArgumentNullException("selector");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TOutput item = selector(list[i]);

                output.Add(item);
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items from another list meet some criteria and can be casted. Faster than Where<>().OfType<>().ToList().
        /// </summary>
        public static List<TOutput> WhereOfTypeToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, bool> predicate)
            where TInput : class
            where TOutput : class
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TInput inItem = list[i];

                if (predicate(inItem))
                {
                    TOutput outItem = inItem as TOutput;

                    if (outItem != null)
                    {
                        output.Add(outItem);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items from another list meet some criteria and can be casted. Faster than Where<>().Cast<>().ToList().
        /// </summary>
        public static List<TOutput> WhereCastToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TInput item = list[i];

                if (predicate(item))
                {
                    output.Add((TOutput)(object)item);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items from another list can be casted and meet some criteria. Faster than OfType<>().Where<>().ToList().
        /// </summary>
        public static List<TOutput> OfTypeWhereToList<TInput, TOutput>(this IList<TInput> list, Func<TOutput, bool> predicate)
            where TInput : class
            where TOutput : class
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TOutput item = list[i] as TOutput;

                if (item != null && predicate(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items from another list can be casted and meet some criteria. Faster than Cast<>().Where<>().ToList().
        /// </summary>
        public static List<TOutput> CastWhereToList<TInput, TOutput>(this IList<TInput> list, Func<TOutput, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TOutput item = (TOutput)(object)list[i];

                if (predicate(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items that meet some criteria are selected from another list. Faster than Where<>().Select<>().ToList().
        /// </summary>
        public static List<TOutput> WhereSelectToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, bool> predicate, Func<TInput, TOutput> selector)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            if (selector == null)
                throw new ArgumentNullException("selector");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TInput inItem = list[i];

                if (predicate(inItem))
                {
                    TOutput outItem = selector(list[i]);

                    output.Add(outItem);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items are selected from another list and meet some criteria. Faster than Select<>().Where<>().ToList().
        /// </summary>
        public static List<TOutput> SelectWhereToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, TOutput> selector, Func<TOutput, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (selector == null)
                throw new ArgumentNullException("selector");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = list.Count;

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                TOutput item = selector(list[i]);

                if (predicate(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a new list where items are selected from another list of lists. Faster than SelectMany<>().ToList().
        /// </summary>
        public static List<TOutput> SelectManyToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, IList<TOutput>> selector)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (selector == null)
                throw new ArgumentNullException("selector");

            int count = list.Count;
            int sumCount = 0;

            for (int i = 0; i < count; i++)
            {
                IList<TOutput> outputList = selector(list[i]);

                sumCount += outputList.Count;
            }

            List<TOutput> output = new List<TOutput>(sumCount);

            for (int i = 0; i < count; i++)
            {
                IList<TOutput> outputList = selector(list[i]);

                int itemCount = outputList.Count;

                for (int j = 0; j < itemCount; j++)
                {
                    output.Add(outputList[j]);
                }
            }

            return output;
        }
    }
}
