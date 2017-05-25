#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif

using System;
using System.Reflection;
using Expanse.Utilities;

namespace Expanse.TinySerialization
{
    public class FieldCacheInfo
    {
        private TinySerializer serializer;

        public SupportedFieldType type;
        public FieldInfo fieldInfo;

#if !AOT_ONLY
        public Delegate getter;
        public Delegate setter;
#endif

        public static SupportedFieldType GetFieldType(FieldInfo fieldInfo)
        {
            Type type = fieldInfo.FieldType;

            // --SUPPORTED-TYPE-SWITCH--
            if (type == typeof(int))
                return SupportedFieldType.Int;
            else if (type == typeof(bool))
                return SupportedFieldType.Bool;
            else if (type == typeof(float))
                return SupportedFieldType.Float;
            else if (type == typeof(double))
                return SupportedFieldType.Double;
            else if (type == typeof(char))
                return SupportedFieldType.Char;
            else if (type == typeof(string))
                return SupportedFieldType.String;
            else if (type == typeof(DateTime))
                return SupportedFieldType.DateTime;
            else if (type == typeof(short))
                return SupportedFieldType.Short;
            else if (type == typeof(long))
                return SupportedFieldType.Long;
            else if (type == typeof(uint))
                return SupportedFieldType.UInt;
            else if (type == typeof(ushort))
                return SupportedFieldType.UShort;
            else if (type == typeof(ulong))
                return SupportedFieldType.ULong;
            else if (type == typeof(byte))
                return SupportedFieldType.Byte;
            else if (type == typeof(sbyte))
                return SupportedFieldType.SByte;
            else if (type == typeof(decimal))
                return SupportedFieldType.Decimal;
            else
                return SupportedFieldType.None;
        }

        public FieldCacheInfo(TinySerializer serializer, FieldInfo fieldInfo, SupportedFieldType type)
        {
            this.serializer = serializer;
            this.fieldInfo = fieldInfo;
            this.type = type;
        }

        public TReturn GetValue<TSource, TReturn>(TSource source)
        {
#if AOT_ONLY
            return (TReturn)fieldInfo.GetValue(source);
#else

            Func<TSource, TReturn> getter = this.getter as Func<TSource, TReturn>;

            if (getter == null)
            {
                if (serializer.SerializationInfo.EmitReflection)
                {
                    getter = EmitUtil.GenerateFieldGetterDelegate<TSource, TReturn>(fieldInfo);
                    this.getter = getter;
                }
                else
                {
                    return (TReturn)fieldInfo.GetValue(source);
                }
            }

            return getter(source);
#endif
        }

        public void SetValue<TSource, TValue>(TSource source, TValue value)
        {
#if AOT_ONLY
            fieldInfo.SetValue(source, value);
#else

            Action<TSource, TValue> setter = this.setter as Action<TSource, TValue>;

            if (setter == null)
            {
                if (serializer.SerializationInfo.EmitReflection)
                {
                    setter = EmitUtil.GenerateFieldSetterDelegate<TSource, TValue>(fieldInfo);
                    this.setter = setter;
                }
                else
                {
                    fieldInfo.SetValue(source, value);
                    return;
                }
            }

            setter(source, value);
#endif
        }
    }
}
