using System;
using System.Collections.Generic;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of List<T> related LINQ-like extension methods.
    /// </summary>
    public static class ListLinq
    {
        /// <summary>
        /// Determines if a list contains a value.
        /// </summary>
        /// <typeparam name="T">Type of sequence.</typeparam>
        /// <param name="list">Source list of elements.</param>
        /// <param name="value">Value to check for in list.</param>
        /// <returns>Returns true if a list contains a value.</returns>
        public static bool ContainsValue<T>(this IList<T> list, T value) where T : IEquatable<T>
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T item = list[i];

                if (item.Equals(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a list contains an object.
        /// </summary>
        /// <typeparam name="T">Type of sequence.</typeparam>
        /// <param name="list">Source list of elements.</param>
        /// <param name="obj">Object to check for in list.</param>
        /// <returns>Returns true if a list contains an object.</returns>
        public static bool ContainsObject<T>(this IList<T> list, T obj) where T : class
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T item = list[i];

                if (item.Equals(obj))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new list where items are casted from one type to another. Faster than OfType().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <returns>Returns a new list where items are casted from one type to another.</returns>
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
        /// Creates a new list where items are casted from one type to another. Faster than Cast().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <returns>Returns a new list where items are casted from one type to another.</returns>
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
        /// Creates a new list where items from another list meet some criteria. Faster than Where().ToList().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list with items from a list that met a specified condition.</returns>
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
        /// Creates a new list where items are selected from another list. Faster than Select().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="selector">Selects elements of TOutput from elements of TInput in list.</param>
        /// <returns>Returns a new list where items are selected from another list.</returns>
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
        /// Creates a new list where items another list meet some criteria and can be casted. Faster than Where().OfType().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list where items another list meet some criteria and can be casted.</returns>
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
        /// Creates a new list where items from another list meet some criteria and can be casted. Faster than Where().Cast().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list where items from another list meet some criteria and can be casted.</returns>
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
        /// Creates a new list where items from another list can be casted and meet some criteria. Faster than OfType().Where().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list where items from another list can be casted and meet some criteria.</returns>
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
        /// Creates a new list where items from another list can be casted and meet some criteria. Faster than Cast().Where().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list where items from another list can be casted and meet some criteria.</returns>
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
        /// Creates a new list where items that meet some criteria are selected from another list. Faster than Where().Select().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <param name="selector">Selects elements of TOutput from elements of TInput in list.</param>
        /// <returns>Returns a new list where items that meet some criteria are selected from another list.</returns>
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
        /// Creates a new list where items are selected from another list and meet some criteria. Faster than Select().Where().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="selector">Selects elements of TOutput from elements of TInput in list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list where items are selected from another list and meet some criteria.</returns>
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
        /// Creates a new list where items are selected from another list of lists. Faster than SelectMany().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="selector">Selects elements of TOutput from elements of TInput in list.</param>
        /// <returns>Returns a new list where items are selected from another list of lists.</returns>
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

        /// <summary>
        /// Creates a new list where items are selected from another list of lists and meet a condition. Faster than SelectMany().Where().ToList().
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="selector">Selects lists of TOutput from elements of TInput in list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list where items are selected from another list of lists and meet a condition.</returns>
        public static List<TOutput> SelectManyWhereToList<TInput, TOutput>(this IList<TInput> list, Func<TInput, IList<TOutput>> selector, Func<TOutput, bool> predicate)
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
                    TOutput outputItem = outputList[j];

                    if (predicate(outputItem))
                    {
                        output.Add(outputItem);
                    }
                }
            }

            return output;
        }

#if UNSAFE
        /// <summary>
        /// Unsafely creates a new list where items from another list meet some criteria.
        /// <para>Less allocation than WhereToList() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="T">List input type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list with items from a list that met a specified condition.</returns>
        public unsafe static List<T> UnsafeWhereToList<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int totalCount = list.Count;
            int count = 0;

            int* indicies = stackalloc int[totalCount];

            for (int i = 0; i < totalCount; i++)
            {
                T item = list[i];

                if (predicate(item))
                {
                    indicies[count++] = i;
                }
            }

            List<T> output = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                int index = indicies[i];
                T item = list[index];
                output.Add(item);
            }

            return output;
        }

        /// <summary>
        /// Unsafely creates a new list of ints where ints from another list meet some criteria.
        /// <para>Less allocation than WhereToList() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="T">List input type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to list.</param>
        /// <returns>Returns a new list with items from a list that met a specified condition.</returns>
        public unsafe static List<int> UnsafeWhereToList(this IList<int> list, Func<int, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int totalCount = list.Count;
            int count = 0;

            int* ints = stackalloc int[totalCount];

            for (int i = 0; i < totalCount; i++)
            {
                int item = list[i];

                if (predicate(item))
                {
                    ints[count++] = item;
                }
            }

            List<int> output = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                output.Add(ints[i]);
            }

            return output;
        }

        /// <summary>
        /// Unsafely creates a new list where items are casted from one type to another.
        /// <para>Less allocation than OfTypeToList() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <returns>Returns a new list where items are casted from one type to another.</returns>
        public unsafe static List<TOutput> UnsafeOfTypeToList<TInput, TOutput>(this IList<TInput> list)
            where TInput : class
            where TOutput : class
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int totalCount = list.Count;
            int count = 0;

            int* indicies = stackalloc int[totalCount];

            for (int i = 0; i < totalCount; i++)
            {
                TInput elem = list[i];
                TOutput item = elem as TOutput;

                if (item != null)
                {
                    indicies[count++] = i;
                }
            }

            List<TOutput> output = new List<TOutput>(count);

            for (int i = 0; i < count; i++)
            {
                int index = indicies[i];
                TInput elem = list[index];
                TOutput item = elem as TOutput;
                output.Add(item);
            }

            return output;
        }
#endif
    }
}
