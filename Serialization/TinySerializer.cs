using System;
using System.Collections.Generic;
using Expanse.Misc;
using Expanse.Utilities;
using UnityEngine;

#if UNSAFE
namespace Expanse.Serialization.TinySerialization
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
            int dataSize = SerializeIntoBuffer(obj, 0);

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

        public int Serialize<TSource>(TSource obj, ref byte[] data)
        {
            int dataSize = SerializeIntoBuffer(obj, 0);

            if (data == null || data.Length < dataSize)
            {
                data = new byte[dataSize];
            }

            fixed (byte* bufferPtr = buffer, dataPtr = data)
            {
                for (int i = 0; i < dataSize; i++)
                {
                    dataPtr[i] = bufferPtr[i];
                }
            }

            return dataSize;
        }

        public int Serialize<TSource>(TSource obj, ref byte[] data, int offset)
        {
            int dataSize = SerializeIntoBuffer(obj, 0);

            int offsetDataSize = offset + dataSize;

            if (data == null)
            {
                data = new byte[offsetDataSize];
            }
            else if (data.Length < offsetDataSize)
            {
                byte[] newData = new byte[offsetDataSize];

                fixed (byte* dataPtr = data, newDataPtr = newData)
                {
                    for (int i = 0; i < offset; i++)
                    {
                        newDataPtr[i] = dataPtr[i];
                    }
                }

                data = newData;
            }

            fixed (byte* bufferPtr = buffer, dataPtr = &data[offset])
            {
                for (int i = 0; i < dataSize; i++)
                {
                    dataPtr[i] = bufferPtr[i];
                }
            }

            return offsetDataSize;
        }

        private int SerializeIntoBuffer<TSource>(TSource obj, int offset)
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
                        int dataSize = customTypeResolver.GetSize(obj);

                        EnsureBufferSize(offset + dataSize);

                        fixed (byte* bufferPtr = buffer)
                        {
                            customTypeResolver.Serialize(bufferPtr, offset);
                        }

                        return offset + dataSize;
                    }
                }
            }

            // Perform standard serialization of supported and custom types
            {
                int dataSize = 0;

                SerializationType serializationType = GetSerializationType(tSource);
                bool emitValueTypeCaster = true;

                switch (serializationType)
                {
                    case SerializationType.Byte:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            byte value = CastTo<byte>.From(obj, emitValueTypeCaster);

                            fixed (byte* bufferPtr = buffer)
                                bufferPtr[offset] = value;
                        }
                        break;
                    case SerializationType.SByte:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            sbyte value = CastTo<sbyte>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = buffer)
                            {
                                sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                                bufferPtr[offset] = value;
                            }
                        }
                        break;
                    case SerializationType.Int16:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            short value = CastTo<short>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                short* bufferPtr = (short*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int32:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            int value = CastTo<int>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                int* bufferPtr = (int*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int64:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            long value = CastTo<long>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt16:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            ushort value = CastTo<ushort>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                ushort* bufferPtr = (ushort*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt32:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            uint value = CastTo<uint>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                uint* bufferPtr = (uint*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt64:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            ulong value = CastTo<ulong>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                ulong* bufferPtr = (ulong*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Half:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Half value = CastTo<Half>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                ushort* bufferPtr = (ushort*)byteBufferPtr;
                                *bufferPtr = value.value;
                            }
                        }
                        break;
                    case SerializationType.Single:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            float value = CastTo<float>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Double:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            double value = CastTo<double>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                double* bufferPtr = (double*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Char:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            char value = CastTo<char>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                char* bufferPtr = (char*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Decimal:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            decimal value = CastTo<decimal>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                decimal* bufferPtr = (decimal*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.DateTime:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            DateTime value = CastTo<DateTime>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.DateTimeOffset:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            DateTimeOffset value = CastTo<DateTimeOffset>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.TimeSpan:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            TimeSpan value = CastTo<TimeSpan>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.Vector2:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Vector2 value = CastTo<Vector2>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                            }
                        }
                        break;
                    case SerializationType.Vector3:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Vector3 value = CastTo<Vector3>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                            }
                        }
                        break;
                    case SerializationType.Vector4:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Vector4 value = CastTo<Vector4>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                                *bufferPtr++ = value.w;
                            }
                        }
                        break;
                    case SerializationType.Quaternion:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Quaternion value = CastTo<Quaternion>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                                *bufferPtr++ = value.w;
                            }
                        }
                        break;
                    case SerializationType.Rect:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Rect value = CastTo<Rect>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.width;
                                *bufferPtr++ = value.height;
                            }
                        }
                        break;
                    case SerializationType.Bounds:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Bounds value = CastTo<Bounds>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                Vector3 center = value.center;

                                *bufferPtr++ = center.x;
                                *bufferPtr++ = center.y;
                                *bufferPtr++ = center.z;

                                Vector3 size = value.size;

                                *bufferPtr++ = size.x;
                                *bufferPtr++ = size.y;
                                *bufferPtr++ = size.z;
                            }
                        }
                        break;
                    case SerializationType.IntVector2:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            IntVector2 value = CastTo<IntVector2>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                            }
                        }
                        break;
                    case SerializationType.IntVector3:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            IntVector3 value = CastTo<IntVector3>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                            }
                        }
                        break;
                    case SerializationType.IntVector4:
                        {
                            dataSize = GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            IntVector4 value = CastTo<IntVector4>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                                *bufferPtr++ = value.w;
                            }
                        }
                        break;
                    case SerializationType.String:
                        {

                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            Array baseValue = CastTo<Array>.From(obj, false);
                            bool hasValue = baseValue != null;
                            int length = hasValue ? baseValue.Length : -1;

                            int elementCount = hasValue ? length : 0;
                            Type elementType = tSource.GetElementType();
                            SerializationType elementSerializationType = GetSerializationType(elementType);
                            int elementSize = GetPrimitiveTypeSize(elementSerializationType);

                            int lengthSize = sizeof(int);
                            dataSize = lengthSize + (elementCount * elementSize);
                            EnsureBufferSize(dataSize + offset);

                            fixed (byte* bufferPtr = buffer)
                            {
                                int* intBufferPtr = (int*)bufferPtr;
                                intBufferPtr[offset] = length;
                            }

                            for (int i = 0; i < elementCount; i++)
                            {
                                switch (elementSerializationType)
                                {
                                    case SerializationType.Byte:
                                        {
                                            byte[] value = (byte[])baseValue;

                                            fixed (byte* bufferPtr = &buffer[offset + lengthSize], valuePtr = value)
                                                bufferPtr[i] = valuePtr[i];
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            sbyte[] value = (sbyte[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                sbyte* bufferPtr = (sbyte*)byteBufferPtr;

                                                fixed (sbyte* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            short[] value = (short[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                short* bufferPtr = (short*)byteBufferPtr;

                                                fixed (short* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            int[] value = (int[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                int* bufferPtr = (int*)byteBufferPtr;

                                                fixed (int* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            long[] value = (long[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                fixed (long* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            ushort[] value = (ushort[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                ushort* bufferPtr = (ushort*)byteBufferPtr;

                                                fixed (ushort* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            uint[] value = (uint[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                uint* bufferPtr = (uint*)byteBufferPtr;

                                                fixed (uint* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            ulong[] value = (ulong[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                ulong* bufferPtr = (ulong*)byteBufferPtr;

                                                fixed (ulong* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            Half[] value = (Half[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                ushort* bufferPtr = (ushort*)byteBufferPtr;

                                                fixed (Half* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i].value;
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            float[] value = (float[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (float* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            double[] value = (double[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                double* bufferPtr = (double*)byteBufferPtr;

                                                fixed (double* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            char[] value = (char[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                char* bufferPtr = (char*)byteBufferPtr;

                                                fixed (char* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            decimal[] value = (decimal[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                decimal* bufferPtr = (decimal*)byteBufferPtr;

                                                fixed (decimal* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            DateTime[] value = (DateTime[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                fixed (DateTime* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            DateTimeOffset[] value = (DateTimeOffset[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                fixed (DateTimeOffset* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            TimeSpan[] value = (TimeSpan[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                fixed (TimeSpan* valuePtr = value)
                                                    bufferPtr[i] = valuePtr[i].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            Vector2[] value = (Vector2[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (Vector2* valuePtr = value)
                                                {
                                                    Vector2 elementValue = valuePtr[i];

                                                    bufferPtr[(i * 2) + 0] = elementValue.x;
                                                    bufferPtr[(i * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            Vector3[] value = (Vector3[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (Vector3* valuePtr = value)
                                                {
                                                    Vector3 elementValue = valuePtr[i];

                                                    bufferPtr[(i * 3) + 0] = elementValue.x;
                                                    bufferPtr[(i * 3) + 1] = elementValue.y;
                                                    bufferPtr[(i * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            Vector4[] value = (Vector4[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (Vector4* valuePtr = value)
                                                {
                                                    Vector4 elementValue = valuePtr[i];

                                                    bufferPtr[(i * 4) + 0] = elementValue.x;
                                                    bufferPtr[(i * 4) + 1] = elementValue.y;
                                                    bufferPtr[(i * 4) + 2] = elementValue.z;
                                                    bufferPtr[(i * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            Quaternion[] value = (Quaternion[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (Quaternion* valuePtr = value)
                                                {
                                                    Quaternion elementValue = valuePtr[i];

                                                    bufferPtr[(i * 4) + 0] = elementValue.x;
                                                    bufferPtr[(i * 4) + 1] = elementValue.y;
                                                    bufferPtr[(i * 4) + 2] = elementValue.z;
                                                    bufferPtr[(i * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            Rect[] value = (Rect[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (Rect* valuePtr = value)
                                                {
                                                    Rect elementValue = valuePtr[i];

                                                    bufferPtr[(i * 4) + 0] = elementValue.x;
                                                    bufferPtr[(i * 4) + 1] = elementValue.y;
                                                    bufferPtr[(i * 4) + 2] = elementValue.width;
                                                    bufferPtr[(i * 4) + 3] = elementValue.height;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            Bounds[] value = (Bounds[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (Bounds* valuePtr = value)
                                                {
                                                    Bounds elementValue = valuePtr[i];

                                                    Vector3 center = elementValue.center;
                                                    Vector3 size = elementValue.size;

                                                    bufferPtr[(i * 6) + 0] = center.x;
                                                    bufferPtr[(i * 6) + 1] = center.y;
                                                    bufferPtr[(i * 6) + 2] = center.z;
                                                    bufferPtr[(i * 6) + 3] = size.x;
                                                    bufferPtr[(i * 6) + 4] = size.y;
                                                    bufferPtr[(i * 6) + 5] = size.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            IntVector2[] value = (IntVector2[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (IntVector2* valuePtr = value)
                                                {
                                                    IntVector2 elementValue = valuePtr[i];

                                                    bufferPtr[(i * 2) + 0] = elementValue.x;
                                                    bufferPtr[(i * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            IntVector3[] value = (IntVector3[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (IntVector3* valuePtr = value)
                                                {
                                                    IntVector3 elementValue = valuePtr[i];

                                                    bufferPtr[(i * 3) + 0] = elementValue.x;
                                                    bufferPtr[(i * 3) + 1] = elementValue.y;
                                                    bufferPtr[(i * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            IntVector4[] value = (IntVector4[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                fixed (IntVector4* valuePtr = value)
                                                {
                                                    IntVector4 elementValue = valuePtr[i];

                                                    bufferPtr[(i * 4) + 0] = elementValue.x;
                                                    bufferPtr[(i * 4) + 1] = elementValue.y;
                                                    bufferPtr[(i * 4) + 2] = elementValue.z;
                                                    bufferPtr[(i * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        throw new UnsupportedException("elementSerializationType for array is not supported: " + elementSerializationType);
                                }
                            }
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {

                        }
                        break;
                    case SerializationType.ObjectArray:
                        {

                        }
                        break;
                    case SerializationType.ObjectList:
                        {

                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {

                        }
                        break;
                    case SerializationType.ObjectNullable:
                        {

                        }
                        break;
                    case SerializationType.Object:
                        {

                        }
                        break;
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + serializationType);
                }

                return offset + dataSize;
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
    }
}
#endif
