using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Expanse
{
    /// <summary>
    /// Object pooling system to allow performant reuse of frequently created <typeparamref name="T"/> instances.
    /// </summary>
    public class ObjectPool<T> : IEnumerable<T>, IEnumerable, ICollection<T> where T : new()
    {
        /// <summary>
        /// Instance used as a reference when generating new instances.
        /// </summary>
        public T SourceItem { get; set; }

        /// <summary>
        /// Backing collection of the pool.
        /// </summary>
        protected Queue<T> Pool;

        /// <summary>
        /// Callback for creating new instances of <typeparamref name="T"/>.
        /// </summary>
        public Func<T, T> ItemGenerator;

        /// <summary>
        /// Callback when an instance of <typeparamref name="T"/> is added to the pool.
        /// </summary>
        public Action<T> AddItem;

        /// <summary>
        /// Callback when an instance of <typeparamref name="T"/> is retrieved from the pool.
        /// </summary>
        public Action<T> GetItem;

        /// <summary>
        /// Callback when an instance of <typeparamref name="T"/> is removed from the pool.
        /// </summary>
        public Action<T> RemoveItem;

        public ObjectPool(T sourceItem, IEnumerable<T> initialPool, Func<T, T> itemGenerator = null, Action<T> onAddItem = null, Action<T> onGetItem = null, Action<T> onRemoveItem = null)
        {
            this.SourceItem = sourceItem;

            this.SetCallbacks(itemGenerator, onAddItem, onGetItem, onRemoveItem);

            bool hasInitialPool = initialPool != null;

            if (hasInitialPool)
            {
                foreach (T item in initialPool)
                {
                    if (AddItem != null)
                        AddItem.Invoke(item);
                }
            }

            this.Pool = hasInitialPool ? new Queue<T>(initialPool) : new Queue<T>();
        }

        public ObjectPool(T sourceItem, int initialPoolSize, Func<T, T> itemGenerator = null, Action<T> onAddItem = null, Action<T> onGetItem = null, Action<T> onRemoveItem = null)
            : this(sourceItem, null, itemGenerator, onAddItem, onGetItem, onRemoveItem)
        {
            if (initialPoolSize > 0 && ItemGenerator != null)
            {
                for (int i = 0; i < initialPoolSize; i++)
                {
                    T objectToPool = ItemGenerator.Invoke(SourceItem);

                    if (objectToPool != null)
                    {
                        this.Add(objectToPool);
                    }
                }
            }
        }

        public ObjectPool(T sourceItem, Func<T, T> itemGenerator = null, Action<T> onAddItem = null, Action<T> onGetItem = null, Action<T> onRemoveItem = null)
            : this(sourceItem, null, itemGenerator, onAddItem, onGetItem, onRemoveItem)
        { }

        public ObjectPool()
        {
            this.Pool = new Queue<T>();

            this.SetCallbacks(null, null, null, null);
        }

        /// <summary>
        /// Adds a <typeparamref name="T"/> instance to the pool.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            if (this.Contains(item))
                throw new ArgumentException("Item is already in the pool");

            if (AddItem != null)
                AddItem.Invoke(item);

            Pool.Enqueue(item);
        }

        /// <summary>
        /// Adds multiple instances of <typeparamref name="T"/> to the pool.
        /// </summary>
        /// <param name="items"></param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Returns a <typeparamref name="T"/> instance from the pool and removes it.
        /// </summary>
        public virtual T Next()
        {
            if (Pool.Any())
            {
                T pooledItem = Pool.Dequeue();

                if (GetItem != null)
                    GetItem(pooledItem);

                return pooledItem;
            }
            else if (ItemGenerator != null)
            {
                T newItem = ItemGenerator.Invoke(SourceItem);

                if (AddItem != null)
                    AddItem.Invoke(newItem);

                if (GetItem != null)
                    GetItem.Invoke(newItem);

                return newItem;
            }

            return default(T);
        }

        /// <summary>
        /// Clears all instances of <typeparamref name="T"/> in the pool.
        /// </summary>
        public virtual void Clear()
        {
            foreach (T item in Pool)
            {
                if (RemoveItem != null)
                    RemoveItem.Invoke(item);
            }

            Pool.Clear();
            Pool = null;
        }

        /// <summary>
        /// Default callback implementation for creating new instances of <typeparamref name="T"/>.
        /// </summary>
        public virtual T DefaultItemGenerator(T item)
        {
            return new T();
        }

        /// <summary>
        /// Default callback implementation when an instance of <typeparamref name="T"/> is added to the pool.
        /// </summary>
        public virtual void DefaultAddItem(T item) { }

        /// <summary>
        /// Default callback implementation when an instance of <typeparamref name="T"/> is retrieved from the pool.
        /// </summary>
        public virtual void DefaultGetItem(T item) { }

        /// <summary>
        /// Default callback implementation when an instance of <typeparamref name="T"/> is removed.
        /// </summary>
        public virtual void DefaultRemoveItem(T item) { }

        /// <summary>
        /// Returns true if an item is in the pool.
        /// </summary>
        public bool Contains(T item)
        {
            return Pool.Contains(item);
        }

        /// <summary>
        /// Returns the amount of instances in the pool.
        /// </summary>
        public int Count
        {
            get { return Pool.Count; }
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        bool ICollection<T>.Remove(T item)
        {
            if (this.Contains(item))
            {
                List<T> tempList = Pool.ToList();

                if (RemoveItem != null)
                    RemoveItem(item);

                tempList.Remove(item);

                Pool = new Queue<T>(tempList);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets all item callbacks if they are not null in which case the default callback is set.
        /// </summary>
        protected void SetCallbacks(Func<T, T> itemGenerator, Action<T> onAddItem, Action<T> onGetItem, Action<T> onRemoveItem)
        {
            this.ItemGenerator = itemGenerator ?? DefaultItemGenerator;
            this.AddItem = onAddItem ?? DefaultAddItem;
            this.GetItem = onGetItem ?? DefaultGetItem;
            this.RemoveItem = onRemoveItem ?? DefaultRemoveItem;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)Pool).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)Pool).GetEnumerator();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)this).CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }
    }
}
