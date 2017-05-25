using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Singleton non-generic base class. Inherit from Singleton<T> instead.
    /// </summary>
    public abstract class Singleton : MonoBehaviour { }

    /// <summary>
    /// Singleton base class that supports Singleton behaviour of type <typeparamref name="T"/>.
    /// </summary>
    public abstract class Singleton<T> : Singleton where T : Singleton<T>
    {
        private static T instance;
        private readonly static object @lock = new object();

        /// <summary>
        /// The instance of type T that is currently the singleton.
        /// </summary>
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
                        SingletonManager singletonManager = SingletonManager.Instance;

                        instance = singletonManager.GetSingletonInstance<T>();
                    }
                }

                return instance ? (T)instance : null;
            }
            protected set { instance = value; }
        }

#pragma warning disable 67
        /// <summary>
        /// Event raised when the singleton of type T is destroyed.
        /// </summary>
        public static event Action Destroyed;
#pragma warning restore

        private static bool isDestroyed;

        /// <summary>
        /// True if the singleton of type T available and not destroyed.
        /// </summary>
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