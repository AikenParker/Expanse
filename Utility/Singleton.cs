using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Expanse;
using System;

namespace Expanse
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        private static object lockObj = new object();
        public static T Instance
        {
            get
            {
                if (IsDestroyed)
                {
                    Debug.LogWarning("Attempted to get destroyed singleton instance");
                    return null;
                }

                lock (lockObj)
                {
                    if (!instance)
                    {
                        T[] allT = FindObjectsOfType<T>();
                        instance = allT.FirstOrDefault();
                        allT.DestroyGameObjects(x => x != instance, true);
                    }

                    if (!instance)
                    {
                        GameObject tempInstance = new GameObject("TEMP", typeof(T));
                        instance = tempInstance.GetComponent<T>().OnCreate();
                        DestroyImmediate(tempInstance);
                    }
                }

                return instance ? (T)instance : null;
            }
            protected set
            {
                instance = value;
            }
        }

#pragma warning disable 67
        public static event Action Destroying;
#pragma warning restore

        public static bool IsDestroyed { get; private set; }

        protected virtual void OnDestroy()
        {
            if (this == instance)
            {
                if (Destroying != null)
                {
                    Destroying();
                    Destroying = null;
                }

                IsDestroyed = true;
            }
        }

        protected virtual T OnCreate()
        {
            Type singletonType = typeof(T);
            GameObject newGameObject = new GameObject(singletonType.Name, singletonType);
            return newGameObject.GetComponent<T>();
        }
    }
}