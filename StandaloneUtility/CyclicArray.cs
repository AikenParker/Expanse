using System;
using System.Collections;
using System.Collections.Generic;

namespace Expanse
{
    /// <summary>
    /// Fixed array data structure that cycles the index when adding more elements than the specified size.
    /// </summary>
    /// <typeparam name="T">Type of the structure to be contained.</typeparam>
    public struct CyclicArray<T> : IEnumerable<T> where T : struct
    {
        private T[] array;      // Internal array data structure
        private int index;      // Index of the most recently added item
        private bool allSet;    // Flag is true if all array slots have been set

        /// <summary>
        /// Creates a new Cyclic array data structure.
        /// </summary>
        /// <param name="size">Length of the array to be allocated.</param>
        public CyclicArray(int size)
        {
            this.array = new T[size];
            this.index = -1;
            this.allSet = false;
        }

        /// <summary>
        /// Adds a new item into the array.
        /// </summary>
        /// <param name="inItem">The item to be added to the array.</param>
        public void Add(T inItem)
        {
            if (++this.index == this.array.Length)
            {
                this.index = 0;
                this.allSet = true;
            }

            array[this.index] = inItem;
        }

        /// <summary>
        /// Adds a new item into the array.
        /// </summary>
        /// <param name="inItem">The item to be added to the array.</param>
        /// <param name="outItem">The item to be replaced by the new item in the array.</param>
        /// <returns>Returns true if an item was replaced in the array.</returns>
        public bool Add(T inItem, out T outItem)
        {
            if (++this.index == this.array.Length)
            {
                this.index = 0;
                this.allSet = true;
            }

            outItem = array[this.index];
            array[this.index] = inItem;

            return allSet;
        }

        /// <summary>
        /// Returns the item in the array with an index relative to the most recent.
        /// Will only accept values between -Length and 0.
        /// </summary>
        /// <param name="index">Index of item relative to index of most recent item.</param>
        /// <returns>The item of index relative to the most recent item.</returns>
        public T this[int index]
        {
            get
            {
                if (index > 0 || index <= -array.Length || (!allSet && index < -this.index))
                    throw new ArgumentOutOfRangeException("index");

                index += this.index;

                if (index < -array.Length)
                    index += array.Length;

                return this.array[index];
            }
            set
            {
                if (index > 0 || index <= -array.Length || (!allSet && index < -this.index))
                    throw new ArgumentOutOfRangeException("index");

                index += this.index;

                if (index < -array.Length)
                    index += array.Length;

                this.array[index] = value;
            }
        }

        /// <summary>
        /// Index of the most recent item added to the array.
        /// </summary>
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        /// Is true if all items in the array have been set at least once.
        /// </summary>
        public bool AllSet
        {
            get { return this.allSet; }
        }

        /// <summary>
        /// Gets the size of the internal array.
        /// </summary>
        public int Length
        {
            get { return this.array.Length; }
        }

        /// <summary>
        /// Gets the internal array.
        /// </summary>
        public T[] Array
        {
            get { return this.array; }
        }

        /// <summary>
        /// Gets an enumerater of the internal array.
        /// </summary>
        /// <returns>The enumerator of the internal array.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)array).GetEnumerator();
        }
    }
}
