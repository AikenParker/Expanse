using System;
using System.Collections.Generic;
using Expanse.Misc;
using Expanse.Utilities;
using UnityEngine;

#if UNSAFE
namespace Expanse.Serialization
{
    public unsafe sealed class TinySerializer : IByteSerializer
    {
        private byte[] buffer;
        private int bufferSize;

        private List<CustomTypeResolver> customTypeResolvers;

        public TinySerializer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            this.buffer = new byte[bufferSize];
        }

        public void AddCustomTypeResolver<T>(CustomTypeResolver customTypeResolver)
            where T : new()
        {
            if (customTypeResolver == null)
                throw new ArgumentNullException("customTypeResolver");

            Type tType = typeof(T);

            if (tType != customTypeResolver.type)
                throw new InvalidArgumentException("T must be equal to customTypeResolver.type");

            if (customTypeResolvers == null)
                customTypeResolvers = new List<CustomTypeResolver>();

            for (int i = 0; i < customTypeResolvers.Count; i++)
            {
                CustomTypeResolver ctr = customTypeResolvers[i];

                if (customTypeResolver == ctr || tType == ctr.type)
                {
                    customTypeResolvers[i] = customTypeResolver;
                    return;
                }
            }

            customTypeResolvers.Add(customTypeResolver);
        }

        public TSource Deserialize<TSource>(byte[] data) where TSource : new()
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<TSource>(TSource obj)
        {
            Type tSource = typeof(TSource);

            // Check if we should serialize using a custom type resolver
            {
                if (customTypeResolvers != null)
                {
                    CustomTypeResolver customTypeResolver = null;

                    for (int i = 0; i < customTypeResolvers.Count; i++)
                    {
                        CustomTypeResolver ctr = customTypeResolvers[i];

                        if (tSource == ctr.type)
                        {
                            customTypeResolver = ctr;
                            break;
                        }
                    }

                    if (customTypeResolver != null)
                    {
                        int objSize = customTypeResolver.GetSize(obj);

                        byte[] data = new byte[objSize];

                        fixed (byte* dataPtr = data)
                        {
                            customTypeResolver.Serialize(dataPtr, 0);
                        }

                        return data;
                    }
                }
            }

            // Check if tSource is an in-built supported serialization type
            // Abstract this to allow for infinite depth calls and re-use in the custom section below
            {
                int dataSize = 0;
                object boxedObj = obj;

                // Warning: Lots of if-else blocks. Replace when switching on types is possible
                // TODO: Support strings, arrays and lists
                // * Allow infinite depth in arrays and lists
                // * Store length as a signed numeric value
                // * Consider negative length to be null

                if (tSource == typeof(int))
                {
                    dataSize = sizeof(int);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeInt32((int*)bufferPtr, 0, (int)boxedObj);
                }
                else if (tSource == typeof(float))
                {
                    dataSize = sizeof(float);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeSingle((float*)bufferPtr, 0, (float)boxedObj);
                }
                else if (tSource == typeof(Vector3))
                {
                    dataSize = sizeof(float) * 3;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeVector3((float*)bufferPtr, 0, (Vector3)boxedObj);
                }
                else if (tSource == typeof(Vector2))
                {
                    dataSize = sizeof(float) * 2;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeVector2((float*)bufferPtr, 0, (Vector2)boxedObj);
                }
                else if (tSource == typeof(double))
                {
                    dataSize = sizeof(double);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeDouble((double*)bufferPtr, 0, (double)boxedObj);
                }
                else if (tSource == typeof(Quaternion))
                {
                    dataSize = sizeof(float) * 4;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeQuaternion((float*)bufferPtr, 0, (Quaternion)boxedObj);
                }
                else if (tSource == typeof(short))
                {
                    dataSize = sizeof(short);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeInt16((short*)bufferPtr, 0, (short)boxedObj);
                }
                else if (tSource == typeof(long))
                {
                    dataSize = sizeof(long);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeInt64((long*)bufferPtr, 0, (long)boxedObj);
                }
                else if (tSource == typeof(byte))
                {
                    dataSize = sizeof(byte);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeByte(bufferPtr, 0, (byte)boxedObj);
                }
                else if (tSource == typeof(char))
                {
                    dataSize = sizeof(char);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeChar((char*)bufferPtr, 0, (char)boxedObj);
                }
                else if (tSource == typeof(uint))
                {
                    dataSize = sizeof(uint);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeUInt32((uint*)bufferPtr, 0, (uint)boxedObj);
                }
                else if (tSource == typeof(ushort))
                {
                    dataSize = sizeof(ushort);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeUInt16((ushort*)bufferPtr, 0, (ushort)boxedObj);
                }
                else if (tSource == typeof(ulong))
                {
                    dataSize = sizeof(ulong);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeUInt64((ulong*)bufferPtr, 0, (ulong)boxedObj);
                }
                else if (tSource == typeof(Rect))
                {
                    dataSize = sizeof(float) * 4;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeRect((float*)bufferPtr, 0, (Rect)boxedObj);
                }
                else if (tSource == typeof(Bounds))
                {
                    dataSize = sizeof(float) * 6;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeBounds((float*)bufferPtr, 0, (Bounds)boxedObj);
                }
                else if (tSource == typeof(decimal))
                {
                    dataSize = sizeof(decimal);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeDecimal((decimal*)bufferPtr, 0, (decimal)boxedObj);
                }
                else if (tSource == typeof(sbyte))
                {
                    dataSize = sizeof(sbyte);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeSByte((sbyte*)bufferPtr, 0, (sbyte)boxedObj);
                }
                else if (tSource == typeof(Half))
                {
                    dataSize = sizeof(ushort);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeHalf((ushort*)bufferPtr, 0, (ushort)boxedObj);
                }
                else if (tSource == typeof(Vector4))
                {
                    dataSize = sizeof(float) * 4;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeVector4((float*)bufferPtr, 0, (Vector4)boxedObj);
                }
                else if (tSource == typeof(DateTime))
                {
                    dataSize = sizeof(long);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeDateTime((long*)bufferPtr, 0, (DateTime)boxedObj);
                }
                else if (tSource == typeof(TimeSpan))
                {
                    dataSize = sizeof(long);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeTimeSpan((long*)bufferPtr, 0, (TimeSpan)boxedObj);
                }
                else if (tSource == typeof(DateTimeOffset))
                {
                    dataSize = sizeof(long);
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeDateTimeOffset((long*)bufferPtr, 0, (DateTimeOffset)boxedObj);
                }
                else if (tSource == typeof(IntVector2))
                {
                    dataSize = sizeof(int) * 2;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeIntVector2((int*)bufferPtr, 0, (IntVector2)boxedObj);
                }
                else if (tSource == typeof(IntVector3))
                {
                    dataSize = sizeof(int) * 3;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeIntVector3((int*)bufferPtr, 0, (IntVector3)boxedObj);
                }
                else if (tSource == typeof(IntVector4))
                {
                    dataSize = sizeof(int) * 4;
                    EnsureBufferSize(dataSize);

                    fixed (byte* bufferPtr = buffer)
                        NativeTypeSerializationResolver.SerializeIntVector4((int*)bufferPtr, 0, (IntVector4)boxedObj);
                }

                if (dataSize > 0)
                {
                    byte[] data = new byte[dataSize];

                    fixed (byte* bufferPtr = buffer, dataPtr = data)
                    {
                        for (int i = 0; i < dataSize; i++)
                        {
                            dataPtr[i] = bufferPtr[i];
                        }
                    }

                    return data;
                }
            }

            // Perform standard custom serialization
            {
                throw new NotImplementedException();
            }
        }

        private void EnsureBufferSize(int size)
        {
            if (bufferSize < size)
            {
                int newBufferSize = MathUtil.NextPowerOfTwo(size);

                byte[] newBuffer = new byte[newBufferSize];

                fixed (byte* bufferPtr = buffer, newBufferPtr = newBuffer)
                {
                    for (int i = 0; i < bufferSize; i++)
                    {
                        newBufferPtr[i] = bufferPtr[i];
                    }
                }

                bufferSize = newBufferSize;
                buffer = newBuffer;
            }
        }

        public static SerializationType GetSerializationType(Type type)
        {
            // TODO: Replace with type switching when available

            if (type.IsValueType)
            {
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
                else // If the ValueType is not supported consider it an Object
                    return SerializationType.Object;
            }
            else
            {
                if (type == typeof(string))
                    return SerializationType.String;
                else if (type.IsArray)
                {
                    Type elementType = type.GetElementType();

                    if (elementType.IsClass)
                        return SerializationType.ObjectArray;
                    else
                    {
                        SerializationType elementSerializationType = GetSerializationType(elementType);

                        if (elementSerializationType == SerializationType.Object)
                            return SerializationType.ObjectArray;
                        else
                            return SerializationType.PrimitiveArray;
                    }
                }
                else if (type.IsGenericType)
                {
                    Type typeDefinition = type.GetGenericTypeDefinition();

                    if (typeDefinition == typeof(List<>))
                    {
                        Type[] genericParameters = type.GetGenericArguments();
                        Type elementType = genericParameters[0];

                        if (elementType.IsClass)
                            return SerializationType.ObjectList;
                        else
                        {
                            SerializationType elementSerializationType = GetSerializationType(elementType);

                            if (elementSerializationType == SerializationType.Object)
                                return SerializationType.ObjectList;
                            else
                                return SerializationType.PrimitiveList;
                        }
                    }
                    else if (typeDefinition == typeof(Nullable<>))
                    {
                        Type[] genericParameters = type.GetGenericArguments();
                        Type elementType = genericParameters[0];

                        if (elementType.IsClass)
                            return SerializationType.ObjectNullable;
                        else
                        {
                            SerializationType elementSerializationType = GetSerializationType(elementType);

                            if (elementSerializationType == SerializationType.Object)
                                return SerializationType.ObjectNullable;
                            else
                                return SerializationType.PrimitiveNullable;
                        }
                    }
                    else
                        return SerializationType.Object;
                }
                else
                    return SerializationType.Object;
            }

            throw new UnexpectedException();
        }

        public abstract class CustomTypeResolver
        {
            public readonly Type type;
            public abstract int GetSize(object obj); // Warning: structs get boxed (Replace with Span)
            public abstract void Deserialize(byte* data, int offset);
            public abstract void Serialize(byte* data, int offset);
        }

        public enum SerializationType : byte
        {
            None = 0,
            Object,
            String,
            Byte,
            SByte,
            Int16,
            Int32,
            Int64,
            UInt16,
            UInt32,
            UInt64,
            Half,
            Single,
            Double,
            Char,
            Decimal,
            DateTime,
            DateTimeOffset,
            TimeSpan,
            Vector2,
            Vector3,
            Vector4,
            Quaternion,
            Rect,
            Bounds,
            IntVector2,
            IntVector3,
            IntVector4,
            PrimitiveArray,
            PrimitiveList,
            ObjectArray,
            ObjectList,
            PrimitiveNullable,
            ObjectNullable
        }

        public static class NativeTypeSerializationResolver
        {
            // TODO: Consider supporting Nullable types of these

            public static void SerializeByte(byte* data, int offset, byte value)
            {
                data[offset] = value;
            }

            public static void SerializeSByte(sbyte* data, int offset, sbyte value)
            {
                data[offset] = value;
            }

            public static void SerializeInt16(short* data, int offset, short value)
            {
                data[offset] = value;
            }

            public static void SerializeInt32(int* data, int offset, int value)
            {
                data[offset] = value;
            }

            public static void SerializeInt64(long* data, int offset, long value)
            {
                data[offset] = value;
            }

            public static void SerializeUInt16(ushort* data, int offset, ushort value)
            {
                data[offset] = value;
            }

            public static void SerializeUInt32(uint* data, int offset, uint value)
            {
                data[offset] = value;
            }

            public static void SerializeUInt64(ulong* data, int offset, ulong value)
            {
                data[offset] = value;
            }

            public static void SerializeHalf(ushort* data, int offset, Half value)
            {
                data[offset] = value.value;
            }

            public static void SerializeSingle(float* data, int offset, float value)
            {
                data[offset] = value;
            }

            public static void SerializeDouble(double* data, int offset, double value)
            {
                data[offset] = value;
            }

            public static void SerializeChar(char* data, int offset, char value)
            {
                data[offset] = value;
            }

            public static void SerializeDecimal(decimal* data, int offset, decimal value)
            {
                data[offset] = value;
            }

            public static void SerializeDateTime(long* data, int offset, DateTime value)
            {
                data[offset] = value.Ticks;
            }

            public static void SerializeDateTimeOffset(long* data, int offset, DateTimeOffset value)
            {
                data[offset] = value.Ticks;
            }

            public static void SerializeTimeSpan(long* data, int offset, TimeSpan value)
            {
                data[offset] = value.Ticks;
            }

            public static void SerializeVector2(float* data, int offset, Vector2 value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
            }

            public static void SerializeVector3(float* data, int offset, Vector3 value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
                data[offset + 2] = value.z;
            }

            public static void SerializeVector4(float* data, int offset, Vector4 value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
                data[offset + 2] = value.z;
                data[offset + 3] = value.w;
            }

            public static void SerializeQuaternion(float* data, int offset, Quaternion value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
                data[offset + 2] = value.z;
                data[offset + 3] = value.w;
            }

            public static void SerializeRect(float* data, int offset, Rect value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
                data[offset + 2] = value.width;
                data[offset + 3] = value.height;
            }

            public static void SerializeBounds(float* data, int offset, Bounds value)
            {
                Vector3 center = value.center;

                data[offset + 0] = center.x;
                data[offset + 1] = center.y;
                data[offset + 2] = center.z;

                Vector3 size = value.size;

                data[offset + 3] = size.x;
                data[offset + 4] = size.y;
                data[offset + 5] = size.z;
            }

            public static void SerializeIntVector2(int* data, int offset, IntVector2 value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
            }

            public static void SerializeIntVector3(int* data, int offset, IntVector3 value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
                data[offset + 2] = value.z;
            }

            public static void SerializeIntVector4(int* data, int offset, IntVector4 value)
            {
                data[offset + 0] = value.x;
                data[offset + 1] = value.y;
                data[offset + 2] = value.z;
                data[offset + 3] = value.w;
            }
        }
    }
}
#endif
