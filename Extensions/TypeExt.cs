using System;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Type related extension methods.
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// Determines if a type is of another type.
        /// </summary>
        /// <param name="type">First type to check.</param>
        /// <param name="other">Second type to check.</param>
        /// <param name="inherited">If true base types are checked.</param>
        /// <returns>Returns true if a type is of another type.</returns>
        public static bool IsOfType(this Type type, Type other, bool inherited = true)
        {
            if (type == other)
                return true;

            return inherited ? type.IsAssignableFromOrTo(other) : false;
        }

        /// <summary>
        /// Determines if a type can be assigned to or from another type.
        /// </summary>
        /// <param name="type">First type to check.</param>
        /// <param name="other">Second type to check.</param>
        /// <returns>Returns true if a type can be assigned to or from another type.</returns>
        public static bool IsAssignableFromOrTo(this Type type, Type other)
        {
            if (type == other)
                return true;

            return type.IsAssignableFrom(other) || other.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines if a type can be assigned into another type.
        /// </summary>
        /// <param name="type">Type to check assignment.</param>
        /// <param name="other">Type to check assignment into.</param>
        /// <returns>Returns true if a type can be assigned into another type.</returns>
        public static bool IsAssignableTo(this Type type, Type other)
        {
            return other.IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the base element type of a type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>Returns the base element type of a type.</returns>
        public static Type GetBaseElementType(this Type type)
        {
            while (type.HasElementType)
                type = type.GetElementType();
            return type;
        }
    }
}
