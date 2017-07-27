using System;
using System.Runtime.CompilerServices;
using Expanse.Utilities;

namespace Expanse.Serialization.TinySerialization
{
    public static class TinySerializerUtil
    {
        public static SerializationType GetSerializationType(Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                    type = Enum.GetUnderlyingType(type);

                if (type == SerializationTypeValues.Int32)
                    return SerializationType.Int32;
                if (type == SerializationTypeValues.Byte)
                    return SerializationType.Byte;
                if (type == SerializationTypeValues.SByte)
                    return SerializationType.SByte;
                if (type == SerializationTypeValues.Bool)
                    return SerializationType.Bool;
                if (type == SerializationTypeValues.Int16)
                    return SerializationType.Int16;
                if (type == SerializationTypeValues.Int64)
                    return SerializationType.Int64;
                if (type == SerializationTypeValues.UInt16)
                    return SerializationType.UInt16;
                if (type == SerializationTypeValues.UInt32)
                    return SerializationType.UInt32;
                if (type == SerializationTypeValues.UInt64)
                    return SerializationType.UInt64;
                if (type == SerializationTypeValues.Half)
                    return SerializationType.Half;
                if (type == SerializationTypeValues.Single)
                    return SerializationType.Single;
                if (type == SerializationTypeValues.Double)
                    return SerializationType.Double;
                if (type == SerializationTypeValues.Char)
                    return SerializationType.Char;
                if (type == SerializationTypeValues.Decimal)
                    return SerializationType.Decimal;
                if (type == SerializationTypeValues.DateTime)
                    return SerializationType.DateTime;
                if (type == SerializationTypeValues.DateTimeOffset)
                    return SerializationType.DateTimeOffset;
                if (type == SerializationTypeValues.TimeSpan)
                    return SerializationType.TimeSpan;
                if (type == SerializationTypeValues.Vector2)
                    return SerializationType.Vector2;
                if (type == SerializationTypeValues.Vector3)
                    return SerializationType.Vector3;
                if (type == SerializationTypeValues.Vector4)
                    return SerializationType.Vector4;
                if (type == SerializationTypeValues.Quaternion)
                    return SerializationType.Quaternion;
                if (type == SerializationTypeValues.Rect)
                    return SerializationType.Rect;
                if (type == SerializationTypeValues.Bounds)
                    return SerializationType.Bounds;
                if (type == SerializationTypeValues.IntVector2)
                    return SerializationType.IntVector2;
                if (type == SerializationTypeValues.IntVector3)
                    return SerializationType.IntVector3;
                if (type == SerializationTypeValues.IntVector4)
                    return SerializationType.IntVector4;

                if (type.IsGenericType)
                {
                    Type typeDefinition = type.GetGenericTypeDefinition();

                    if (typeDefinition == SerializationTypeValues.UnboundNullable)
                    {
                        Type[] genericParameters = type.GetGenericArguments();
                        Type elementType = genericParameters[0];

                        SerializationType elementSerializationType = GetSerializationType(elementType);

                        if (IsSerializationTypePrimitive(elementSerializationType))
                            return SerializationType.PrimitiveNullable;
                        else
                            return SerializationType.ObjectNullable;
                    }
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    return SerializationType.String;
                }
                if (type.IsArray)
                {
                    Type elementType = type.GetElementType();

                    if (elementType.IsClass)
                        return SerializationType.ObjectArray;
                    else
                    {
                        SerializationType elementSerializationType = GetSerializationType(elementType);

                        if (IsSerializationTypePrimitive(elementSerializationType))
                            return SerializationType.PrimitiveArray;

                        return SerializationType.ObjectArray;
                    }
                }
                if (type.IsGenericType)
                {
                    Type typeDefinition = type.GetGenericTypeDefinition();

                    if (typeDefinition == SerializationTypeValues.UnboundList)
                    {
                        Type[] genericParameters = type.GetGenericArguments();
                        Type elementType = genericParameters[0];

                        if (elementType.IsClass)
                            return SerializationType.ObjectList;
                        else
                        {
                            SerializationType elementSerializationType = GetSerializationType(elementType);

                            if (IsSerializationTypePrimitive(elementSerializationType))
                                return SerializationType.PrimitiveList;

                            return SerializationType.ObjectList;
                        }
                    }
                    else if (typeDefinition == SerializationTypeValues.UnboundNullable)
                    {
                        return SerializationType.ObjectNullable;
                    }
                }
            }

            return SerializationType.Object;
        }

#if NET_4_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsSerializationTypePrimitive(SerializationType serializationType)
        {
            return serializationType != SerializationType.None && serializationType != SerializationType.Object &&
                serializationType != SerializationType.String && serializationType != SerializationType.ObjectArray &&
                serializationType != SerializationType.ObjectList && serializationType != SerializationType.ObjectNullable &&
                serializationType != SerializationType.PrimitiveNullable && serializationType != SerializationType.PrimitiveArray &&
                serializationType != SerializationType.PrimitiveList;
        }

#if NET_4_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int GetPrimitiveTypeSize(SerializationType serializationType)
        {
            switch (serializationType)
            {
                case SerializationType.Byte:
                    return SerializationTypeSizes.BYTE;
                case SerializationType.SByte:
                    return SerializationTypeSizes.SBYTE;
                case SerializationType.Bool:
                    return SerializationTypeSizes.BOOL;
                case SerializationType.Int16:
                    return SerializationTypeSizes.INT16;
                case SerializationType.Int32:
                    return SerializationTypeSizes.INT32;
                case SerializationType.Int64:
                    return SerializationTypeSizes.INT64;
                case SerializationType.UInt16:
                    return SerializationTypeSizes.UINT16;
                case SerializationType.UInt32:
                    return SerializationTypeSizes.UINT32;
                case SerializationType.UInt64:
                    return SerializationTypeSizes.UINT64;
                case SerializationType.Half:
                    return SerializationTypeSizes.HALF;
                case SerializationType.Single:
                    return SerializationTypeSizes.SINGLE;
                case SerializationType.Double:
                    return SerializationTypeSizes.DOUBLE;
                case SerializationType.Char:
                    return SerializationTypeSizes.CHAR;
                case SerializationType.Decimal:
                    return SerializationTypeSizes.DECIMAL;
                case SerializationType.DateTime:
                    return SerializationTypeSizes.DATE_TIME;
                case SerializationType.DateTimeOffset:
                    return SerializationTypeSizes.DATE_TIME_OFFSET;
                case SerializationType.TimeSpan:
                    return SerializationTypeSizes.TIME_SPAN;
                case SerializationType.Vector2:
                    return SerializationTypeSizes.VECTOR2;
                case SerializationType.Vector3:
                    return SerializationTypeSizes.VECTOR3;
                case SerializationType.Vector4:
                    return SerializationTypeSizes.VECTOR4;
                case SerializationType.Quaternion:
                    return SerializationTypeSizes.QUATERNION;
                case SerializationType.Rect:
                    return SerializationTypeSizes.RECT;
                case SerializationType.Bounds:
                    return SerializationTypeSizes.BOUNDS;
                case SerializationType.IntVector2:
                    return SerializationTypeSizes.INT_VECTOR2;
                case SerializationType.IntVector3:
                    return SerializationTypeSizes.INT_VECTOR3;
                case SerializationType.IntVector4:
                    return SerializationTypeSizes.INT_VECTOR4;
            }

            throw new InvalidArgumentException("serializationType must be a primitive serialization type.");
        }

#if NET_4_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int GetPrefixLengthSize(int length)
        {
            if (length < sbyte.MaxValue)
                return 1;
            else if (length < short.MaxValue)
                return 3;
            return 5;
        }
    }
}
