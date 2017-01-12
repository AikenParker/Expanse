using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// A collection of Type related extension methods.
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// Determines if a type can be assigned into another type.
        /// </summary>
        public static bool IsAssignableTo(this Type type, Type other)
        {
            return other.IsAssignableFrom(type);
        }
    }
}
