using System;
using System.Collections.Generic;
using System.Reflection;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Reflection related utility functionality.
    /// </summary>
    public static class ReflectionUtil
    {
        public const string NULL_TYPE_NAME = "null";

        private static Assembly[] assemblies;
        private static Type[] types;

        /// <summary>
        /// Array containing all Assemblies on the CurrentDomain.
        /// </summary>
        public static Assembly[] Assemblies
        {
            get
            {
                if (assemblies == null)
                {
                    assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }

                return assemblies;
            }
        }

        /// <summary>
        /// Array containing all Types on the CurrentDomain.
        /// </summary>
        public static Type[] Types
        {
            get
            {
                if (types == null)
                {
                    List<Type> typeList = new List<Type>();
                    int length = 0;
                    int capacity = typeList.Capacity;

                    Assembly[] assemblies = ReflectionUtil.Assemblies;

                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        Assembly assembly = assemblies[i];

                        Type[] assemblyTypes = assembly.GetTypes();
                        length += assemblyTypes.Length;

                        if (capacity < length)
                        {
                            typeList.Capacity = capacity = length;
                        }

                        for (int j = 0; j < assemblyTypes.Length; j++)
                        {
                            Type assemblyType = assemblyTypes[j];
                            typeList.Add(assemblyType);
                        }
                    }

                    types = new Type[length];

                    for (int i = 0; i < length; i++)
                    {
                        types[i] = typeList[i];
                    }
                }

                return types;
            }
        }

        /// <summary>
        /// Gets a Type from a full type name.
        /// </summary>
        /// <param name="typeName">Full name of the type to get.</param>
        /// <returns>Returns the type from a full name.</returns>
        public static Type GetTypeFromName(string typeName)
        {
            if (typeName.Equals(NULL_TYPE_NAME))
                return null;

            for (int i = 1; i < Types.Length; i++)
            {
                if (Types[i].FullName.Equals(typeName))
                    return Types[i];
            }

            return null;
        }

        /// <summary>
        /// Gets the backing field info of an auto-property.
        /// </summary>
        /// <param name="propertyInfo">Auto property info with a backing field.</param>
        /// <returns>Returns the backing field info for the auto property.</returns>
        public static FieldInfo GetAutoPropertyBackingField(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("proeprty must be readable to have a backing field");

            Type declaringType = propertyInfo.DeclaringType;
            string backingFieldName = string.Format("<{0}>k__BackingField", propertyInfo.Name);

            FieldInfo backingFieldInfo = declaringType.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

            if (backingFieldInfo == null)
                throw new InvalidArgumentException("propertyInfo does not have a backing field");

            return backingFieldInfo;
        }

        /// <summary>
        /// Trys to get the backing field info of an auto-property.
        /// </summary>
        /// <param name="propertyInfo">Auto property info with a backing field.</param>
        /// <param name="backingFieldInfo">The backing field for the auto property.</param>
        /// <returns>Returns true if backing field info is assigned.</returns>
        public static bool TryGetAutoPropertyBackingField(PropertyInfo propertyInfo, out FieldInfo backingFieldInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("proeprty must be readable to have a backing field");

            Type declaringType = propertyInfo.DeclaringType;
            string backingFieldName = string.Format("<{0}>k__BackingField", propertyInfo.Name);

            backingFieldInfo = declaringType.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

            return backingFieldInfo != null;
        }
    }
}
