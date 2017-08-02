using System;
using System.Reflection;
using Expanse.Misc;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Serialization.TinySerialization
{
    public sealed class AdvancedTypeInfo
    {
        public Type type;
        public bool emittedFieldGetters;
        public bool emittedFieldSetters;
        public bool emittedPropertyGetters;
        public bool emittedPropertySetters;
        public FieldTypeInfo[] fieldTypeInfos;
        public PropertyTypeInfo[] propertyTypeInfos;

        public AdvancedTypeInfo(Type type)
        {
            this.type = type;
        }

        public static AdvancedTypeInfo GetInfo(Type type)
        {
            AdvancedTypeInfo advancedTypeInfo = new AdvancedTypeInfo(type);

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            int fieldInfoCount = fieldInfos.Length;
            advancedTypeInfo.fieldTypeInfos = new FieldTypeInfo[fieldInfoCount];

            for (int i = 0; i < fieldInfoCount; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                FieldTypeInfo fieldTypeInfo = new FieldTypeInfo(fieldInfo);
                advancedTypeInfo.fieldTypeInfos[i] = fieldTypeInfo;
            }

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            int propertyInfoCount = propertyInfos.Length;
            advancedTypeInfo.propertyTypeInfos = new PropertyTypeInfo[propertyInfoCount];

            for (int i = 0; i < propertyInfoCount; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                PropertyTypeInfo propertyTypeInfo = new PropertyTypeInfo(propertyInfo);
                advancedTypeInfo.propertyTypeInfos[i] = propertyTypeInfo;
            }

            return advancedTypeInfo;
        }

        public void EmitFieldGetters()
        {
            for (int i = 0; i < fieldTypeInfos.Length; i++)
            {
                FieldTypeInfo fieldTypeInfo = fieldTypeInfos[i];

                switch (fieldTypeInfo.fieldSerializationType)
                {
                    case SerializationType.String:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<string>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Byte:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<byte>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.SByte:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<sbyte>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Bool:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<bool>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Int16:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<short>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Int32:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<int>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Int64:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<long>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.UInt16:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<ushort>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.UInt32:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<uint>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.UInt64:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<ulong>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Half:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Half>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Single:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<float>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Double:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<double>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Char:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<char>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Decimal:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<decimal>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.DateTime:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<DateTime>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<DateTimeOffset>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.TimeSpan:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<TimeSpan>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Vector2:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Vector2>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Vector3:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Vector3>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Vector4:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Vector4>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Quaternion:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Quaternion>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Rect:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Rect>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Bounds:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Bounds>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.IntVector2:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<IntVector2>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.IntVector3:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<IntVector3>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.IntVector4:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<IntVector4>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate(fieldTypeInfo.fieldInfo);
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + fieldTypeInfo.fieldSerializationType);
                }
            }

            emittedFieldGetters = true;
        }

        public void EmitFieldSetters()
        {
            for (int i = 0; i < fieldTypeInfos.Length; i++)
            {
                FieldTypeInfo fieldTypeInfo = fieldTypeInfos[i];

                switch (fieldTypeInfo.fieldSerializationType)
                {
                    case SerializationType.String:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<string>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Byte:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<byte>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.SByte:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<sbyte>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Bool:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<bool>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Int16:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<short>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Int32:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<int>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Int64:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<long>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.UInt16:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<ushort>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.UInt32:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<uint>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.UInt64:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<ulong>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Half:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Half>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Single:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<float>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Double:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<double>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Char:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<char>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Decimal:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<decimal>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.DateTime:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<DateTime>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<DateTimeOffset>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.TimeSpan:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<TimeSpan>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Vector2:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Vector2>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Vector3:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Vector3>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Vector4:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Vector4>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Quaternion:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Quaternion>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Rect:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Rect>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.Bounds:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<Bounds>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.IntVector2:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<IntVector2>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.IntVector3:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<IntVector3>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.IntVector4:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate<IntVector4>(fieldTypeInfo.fieldInfo);
                        break;
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        fieldTypeInfo.setter = EmitUtil.GenerateFieldSetterDelegate(fieldTypeInfo.fieldInfo);
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + fieldTypeInfo.fieldSerializationType);
                }
            }

            emittedFieldSetters = true;
        }

        public void EmitPropertyGetters()
        {
            for (int i = 0; i < propertyTypeInfos.Length; i++)
            {
                PropertyTypeInfo propertyTypeInfo = propertyTypeInfos[i];

                switch (propertyTypeInfo.propertySerializationType)
                {
                    case SerializationType.String:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<string>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Byte:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<byte>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.SByte:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<sbyte>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Bool:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<bool>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Int16:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<short>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Int32:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<int>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Int64:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<long>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.UInt16:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<ushort>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.UInt32:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<uint>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.UInt64:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<ulong>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Half:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Half>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Single:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<float>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Double:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<double>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Char:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<char>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Decimal:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<decimal>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.DateTime:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<DateTime>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<DateTimeOffset>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.TimeSpan:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<TimeSpan>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Vector2:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Vector2>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Vector3:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Vector3>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Vector4:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Vector4>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Quaternion:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Quaternion>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Rect:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Rect>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Bounds:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<Bounds>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.IntVector2:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<IntVector2>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.IntVector3:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<IntVector3>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.IntVector4:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate<IntVector4>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        propertyTypeInfo.getter = EmitUtil.GeneratePropertyGetterDelegate(propertyTypeInfo.propertyInfo);
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + propertyTypeInfo.propertySerializationType);
                }
            }

            emittedPropertyGetters = true;
        }

        public void EmitPropertySetters()
        {
            for (int i = 0; i < propertyTypeInfos.Length; i++)
            {
                PropertyTypeInfo propertyTypeInfo = propertyTypeInfos[i];

                switch (propertyTypeInfo.propertySerializationType)
                {
                    case SerializationType.String:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<string>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Byte:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<byte>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.SByte:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<sbyte>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Bool:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<bool>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Int16:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<short>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Int32:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<int>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Int64:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<long>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.UInt16:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<ushort>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.UInt32:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<uint>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.UInt64:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<ulong>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Half:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Half>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Single:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<float>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Double:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<double>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Char:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<char>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Decimal:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<decimal>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.DateTime:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<DateTime>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<DateTimeOffset>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.TimeSpan:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<TimeSpan>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Vector2:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Vector2>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Vector3:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Vector3>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Vector4:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Vector4>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Quaternion:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Quaternion>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Rect:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Rect>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.Bounds:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<Bounds>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.IntVector2:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<IntVector2>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.IntVector3:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<IntVector3>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.IntVector4:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate<IntVector4>(propertyTypeInfo.propertyInfo);
                        break;
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        propertyTypeInfo.setter = EmitUtil.GeneratePropertySetterDelegate(propertyTypeInfo.propertyInfo);
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + propertyTypeInfo.propertySerializationType);
                }
            }

            emittedPropertySetters = true;
        }

        public sealed class FieldTypeInfo
        {
            public FieldInfo fieldInfo;
            public Type fieldType;
            public SerializationType fieldSerializationType;
            public Delegate getter;
            public Delegate setter;

            public FieldTypeInfo(FieldInfo fieldInfo)
            {
                this.fieldInfo = fieldInfo;
                this.fieldType = fieldInfo.FieldType;
                this.fieldSerializationType = TinySerializerUtil.GetSerializationType(fieldType);
            }
        }

        public sealed class PropertyTypeInfo
        {
            public PropertyInfo propertyInfo;
            public Type propertyType;
            public SerializationType propertySerializationType;
            public Delegate getter;
            public Delegate setter;

            public PropertyTypeInfo(PropertyInfo propertyInfo)
            {
                this.propertyInfo = propertyInfo;
                this.propertyType = propertyInfo.PropertyType;
                this.propertySerializationType = TinySerializerUtil.GetSerializationType(propertyType);
            }
        }
    }

    public static class AdvancedTypeInfo<T>
    {
        public static readonly AdvancedTypeInfo info = GetInfo();

        private static AdvancedTypeInfo GetInfo()
        {
            return AdvancedTypeInfo.GetInfo(typeof(T));
        }
    }
}
