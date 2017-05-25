using System;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Object pooling system to allow performant reuse of frequently created <typeparamref name="T"/> component instances.
    /// </summary>
    public class ComponentPool<T> : ObjectPool<T> where T : Component, new()
    {
        public ComponentPool(T prefab, IEnumerable<T> initialPool, Func<T, T> componentInstantiator = null, Action<T> onAddComponent = null, Action<T> onGetComponent = null, Action<T> onRemoveComponent = null)
            : base(prefab, initialPool, componentInstantiator, onAddComponent, onGetComponent, onRemoveComponent)
        { }

        public ComponentPool(T prefab, int initialCapacity, Func<T, T> componentInstantiator = null, Action<T> onAddComponent = null, Action<T> onGetComponent = null, Action<T> onRemoveComponent = null)
            : base(prefab, initialCapacity, componentInstantiator, onAddComponent, onGetComponent, onRemoveComponent)
        { }

        public ComponentPool(T prefab, Func<T, T> componentInstantiator = null, Action<T> onAddComponent = null, Action<T> onGetComponent = null, Action<T> onRemoveComponent = null)
            : this(prefab, null, componentInstantiator, onAddComponent, onGetComponent, onRemoveComponent)
        { }

        public ComponentPool() : base() { }

        /// <summary>
        /// Default callback implementation for instantiating new instances of Component.
        /// </summary>
        public override T DefaultItemGenerator(T prefab)
        {
            return UnityEngine.Object.Instantiate(prefab);
        }

        /// <summary>
        /// Default callback implementation when an instance of Component is added to the pool.
        /// </summary>
        public override void DefaultAddItem(T item)
        {
            item.gameObject.SetActive(false);
        }

        /// <summary>
        /// Default callback implementation when an instance of Component is retrieved from the pool.
        /// </summary>
        public override void DefaultGetItem(T item)
        {
            item.gameObject.SetActive(true);
        }

        /// <summary>
        /// Default callback implementation when an instance of GameObject is removed.
        /// </summary>
        public override void DefaultRemoveItem(T item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}
