using System;
using System.Collections.Generic;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of T[] related LINQ-like extension methods.
    /// </summary>
    public static class ArrayLinq
    {
        /// <summary>
        /// Returns a new array where items are casted from one type to another. Faster than Cast<>().ToArray().
        /// </summary>
        public static TOutput[] CastToArray<TInput, TOutput>(this IList<TInput> list)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int count = list.Count;

            TOutput[] output = new TOutput[count];

            for (int i = 0; i < count; i++)
            {
                output[i] = (TOutput)(object)list[i];
            }

            return output;
        }

        /// <summary>
        /// Returns a new array where items are selected from another list. Faster than Select<>().ToArray().
        /// </summary>
        public static TOutput[] SelectToArray<TInput, TOutput>(this IList<TInput> list, Func<TInput, TOutput> selector)
        {
            if (list == null)
                throw new ArgumentNullException("source");

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
        /// Returns a new array where items are selected from another list of lists. Faster than SelectMany<>().ToArray().
        /// </summary>
        public static TOutput[] SelectManyToArray<TInput, TOutput>(this IList<TInput> list, Func<TInput, IList<TOutput>> selector)
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
    }
}
