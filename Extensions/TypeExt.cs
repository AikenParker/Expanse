using System;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Type related extension methods.
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// Determines if a type can be assigned into another type.
        /// </summary>
        /// <param name="type">Type to check assignment.</param>
        /// <param name="other">Type to check assignment into.</param>
        /// <returns>Returns true if type type can be assigned into type other.</returns>
        public static bool IsAssignableTo(this Type type, Type other)
        {
            return other.IsAssignableFrom(type);
        }
    }
}
