﻿using System;
using System.Collections.Generic;
using Expanse.Extensions;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Manages the finding, prefabrication and auto-creation of other singleton instances.
    /// </summary>
    public class SingletonManager : Singleton<SingletonManager>
    {
        private static SingletonManager singletonManager;

        public static SingletonManager GetSingletonManager()
        {
            if (singletonManager == null)
            {
                singletonManager = FindObjectOfType<SingletonManager>();
            }

            if (singletonManager == null)
            {
                GameObject singletonGameObject = new GameObject(typeof(SingletonManager).Name);
                singletonGameObject.hideFlags = HideFlags.HideInHierarchy;
                singletonManager = singletonGameObject.AddComponent<SingletonManager>();
                Instance = singletonManager;
            }

            return singletonManager;
        }

        public bool autoCreate = true;
        public List<Singleton> singletonPrefabs = new List<Singleton>();

        public T GetSingletonInstance<T>() where T : Singleton<T>
        {
            // First attempt to find a singleton instance in the scene

            T requestedSingletonInstance = FindObjectOfType<T>();

            if (requestedSingletonInstance != null)
                return requestedSingletonInstance;

            // Second attempt to instantiate a new instance from the singleton prefab list

            T requestedSingletonPrefab = null;

            Type requestedType = typeof(T);

            for (int i = 0; i < singletonPrefabs.Count; i++)
            {
                Singleton singletonPrefab = singletonPrefabs[i];

                if (singletonPrefab.GetType().IsAssignableTo(requestedType))
                {
                    requestedSingletonPrefab = (T)singletonPrefab;

                    break;
                }
            }

            if (requestedSingletonPrefab != null)
                requestedSingletonInstance = Instantiate(requestedSingletonPrefab);

            if (requestedSingletonInstance != null)
                return requestedSingletonInstance;

            // Third attempt to instantiate a new singleton instance

            if (autoCreate)
            {
                GameObject singletonGameObject = new GameObject(requestedType.Name, requestedType);
                requestedSingletonInstance = singletonGameObject.GetComponent<T>();
            }

            if (requestedSingletonInstance != null)
                return requestedSingletonInstance;

            throw new MissingReferenceException("Unable to find or create a singleton of type: " + requestedType.Name);
        }
    }
}
