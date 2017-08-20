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

            typeInfoCache = new TinySerializerTypeInfo[30];

            // lastTypeInfo must never be null
            lastTypeInfo = AddTypeInfo(SerializationTypeValues.Int32, SerializationTypeHashCodes.Int32);

            // Not strictly required: simply for pre-caching
            AddTypeInfo(SerializationTypeValues.Bool, SerializationTypeHashCodes.Bool);
            AddTypeInfo(SerializationTypeValues.Byte, SerializationTypeHashCodes.Byte);
            AddTypeInfo(SerializationTypeValues.Char, SerializationTypeHashCodes.Char);
            AddTypeInfo(SerializationTypeValues.Double, SerializationTypeHashCodes.Double);
            AddTypeInfo(SerializationTypeValues.Int16, SerializationTypeHashCodes.Int16);
            AddTypeInfo(SerializationTypeValues.Int64, SerializationTypeHashCodes.Int64);
            AddTypeInfo(SerializationTypeValues.Quaternion, SerializationTypeHashCodes.Quaternion);
            AddTypeInfo(SerializationTypeValues.SByte, SerializationTypeHashCodes.SByte);
            AddTypeInfo(SerializationTypeValues.Single, SerializationTypeHashCodes.Single);
            AddTypeInfo(SerializationTypeValues.String, SerializationTypeHashCodes.String);
            AddTypeInfo(SerializationTypeValues.UInt16, SerializationTypeHashCodes.UInt16);
            AddTypeInfo(SerializationTypeValues.UInt32, SerializationTypeHashCodes.UInt32);
            AddTypeInfo(SerializationTypeValues.UInt64, SerializationTypeHashCodes.UInt64);
            AddTypeInfo(SerializationTypeValues.Vector2, SerializationTypeHashCodes.Vector2);
            AddTypeInfo(SerializationTypeValues.Vector3, SerializationTypeHashCodes.Vector3);
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
                int newTypeInfoCacheLength = MathUtil.NextPowerOfTwo(typeInfoCacheCount + 1);

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
        public ArrayAccessInfo arrayAccessInfo;
        public bool isGenericType;
        public Type genericTypeDefinition;
        public int genericTypeDefinitionHashCode;
        public Type[] genericArguments;
        public Type elementType;
        public int elementTypeHashCode;
        public TinySerializerTypeInfo elementTypeInfo;

        public bool inspectedFields;
        public bool inspectedProperties;
        public bool emittedDefaultConstructor;
        public bool emittedObjectDefaultConstructor;
        public bool emittedFieldGetters;
        public bool emittedFieldGettersByTypedRef;
        public bool emittedFieldSetters;
        public bool emittedFieldSettersByTypedRef;
        public bool emittedPropertyGetters;
        public bool emittedPropertyGettersByTypedRef;
        public bool emittedPropertySetters;
        public bool emittedPropertySettersByTypedRef;
        public Delegate defaultConstructor;
        public Delegate objectDefaultConstructor;
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
                if (isEnum)
                {
                    if (underlyingTypeHashCode == SerializationTypeHashCodes.Int32)
                        serializationType = SerializationType.Int32;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.Byte)
                        serializationType = SerializationType.Byte;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.SByte)
                        serializationType = SerializationType.SByte;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.Int16)
                        serializationType = SerializationType.Int16;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.Int64)
                        serializationType = SerializationType.Int64;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.UInt16)
                        serializationType = SerializationType.UInt16;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.UInt32)
                        serializationType = SerializationType.UInt32;
                    else if (underlyingTypeHashCode == SerializationTypeHashCodes.UInt64)
                        serializationType = SerializationType.UInt64;
                }
                else if (typeHashCode == SerializationTypeHashCodes.Int32)
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
            else
            {
                if (serializationType == SerializationType.ObjectArray)
                {
                    arrayAccessInfo = ArrayAccessInfo.GetArrayAccessInfo(elementType, elementTypeHashCode);
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

        public void EmitDefaultConstructor<TSource>() where TSource : new()
        {
            if (emittedDefaultConstructor)
                return;

            emittedDefaultConstructor = true;

            defaultConstructor = EmitUtil.GenerateDefaultConstructorDelegate<TSource>();
        }

        public void EmitObjectDefaultConstructor()
        {
            if (emittedObjectDefaultConstructor)
                return;

            emittedObjectDefaultConstructor = true;

            objectDefaultConstructor = EmitUtil.GenerateDefaultConstructorDelegate(type);
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
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<string>(fieldInfo);
                        break;
                    case SerializationType.Byte:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<byte>(fieldInfo);
                        break;
                    case SerializationType.SByte:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<sbyte>(fieldInfo);
                        break;
                    case SerializationType.Bool:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<bool>(fieldInfo);
                        break;
                    case SerializationType.Int16:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<short>(fieldInfo);
                        break;
                    case SerializationType.Int32:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<int>(fieldInfo);
                        break;
                    case SerializationType.Int64:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<long>(fieldInfo);
                        break;
                    case SerializationType.UInt16:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<ushort>(fieldInfo);
                        break;
                    case SerializationType.UInt32:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<uint>(fieldInfo);
                        break;
                    case SerializationType.UInt64:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<ulong>(fieldInfo);
                        break;
                    case SerializationType.Half:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Half>(fieldInfo);
                        break;
                    case SerializationType.Single:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<float>(fieldInfo);
                        break;
                    case SerializationType.Double:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<double>(fieldInfo);
                        break;
                    case SerializationType.Char:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<char>(fieldInfo);
                        break;
                    case SerializationType.Decimal:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<decimal>(fieldInfo);
                        break;
                    case SerializationType.DateTime:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<DateTime>(fieldInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<DateTimeOffset>(fieldInfo);
                        break;
                    case SerializationType.TimeSpan:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<TimeSpan>(fieldInfo);
                        break;
                    case SerializationType.Vector2:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Vector2>(fieldInfo);
                        break;
                    case SerializationType.Vector3:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Vector3>(fieldInfo);
                        break;
                    case SerializationType.Vector4:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Vector4>(fieldInfo);
                        break;
                    case SerializationType.Quaternion:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Quaternion>(fieldInfo);
                        break;
                    case SerializationType.Rect:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Rect>(fieldInfo);
                        break;
                    case SerializationType.Bounds:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<Bounds>(fieldInfo);
                        break;
                    case SerializationType.IntVector2:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<IntVector2>(fieldInfo);
                        break;
                    case SerializationType.IntVector3:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<IntVector3>(fieldInfo);
                        break;
                    case SerializationType.IntVector4:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate<IntVector4>(fieldInfo);
                        break;
                    // TODO: Use generic delegates for primitive array, list and nullables
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        fieldTypeInfo.getter = EmitUtil.GenerateFieldGetterDelegate(fieldInfo);
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + fieldSerializationType);
                }
            }
        }

        public void EmitFieldGettersByTypedRef()
        {
            if (emittedFieldGettersByTypedRef)
                return;

            emittedFieldGettersByTypedRef = true;

            for (int i = 0; i < fieldTypeInfos.Length; i++)
            {
                FieldTypeInfo fieldTypeInfo = fieldTypeInfos[i];
                SerializationType fieldSerializationType = fieldTypeInfo.fieldTypeInfo.serializationType;
                FieldInfo fieldInfo = fieldTypeInfo.fieldInfo;

                switch (fieldSerializationType)
                {
                    case SerializationType.String:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<string>(fieldInfo);
                        break;
                    case SerializationType.Byte:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<byte>(fieldInfo);
                        break;
                    case SerializationType.SByte:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<sbyte>(fieldInfo);
                        break;
                    case SerializationType.Bool:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<bool>(fieldInfo);
                        break;
                    case SerializationType.Int16:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<short>(fieldInfo);
                        break;
                    case SerializationType.Int32:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<int>(fieldInfo);
                        break;
                    case SerializationType.Int64:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<long>(fieldInfo);
                        break;
                    case SerializationType.UInt16:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<ushort>(fieldInfo);
                        break;
                    case SerializationType.UInt32:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<uint>(fieldInfo);
                        break;
                    case SerializationType.UInt64:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<ulong>(fieldInfo);
                        break;
                    case SerializationType.Half:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Half>(fieldInfo);
                        break;
                    case SerializationType.Single:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<float>(fieldInfo);
                        break;
                    case SerializationType.Double:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<double>(fieldInfo);
                        break;
                    case SerializationType.Char:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<char>(fieldInfo);
                        break;
                    case SerializationType.Decimal:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<decimal>(fieldInfo);
                        break;
                    case SerializationType.DateTime:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<DateTime>(fieldInfo);
                        break;
                    case SerializationType.DateTimeOffset:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<DateTimeOffset>(fieldInfo);
                        break;
                    case SerializationType.TimeSpan:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<TimeSpan>(fieldInfo);
                        break;
                    case SerializationType.Vector2:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Vector2>(fieldInfo);
                        break;
                    case SerializationType.Vector3:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Vector3>(fieldInfo);
                        break;
                    case SerializationType.Vector4:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Vector4>(fieldInfo);
                        break;
                    case SerializationType.Quaternion:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Quaternion>(fieldInfo);
                        break;
                    case SerializationType.Rect:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Rect>(fieldInfo);
                        break;
                    case SerializationType.Bounds:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<Bounds>(fieldInfo);
                        break;
                    case SerializationType.IntVector2:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<IntVector2>(fieldInfo);
                        break;
                    case SerializationType.IntVector3:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<IntVector3>(fieldInfo);
                        break;
                    case SerializationType.IntVector4:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef<IntVector4>(fieldInfo);
                        break;
                    // TODO: Use generic delegates for primitive array, list and nullables
                    case SerializationType.PrimitiveArray:
                    case SerializationType.PrimitiveList:
                    case SerializationType.PrimitiveNullable:
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        fieldTypeInfo.getterByTypedRef = EmitUtil.GenerateFieldGetterDelegateByTypedRef(fieldInfo);
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

            emittedFieldSetters = true;
        }

        public void EmitFieldSettersByTypedRef()
        {
            if (emittedFieldSettersByTypedRef)
                return;

            emittedFieldSettersByTypedRef = true;
        }

        public void EmitPropertyGetters()
        {
            if (emittedPropertyGetters)
                return;

            emittedPropertyGetters = true;
        }

        public void EmitPropertyGettersByTypedRef()
        {
            if (emittedPropertyGettersByTypedRef)
                return;

            emittedPropertyGettersByTypedRef = true;
        }

        public void EmitPropertySetters()
        {
            if (emittedPropertySetters)
                return;

            emittedPropertySetters = true;
        }

        public void EmitPropertySettersByTypedRef()
        {
            if (emittedPropertySettersByTypedRef)
                return;

            emittedPropertySettersByTypedRef = true;
        }

        public sealed class FieldTypeInfo
        {
            public FieldInfo fieldInfo;
            public Type fieldType;
            public int fieldTypeHashCode;
            public TinySerializerTypeInfo fieldTypeInfo;
            public Delegate getter;
            public Delegate setter;
            public Delegate getterByTypedRef;
            public Delegate setterByTypedRef;

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
            public Delegate getterByTypedRef;
            public Delegate setterByTypedRef;

            public PropertyTypeInfo(PropertyInfo propertyInfo)
            {
                this.propertyInfo = propertyInfo;

                propertyType = propertyInfo.PropertyType;
                propertyTypeHashCode = propertyType.TypeHandle.Value.ToInt32();
                propertyTypeInfo = GetTypeInfo(propertyType, propertyTypeHashCode);
            }
        }

        public class ArrayAccessInfo
        {
            private static int arrayAccessInfoCount;
            private static ArrayAccessInfo[] arrayAccessInfoCache;

            static ArrayAccessInfo()
            {
                arrayAccessInfoCache = new ArrayAccessInfo[10];
            }

            public static ArrayAccessInfo GetArrayAccessInfo(Type type, int typeHashCode)
            {
                for (int i = 0; i < arrayAccessInfoCount; i++)
                {
                    ArrayAccessInfo arrayAccessInfo = arrayAccessInfoCache[i];

                    if (arrayAccessInfo.typeHashCode == typeHashCode)
                    {
                        return arrayAccessInfo;
                    }
                }

                ArrayAccessInfo newArrayAccessInfo = new ArrayAccessInfo(type, typeHashCode);

                if (arrayAccessInfoCount == arrayAccessInfoCache.Length)
                {
                    int newArrayAccessInfoCacheLength = MathUtil.NextPowerOfTwo(arrayAccessInfoCount + 1);

                    ArrayAccessInfo[] newArrayAccessInfoCache = new ArrayAccessInfo[newArrayAccessInfoCacheLength];

                    Array.Copy(arrayAccessInfoCache, newArrayAccessInfoCache, arrayAccessInfoCount);
                    arrayAccessInfoCache = newArrayAccessInfoCache;
                }

                arrayAccessInfoCache[arrayAccessInfoCount] = newArrayAccessInfo;
                arrayAccessInfoCount++;
                return newArrayAccessInfo;
            }

            public Type type;
            public int typeHashCode;

            public bool emittedGetterDelegate;
            public bool emittedSetterDelegate;
            public Delegate getter;
            public Delegate setter;

            public ArrayAccessInfo(Type type, int typeHashCode)
            {
                this.type = type;
                this.typeHashCode = typeHashCode;
            }

            public void EmitGetterDelegate()
            {
                if (emittedGetterDelegate)
                    return;

                emittedGetterDelegate = true;

                getter = EmitUtil.GenerateArrayValueGetterDelegate(type);
            }

            public void EmitSetterDelegate()
            {
                if (emittedSetterDelegate)
                    return;

                emittedSetterDelegate = true;
            }
        }
    }
}
#endif
