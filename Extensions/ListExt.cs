using Expanse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// A collection of List related extension methods.
    /// </summary>
    public static class ListExt
    {
        /// <summary>
        /// Removes the first element and returns it.
        /// </summary>
        public static T Pop<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            T obj = list[0];
            list.RemoveAt(0);
            return obj;
        }

        /// <summary>
        /// Removes the last element and returns it.
        /// </summary>
        public static T PopLast<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int lastIndex = list.Count - 1;
            T obj = list[lastIndex];
            list.RemoveAt(lastIndex);
            return obj;
        }

        /// <summary>
        /// Adds new element in the first index.
        /// </summary>
        public static void Push<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            list.Insert(0, item);
        }

        /// <summary>
        /// Returns the first element.
        /// </summary>
        public static T Peek<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            return list[0];
        }

        /// <summary>
        /// Moves first element to end and returns it.
        /// </summary>
        public static T Requeue<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            T obj = list.Dequeue();
            list.Enqueue(obj);
            return obj;
        }

        /// <summary>
        /// Removes first element and returns it.
        /// </summary>
        public static T Dequeue<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            T obj = list[0];
            list.RemoveAt(0);
            return obj;
        }

        /// <summary>
        /// Adds new element to the end.
        /// </summary>
        public static void Enqueue<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            list.Add(item);
        }

        /// <summary>
        /// Moves an element to an index.
        /// </summary>
        public static void Move<T>(this IList<T> list, T item, int index)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            list.Remove(item);
            list.Insert(index, item);
        }

        /// <summary>
        /// Returns next element. If index is last return first.
        /// </summary>
        public static T Next<T>(this IList<T> list, int index)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int nextIndex = index + 1;

            if (nextIndex >= list.Count)
                return list[0];
            else
                return list[nextIndex];
        }

        /// <summary>
        /// Returns previous element. If index is first return last.
        /// </summary>
        public static T Previous<T>(this IList<T> list, int index)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            int previousIndex = index - 1;

            if (previousIndex < 0)
                return list[list.Count - 1];
            else
                return list[previousIndex];
        }

        /// <summary>
        /// Returns an the element in the index safely (No Exceptions)
        /// </summary>
        public static T SafeGet<T>(this IList<T> list, int index) where T : class
        {
            if (list.IsNullOrEmpty())
                return null;
            if (!list.HasIndexValue(index))
                return null;

            return list[index];
        }

        /// <summary>
        /// Returns the index of an element in a list.
        /// </summary>
        public static int IndexOf<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            return list.IndexOf(item);
        }

        /// <summary>
        /// Determines if a list has an index.
        /// </summary
        public static bool HasIndex<T>(this IList<T> list, int index)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            return index >= 0 && index < list.Count;
        }

        /// <summary>
        /// Determines if a list has an index with a value.
        /// </summary>
        public static bool HasIndexValue<T>(this IList<T> list, int index)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (index >= 0 && index < list.Count)
                return list[index] != null;

            return false;
        }

        /// <summary>
        /// Returns true if a selected list contains an item.
        /// </summary>
        public static bool Contains<T, U>(this IList<T> list, U item, Func<T, U> selector)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            var selected = list.Select(selector);

            return selected.Contains(item);
        }

        /// <summary>
        /// Removes the first element in the list.
        /// </summary>
        /// <returns>Returns true if an item was removed</returns>
        public static bool RemoveFirst<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (list.Count > 0)
            {
                list.RemoveAt(0);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the first element in the list that is equal to the item.
        /// </summary>
        /// <returns>Returns true if an item was removed</returns>
        public static bool RemoveFirst<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(item))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the first element in the list that meets the predicate.
        /// </summary>
        /// <returns>Returns true if an item was removed</returns>
        public static bool RemoveFirst<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the last element in the list.
        /// </summary>
        /// <returns>Returns true if an item was removed</returns>
        public static bool RemoveLast<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            int listCount = list.Count;

            if (listCount > 0)
            {
                list.RemoveAt(listCount - 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the last element in the list that is equal to the item.
        /// </summary>
        /// <returns>Returns true if an item was removed</returns>
        public static bool RemoveLast<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Equals(item))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the last element in the list that meets the predicate.
        /// </summary>
        /// <returns>Returns true if an item was removed</returns>
        public static bool RemoveLast<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Suffles the order of elements in the list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, Random rng = null)
        {
            if (list == null)
                throw new ArgumentNullException("source");

            if (rng != null)
                rng.Shuffle(list);
            else
                RandomUtil.Shuffle(list);
        }

        /// <summary>
        /// Creates a new list where items are casted from one type to another. Faster than OfType<>().ToList().
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
        /// Creates a new list where items are casted from one type to another. Faster than Cast<>().ToList().
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
        /// Creates a new list where items from another list meet some criteria. Faster than Where<>().ToList().
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
        /// Creates a new list where items are selected from another list. Faster than Select<>().ToList().
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
        /// Creates a new list where items from another list meet some criteria and can be casted. Faster than Where<>().OfType<>().ToList().
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
        /// Creates a new list where items from another list meet some criteria and can be casted. Faster than Where<>().Cast<>().ToList().
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
        /// Creates a new list where items from another list can be casted and meet some criteria. Faster than OfType<>().Where<>().ToList().
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
        /// Creates a new list where items from another list can be casted and meet some criteria. Faster than Cast<>().Where<>().ToList().
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
        /// Creates a new list where items that meet some criteria are selected from another list. Faster than Where<>().Select<>().ToList().
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
        /// Creates a new list where items are selected from another list and meet some criteria. Faster than Select<>().Where<>().ToList().
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
    }
}