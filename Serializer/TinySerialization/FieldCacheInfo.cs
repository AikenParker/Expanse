using System;
using System.Reflection;

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
                return SupportedFieldType.INT;
            else if (type == typeof(bool))
                return SupportedFieldType.BOOL;
            else if (type == typeof(float))
                return SupportedFieldType.FLOAT;
            else if (type == typeof(double))
                return SupportedFieldType.DOUBLE;
            else if (type == typeof(char))
                return SupportedFieldType.CHAR;
            else if (type == typeof(string))
                return SupportedFieldType.STRING;
            else if (type == typeof(DateTime))
                return SupportedFieldType.DATE_TIME;
            else if (type == typeof(short))
                return SupportedFieldType.SHORT;
            else if (type == typeof(long))
                return SupportedFieldType.LONG;
            else if (type == typeof(uint))
                return SupportedFieldType.UINT;
            else if (type == typeof(ushort))
                return SupportedFieldType.USHORT;
            else if (type == typeof(ulong))
                return SupportedFieldType.ULONG;
            else if (type == typeof(byte))
                return SupportedFieldType.BYTE;
            else if (type == typeof(sbyte))
                return SupportedFieldType.SBYTE;
            else if (type == typeof(decimal))
                return SupportedFieldType.DECIMAL;
            else
                return SupportedFieldType.NONE;
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
                    getter = EmitUtil.GenerateGetter<TSource, TReturn>(fieldInfo);
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
                    setter = EmitUtil.GenerateSetter<TSource, TValue>(fieldInfo);
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
