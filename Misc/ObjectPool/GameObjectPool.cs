using System;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Object pooling system to allow performant reuse of frequently created GameObject instances.
    /// </summary>
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObjectPool(GameObject prefab, IEnumerable<GameObject> initialPool, Func<GameObject, GameObject> gameObjectInstantiator = null, Action<GameObject> onAddGameObject = null, Action<GameObject> onGetGameObject = null, Action<GameObject> onRemoveGameObject = null)
            : base(prefab, initialPool, gameObjectInstantiator, onAddGameObject, onGetGameObject, onRemoveGameObject)
        { }

        public GameObjectPool(GameObject prefab, int initialCapacity, Func<GameObject, GameObject> gameObjectInstantiator = null, Action<GameObject> onAddGameObject = null, Action<GameObject> onGetGameObject = null, Action<GameObject> onRemoveGameObject = null)
            : base(prefab, initialCapacity, gameObjectInstantiator, onAddGameObject, onGetGameObject, onRemoveGameObject)
        { }

        public GameObjectPool(GameObject prefab, Func<GameObject, GameObject> gameObjectInstantiator = null, Action<GameObject> onAddGameObject = null, Action<GameObject> onGetGameObject = null, Action<GameObject> onRemoveGameObject = null)
            : this(prefab, null, gameObjectInstantiator, onAddGameObject, onGetGameObject, onRemoveGameObject)
        { }

        public GameObjectPool() : base() { }

        /// <summary>
        /// Default callback implementation for instantiating new instances of GameObject.
        /// </summary>
        public override GameObject DefaultItemGenerator(GameObject prefab)
        {
            return UnityEngine.Object.Instantiate(prefab);
        }

        /// <summary>
        /// Default callback implementation when an instance of GameObject is added to the pool.
        /// </summary>
        public override void DefaultAddItem(GameObject item)
        {
            item.SetActive(false);
        }

        /// <summary>
        /// Default callback implementation when an instance of GameObject is retrieved from the pool.
        /// </summary>
        public override void DefaultGetItem(GameObject item)
        {
            item.SetActive(true);
        }

        /// <summary>
        /// Default callback implementation when an instance of GameObject is removed.
        /// </summary>
        public override void DefaultRemoveItem(GameObject item)
        {
            UnityEngine.Object.Destroy(item);
        }
    }
}
