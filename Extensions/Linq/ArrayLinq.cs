using System;
using System.Collections.Generic;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Array related LINQ-like extension methods.
    /// </summary>
    public static class ArrayLinq
    {
        /// <summary>
        /// Returns the first element in a list that meets a condition.
        /// </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="list">Source list.</param>
        /// <param name="predicate">Condition to meet.</param>
        /// <returns>Returns the first element in a list that meets a condition.</returns>
        public static T Find<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int listCount = list.Count;

            for (int i = 0; i < listCount; i++)
            {
                T item = list[i];

                if (predicate(item))
                    return item;
            }

            return default(T);
        }

        /// <summary>
        /// Returns a new array where items are casted from one type to another.
        /// <para>Faster than Cast().ToArray().</para>
        /// </summary>
        /// <typeparam name="TInput">Array input type.</typeparam>
        /// <typeparam name="TOutput">Array output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <returns>Returns a new array of type <typeparamref name="TOutput"/></returns>
        public static TOutput[] CastToArray<TInput, TOutput>(this IList<TInput> list)
            where TInput : class
            where TOutput : class
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int count = list.Count;

            TOutput[] output = new TOutput[count];

            for (int i = 0; i < count; i++)
            {
                output[i] = list[i] as TOutput;
            }
            return output;
        }

        /// <summary>
        /// Returns a new array where items are selected from another list.
        /// <para>Faster than Select().ToArray().</para>
        /// </summary>
        /// <typeparam name="TInput">Array input type.</typeparam>
        /// <typeparam name="TOutput">Array output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="selector">Selects items of type <typeparamref name="TOutput"/> from input list.</param>
        /// <returns>Returns a new array of type <typeparamref name="TOutput"/></returns>
        public static TOutput[] SelectToArray<TInput, TOutput>(this IList<TInput> list, Func<TInput, TOutput> selector)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (selector == null)
                throw new ArgumentNullException("selector");

            int count = list.Count;

            TOutput[] output = new TOutput[count];

            for (int i = 0; i < count; i++)
            {
                TOutput item = selector(list[i]);

                output[i] = item;
            }

            return output;
        }

        /// <summary>
        /// Returns a new array where items are selected from another list of lists.
        /// <para>Faster than SelectMany().ToArray().</para>
        /// </summary>
        /// <typeparam name="TInput">Array input type.</typeparam>
        /// <typeparam name="TOutput">Array output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="selector">Selects lists of items of type <typeparamref name="TOutput"/> from input list.</param>
        /// <returns>Returns a new array of type <typeparamref name="TOutput"/></returns>
        public static TOutput[] SelectManyToArray<TInput, TOutput>(this IList<TInput> list, Func<TInput, IList<TOutput>> selector)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (selector == null)
                throw new ArgumentNullException("selector");

            int count = list.Count;
            int sumCount = 0;

            for (int i = 0; i < count; i++)
            {
                IList<TOutput> outputList = selector(list[i]);

                sumCount += outputList.Count;
            }

            TOutput[] output = new TOutput[sumCount];
            int index = 0;

            for (int i = 0; i < count; i++)
            {
                IList<TOutput> outputList = selector(list[i]);

                int itemCount = outputList.Count;

                for (int j = 0; j < itemCount; j++)
                {
                    output[index++] = outputList[j];
                }
            }

            return output;
        }

#if UNSAFE
        /// <summary>
        /// Unsafely creates a new array where items from another list meet some criteria.
        /// <para>Less allocation than Where().ToArray() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="T">Array input type,</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to array.</param>
        /// <returns>Returns a new array with items from a list that met a specified condition.</returns>
        public unsafe static T[] UnsafeWhereToArray<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("list");

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

            T[] output = new T[count];

            for (int i = 0; i < count; i++)
            {
                int index = indicies[i];
                T item = list[index];
                output[i] = item;
            }

            return output;
        }

        /// <summary>
        /// Unsafely creates a new array of ints where ints from another list meet some criteria.
        /// <para>Less allocation than Where().ToArray() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="T">Array input type,</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to array.</param>
        /// <returns>Returns a new array with items from a list that met a specified condition.</returns>
        public unsafe static int[] UnsafeWhereToArray(this IList<int> list, Func<int, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("list");

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

            int[] output = new int[count];

            for (int i = 0; i < count; i++)
            {
                output[i] = ints[i];
            }

            return output;
        }

        /// <summary>
        /// Unsafely creates a new array of float where float from another list meet some criteria.
        /// <para>Less allocation than Where().ToArray() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="T">Array input type,</typeparam>
        /// <param name="list">Source input list.</param>
        /// <param name="predicate">Condition to be met before adding to array.</param>
        /// <returns>Returns a new array with items from a list that met a specified condition.</returns>
        public unsafe static float[] UnsafeWhereToArray(this IList<float> list, Func<float, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int totalCount = list.Count;
            int count = 0;

            float* floats = stackalloc float[totalCount];

            for (int i = 0; i < totalCount; i++)
            {
                float item = list[i];

                if (predicate(item))
                {
                    floats[count++] = item;
                }
            }

            float[] output = new float[count];

            for (int i = 0; i < count; i++)
            {
                output[i] = floats[i];
            }

            return output;
        }

        /// <summary>
        /// Unsafely creates a new array where items are casted from one type to another.
        /// <para>Less allocation than OfType().ToArray() but may be slower or faster.</para>
        /// </summary>
        /// <typeparam name="TInput">List input type.</typeparam>
        /// <typeparam name="TOutput">List output type.</typeparam>
        /// <param name="list">Source input list.</param>
        /// <returns>Returns a new array where items are casted from one type to another.</returns>
        public unsafe static TOutput[] UnsafeOfTypeToArray<TInput, TOutput>(this IList<TInput> list)
            where TInput : class
            where TOutput : class
        {
            if (list == null)
                throw new ArgumentNullException("list");

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

            TOutput[] output = new TOutput[count];

            for (int i = 0; i < count; i++)
            {
                int index = indicies[i];
                TInput elem = list[index];
                TOutput item = elem as TOutput;
                output[i] = item;
            }

            return output;
        }
#endif
    }
}
