using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Expanse
{
    /// <summary>
    /// Easy to use object pooler.
    /// </summary>
    public class PooledObject
    {
        public GameObject pooledObject;         // Object to be pooled and cloned (Usually a prefab)
        public Transform parentObject;          // Pooled objects are automatically parented to this transform.
        public int maxObjects;                  // Maximum amount of pooled and active objects allowed in this system.
        public int initialSpawn;                // How many objects to spawn into the pool when initialized.
        public bool wrapNext = true;            // Will retrieve the oldest active object if the pool is empty and the system has reached max objects.

        public Stack<GameObject> pooledStack;
        public HashSet<GameObject> activeObjects;

        public int TotalObjectCount
        {
            get { return pooledStack.Count + activeObjects.Count; }
        }

        private Action<GameObject> SpawnAction;

        protected PooledObject() { }

        public PooledObject(PooledObjectData pooledObjectData, Action<GameObject> SpawnActionVal)
        {
            pooledStack = new Stack<GameObject>();
            activeObjects = new HashSet<GameObject>();
            pooledObject = pooledObjectData.pooledObject;
            parentObject = pooledObjectData.parentObject;
            maxObjects = pooledObjectData.maxObjects;
            initialSpawn = pooledObjectData.initialSpawn;
            wrapNext = pooledObjectData.wrapNext;
            SpawnAction = SpawnActionVal;

            while (TotalObjectCount < initialSpawn && (TotalObjectCount < maxObjects || maxObjects == 0))
            {
                pooledStack.Push(SpawnNew());
            }
        }

        protected virtual GameObject SpawnNew()
        {
            GameObject newObject = GameObject.Instantiate(pooledObject);
            newObject.SetActive(false);
            newObject.transform.SetParent(parentObject);
            if (SpawnAction != null)
                SpawnAction(newObject);
            return newObject;
        }

        public virtual GameObject Next()
        {
            GameObject nextObject = null;
            if (pooledStack.Any())
            {
                // Take from pool first
                nextObject = pooledStack.Pop();
                nextObject.SetActive(true);
                activeObjects.Add(nextObject);
            }
            else if (TotalObjectCount < maxObjects || maxObjects == 0)
            {
                // Spawn more if able
                nextObject = SpawnNew();
                nextObject.SetActive(true);
                activeObjects.Add(nextObject);
            }
            else if (wrapNext && activeObjects.Any())
            {
                // Take from oldest active if allowed
                Pool(activeObjects.SafeGet(0));
                nextObject = pooledStack.Pop();
                nextObject.SetActive(true);
                activeObjects.Add(nextObject);
            }
            return nextObject;
        }

        public virtual void Pool(GameObject objectToPool)
        {
            if (activeObjects.Contains(objectToPool))
            {
                activeObjects.Remove(objectToPool);
                objectToPool.SetActive(false);
                pooledStack.Push(objectToPool);
            }
        }

        /// <summary>
        /// Pair this with a PooledObject if you want to serialize the data.
        /// </summary>
        [System.Serializable]
        public struct PooledObjectData
        {
            [ReadOnly(EditableInEditor = true)]
            public GameObject pooledObject;
            [ReadOnly(EditableInEditor = true)]
            public Transform parentObject;
            [ReadOnly(EditableInEditor = true)]
            public int maxObjects;
            [ReadOnly(EditableInEditor = true)]
            public int initialSpawn;
            [ReadOnly(EditableInEditor = true)]
            public bool wrapNext;
        }
    }
}