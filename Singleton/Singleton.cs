using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Singleton non-generic base class. Inherit from Singleton<T> instead.
    /// </summary>
    public abstract class Singleton : MonoBehaviour { }

    /// <summary>
    /// Singleton base class.
    /// </summary>
    public abstract class Singleton<T> : Singleton where T : Singleton<T>
    {
        private static T instance;
        private readonly static object @lock = new object();

        public static T Instance
        {
            get
            {
                if (isDestroyed)
                {
                    Debug.LogWarning("Attempted to get a destroyed singleton instance");
                    return null;
                }

                lock (@lock)
                {
                    if (!instance)
                    {
                        SingletonManager singletonManager = SingletonManager.SafeInstance;

                        instance = singletonManager.GetSingletonInstance<T>();
                    }
                }

                return instance ? (T)instance : null;
            }
        }

        protected static T SafeInstance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    Type singletonType = typeof(T);
                    GameObject singletonGameObject = new GameObject(singletonType.Name, singletonType);
                    singletonGameObject.hideFlags = HideFlags.HideInHierarchy;
                    instance = singletonGameObject.GetComponent<T>();
                }

                return instance;
            }
        }

#pragma warning disable 67
        public static event Action Destroyed;
#pragma warning restore

        private static bool isDestroyed;

        public static bool IsAvailable
        {
            get { return !isDestroyed; }
        }

        protected virtual void OnDestroy()
        {
            if (this == instance)
            {
                if (Destroyed != null)
                {
                    Destroyed();
                    Destroyed = null;
                }

                isDestroyed = true;
            }
        }
    }
}