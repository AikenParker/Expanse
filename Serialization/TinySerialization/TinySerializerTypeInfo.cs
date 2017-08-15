#if UNSAFE

using System;
using System.Reflection;
using Expanse.Misc;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Serialization.TinySerialization
{
    public sealed class TinySerializerTypeInfo
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static int typeInfoCacheCount;
        private static TinySerializerTypeInfo[] typeInfoCache;
        private static TinySerializerTypeInfo lastTypeInfo;

        static TinySerializerTypeInfo()
        {
            typeInfoCacheCount = 0;

            typeInfoCache = new TinySerializerTypeInfo[10];

            lastTypeInfo = AddTypeInfo(SerializationTypeValues.Int32, SerializationTypeHashCodes.Int32);
            AddTypeInfo(SerializationTypeValues.Single, SerializationTypeHashCodes.Single);
            AddTypeInfo(SerializationTypeValues.Byte, SerializationTypeHashCodes.Byte);
            AddTypeInfo(SerializationTypeValues.Bool, SerializationTypeHashCodes.Bool);
            AddTypeInfo(SerializationTypeValues.String, SerializationTypeHashCodes.String);
        }

        private static TinySerializerTypeInfo AddTypeInfo(Type type, int typeHashCode)
        {
            TinySerializerTypeInfo newTypeInfo = new TinySerializerTypeInfo(type, typeHashCode);
            typeInfoCache[typeInfoCacheCount] = newTypeInfo;
            typeInfoCacheCount++;
            return newTypeInfo;
        }

        public static TinySerializerTypeInfo GetTypeInfo(Type type)
        {
            int typeHashCode = type.TypeHandle.Value.ToInt32();

            return GetTypeInfo(type, typeHashCode);
        }

        public static TinySerializerTypeInfo GetTypeInfo(Type type, int typeHashCode)
        {
            if (lastTypeInfo.typeHashCode == typeHashCode)
                return lastTypeInfo;

            for (int i = 0; i < typeInfoCacheCount; i++)
            {
                TinySerializerTypeInfo typeInfo = typeInfoCache[i];

                if (typeInfo.typeHashCode == typeHashCode)
                {
                    lastTypeInfo = typeInfo;
                    return typeInfo;
                }
            }

            TinySerializerTypeInfo newTypeInfo = new TinySerializerTypeInfo(type, typeHashCode);

            if (typeInfoCacheCount == typeInfoCache.Length)
            {
                int newTypeInfoCacheLength = MathUtil.NextPowerOfTwo(typeInfoCacheCount);

                TinySerializerTypeInfo[] newTypeInfoCache = new TinySerializerTypeInfo[newTypeInfoCacheLength];

                Array.Copy(typeInfoCache, newTypeInfoCache, typeInfoCacheCount);
                typeInfoCache = newTypeInfoCache;
            }

            typeInfoCache[typeInfoCacheCount] = newTypeInfo;
            typeInfoCacheCount++;
            lastTypeInfo = newTypeInfo;
            return newTypeInfo;
        }

        public Type type;
        public int typeHashCode;
        public bool isEnum;
        public Type underlyingType;
        public int underlyingTypeHashCode;
        public SerializationType serializationType;
        public bool isValueType;
        public bool isPrimitiveType;
        public int primitiveSize;
        public bool isArray;
        public int arrayRank;
        public bool isGenericType;
        public Type genericTypeDefinition;
        public int genericTypeDefinitionHashCode;
        public Type[] genericArguments;
        public Type elementType;
        public int elementTypeHashCode;
        public TinySerializerTypeInfo elementTypeInfo;

        public bool inspectedFields;
        public bool inspectedProperties;
        public bool emittedFieldGetters;
        public bool emittedFieldSetters;
        public bool emittedPropertyGetters;
        public bool emittedPropertySetters;
        public FieldTypeInfo[] fieldTypeInfos;
        public PropertyTypeInfo[] propertyTypeInfos;

        public TinySerializerTypeInfo()
        {
            typeHashCode = -1;
        }

        public TinySerializerTypeInfo(Type type, int typeHashCode)
        {
            this.type = type;
            this.typeHashCode = typeHashCode;

            isValueType = type.IsValueType;
            isEnum = type.IsEnum;

            if (isValueType && isEnum)
            {
                underlyingType = Enum.GetUnderlyingType(type);
                underlyingTypeHashCode = underlyingType.TypeHandle.Value.ToInt32();
            }

            if (!isValueType)
            {
                isArray = type.IsArray;

                if (isArray)
                {
                    arrayRank = type.GetArrayRank();

                    elementType = type.GetElementType();
                    elementTypeHashCode = elementType.TypeHandle.Value.ToInt32();
                }
            }

            isGenericType = type.IsGenericType;

            if (isGenericType)
            {
                genericTypeDefinition = type.GetGenericTypeDefinition();
                genericTypeDefinitionHashCode = genericTypeDefinition.TypeHandle.Value.ToInt32();
                genericArguments = type.GetGenericArguments();

                elementType = genericArguments[0];
                elementTypeHashCode = elementType.TypeHandle.Value.ToInt32();
            }

            if (isArray || isGenericType)
            {
                elementTypeInfo = GetTypeInfo(elementType, elementTypeHashCode);
            }

            serializationType = SerializationType.Object;

            if (isValueType)
            {
                if (typeHashCode == SerializationTypeHashCodes.Int32)
                    serializationType = SerializationType.Int32;
                else if (typeHashCode == SerializationTypeHashCodes.Byte)
                    serializationType = SerializationType.Byte;
                else if (typeHashCode == SerializationTypeHashCodes.SByte)
                    serializationType = SerializationType.SByte;
                else if (typeHashCode == SerializationTypeHashCodes.Bool)
                    serializationType = SerializationType.Bool;
                else if (typeHashCode == SerializationTypeHashCodes.Int16)
                    serializationType = SerializationType.Int16;
                else if (typeHashCode == SerializationTypeHashCodes.Int64)
                    serializationType = SerializationType.Int64;
                else if (typeHashCode == SerializationTypeHashCodes.UInt16)
                    serializationType = SerializationType.UInt16;
                else if (typeHashCode == SerializationTypeHashCodes.UInt32)
                    serializationType = SerializationType.UInt32;
                else if (typeHashCode == SerializationTypeHashCodes.UInt64)
                    serializationType = SerializationType.UInt64;
                else if (typeHashCode == SerializationTypeHashCodes.Half)
                    serializationType = SerializationType.Half;
                else if (typeHashCode == SerializationTypeHashCodes.Single)
                    serializationType = SerializationType.Single;
                else if (typeHashCode == SerializationTypeHashCodes.Double)
                    serializationType = SerializationType.Double;
                else if (typeHashCode == SerializationTypeHashCodes.Char)
                    serializationType = SerializationType.Char;
                else if (typeHashCode == SerializationTypeHashCodes.Decimal)
                    serializationType = SerializationType.Decimal;
                else if (typeHashCode == SerializationTypeHashCodes.DateTime)
                    serializationType = SerializationType.DateTime;
                else if (typeHashCode == SerializationTypeHashCodes.DateTimeOffset)
                    serializationType = SerializationType.DateTimeOffset;
                else if (typeHashCode == SerializationTypeHashCodes.TimeSpan)
                    serializationType = SerializationType.TimeSpan;
                else if (typeHashCode == SerializationTypeHashCodes.Vector2)
                    serializationType = SerializationType.Vector2;
                else if (typeHashCode == SerializationTypeHashCodes.Vector3)
                    serializationType = SerializationType.Vector3;
                else if (typeHashCode == SerializationTypeHashCodes.Vector4)
                    serializationType = SerializationType.Vector4;
                else if (typeHashCode == SerializationTypeHashCodes.Quaternion)
                    serializationType = SerializationType.Quaternion;
                else if (typeHashCode == SerializationTypeHashCodes.Rect)
                    serializationType = SerializationType.Rect;
                else if (typeHashCode == SerializationTypeHashCodes.Bounds)
                    serializationType = SerializationType.Bounds;
                else if (typeHashCode == SerializationTypeHashCodes.IntVector2)
                    serializationType = SerializationType.IntVector2;
                else if (typeHashCode == SerializationTypeHashCodes.IntVector3)
                    serializationType = SerializationType.IntVector3;
                else if (typeHashCode == SerializationTypeHashCodes.IntVector4)
                    serializationType = SerializationType.IntVector4;
                else if (isGenericType)
                {
                    if (genericTypeDefinitionHashCode == SerializationTypeHashCodes.UnboundNullable)
                    {
                        if (TinySerializerUtil.IsSerializationTypePrimitive(elementTypeInfo.serializationType))
                            serializationType = SerializationType.PrimitiveNullable;
                        else
                            serializationType = SerializationType.ObjectNullable;
                    }
                }
            }
            else
            {
                if (typeHashCode == SerializationTypeHashCodes.String)
                {
                    serializationType = SerializationType.String;
                }
                else if (isArray)
                {
                    if (TinySerializerUtil.IsSerializationTypePrimitive(elementTypeInfo.serializationType))
                        serializationType = SerializationType.PrimitiveArray;
                    else
                        serializationType = SerializationType.ObjectArray;
                }
                else if (isGenericType)
                {
                    if (genericTypeDefinitionHashCode == SerializationTypeHashCodes.UnboundList)
                    {
                        if (TinySerializerUtil.IsSerializationTypePrimitive(elementTypeInfo.serializationType))
                            serializationType = SerializationType.PrimitiveList;
                        else
                            serializationType = SerializationType.ObjectList;
                    }
                    else if (genericTypeDefinitionHashCode == SerializationTypeHashCodes.UnboundNullable)
                    {
                        serializationType = SerializationType.ObjectNullable;
                    }
                }
            }

            if (isValueType)
            {
                isPrimitiveType = TinySerializerUtil.IsSerializationTypePrimitive(serializationType);

                if (isPrimitiveType)
                {
                    primitiveSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                }
            }
        }

        public void InspectFields()
        {
            if (inspectedFields)
                return;

            inspectedFields = true;

            FieldInfo[] fieldInfos = type.GetFields(BINDING_FLAGS);
            int fieldInfoCount = fieldInfos.Length;

            fieldTypeInfos = new FieldTypeInfo[fieldInfoCount];

            for (int i = 0; i < fieldInfoCount; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                FieldTypeInfo fieldTypeInfo = new FieldTypeInfo(fieldInfo);
                fieldTypeInfos[i] = fieldTypeInfo;
            }
        }

        public void InspectProperties()
        {
            if (inspectedProperties)
                return;

            inspectedProperties = true;

            PropertyInfo[] propertyInfos = type.GetProperties(BINDING_FLAGS);
            int propertyInfoCount = propertyInfos.Length;

            propertyTypeInfos = new PropertyTypeInfo[propertyInfoCount];

            for (int i = 0; i < propertyInfoCount; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                PropertyTypeInfo propertyTypeInfo = new PropertyTypeInfo(propertyInfo);
                propertyTypeInfos[i] = propertyTypeInfo;
            }
        }

        public void EmitFieldGetters()
        {
            if (emittedFieldGetters)
                return;

            emittedFieldGetters = true;

            for (int i = 0; i < fieldTypeInfos.Length; i++)
            {
                FieldTypeInfo fieldTypeInfo = fieldTypeInfos[i];
                SerializationType fieldSerializationType = fieldTypeInfo.fieldTypeInfo.serializationType;
                FieldInfo fieldInfo = fieldTypeInfo.fieldInfo;

                switch (fieldSerializationType)
                {
                    case SerializationType.String:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<string>(fieldInfo);
                        break;
                    case SerializationType.Byte:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<byte>(fieldInfo);
                        break;
                    case SerializationType.SByte:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<sbyte>(fieldInfo);
                        break;
                    case SerializationType.Bool:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<bool>(fieldInfo);
                        break;
                    case SerializationType.Int16:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<short>(fieldInfo);
                        break;
                    case SerializationType.Int32:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<int>(fieldInfo);
                        break;
                    case SerializationType.Int64:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<long>(fieldInfo);
                        break;
                    case SerializationType.UInt16:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<ushort>(fieldInfo);
                        break;
                    case SerializationType.UInt32:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<uint>(fieldInfo);
                        break;
                    case SerializationType.UInt64:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<ulong>(fieldInfo);
                        break;
                    case SerializationType.Half:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Half>(fieldInfo);
                        break;
                    case SerializationType.Single:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<float>(fieldInfo);
                        break;
                    case SerializationType.Double:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<double>(fieldInfo);
                        break;
                    case SerializationType.Char:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<char>(fieldInfo);
                        break;
                    case SerializationType.Decimal:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<decimal>(fieldInfo);
                        break;
                    case SerializationType.DateTime:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<DateTime>(fieldInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<DateTimeOffset>(fieldInfo);
                        break;
                    case SerializationType.TimeSpan:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<TimeSpan>(fieldInfo);
                        break;
                    case SerializationType.Vector2:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Vector2>(fieldInfo);
                        break;
                    case SerializationType.Vector3:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Vector3>(fieldInfo);
                        break;
                    case SerializationType.Vector4:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Vector4>(fieldInfo);
                        break;
                    case SerializationType.Quaternion:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Quaternion>(fieldInfo);
                        break;
                    case SerializationType.Rect:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Rect>(fieldInfo);
                        break;
                    case SerializationType.Bounds:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Bounds>(fieldInfo);
                        break;
                    case SerializationType.IntVector2:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<IntVector2>(fieldInfo);
                        break;
                    case SerializationType.IntVector3:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<IntVector3>(fieldInfo);
                        break;
                    case SerializationType.IntVector4:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef<IntVector4>(fieldInfo);
                        break;
                    // TODO: Use generic delegates for primitive array, list and nullables
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegateByTypedRef(fieldInfo);
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + fieldSerializationType);
                }
            }
        }

        public void EmitFieldSetters()
        {
            if (emittedFieldSetters)
                return;

            Debug.LogWarning("Not implemented EmitFieldSetters delegate assignment");

            emittedFieldSetters = true;
        }

        public void EmitPropertyGetters()
        {
            if (emittedPropertyGetters)
                return;

            Debug.LogWarning("Not implemented EmitFieldSetters delegate assignment");

            emittedPropertyGetters = true;
        }

        public void EmitPropertySetters()
        {
            if (emittedPropertySetters)
                return;

            Debug.LogWarning("Not implemented EmitFieldSetters delegate assignment");

            emittedPropertySetters = true;
        }

        public sealed class FieldTypeInfo
        {
            public FieldInfo fieldInfo;
            public Type fieldType;
            public int fieldTypeHashCode;
            public TinySerializerTypeInfo fieldTypeInfo;
            public Delegate getter;
            public Delegate setter;

            public FieldTypeInfo(FieldInfo fieldInfo)
            {
                this.fieldInfo = fieldInfo;

                fieldType = fieldInfo.FieldType;
                fieldTypeHashCode = fieldType.TypeHandle.Value.ToInt32();
                fieldTypeInfo = GetTypeInfo(fieldType, fieldTypeHashCode);
            }
        }

        public sealed class PropertyTypeInfo
        {
            public PropertyInfo propertyInfo;
            public Type propertyType;
            public int propertyTypeHashCode;
            public TinySerializerTypeInfo propertyTypeInfo;
            public Delegate getter;
            public Delegate setter;

            public PropertyTypeInfo(PropertyInfo propertyInfo)
            {
                this.propertyInfo = propertyInfo;

                propertyType = propertyInfo.PropertyType;
                propertyTypeHashCode = propertyType.TypeHandle.Value.ToInt32();
                propertyTypeInfo = GetTypeInfo(propertyType, propertyTypeHashCode);
            }
        }
    }
}
#endif
