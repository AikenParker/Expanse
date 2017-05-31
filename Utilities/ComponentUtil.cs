#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Expanse.Extensions;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of UnityEngine.Component related utility functionality.
    /// </summary>
    public static class ComponentUtil
    {
        private static BindingFlags copyComponentBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

        /// <summary>
        /// The binding flags used when finding field and property infos of a given component type.
        /// </summary>
        public static BindingFlags CopyComponentBindingFlags
        {
            get { return copyComponentBindingFlags; }
            set { copyComponentBindingFlags = value; }
        }

        private static Dictionary<Type, CopyComponentCacheInfo> copyComponentCache = new Dictionary<Type, CopyComponentCacheInfo>();

        private class CopyComponentCacheInfo
        {
#if AOT_ONLY
            public FieldInfo[] fieldMembers;
            public PropertyInfo[] propertyMembers;
#else
            public Func<object, object>[] fieldGetters;
            public Action<object, object>[] fieldSetters;
            public Func<object, object>[] propertyGetters;
            public Action<object, object>[] propertySetters;
#endif
        }

        // Gets all valid field infos of a given type
        private static FieldInfo[] GetFieldMembersOfType(Type type)
        {
            return type.GetFields(copyComponentBindingFlags).Where(x => !x.HasAttribute<ObsoleteAttribute>()).ToArray();
        }

        // Gets all valid property infos of a given type
        private static PropertyInfo[] GetPropertyMembersOfType(Type type)
        {
            return type.GetProperties(copyComponentBindingFlags).Where(x => !x.HasAttribute<ObsoleteAttribute>() && x.CanWrite && x.CanRead).ToArray();
        }

        /// <summary>
        /// Clears the cache used by the CopyComponent method.
        /// </summary>
        public static void ClearCopyComponentCache()
        {
            copyComponentCache.Clear();
        }

        /// <summary>
        /// Copies over field and property values from one component to another.
        /// </summary>
        /// <returns>The source object with newly changed properties and fields.</returns>
        public static void CopyComponent<T>(T copyTo, T copyFrom, bool useCache = true) where T : Component
        {
            if (copyTo == null)
                throw new ArgumentNullException("copyTo");

            if (copyFrom == null)
                throw new ArgumentNullException("other");

            // Using reflection or the cache to get the properties and fields
            Type type = copyTo.GetType();

            FieldInfo[] fieldMembers;
            PropertyInfo[] propertyMembers;

#if AOT_ONLY
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

                    cacheInfo.fieldMembers = fieldMembers = GetFieldMembersOfType(type);
                    cacheInfo.propertyMembers = propertyMembers = GetPropertyMembersOfType(type);

                    copyComponentCache.Add(type, cacheInfo);
                }
            }
            else
            {
                fieldMembers = GetFieldMembersOfType(type);
                propertyMembers = GetPropertyMembersOfType(type);
            }

            // Set the field values
            for (int i = 0; i < fieldMembers.Length; i++)
            {
                FieldInfo fieldMember = fieldMembers[i];

                fieldMember.SetValue(copyTo, fieldMember.GetValue(copyFrom));
            }

            // Set the property values
            for (int i = 0; i < propertyMembers.Length; i++)
            {
                PropertyInfo propertyMember = propertyMembers[i];

                try { propertyMember.SetValue(copyTo, propertyMember.GetValue(copyFrom, null), null); }
                catch { } // Just in case of an exception throw
            }
#else
            if (useCache)
            {
                Func<object, object>[] fieldGetters;
                Action<object, object>[] fieldSetters;
                Func<object, object>[] propertyGetters;
                Action<object, object>[] propertySetters;

                int fieldCount;
                int propertyCount;

                if (copyComponentCache.ContainsKey(type))
                {
                    CopyComponentCacheInfo cacheInfo = copyComponentCache[type];

                    fieldGetters = cacheInfo.fieldGetters;
                    fieldSetters = cacheInfo.fieldSetters;
                    propertyGetters = cacheInfo.propertyGetters;
                    propertySetters = cacheInfo.propertySetters;

                    fieldCount = fieldGetters == null ? 0 : fieldGetters.Length;
                    propertyCount = propertyGetters == null ? 0 : propertyGetters.Length;
                }
                else
                {
                    CopyComponentCacheInfo cacheInfo = new CopyComponentCacheInfo();

                    fieldMembers = GetFieldMembersOfType(type);
                    propertyMembers = GetPropertyMembersOfType(type);

                    fieldCount = fieldMembers.Length;
                    propertyCount = propertyMembers.Length;

                    cacheInfo.fieldGetters = fieldGetters = new Func<object, object>[fieldCount];
                    cacheInfo.fieldSetters = fieldSetters = new Action<object, object>[fieldCount];
                    cacheInfo.propertyGetters = propertyGetters = new Func<object, object>[propertyCount];
                    cacheInfo.propertySetters = propertySetters = new Action<object, object>[propertyCount];

                    for (int i = 0; i < fieldCount; i++)
                    {
                        var fieldInfo = fieldMembers[i];
                        fieldGetters[i] = EmitUtil.GenerateFieldGetterDelegate(fieldInfo);
                        fieldSetters[i] = EmitUtil.GenerateFieldSetterDelegate(fieldInfo);
                    }

                    for (int i = 0; i < propertyCount; i++)
                    {
                        var propertyInfo = propertyMembers[i];
                        propertyGetters[i] = EmitUtil.GeneratePropertyGetterDelegate(propertyInfo);
                        propertySetters[i] = EmitUtil.GeneratePropertySetterDelegate(propertyInfo);
                    }

                    copyComponentCache.Add(type, cacheInfo);
                }

                // Set the field values
                for (int i = 0; i < fieldCount; i++)
                {
                    Func<object, object> fieldGetter = fieldGetters[i];
                    Action<object, object> fieldSetter = fieldSetters[i];

                    fieldSetter.Invoke(copyTo, fieldGetter.Invoke(copyFrom));
                }

                // Set the property values
                for (int i = 0; i < propertyCount; i++)
                {
                    Func<object, object> propertyGetter = propertyGetters[i];
                    Action<object, object> propertySetter = propertySetters[i];

                    propertySetter.Invoke(copyTo, propertyGetter.Invoke(copyFrom));
                }
            }
            else
            {
                fieldMembers = GetFieldMembersOfType(type);
                propertyMembers = GetPropertyMembersOfType(type);

                // Set the field values
                for (int i = 0; i < fieldMembers.Length; i++)
                {
                    FieldInfo fieldMember = fieldMembers[i];

                    fieldMember.SetValue(copyTo, fieldMember.GetValue(copyFrom));
                }

                // Set the property values
                for (int i = 0; i < propertyMembers.Length; i++)
                {
                    PropertyInfo propertyMember = propertyMembers[i];

                    try { propertyMember.SetValue(copyTo, propertyMember.GetValue(copyFrom, null), null); }
                    catch { } // Just in case of an exception throw
                }
            }
#endif
        }
    }
}
