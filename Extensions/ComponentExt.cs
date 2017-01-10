using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public static class ComponentExt
    {
        private const BindingFlags COPY_COMPONENT_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

        private static Dictionary<Type, CopyComponentCacheInfo> copyComponentCache = new Dictionary<Type, CopyComponentCacheInfo>();

        private struct CopyComponentCacheInfo
        {
            public FieldInfo[] fieldMembers;
            public PropertyInfo[] propertyMembers;
        }

        /// <summary>
        /// Copies over field and property values from one component to another.
        /// </summary>
        /// <returns>The source object with newly changed properties and fields.</returns>
        public static T CopyComponent<T>(this T source, T other, bool useCache = true) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (other == null)
                throw new ArgumentNullException("other");

            // Using reflection or the cache to get the properties and fields
            Type type = source.GetType();

            FieldInfo[] fieldMembers;
            PropertyInfo[] propertyMembers;

            if (useCache)
            {
                if (copyComponentCache.ContainsKey(type))
                {
                    CopyComponentCacheInfo cacheInfo = copyComponentCache[type];

                    fieldMembers = cacheInfo.fieldMembers;
                    propertyMembers = cacheInfo.propertyMembers;
                }
                else
                {
                    CopyComponentCacheInfo cacheInfo = new CopyComponentCacheInfo();

                    cacheInfo.fieldMembers = fieldMembers = type.GetFields(COPY_COMPONENT_BINDING_FLAGS).Where(x => !x.HasAttribute<ObsoleteAttribute>()).ToArray();
                    cacheInfo.propertyMembers = propertyMembers = type.GetProperties(COPY_COMPONENT_BINDING_FLAGS).Where(x => !x.HasAttribute<ObsoleteAttribute>() && x.CanWrite).ToArray();

                    copyComponentCache.Add(type, cacheInfo);
                }
            }
            else
            {
                fieldMembers = type.GetFields(COPY_COMPONENT_BINDING_FLAGS).Where(x => !x.HasAttribute<ObsoleteAttribute>()).ToArray();
                propertyMembers = type.GetProperties(COPY_COMPONENT_BINDING_FLAGS).Where(x => !x.HasAttribute<ObsoleteAttribute>() && x.CanWrite).ToArray();
            }

            // Set the field values
            for (int i = 0; i < fieldMembers.Length; i++)
            {
                FieldInfo fieldMember = fieldMembers[i];

                fieldMember.SetValue(source, fieldMember.GetValue(other));
            }

            // Set the property values
            for (int i = 0; i < propertyMembers.Length; i++)
            {
                PropertyInfo propertyMember = propertyMembers[i];

                try { propertyMember.SetValue(source, propertyMember.GetValue(other, null), null); }
                catch { } // Just in case of an exception throw
            }

            return source as T;
        }
    }
}
