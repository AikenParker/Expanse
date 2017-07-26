using System;

namespace Expanse.Serialization.TinySerialization
{
    public struct SimpleTypeInfo
    {
        public Type type;
        public int typeHashCode;
        public SerializationType serializationType;
        public bool isValueType;
        public bool isPrimitiveType;
        public int primitiveSize;
        public bool isArray;
        public int arrayRank;
        public bool isGenericType;
        public Type genericTypeDefinition;
        public Type[] genericArguments;
        public Type elementType;
        public SerializationType elementSerializationType;
        public bool isElementPrimitiveType;
        public int elementPrimitiveSize;

        public SimpleTypeInfo(Type type, int typeHashCode, SerializationType serializationType, bool isValueType, bool isPrimitiveType, int primitiveSize, bool isArray, int arrayRank, bool isGenericType, Type genericTypeDefinition, Type[] genericArguments, Type elementType, SerializationType elementSerializationType, bool isElementPrimitiveType, int elementPrimitiveSize)
        {
            this.type = type;
            this.typeHashCode = typeHashCode;
            this.serializationType = serializationType;
            this.isValueType = isValueType;
            this.isPrimitiveType = isPrimitiveType;
            this.primitiveSize = primitiveSize;
            this.isArray = isArray;
            this.arrayRank = arrayRank;
            this.isGenericType = isGenericType;
            this.genericTypeDefinition = genericTypeDefinition;
            this.genericArguments = genericArguments;
            this.elementType = elementType;
            this.elementSerializationType = elementSerializationType;
            this.isElementPrimitiveType = isElementPrimitiveType;
            this.elementPrimitiveSize = elementPrimitiveSize;
        }

        public static SimpleTypeInfo GetInfo(Type type)
        {
            bool isValueType = type.IsValueType;

            if (isValueType && type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            int typeHashCode = type.GetHashCode();
            bool isArray = type.IsArray;
            int arrayRank = isArray ? type.GetArrayRank() : 0;
            bool isGenericType = type.IsGenericType;
            Type genericTypeDefinition = isGenericType ? type.GetGenericTypeDefinition() : null;
            Type[] genericArguments = isGenericType ? type.GetGenericArguments() : null;
            bool hasElement = isGenericType || isArray;
            Type elementType = isGenericType ? genericArguments[0] : (isArray ? type.GetElementType() : null);
            SerializationType elementSerializationType = isGenericType || isArray ? TinySerializerUtil.GetSerializationType(elementType) : SerializationType.None;

            SerializationType serializationType = SerializationType.Object;

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
                    if (genericTypeDefinition == SerializationTypeValues.UnboundNullable)
                    {
                        if (TinySerializerUtil.IsSerializationTypePrimitive(elementSerializationType))
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
                    if (TinySerializerUtil.IsSerializationTypePrimitive(elementSerializationType))
                        serializationType = SerializationType.PrimitiveArray;
                    else
                        serializationType = SerializationType.ObjectArray;
                }
                else if (isGenericType)
                {
                    if (genericTypeDefinition == SerializationTypeValues.UnboundList)
                    {
                        if (TinySerializerUtil.IsSerializationTypePrimitive(elementSerializationType))
                            serializationType = SerializationType.PrimitiveList;
                        else
                            serializationType = SerializationType.ObjectList;
                    }
                    else if (genericTypeDefinition == SerializationTypeValues.UnboundNullable)
                    {
                        serializationType = SerializationType.ObjectNullable;
                    }
                }
            }

            bool isPrimitiveType = isValueType ? TinySerializerUtil.IsSerializationTypePrimitive(serializationType) : false;
            int primitiveSize = isPrimitiveType ? TinySerializerUtil.GetPrimitiveTypeSize(serializationType) : 0;

            bool isElementPrimitiveType = hasElement ? TinySerializerUtil.IsSerializationTypePrimitive(elementSerializationType) : false;
            int elementPrimitiveSize = hasElement ? TinySerializerUtil.GetPrimitiveTypeSize(elementSerializationType) : 0;

            return new SimpleTypeInfo(type, typeHashCode, serializationType, isValueType, isPrimitiveType, primitiveSize, isArray, arrayRank, isGenericType, genericTypeDefinition, genericArguments, elementType, elementSerializationType, isElementPrimitiveType, elementPrimitiveSize);
        }
    }

    public static class SimpleTypeInfo<T>
    {
        public static readonly SimpleTypeInfo info = GetInfo();

        public static SimpleTypeInfo GetInfo()
        {
            return SimpleTypeInfo.GetInfo(typeof(T));
        }
    }
}
