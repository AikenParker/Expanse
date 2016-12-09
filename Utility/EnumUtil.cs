using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Expanse
{
    public static class EnumUtil
    {
        private static void ThrowIfNotEnum(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", type.FullName));
        }

        public static IEnumerable<T> GetValues<T>() where T : struct, IFormattable, IConvertible, IComparable
        {
            ThrowIfNotEnum(typeof(T));

            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<object> GetValues(Type enumType)
        {
            ThrowIfNotEnum(enumType);

            return Enum.GetValues(enumType).Cast<object>();
        }

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
