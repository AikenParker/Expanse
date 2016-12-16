using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Expanse
{
    public class ObjectPool<T> : IEnumerable<T>, IEnumerable, ICollection<T>, ICollection where T : new()
    {
        public T SourceItem { get; set; }

        protected Queue<T> Pool;

        public Func<T, T> ItemGenerator;
        public Action<T> AddItem;
        public Action<T> GetItem;
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

        public virtual void Add(T item)
        {
            if (this.Contains(item))
                throw new ArgumentException("Item is already in the pool");

            if (AddItem != null)
                AddItem.Invoke(item);

            Pool.Enqueue(item);
        }

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

        public virtual T DefaultItemGenerator(T item)
        {
            return new T();
        }

        public virtual void DefaultAddItem(T item)
        {

        }

        public virtual void DefaultGetItem(T item)
        {

        }

        public virtual void DefaultRemoveItem(T item)
        {

        }

        public bool Contains(T item)
        {
            return Pool.Contains(item);
        }

        public int Count
        {
            get { return Pool.Count; }
        }

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
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)Pool).CopyTo(array, index);
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)Pool).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)Pool).SyncRoot; }
        }
    }

    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObjectPool(GameObject prefab, IEnumerable<GameObject> initialPool, Func<GameObject, GameObject> gameObjectInstantiator = null, Action<GameObject> onAddGameObject = null, Action<GameObject> onGetGameObject = null, Action<GameObject> onRemoveGameObject = null)
            : base(prefab, initialPool, gameObjectInstantiator, onAddGameObject, onGetGameObject, onRemoveGameObject)
        { }

        public GameObjectPool(GameObject prefab, int initialCapacity, Func<GameObject, GameObject> gameObjectInstantiator = null, Action<GameObject> onAddGameObject = null, Action<GameObject> onGetGameObject = null, Action<GameObject> onRemoveGameObject = null)
            : this(prefab, null, gameObjectInstantiator, onAddGameObject, onGetGameObject, onRemoveGameObject)
        { }

        public GameObjectPool(GameObject prefab, Func<GameObject, GameObject> gameObjectInstantiator = null, Action<GameObject> onAddGameObject = null, Action<GameObject> onGetGameObject = null, Action<GameObject> onRemoveGameObject = null)
            : this(prefab, null, gameObjectInstantiator, onAddGameObject, onGetGameObject, onRemoveGameObject)
        { }

        public GameObjectPool() : base() { }

        public override GameObject DefaultItemGenerator(GameObject prefab)
        {
            return UnityEngine.Object.Instantiate(prefab);
        }

        public override void DefaultAddItem(GameObject item)
        {
            item.SetActive(false);
        }

        public override void DefaultGetItem(GameObject item)
        {
            item.SetActive(true);
        }

        public override void DefaultRemoveItem(GameObject item)
        {
            UnityEngine.Object.Destroy(item);
        }
    }

    public class ComponentPool<T> : ObjectPool<T> where T : Component, new()
    {
        public ComponentPool(T prefab, IEnumerable<T> initialPool, Func<T, T> componentInstantiator = null, Action<T> onAddComponent = null, Action<T> onGetComponent = null, Action<T> onRemoveComponent = null)
            : base(prefab, initialPool, componentInstantiator, onAddComponent, onGetComponent, onRemoveComponent)
        { }

        public ComponentPool(T prefab, int initialCapacity, Func<T, T> componentInstantiator = null, Action<T> onAddComponent = null, Action<T> onGetComponent = null, Action<T> onRemoveComponent = null)
            : this(prefab, null, componentInstantiator, onAddComponent, onGetComponent, onRemoveComponent)
        { }

        public ComponentPool(T prefab, Func<T, T> componentInstantiator = null, Action<T> onAddComponent = null, Action<T> onGetComponent = null, Action<T> onRemoveComponent = null)
            : this(prefab, null, componentInstantiator, onAddComponent, onGetComponent, onRemoveComponent)
        { }

        public ComponentPool() : base() { }

        public override T DefaultItemGenerator(T prefab)
        {
            return UnityEngine.Object.Instantiate(prefab);
        }

        public override void DefaultAddItem(T item)
        {
            item.gameObject.SetActive(false);
        }

        public override void DefaultGetItem(T item)
        {
            item.gameObject.SetActive(true);
        }

        public override void DefaultRemoveItem(T item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}