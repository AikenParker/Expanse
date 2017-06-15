using System;
using System.Collections.Generic;
using System.Linq;
using Expanse.Extensions;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of UnityEngine.GameObject related utility functionality.
    /// </summary>
    public static class GameObjectUtil
    {
        private static Dictionary<Type, List<Type>> interfaceTypeCache = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Using another component as a source add a component to a game object.
        /// </summary>
        /// <param name="gameObject">Source game object.</param>
        /// <param name="source">Source component to add.</param>
        /// <returns>Returns a new component added onto the game object with vlaues copied from source.</returns>
        public static T AddComponent<T>(GameObject gameObject, T source) where T : Component
        {
            T newComponent = gameObject.AddComponent<T>();
            newComponent.CopyComponent<T>(source);
            return newComponent;
        }

        /// <summary>
        /// Sets the tag of a game object.
        /// </summary>
        /// <param name="gameObject">Source game obejct.</param>
        /// <param name="tag">Tag value to apply onto the source game object.</param>
        public static void SetTag(GameObject gameObject, string tag)
        {
            gameObject.tag = tag;
        }

        /// <summary>
        /// Sets the tag of a game object and all children.
        /// </summary>
        /// <param name="gameObject">Root source game object.</param>
        /// <param name="tag">Tag value to apply to the source root game object and its children.</param>
        public static void SetTagRecursively(GameObject gameObject, string tag)
        {
            gameObject.tag = tag;

            Transform transform = gameObject.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                child.gameObject.SetTagRecursively(tag);
            }
        }

        /// <summary>
        /// Sets the layer of a game object.
        /// </summary>
        /// <param name="gameObject">Source game object.</param>
        /// <param name="layer">Layer value to apply to the source game object.</param>
        public static void SetLayer(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
        }

        /// <summary>
        /// Sets the layer of a game object and all children.
        /// </summary>
        /// <param name="gameObject">Source root game object.</param>
        /// <param name="layer">Layer value to apply to the source game object and its children.</param>
        public static void SetLayerRecursively(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            Transform transform = gameObject.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                child.gameObject.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// Destroys all game objects in a list and then clears it.
        /// </summary>
        /// <param name="source">List of all game objects to destroy.</param>
        /// <param name="immediate">If true use DestroyImmediate() instead of Destroy().</param>
        public static void DestroyGameObjects(IList<GameObject> source, bool immediate = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < source.Count; i++)
            {
                GameObject gameObject = source[i];

                if (gameObject == null)
                    continue;

                if (immediate)
                    GameObject.DestroyImmediate(gameObject);
                else
                    GameObject.Destroy(gameObject);
            }

            source.Clear();
        }

        /// <summary>
        /// Destroys all game objects in a list and then clears it.
        /// </summary>
        /// <param name="source">List of all components with gameobjects to destroy.</param>
        /// <param name="immediate">If true use DestroyImmediate() instead of Destroy().</param>
        public static void DestroyGameObjects<T>(IList<T> source, bool immediate = false) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < source.Count; i++)
            {
                T component = source[i];

                if (component == null)
                    continue;

                GameObject gameObject = component.gameObject;

                if (gameObject == null)
                    continue;

                if (immediate)
                    GameObject.DestroyImmediate(gameObject);
                else
                    GameObject.Destroy(gameObject);
            }

            source.Clear();
        }

        /// <summary>
        /// Destroys all components in a list and then clears it.
        /// </summary>
        /// <param name="source">List of all ocmponents to destroy.</param>
        /// <param name="immediate">If true use DestroyImmediate() instead of Destroy().</param>
        public static void DestroyComponents<T>(IList<T> source, bool immediate = false) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < source.Count; i++)
            {
                T component = source[i];

                if (component == null)
                    continue;

                if (immediate)
                    GameObject.DestroyImmediate(component);
                else
                    GameObject.Destroy(component);
            }

            source.Clear();
        }

        /// <summary>
        /// Clears the InterfaceTypeCache used by FindInterface methods.
        /// </summary>
        public static void ClearInterfaceTypeCache()
        {
            interfaceTypeCache.Clear();
        }

        /// <summary>
        /// Returns the first active loaded interface of Type T.
        /// </summary>
        /// <typeparam name="T">Type of interface to find.</typeparam>
        /// <param name="useCache">If true the interface types cache is used. Recommended if calling multiple times with the same type.</param>
        /// <returns>Returns the first instance of the interface found.</returns>
        public static T FindInterfaceOfType<T>(bool useCache = false) where T : class
        {
            return FindInterfaceOfType(typeof(T), useCache) as T;
        }

        /// <summary>
        /// Returns the first active loaded interface of Type interfaceType.
        /// </summary>
        /// <param name="interfaceType">Type of interface to find.</param>
        /// <param name="useCache">If true the interface types cache is used. Recommended if calling multiple times with the same type.</param>
        /// <returns>Returns the first instance of the interface found.</returns>
        public static UnityEngine.Object FindInterfaceOfType(Type interfaceType, bool useCache = false)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("T must be an interface type");

            List<Type> interfaceTypes = GetInterfaceTypes(interfaceType, useCache);

            for (int i = 0; i < interfaceTypes.Count; i++)
            {
                UnityEngine.Object interfaceObject = UnityEngine.Object.FindObjectOfType(interfaceTypes[i]);

                if (interfaceObject != null)
                    return interfaceObject;
            }

            return null;
        }

        /// <summary>
        /// Returns all active loaded interfaces of Type T.
        /// </summary>
        /// <typeparam name="T">Type of interface to find.</typeparam>
        /// <param name="useCache">If true the interface types cache is used. Recommended if calling multiple times with the same type.</param>
        /// <returns>Returns all instances of the interface found.</returns>
        public static List<T> FindInterfacesOfType<T>(bool useCache = false) where T : class
        {
            return FindInterfacesOfType(typeof(T), useCache).CastToList<UnityEngine.Object, T>();
        }

        /// <summary>
        /// Returns all active loaded interfaces of Type interfaceType.
        /// </summary>
        /// <param name="interfaceType">Type of interface to find.</param>
        /// <param name="useCache">If true the interface types cache is used. Recommended if calling multiple times with the same type.</param>
        /// <returns>Returns all instances of the interface found.</returns>
        public static List<UnityEngine.Object> FindInterfacesOfType(Type interfaceType, bool useCache = false)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("T must be an interface type");

            List<UnityEngine.Object> interfaceObjectList = new List<UnityEngine.Object>();

            List<Type> interfaceTypes = GetInterfaceTypes(interfaceType, useCache);

            for (int i = 0; i < interfaceTypes.Count; i++)
            {
                interfaceObjectList.AddRange(UnityEngine.Object.FindObjectsOfType(interfaceTypes[i]));
            }

            return interfaceObjectList;
        }

        // Returns all class types in domain that implement an interface type and inherit from UnityEngine.Object.
        private static List<Type> GetInterfaceTypes(Type interfaceType, bool useCache)
        {
            if (!useCache || !interfaceTypeCache.ContainsKey(interfaceType))
            {
                List<Type> interfaceTypes = ReflectionUtil.Types.WhereToList(x => x.IsClass && x.IsAssignableTo(typeof(UnityEngine.Object)) && x.GetInterfaces().Contains(interfaceType));

                if (!useCache)
                    return interfaceTypes;
                else
                    interfaceTypeCache.Add(interfaceType, interfaceTypes);
            }

            return interfaceTypeCache[interfaceType];
        }
    }
}
