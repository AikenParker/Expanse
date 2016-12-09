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
        /// <summary>
        /// Copies over field and property values from one component to another.
        /// </summary>
        /// <returns>The source object with newly changed properties and fields.</returns>
        public static T CopyComponent<T>(this Component source, T other) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (other == null)
                throw new ArgumentNullException("other");

            Type type = source.GetType();

            // Make sure each type is the same
            if (type != other.GetType())
                throw new ArgumentException("Other must be the same type as the source.");

            // Using reflection get the type's properties and fields
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] propertyMembers = type.GetProperties(flags);
            FieldInfo[] fieldMembers = type.GetFields(flags);

            // Set the property values
            foreach (var member in propertyMembers)
            {
                // Check if the property is writable and not obsolete
                if (member.CanWrite && !member.IsDefined(typeof(ObsoleteAttribute), true))
                {
                    try { member.SetValue(source, member.GetValue(other, null), null); }
                    catch { } // Just in case of an exception throw
                }
            }

            // Set the field values
            foreach (var member in fieldMembers)
            {
                // Again make sure the field is not obsolete
                if (!member.IsDefined(typeof(ObsoleteAttribute), true))
                    member.SetValue(source, member.GetValue(other));
            }

            return source as T;
        }
    }
}
