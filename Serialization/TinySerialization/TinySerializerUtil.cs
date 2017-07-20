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

                if (type == typeof(int))
                    return SerializationType.Int32;
                else if (type == typeof(byte))
                    return SerializationType.Byte;
                else if (type == typeof(sbyte))
                    return SerializationType.SByte;
                else if (type == typeof(short))
                    return SerializationType.Int16;
                else if (type == typeof(long))
                    return SerializationType.Int64;
                else if (type == typeof(ushort))
                    return SerializationType.UInt16;
                else if (type == typeof(uint))
                    return SerializationType.UInt32;
                else if (type == typeof(ulong))
                    return SerializationType.UInt64;
                else if (type == typeof(Half))
                    return SerializationType.Half;
                else if (type == typeof(float))
                    return SerializationType.Single;
                else if (type == typeof(double))
                    return SerializationType.Double;
                else if (type == typeof(char))
                    return SerializationType.Char;
                else if (type == typeof(decimal))
                    return SerializationType.Decimal;
                else if (type == typeof(DateTime))
                    return SerializationType.DateTime;
                else if (type == typeof(DateTimeOffset))
                    return SerializationType.DateTimeOffset;
                else if (type == typeof(TimeSpan))
                    return SerializationType.TimeSpan;
                else if (type == typeof(Vector2))
                    return SerializationType.Vector2;
                else if (type == typeof(Vector3))
                    return SerializationType.Vector3;
                else if (type == typeof(Vector4))
                    return SerializationType.Vector4;
                else if (type == typeof(Quaternion))
                    return SerializationType.Quaternion;
                else if (type == typeof(Rect))
                    return SerializationType.Rect;
                else if (type == typeof(Bounds))
                    return SerializationType.Bounds;
                else if (type == typeof(IntVector2))
                    return SerializationType.IntVector2;
                else if (type == typeof(IntVector3))
                    return SerializationType.IntVector3;
                else if (type == typeof(IntVector4))
                    return SerializationType.IntVector4;
            }
            else
            {
                if (type == typeof(string))
                {
                    return SerializationType.String;
                }
                else if (type.IsArray)
                {
                    Type elementType = type.GetElementType();

                    if (elementType.IsClass)
                        return SerializationType.ObjectArray;
                    else
                    {
                        SerializationType elementSerializationType = GetSerializationType(elementType);

                        if (IsSerializationTypePrimitive(elementSerializationType))
                            return SerializationType.PrimitiveArray;
                        else
                            return SerializationType.ObjectArray;
                    }
                }
                else if (type.IsGenericType)
                {
                    Type typeDefinition = type.GetGenericTypeDefinition();

                    if (typeDefinition == typeof(List<>))
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
                            else
                                return SerializationType.ObjectList;
                        }
                    }
                    else if (typeDefinition == typeof(Nullable<>))
                    {
                        // TODO: Cache generic params
                        Type[] genericParameters = type.GetGenericArguments();
                        Type elementType = genericParameters[0];

                        if (elementType.IsClass)
                            return SerializationType.ObjectNullable;
                        else
                        {
                            SerializationType elementSerializationType = GetSerializationType(elementType);

                            if (IsSerializationTypePrimitive(elementSerializationType))
                                return SerializationType.PrimitiveNullable;
                            else
                                return SerializationType.ObjectNullable;
                        }
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

        public static int GetPrimitiveTypeSize(SerializationType serializationType)
        {
            switch (serializationType)
            {
                case SerializationType.Byte:
                    return sizeof(byte);
                case SerializationType.SByte:
                    return sizeof(sbyte);
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
