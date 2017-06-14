using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection Enum related extension methods.
    /// </summary>
    public static class EnumExt
    {
        // Throws an ArgumentException if supplied type is not an Enum.
        private static void ThrowIfNotEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
        }

        /// <summary>
        /// Returns true if the flag on enum value is set.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="value">Current value of the enum.</param>
        /// <param name="flag">Enum flag to check for.</param>
        /// <returns>Returns true if value has the flag value set.</returns>
        public static bool HasFlag<T>(this T value, T flag) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        /// <summary>
        /// Returns all flags set on enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="value">Current value of the enum.</param>
        /// <returns>Returns all individual set flags of the value.</returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum<T>(true);
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (value.HasFlag(flag))
                    yield return flag;
            }
        }

        /// <summary>
        /// Sets the flags on enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="value">Current value of the enum.</param>
        /// <param name="flags">Enum flags to set the state of.</param>
        /// <param name="state">If the flags should be set or unset.</param>
        /// <returns>Returns a new value with the specified flags set to state.</returns>
        public static T SetFlags<T>(this T value, T flags, bool state) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (state)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        /// <summary>
        /// Raises the flags on enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="value">Current value of the enum.</param>
        /// <param name="flags">Enum flags to set the state of.</param>
        /// <returns>Returns a new value with the specified flags set.</returns>
        public static T SetFlags<T>(this T value, T flags) where T : struct, IFormattable, IConvertible, IComparable
        {
            return value.SetFlags(flags, true);
        }

        /// <summary>
        /// Clears the flags on enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="value">Current value of the enum.</param>
        /// <param name="flags">Enum flags to set the state of.</param>
        /// <returns>Returns a new value with the specified flags unset.</returns>
        public static T ClearFlags<T>(this T value, T flags) where T : struct, IFormattable, IConvertible, IComparable
        {
            return value.SetFlags(flags, false);
        }

        /// <summary>
        /// Combines flags and returns a combined enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="flags">Enum flags values to combine into one.</param>
        /// <returns>Returns a new enum value with combined enum values from the specified flags.</returns>
        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum<T>(true);
            long lValue = 0;
            foreach (T flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }
    }
}
