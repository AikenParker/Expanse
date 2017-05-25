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
        /// <summary>
        /// Throws an ArgumentException if supplied type is not an Enum.
        /// </summary>
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
        public static T SetFlags<T>(this T value, T flags, bool on) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
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
        public static T SetFlags<T>(this T value, T flags) where T : struct, IFormattable, IConvertible, IComparable
        {
            return value.SetFlags(flags, true);
        }

        /// <summary>
        /// Clears the flags on enum value.
        /// </summary>
        public static T ClearFlags<T>(this T value, T flags) where T : struct, IFormattable, IConvertible, IComparable
        {
            return value.SetFlags(flags, false);
        }

        /// <summary>
        /// Combines flags and returns a combined enum value.
        /// </summary>
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

        /// <summary>
        /// Returns the description from the DiscriptionAttribute of an enum value.
        /// </summary>
        public static string GetDescription<T>(this T value) where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum<T>(false);
            string name = Enum.GetName(typeof(T), value);
            if (name != null)
            {
                FieldInfo field = typeof(T).GetField(name);
                if (field != null)
                {
                    System.ComponentModel.DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
