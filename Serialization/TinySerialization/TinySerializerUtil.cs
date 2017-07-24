using System;
using System.Collections.Generic;
using Expanse.Misc;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Serialization.TinySerialization
{
    public static class TinySerializerUtil
    {
        public static SerializationType GetSerializationType(Type type)
        {
            // TODO: Replace with type switching when available

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
                        // TODO: Cache generic params

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
                    // TODO: Check array rank

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
                        // TODO: Cache generic params

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

        public static bool IsSerializationTypePrimitive(SerializationType serializationType)
        {
            return serializationType != SerializationType.None && serializationType != SerializationType.Object &&
                serializationType != SerializationType.String && serializationType != SerializationType.ObjectArray &&
                serializationType != SerializationType.ObjectList && serializationType != SerializationType.ObjectNullable &&
                serializationType != SerializationType.PrimitiveNullable && serializationType != SerializationType.PrimitiveArray &&
                serializationType != SerializationType.PrimitiveList;
        }

        // Potentially remove this and replace with constants
        public static int GetPrimitiveTypeSize(SerializationType serializationType)
        {
            switch (serializationType)
            {
                case SerializationType.Byte:
                    return sizeof(byte);
                case SerializationType.SByte:
                    return sizeof(sbyte);
                case SerializationType.Bool:
                    return sizeof(bool);
                case SerializationType.Int16:
                    return sizeof(short);
                case SerializationType.Int32:
                    return sizeof(int);
                case SerializationType.Int64:
                    return sizeof(long);
                case SerializationType.UInt16:
                    return sizeof(ushort);
                case SerializationType.UInt32:
                    return sizeof(uint);
                case SerializationType.UInt64:
                    return sizeof(ulong);
                case SerializationType.Half:
                    return sizeof(ushort);
                case SerializationType.Single:
                    return sizeof(float);
                case SerializationType.Double:
                    return sizeof(double);
                case SerializationType.Char:
                    return sizeof(char);
                case SerializationType.Decimal:
                    return sizeof(decimal);
                case SerializationType.DateTime:
                    return sizeof(long);
                case SerializationType.DateTimeOffset:
                    return sizeof(long);
                case SerializationType.TimeSpan:
                    return sizeof(long);
                case SerializationType.Vector2:
                    return sizeof(float) * 2;
                case SerializationType.Vector3:
                    return sizeof(float) * 3;
                case SerializationType.Vector4:
                    return sizeof(float) * 4;
                case SerializationType.Quaternion:
                    return sizeof(float) * 4;
                case SerializationType.Rect:
                    return sizeof(float) * 4;
                case SerializationType.Bounds:
                    return sizeof(float) * 6;
                case SerializationType.IntVector2:
                    return sizeof(int) * 2;
                case SerializationType.IntVector3:
                    return sizeof(int) * 3;
                case SerializationType.IntVector4:
                    return sizeof(int) * 4;
            }

            throw new InvalidArgumentException("serializationType must be a primitive serialization type.");
        }
    }
}
