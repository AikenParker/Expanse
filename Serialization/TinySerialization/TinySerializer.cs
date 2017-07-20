using System;
using System.Collections;
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

        public TTarget Deserialize<TTarget>(byte[] data, int offset)
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

                SerializationType serializationType = TinySerializerUtil.GetSerializationType(tSource);
                bool emitValueTypeCaster = true;

                switch (serializationType)
                {
                    case SerializationType.Byte:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            byte value = CastTo<byte>.From(obj, emitValueTypeCaster);

                            fixed (byte* bufferPtr = &buffer[offset])
                                *bufferPtr = value;
                        }
                        break;
                    case SerializationType.SByte:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            sbyte value = CastTo<sbyte>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Bool:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            bool value = CastTo<bool>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                bool* bufferPtr = (bool*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int16:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Vector2 value = CastTo<Vector2>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr = value.y;
                            }
                        }
                        break;
                    case SerializationType.Vector3:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Vector3 value = CastTo<Vector3>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr = value.z;
                            }
                        }
                        break;
                    case SerializationType.Vector4:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Vector4 value = CastTo<Vector4>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                                *bufferPtr = value.w;
                            }
                        }
                        break;
                    case SerializationType.Quaternion:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Quaternion value = CastTo<Quaternion>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                                *bufferPtr = value.w;
                            }
                        }
                        break;
                    case SerializationType.Rect:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            Rect value = CastTo<Rect>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.width;
                                *bufferPtr = value.height;
                            }
                        }
                        break;
                    case SerializationType.Bounds:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
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
                                *bufferPtr = size.z;
                            }
                        }
                        break;
                    case SerializationType.IntVector2:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            IntVector2 value = CastTo<IntVector2>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                int* bufferPtr = (int*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr = value.y;
                            }
                        }
                        break;
                    case SerializationType.IntVector3:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            IntVector3 value = CastTo<IntVector3>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                int* bufferPtr = (int*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr = value.z;
                            }
                        }
                        break;
                    case SerializationType.IntVector4:
                        {
                            dataSize = TinySerializerUtil.GetPrimitiveTypeSize(serializationType);
                            EnsureBufferSize(dataSize + offset);

                            IntVector4 value = CastTo<IntVector4>.From(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                int* bufferPtr = (int*)byteBufferPtr;

                                *bufferPtr++ = value.x;
                                *bufferPtr++ = value.y;
                                *bufferPtr++ = value.z;
                                *bufferPtr = value.w;
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
                            SerializationType elementSerializationType = TinySerializerUtil.GetSerializationType(elementType);
                            int elementSize = TinySerializerUtil.GetPrimitiveTypeSize(elementSerializationType);

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
                                    case SerializationType.Bool:
                                        {
                                            bool[] value = (bool[])baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                bool* bufferPtr = (bool*)byteBufferPtr;

                                                fixed (bool* valuePtr = value)
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
                                                int* bufferPtr = (int*)byteBufferPtr;

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
                                                int* bufferPtr = (int*)byteBufferPtr;

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
                                                int* bufferPtr = (int*)byteBufferPtr;

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
                            IList baseValue = CastTo<IList>.From(obj, false);
                            bool hasValue = baseValue != null;
                            int length = hasValue ? baseValue.Count : -1;

                            int elementCount = hasValue ? length : 0;
                            Type[] genericParameters = tSource.GetGenericArguments();
                            Type elementType = genericParameters[0];
                            SerializationType elementSerializationType = TinySerializerUtil.GetSerializationType(elementType);
                            int elementSize = TinySerializerUtil.GetPrimitiveTypeSize(elementSerializationType);

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
                                            List<byte> value = (List<byte>)baseValue;

                                            fixed (byte* bufferPtr = &buffer[offset + lengthSize])
                                                bufferPtr[i] = value[i];
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            List<sbyte> value = (List<sbyte>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                sbyte* bufferPtr = (sbyte*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Bool:
                                        {
                                            List<bool> value = (List<bool>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                bool* bufferPtr = (bool*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            List<short> value = (List<short>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                short* bufferPtr = (short*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            List<int> value = (List<int>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                int* bufferPtr = (int*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            List<long> value = (List<long>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            List<ushort> value = (List<ushort>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                ushort* bufferPtr = (ushort*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            List<uint> value = (List<uint>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                uint* bufferPtr = (uint*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            List<ulong> value = (List<ulong>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                ulong* bufferPtr = (ulong*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            List<Half> value = (List<Half>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                ushort* bufferPtr = (ushort*)byteBufferPtr;

                                                bufferPtr[i] = value[i].value;
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            List<float> value = (List<float>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            List<double> value = (List<double>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                double* bufferPtr = (double*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            List<char> value = (List<char>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                char* bufferPtr = (char*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            List<decimal> value = (List<decimal>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                decimal* bufferPtr = (decimal*)byteBufferPtr;

                                                bufferPtr[i] = value[i];
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            List<DateTime> value = (List<DateTime>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                bufferPtr[i] = value[i].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            List<DateTimeOffset> value = (List<DateTimeOffset>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                bufferPtr[i] = value[i].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            List<TimeSpan> value = (List<TimeSpan>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                long* bufferPtr = (long*)byteBufferPtr;

                                                bufferPtr[i] = value[i].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            List<Vector2> value = (List<Vector2>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                Vector2 elementValue = value[i];

                                                bufferPtr[(i * 2) + 0] = elementValue.x;
                                                bufferPtr[(i * 2) + 1] = elementValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            List<Vector3> value = (List<Vector3>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                Vector3 elementValue = value[i];

                                                bufferPtr[(i * 3) + 0] = elementValue.x;
                                                bufferPtr[(i * 3) + 1] = elementValue.y;
                                                bufferPtr[(i * 3) + 2] = elementValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            List<Vector4> value = (List<Vector4>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                Vector4 elementValue = value[i];

                                                bufferPtr[(i * 4) + 0] = elementValue.x;
                                                bufferPtr[(i * 4) + 1] = elementValue.y;
                                                bufferPtr[(i * 4) + 2] = elementValue.z;
                                                bufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            List<Quaternion> value = (List<Quaternion>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                Quaternion elementValue = value[i];

                                                bufferPtr[(i * 4) + 0] = elementValue.x;
                                                bufferPtr[(i * 4) + 1] = elementValue.y;
                                                bufferPtr[(i * 4) + 2] = elementValue.z;
                                                bufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            List<Rect> value = (List<Rect>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                Rect elementValue = value[i];

                                                bufferPtr[(i * 4) + 0] = elementValue.x;
                                                bufferPtr[(i * 4) + 1] = elementValue.y;
                                                bufferPtr[(i * 4) + 2] = elementValue.width;
                                                bufferPtr[(i * 4) + 3] = elementValue.height;
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            List<Bounds> value = (List<Bounds>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                float* bufferPtr = (float*)byteBufferPtr;

                                                Bounds elementValue = value[i];

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
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            List<IntVector2> value = (List<IntVector2>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                int* bufferPtr = (int*)byteBufferPtr;

                                                IntVector2 elementValue = value[i];

                                                bufferPtr[(i * 2) + 0] = elementValue.x;
                                                bufferPtr[(i * 2) + 1] = elementValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            List<IntVector3> value = (List<IntVector3>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                int* bufferPtr = (int*)byteBufferPtr;

                                                IntVector3 elementValue = value[i];

                                                bufferPtr[(i * 3) + 0] = elementValue.x;
                                                bufferPtr[(i * 3) + 1] = elementValue.y;
                                                bufferPtr[(i * 3) + 2] = elementValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            List<IntVector4> value = (List<IntVector4>)baseValue;

                                            fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                            {
                                                int* bufferPtr = (int*)byteBufferPtr;

                                                IntVector4 elementValue = value[i];

                                                bufferPtr[(i * 4) + 0] = elementValue.x;
                                                bufferPtr[(i * 4) + 1] = elementValue.y;
                                                bufferPtr[(i * 4) + 2] = elementValue.z;
                                                bufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new UnsupportedException("elementSerializationType for list is not supported: " + elementSerializationType);
                                }
                            }
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            Type[] genericParameters = tSource.GetGenericArguments();
                            Type elementType = genericParameters[0];
                            SerializationType elementSerializationType = TinySerializerUtil.GetSerializationType(elementType);
                            int elementSize = TinySerializerUtil.GetPrimitiveTypeSize(elementSerializationType);

                            switch (elementSerializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        byte? value = CastTo<byte?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                            {
                                                *bufferPtr = 1;
                                                bufferPtr[1] = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.SByte:
                                    {
                                        sbyte? value = CastTo<sbyte?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                sbyte* bufferPtr = (sbyte*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Bool:
                                    {
                                        bool? value = CastTo<bool?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        byte data;
                                        if (hasValue)
                                        {
                                            bool actualValue = value.Value;
                                            data = actualValue ? (byte)2 : (byte)1;
                                        }
                                        else
                                        {
                                            data = 0;
                                        }

                                        dataSize = sizeof(bool);
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* bufferPtr = buffer)
                                            *bufferPtr = data;
                                    }
                                    break;
                                case SerializationType.Int16:
                                    {
                                        short? value = CastTo<short?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                short* bufferPtr = (short*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Int32:
                                    {
                                        int? value = CastTo<int?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                int* bufferPtr = (int*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Int64:
                                    {
                                        long? value = CastTo<long?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                long* bufferPtr = (long*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt16:
                                    {
                                        ushort? value = CastTo<ushort?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                ushort* bufferPtr = (ushort*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt32:
                                    {
                                        uint? value = CastTo<uint?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                uint* bufferPtr = (uint*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt64:
                                    {
                                        ulong? value = CastTo<ulong?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                ulong* bufferPtr = (ulong*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Half:
                                    {
                                        Half? value = CastTo<Half?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                ushort* bufferPtr = (ushort*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value.value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Single:
                                    {
                                        float? value = CastTo<float?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                float* bufferPtr = (float*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Double:
                                    {
                                        double? value = CastTo<double?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                double* bufferPtr = (double*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Char:
                                    {
                                        char? value = CastTo<char?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                char* bufferPtr = (char*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Decimal:
                                    {
                                        decimal? value = CastTo<decimal?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                decimal* bufferPtr = (decimal*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTime:
                                    {
                                        DateTime? value = CastTo<DateTime?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                long* bufferPtr = (long*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value.Ticks;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTimeOffset:
                                    {
                                        DateTimeOffset? value = CastTo<DateTimeOffset?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                long* bufferPtr = (long*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value.Ticks;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.TimeSpan:
                                    {
                                        TimeSpan? value = CastTo<TimeSpan?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;
                                                long* bufferPtr = (long*)&byteBufferPtr[1];
                                                *bufferPtr = value.Value.Ticks;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector2:
                                    {
                                        Vector2? value = CastTo<Vector2?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                float* bufferPtr = (float*)&byteBufferPtr[1];

                                                Vector2 actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr = actualValue.y;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector3:
                                    {
                                        Vector3? value = CastTo<Vector3?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                float* bufferPtr = (float*)&byteBufferPtr[1];

                                                Vector3 actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr++ = actualValue.y;
                                                *bufferPtr = actualValue.z;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector4:
                                    {
                                        Vector4? value = CastTo<Vector4?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                float* bufferPtr = (float*)&byteBufferPtr[1];

                                                Vector4 actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr++ = actualValue.y;
                                                *bufferPtr++ = actualValue.z;
                                                *bufferPtr = actualValue.w;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Quaternion:
                                    {
                                        Quaternion? value = CastTo<Quaternion?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                float* bufferPtr = (float*)&byteBufferPtr[1];

                                                Quaternion actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr++ = actualValue.y;
                                                *bufferPtr++ = actualValue.z;
                                                *bufferPtr = actualValue.w;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Rect:
                                    {
                                        Rect? value = CastTo<Rect?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                float* bufferPtr = (float*)&byteBufferPtr[1];

                                                Rect actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr++ = actualValue.y;
                                                *bufferPtr++ = actualValue.width;
                                                *bufferPtr = actualValue.height;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.Bounds:
                                    {
                                        Bounds? value = CastTo<Bounds?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                float* bufferPtr = (float*)&byteBufferPtr[1];

                                                Bounds actualValue = value.Value;

                                                Vector3 center = actualValue.center;

                                                *bufferPtr++ = center.x;
                                                *bufferPtr++ = center.y;
                                                *bufferPtr++ = center.z;

                                                Vector3 size = actualValue.size;

                                                *bufferPtr++ = size.x;
                                                *bufferPtr++ = size.y;
                                                *bufferPtr = size.z;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector2:
                                    {
                                        IntVector2? value = CastTo<IntVector2?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                int* bufferPtr = (int*)&byteBufferPtr[1];

                                                IntVector2 actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr = actualValue.y;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector3:
                                    {
                                        IntVector3? value = CastTo<IntVector3?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                int* bufferPtr = (int*)&byteBufferPtr[1];

                                                IntVector3 actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr++ = actualValue.y;
                                                *bufferPtr = actualValue.z;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector4:
                                    {
                                        IntVector4? value = CastTo<IntVector4?>.From(obj, emitValueTypeCaster);
                                        bool hasValue = value.HasValue;

                                        if (hasValue)
                                        {
                                            dataSize = sizeof(byte) + elementSize;
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* byteBufferPtr = &buffer[offset])
                                            {
                                                *byteBufferPtr = 1;

                                                int* bufferPtr = (int*)&byteBufferPtr[1];

                                                IntVector4 actualValue = value.Value;

                                                *bufferPtr++ = actualValue.x;
                                                *bufferPtr++ = actualValue.y;
                                                *bufferPtr++ = actualValue.z;
                                                *bufferPtr = actualValue.w;
                                            }
                                        }
                                        else
                                        {
                                            dataSize = sizeof(byte);
                                            EnsureBufferSize(offset + dataSize);

                                            fixed (byte* bufferPtr = &buffer[offset])
                                                *bufferPtr = 0;
                                        }
                                    }
                                    break;
                                default:
                                    throw new UnsupportedException("elementSerializationType for nullable is not supported: " + elementSerializationType);
                            }
                        }
                        break;
                    case SerializationType.Object:
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
                    case SerializationType.ObjectNullable:
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
    }
}
#endif
