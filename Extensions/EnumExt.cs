using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Expanse
{
    namespace Ext
    {
        /// <summary>
        /// Enumeration utility extension methods.
        /// </summary>
        public static class EnumExt
        {
            private static void CheckIsEnum<T>(bool withFlags)
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
                if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                    throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
            }

            public static bool IsFlagSet<T>(this T value, T flag) where T : struct, IFormattable, IConvertible, IComparable
            {
                CheckIsEnum<T>(true);
                long lValue = Convert.ToInt64(value);
                long lFlag = Convert.ToInt64(flag);
                return (lValue & lFlag) != 0;
            }

            public static IEnumerable<T> GetFlags<T>(this T value) where T : struct, IFormattable, IConvertible, IComparable
            {
                CheckIsEnum<T>(true);
                foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
                {
                    if (value.IsFlagSet(flag))
                        yield return flag;
                }
            }

            public static T SetFlags<T>(this T value, T flags, bool on) where T : struct, IFormattable, IConvertible, IComparable
            {
                CheckIsEnum<T>(true);
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

            public static T SetFlags<T>(this T value, T flags) where T : struct, IFormattable, IConvertible, IComparable
            {
                return value.SetFlags(flags, true);
            }

            public static T ClearFlags<T>(this T value, T flags) where T : struct, IFormattable, IConvertible, IComparable
            {
                return value.SetFlags(flags, false);
            }

            public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct, IFormattable, IConvertible, IComparable
            {
                CheckIsEnum<T>(true);
                long lValue = 0;
                foreach (T flag in flags)
                {
                    long lFlag = Convert.ToInt64(flag);
                    lValue |= lFlag;
                }
                return (T)Enum.ToObject(typeof(T), lValue);
            }

            public static string GetDescription<T>(this T value) where T : struct, IFormattable, IConvertible, IComparable
            {
                CheckIsEnum<T>(false);
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
}
