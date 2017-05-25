using System;
using System.Collections.Generic;
using System.Linq;
using Expanse.Extensions;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of UnityEngine.GameObject related utility functionality.
    /// </summary>
    public static class GameObjectUtil
    {
        private static Dictionary<Type, List<Type>> interfaceTypeCache = new Dictionary<Type, List<Type>>();

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
        public static T FindInterfaceOfType<T>(bool useCache = false) where T : class
        {
            return FindInterfaceOfType(typeof(T), useCache) as T;
        }

        /// <summary>
        /// Returns the first active loaded interface of Type interfaceType.
        /// </summary>
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
        public static List<T> FindInterfacesOfType<T>(bool useCache = false) where T : class
        {
            return FindInterfacesOfType(typeof(T), useCache).CastToList<UnityEngine.Object, T>();
        }

        /// <summary>
        /// Returns all active loaded interfaces of Type interfaceType.
        /// </summary>
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

        /// <summary>
        /// Returns all class types in domain that implement an interface type and inherit from UnityEngine.Object.
        /// </summary>
        private static List<Type> GetInterfaceTypes(Type interfaceType, bool useCache)
        {
            if (!useCache || !interfaceTypeCache.ContainsKey(interfaceType))
            {
                List<Type> interfaceTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass && x.IsAssignableTo(typeof(UnityEngine.Object)) && x.GetInterfaces().Contains(interfaceType)).ToList();

                if (!useCache)
                    return interfaceTypes;
                else
                    interfaceTypeCache.Add(interfaceType, interfaceTypes);
            }

            return interfaceTypeCache[interfaceType];
        }
    }
}
