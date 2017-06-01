using System;
using System.Collections.Generic;
using System.Linq;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Enum related utility functionality.
    /// </summary>
    public static class EnumUtil
    {
        // Throws an argument exception if passed type is not an enum.
        private static void ThrowIfNotEnum(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", type.FullName));
        }

        /// <summary>
        /// Returns all defined enum values of an enum type.
        /// </summary>
        /// <typeparam name="T">Type of the Enum.</typeparam>
        /// <returns>Returns an IEnumerable of all defined values in the enum type.</returns>
        public static IEnumerable<T> GetValues<T>() where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum(typeof(T));

            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns all defined enum values from an enum type.
        /// </summary>
        /// <param name="enumType">Type of the Enum.</param>
        /// <returns>Returns an IEnumerable of all defined values in the enum type.</returns>
        public static IEnumerable<object> GetValues(Type enumType)
        {
            ThrowIfNotEnum(enumType);

            return Enum.GetValues(enumType).Cast<object>();
        }

        /// <summary>
        /// Attempts to parse a string into an enum type object.
        /// </summary>
        /// <typeparam name="T">Type of the enum to parse to.</typeparam>
        /// <param name="value">Name of the defined enum value.</param>
        /// <param name="enumObj">Enum value that was parsed from the string value.</param>
        /// <returns>Returns true if the string value was able to be parsed.</returns>
        public static bool TryParse<T>(string value, out T enumObj) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum(typeof(T));

            try
            {
                enumObj = (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                enumObj = default(T);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to parse a string into an enum type object.
        /// </summary>
        /// <param name="value">Name of the defined enum value.</param>
        /// <param name="enumType">Type of the enum to parse to.</param>
        /// <param name="enumObj">Enum value that was parsed from the string value.</param>
        /// <returns>Returns true if the string value was able to be parsed.</returns>
        public static bool TryParse(string value, Type enumType, out object enumObj)
        {
            ThrowIfNotEnum(enumType);

            try
            {
                enumObj = Enum.Parse(enumType, value);
            }
            catch
            {
                enumObj = null;
                return false;
            }

            return true;
        }
    }
}
