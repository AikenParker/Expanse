#if UNSAFE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Expanse.Misc;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Serialization.TinySerialization
{
    public unsafe sealed class TinySerializer : IByteSerializer
    {
        private byte[] buffer;
        private int bufferSize;

        private TinySerializerSettings settings = TinySerializerSettings.Default;

        private List<CustomTypeResolver> customTypeResolvers;

        public TinySerializer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            this.buffer = new byte[bufferSize];
        }

        public TinySerializer(int bufferSize, TinySerializerSettings settings) : this(bufferSize)
        {
            this.settings = settings;
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

        public TTarget Deserialize<TTarget>(byte[] data) where TTarget : new()
        {
            TTarget obj = EmitHelper<TTarget>.CreateInstance<TTarget>(true, false);

            throw new NotImplementedException();
        }

        public TTarget Deserialize<TTarget>(byte[] data, int offset) where TTarget : new()
        {
            TTarget obj = EmitHelper<TTarget>.CreateInstance<TTarget>(true, false);

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

        public byte[] ExperimentalSerialize<TSource>(TSource obj)
        {
            Type tSource = typeof(TSource);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tSource);

            int dataSize;

            if (typeInfo.isPrimitiveType)
            {
                TypedReference objRef = __makeref(obj);
                dataSize = SerializePrimitiveIntoBuffer(objRef, typeInfo, 0);
            }
            else
            {
                // WIP
                dataSize = SerializeIntoBuffer(obj, 0);
            }

            fixed (byte* bufferPtr = buffer)
            {
                byte[] data = new byte[dataSize];

                fixed (byte* dataPtr = data)
                {
                    for (int i = 0; i < dataSize; i++)
                    {
                        dataPtr[i] = bufferPtr[i];
                    }
                }

                return data;
            }
        }

        // TODO: Make this the primary serialization method
        // Returns size
        private int SerializePrimitiveIntoBuffer(TypedReference objRef, TinySerializerTypeInfo typeInfo, int offset)
        {
            int dataSize = 0;

            switch (typeInfo.serializationType)
            {
                case SerializationType.Byte:
                    break;
                case SerializationType.SByte:
                    break;
                case SerializationType.Bool:
                    break;
                case SerializationType.Int16:
                    break;
                case SerializationType.Int32:
                    {
                        dataSize = SerializationTypeSizes.INT32;
                        EnsureBufferSize(offset + dataSize);

                        int value = __refvalue(objRef, int);

                        fixed (byte* bufferPtr = &buffer[offset])
                        {
                            int* intBufferPtr = (int*)bufferPtr;
                            *intBufferPtr = value;
                        }
                    }
                    break;
                case SerializationType.Int64:
                    break;
                case SerializationType.UInt16:
                    break;
                case SerializationType.UInt32:
                    break;
                case SerializationType.UInt64:
                    break;
                case SerializationType.Half:
                    break;
                case SerializationType.Single:
                    break;
                case SerializationType.Double:
                    break;
                case SerializationType.Char:
                    break;
                case SerializationType.Decimal:
                    break;
                case SerializationType.DateTime:
                    break;
                case SerializationType.DateTimeOffset:
                    break;
                case SerializationType.TimeSpan:
                    break;
                case SerializationType.Vector2:
                    break;
                case SerializationType.Vector3:
                    break;
                case SerializationType.Vector4:
                    break;
                case SerializationType.Quaternion:
                    break;
                case SerializationType.Rect:
                    break;
                case SerializationType.Bounds:
                    break;
                case SerializationType.IntVector2:
                    break;
                case SerializationType.IntVector3:
                    break;
                case SerializationType.IntVector4:
                    break;
                case SerializationType.String:
                    break;
                case SerializationType.PrimitiveArray:
                    break;
                case SerializationType.PrimitiveList:
                    break;
                case SerializationType.PrimitiveNullable:
                    break;
                default:
                    throw new UnsupportedException("Unsupported typed ref serialization type: " + typeInfo.serializationType);
            }

            return dataSize;
        }

        // Returns new offset
        private int SerializeIntoBuffer<TSource>(TSource obj, int offset)
        {
            SimpleTypeInfo simpleTypeInfo = SimpleTypeInfo<TSource>.info;

            // Check if we should serialize using a custom type resolver
            {
                if (customTypeResolvers != null)
                {
                    Type tSource = simpleTypeInfo.type;
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
                            customTypeResolver.Serialize(obj, bufferPtr, offset);
                        }

                        return offset + dataSize;
                    }
                }
            }

            // Perform standard serialization of supported and custom types
            {
                int dataSize = 0;

                bool emitValueTypeCaster = settings.emitValueTypeCasters;

                switch (simpleTypeInfo.serializationType)
                {
                    case SerializationType.Byte:
                        {
                            dataSize = SerializationTypeSizes.BYTE;
                            EnsureBufferSize(dataSize + offset);

                            byte value = EmitHelper<byte>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* bufferPtr = &buffer[offset])
                                *bufferPtr = value;
                        }
                        break;
                    case SerializationType.SByte:
                        {
                            dataSize = SerializationTypeSizes.SBYTE;
                            EnsureBufferSize(dataSize + offset);

                            sbyte value = EmitHelper<sbyte>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Bool:
                        {
                            dataSize = SerializationTypeSizes.BOOL;
                            EnsureBufferSize(dataSize + offset);

                            bool value = EmitHelper<bool>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                bool* bufferPtr = (bool*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int16:
                        {
                            dataSize = SerializationTypeSizes.INT16;
                            EnsureBufferSize(dataSize + offset);

                            short value = EmitHelper<short>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                short* bufferPtr = (short*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int32:
                        {
                            dataSize = SerializationTypeSizes.INT32;
                            EnsureBufferSize(dataSize + offset);

                            int value = EmitHelper<int>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                int* bufferPtr = (int*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int64:
                        {
                            dataSize = SerializationTypeSizes.INT64;
                            EnsureBufferSize(dataSize + offset);

                            long value = EmitHelper<long>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt16:
                        {
                            dataSize = SerializationTypeSizes.UINT16;
                            EnsureBufferSize(dataSize + offset);

                            ushort value = EmitHelper<ushort>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                ushort* bufferPtr = (ushort*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt32:
                        {
                            dataSize = SerializationTypeSizes.UINT32;
                            EnsureBufferSize(dataSize + offset);

                            uint value = EmitHelper<uint>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                uint* bufferPtr = (uint*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt64:
                        {
                            dataSize = SerializationTypeSizes.UINT64;
                            EnsureBufferSize(dataSize + offset);

                            ulong value = EmitHelper<ulong>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                ulong* bufferPtr = (ulong*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Half:
                        {
                            dataSize = SerializationTypeSizes.HALF;
                            EnsureBufferSize(dataSize + offset);

                            Half value = EmitHelper<Half>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                ushort* bufferPtr = (ushort*)byteBufferPtr;
                                *bufferPtr = value.value;
                            }
                        }
                        break;
                    case SerializationType.Single:
                        {
                            dataSize = SerializationTypeSizes.SINGLE;
                            EnsureBufferSize(dataSize + offset);

                            float value = EmitHelper<float>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                float* bufferPtr = (float*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Double:
                        {
                            dataSize = SerializationTypeSizes.DOUBLE;
                            EnsureBufferSize(dataSize + offset);

                            double value = EmitHelper<double>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                double* bufferPtr = (double*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Char:
                        {
                            dataSize = SerializationTypeSizes.CHAR;
                            EnsureBufferSize(dataSize + offset);

                            char value = EmitHelper<char>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                char* bufferPtr = (char*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Decimal:
                        {
                            dataSize = SerializationTypeSizes.DECIMAL;
                            EnsureBufferSize(dataSize + offset);

                            decimal value = EmitHelper<decimal>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                decimal* bufferPtr = (decimal*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.DateTime:
                        {
                            dataSize = SerializationTypeSizes.DATE_TIME;
                            EnsureBufferSize(dataSize + offset);

                            DateTime value = EmitHelper<DateTime>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.DateTimeOffset:
                        {
                            dataSize = SerializationTypeSizes.DATE_TIME_OFFSET;
                            EnsureBufferSize(dataSize + offset);

                            DateTimeOffset value = EmitHelper<DateTimeOffset>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.TimeSpan:
                        {
                            dataSize = SerializationTypeSizes.TIME_SPAN;
                            EnsureBufferSize(dataSize + offset);

                            TimeSpan value = EmitHelper<TimeSpan>.CastFrom(obj, emitValueTypeCaster);

                            fixed (byte* byteBufferPtr = &buffer[offset])
                            {
                                long* bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.Vector2:
                        {
                            dataSize = SerializationTypeSizes.VECTOR2;
                            EnsureBufferSize(dataSize + offset);

                            Vector2 value = EmitHelper<Vector2>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.VECTOR3;
                            EnsureBufferSize(dataSize + offset);

                            Vector3 value = EmitHelper<Vector3>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.VECTOR4;
                            EnsureBufferSize(dataSize + offset);

                            Vector4 value = EmitHelper<Vector4>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.QUATERNION;
                            EnsureBufferSize(dataSize + offset);

                            Quaternion value = EmitHelper<Quaternion>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.RECT;
                            EnsureBufferSize(dataSize + offset);

                            Rect value = EmitHelper<Rect>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.BOUNDS;
                            EnsureBufferSize(dataSize + offset);

                            Bounds value = EmitHelper<Bounds>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.INT_VECTOR2;
                            EnsureBufferSize(dataSize + offset);

                            IntVector2 value = EmitHelper<IntVector2>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.INT_VECTOR3;
                            EnsureBufferSize(dataSize + offset);

                            IntVector3 value = EmitHelper<IntVector3>.CastFrom(obj, emitValueTypeCaster);

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
                            dataSize = SerializationTypeSizes.INT_VECTOR4;
                            EnsureBufferSize(dataSize + offset);

                            IntVector4 value = EmitHelper<IntVector4>.CastFrom(obj, emitValueTypeCaster);

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
                            string value = EmitHelper<string>.CastFrom(obj, false);
                            bool hasValue = value != null;
                            int length = hasValue ? value.Length : -1;
                            int lengthSize = sizeof(int);

                            if (settings.variablePrefixLengthSize)
                                lengthSize = TinySerializerUtil.GetPrefixLengthSize(length);

                            int charCount = hasValue ? length : 0;

                            Encoding systemEncoding = null;

                            switch (settings.defaultStringEncodeType)
                            {
                                case StringEncodeType.Char:
                                    {
                                        int charSize = sizeof(char);

                                        dataSize = lengthSize + (charCount * charSize);
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* bufferPtr = &buffer[offset])
                                        {
                                            switch (lengthSize)
                                            {
                                                case 1:
                                                    {
                                                        sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                        *sbyteBufferPtr = (sbyte)length;
                                                    }
                                                    break;

                                                case 3:
                                                    {
                                                        sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                        *sbyteBufferPtr++ = -2;
                                                        short* shortBufferPtr = (short*)sbyteBufferPtr;
                                                        *shortBufferPtr = (short)length;
                                                    }
                                                    break;

                                                case 4:
                                                    {
                                                        int* intBufferPtr = (int*)bufferPtr;
                                                        *intBufferPtr = length;
                                                    }
                                                    break;

                                                case 5:
                                                    {
                                                        sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                        *sbyteBufferPtr++ = -4;
                                                        int* intBufferPtr = (int*)sbyteBufferPtr;
                                                        *intBufferPtr = length;
                                                    }
                                                    break;

                                                default:
                                                    throw new UnsupportedException("Unsupported length size: " + lengthSize);
                                            }

                                            if (hasValue)
                                            {
                                                char* charBufferPtr = (char*)&bufferPtr[lengthSize];

                                                fixed (char* charValuePtr = value)
                                                {
                                                    for (int i = 0; i < charCount; i++)
                                                    {
                                                        *charBufferPtr++ = charValuePtr[i];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case StringEncodeType.Byte:
                                    {
                                        int charSize = sizeof(byte);

                                        dataSize = lengthSize + (charCount * charSize);
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* bufferPtr = &buffer[offset])
                                        {
                                            switch (lengthSize)
                                            {
                                                case 1:
                                                    {
                                                        sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                        *sbyteBufferPtr = (sbyte)length;
                                                    }
                                                    break;

                                                case 3:
                                                    {
                                                        sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                        *sbyteBufferPtr++ = -2;
                                                        short* shortBufferPtr = (short*)sbyteBufferPtr;
                                                        *shortBufferPtr = (short)length;
                                                    }
                                                    break;

                                                case 4:
                                                    {
                                                        int* intBufferPtr = (int*)bufferPtr;
                                                        *intBufferPtr = length;
                                                    }
                                                    break;

                                                case 5:
                                                    {
                                                        sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                        *sbyteBufferPtr++ = -4;
                                                        int* intBufferPtr = (int*)sbyteBufferPtr;
                                                        *intBufferPtr = length;
                                                    }
                                                    break;

                                                default:
                                                    throw new UnsupportedException("Unsupported length size: " + lengthSize);
                                            }

                                            if (hasValue)
                                            {
                                                byte* byteBufferPtr = &bufferPtr[lengthSize];

                                                fixed (char* charValuePtr = value)
                                                {
                                                    for (int i = 0; i < charCount; i++)
                                                    {
                                                        *byteBufferPtr++ = (byte)charValuePtr[i];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case StringEncodeType.Default:
                                    {
                                        systemEncoding = Encoding.Default;
                                        goto Encoding;
                                    }

                                case StringEncodeType.ASCII:
                                    {
                                        systemEncoding = Encoding.ASCII;
                                        goto Encoding;
                                    }

                                case StringEncodeType.UTF7:
                                    {
                                        systemEncoding = Encoding.UTF7;
                                        goto Encoding;
                                    }

                                case StringEncodeType.UTF8:
                                    {
                                        systemEncoding = Encoding.UTF8;
                                        goto Encoding;
                                    }

                                case StringEncodeType.Unicode:
                                    {
                                        systemEncoding = Encoding.Unicode;
                                        goto Encoding;
                                    }

                                case StringEncodeType.UTF32:
                                    {
                                        systemEncoding = Encoding.UTF32;
                                        goto Encoding;
                                    }

                                case StringEncodeType.BigEndianUnicode:
                                    {
                                        systemEncoding = Encoding.BigEndianUnicode;
                                        goto Encoding;
                                    }

                                    Encoding:
                                    {
                                        systemEncoding = systemEncoding ?? Encoding.Default;

                                        if (hasValue)
                                        {
                                            fixed (char* charValuePtr = value)
                                            {
                                                int byteCount = systemEncoding.GetByteCount(charValuePtr, charCount);

                                                dataSize = lengthSize + byteCount;
                                                EnsureBufferSize(offset + dataSize);

                                                fixed (byte* bufferPtr = &buffer[offset])
                                                {
                                                    switch (lengthSize)
                                                    {
                                                        case 1:
                                                            {
                                                                sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                                *sbyteBufferPtr = (sbyte)length;
                                                            }
                                                            break;

                                                        case 3:
                                                            {
                                                                sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                                *sbyteBufferPtr++ = -2;
                                                                short* shortBufferPtr = (short*)sbyteBufferPtr;
                                                                *shortBufferPtr = (short)length;
                                                            }
                                                            break;

                                                        case 4:
                                                            {
                                                                int* intBufferPtr = (int*)bufferPtr;
                                                                *intBufferPtr = length;
                                                            }
                                                            break;

                                                        case 5:
                                                            {
                                                                sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                                *sbyteBufferPtr++ = -4;
                                                                int* intBufferPtr = (int*)sbyteBufferPtr;
                                                                *intBufferPtr = length;
                                                            }
                                                            break;

                                                        default:
                                                            throw new UnsupportedException("Unsupported length size: " + lengthSize);
                                                    }

                                                    byte* byteBufferPtr = &bufferPtr[lengthSize];
                                                    systemEncoding.GetBytes(charValuePtr, charCount, byteBufferPtr, byteCount);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            fixed (byte* bufferPtr = buffer)
                                            {
                                                switch (lengthSize)
                                                {
                                                    case 1:
                                                        {
                                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                            *sbyteBufferPtr = (sbyte)length;
                                                        }
                                                        break;

                                                    case 3:
                                                        {
                                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                            *sbyteBufferPtr++ = -2;
                                                            short* shortBufferPtr = (short*)sbyteBufferPtr;
                                                            *shortBufferPtr = (short)length;
                                                        }
                                                        break;

                                                    case 4:
                                                        {
                                                            int* intBufferPtr = (int*)bufferPtr;
                                                            *intBufferPtr = length;
                                                        }
                                                        break;

                                                    case 5:
                                                        {
                                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                                            *sbyteBufferPtr++ = -4;
                                                            int* intBufferPtr = (int*)sbyteBufferPtr;
                                                            *intBufferPtr = length;
                                                        }
                                                        break;

                                                    default:
                                                        throw new UnsupportedException("Unsupported length size: " + lengthSize);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    throw new UnsupportedException("Unsupported string encode type: " + settings.defaultStringEncodeType);
                            }
                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            Array baseValue = EmitHelper<Array>.CastFrom(obj, false);
                            bool hasValue = baseValue != null;
                            int length = hasValue ? baseValue.Length : -1;
                            int lengthSize = sizeof(int);

                            int elementCount = hasValue ? length : 0;
                            SerializationType elementSerializationType = simpleTypeInfo.elementSerializationType;
                            int elementSize = simpleTypeInfo.elementPrimitiveSize;

                            if (settings.variablePrefixLengthSize)
                                lengthSize = TinySerializerUtil.GetPrefixLengthSize(length);

                            dataSize = lengthSize + (elementCount * elementSize);

                            int compressedIterations = 0;
                            bool compressBoolArray = settings.compressBoolArray;
                            if (compressBoolArray && simpleTypeInfo.elementType == SerializationTypeValues.Bool)
                            {
                                compressedIterations = elementCount / 8;

                                if (elementCount % 8 != 0)
                                    compressedIterations++;

                                dataSize = lengthSize + compressedIterations;
                            }

                            EnsureBufferSize(offset + dataSize);

                            fixed (byte* bufferPtr = &buffer[offset])
                            {
                                switch (lengthSize)
                                {
                                    case 1:
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                            *sbyteBufferPtr = (sbyte)length;
                                        }
                                        break;

                                    case 3:
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                            *sbyteBufferPtr++ = -2;
                                            short* shortBufferPtr = (short*)sbyteBufferPtr;
                                            *shortBufferPtr = (short)length;
                                        }
                                        break;

                                    case 4:
                                        {
                                            int* intBufferPtr = (int*)bufferPtr;
                                            *intBufferPtr = length;
                                        }
                                        break;

                                    case 5:
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                            *sbyteBufferPtr++ = -4;
                                            int* intBufferPtr = (int*)sbyteBufferPtr;
                                            *intBufferPtr = length;
                                        }
                                        break;

                                    default:
                                        throw new UnsupportedException("Unsupported length size: " + lengthSize);
                                }
                            }

                            switch (elementSerializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            byte[] value = (byte[])baseValue;

                                            fixed (byte* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    byteBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.SByte:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)byteBufferPtr;

                                            sbyte[] value = (sbyte[])baseValue;

                                            fixed (sbyte* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    sbyteBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Bool:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            bool[] value = (bool[])baseValue;

                                            if (compressBoolArray)
                                            {
                                                fixed (bool* valuePtr = value)
                                                {
                                                    int index = 0;
                                                    for (int i = 0; i < compressedIterations; i++)
                                                    {
                                                        byte result = 0;

                                                        if (valuePtr[index])
                                                            result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        if (++index < elementCount)
                                                            if (valuePtr[index])
                                                                result |= (byte)(1 << (7 - index));

                                                        byteBufferPtr[i] = result;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                bool* boolBufferPtr = (bool*)byteBufferPtr;

                                                fixed (bool* valuePtr = value)
                                                {
                                                    for (int i = 0; i < elementCount; i++)
                                                    {
                                                        boolBufferPtr[i] = valuePtr[i];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Int16:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            short* shortBufferPtr = (short*)byteBufferPtr;

                                            short[] value = (short[])baseValue;

                                            fixed (short* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    shortBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Int32:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            int[] value = (int[])baseValue;

                                            fixed (int* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    intBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Int64:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            short* shortBufferPtr = (short*)byteBufferPtr;

                                            short[] value = (short[])baseValue;

                                            fixed (short* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    shortBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.UInt16:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            ushort* ushortBufferPtr = (ushort*)byteBufferPtr;

                                            ushort[] value = (ushort[])baseValue;

                                            fixed (ushort* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    ushortBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.UInt32:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            uint* uintBufferPtr = (uint*)byteBufferPtr;

                                            uint[] value = (uint[])baseValue;

                                            fixed (uint* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    uintBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.UInt64:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            ulong* ulongBufferPtr = (ulong*)byteBufferPtr;

                                            ulong[] value = (ulong[])baseValue;

                                            fixed (ulong* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    ulongBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Half:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            ushort* ushortBufferPtr = (ushort*)byteBufferPtr;

                                            Half[] value = (Half[])baseValue;

                                            fixed (Half* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    ushortBufferPtr[i] = valuePtr[i].value;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Single:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            float[] value = (float[])baseValue;

                                            fixed (float* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    floatBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Double:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            double* doubleBufferPtr = (double*)byteBufferPtr;

                                            double[] value = (double[])baseValue;

                                            fixed (double* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    doubleBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Char:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            char* charBufferPtr = (char*)byteBufferPtr;

                                            char[] value = (char[])baseValue;

                                            fixed (char* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    charBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Decimal:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            decimal* decimalBufferPtr = (decimal*)byteBufferPtr;

                                            decimal[] value = (decimal[])baseValue;

                                            fixed (decimal* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    decimalBufferPtr[i] = valuePtr[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.DateTime:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            long* longBufferPtr = (long*)byteBufferPtr;

                                            DateTime[] value = (DateTime[])baseValue;

                                            fixed (DateTime* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    longBufferPtr[i] = valuePtr[i].Ticks;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.DateTimeOffset:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            long* longBufferPtr = (long*)byteBufferPtr;

                                            DateTimeOffset[] value = (DateTimeOffset[])baseValue;

                                            fixed (DateTimeOffset* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    longBufferPtr[i] = valuePtr[i].Ticks;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.TimeSpan:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            long* longBufferPtr = (long*)byteBufferPtr;

                                            TimeSpan[] value = (TimeSpan[])baseValue;

                                            fixed (TimeSpan* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    longBufferPtr[i] = valuePtr[i].Ticks;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Vector2:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            Vector2[] value = (Vector2[])baseValue;

                                            fixed (Vector2* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    Vector2 elementValue = valuePtr[i];

                                                    floatBufferPtr[(i * 2) + 0] = elementValue.x;
                                                    floatBufferPtr[(i * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Vector3:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            Vector3[] value = (Vector3[])baseValue;

                                            fixed (Vector3* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    Vector3 elementValue = valuePtr[i];

                                                    floatBufferPtr[(i * 3) + 0] = elementValue.x;
                                                    floatBufferPtr[(i * 3) + 1] = elementValue.y;
                                                    floatBufferPtr[(i * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Vector4:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            Vector4[] value = (Vector4[])baseValue;

                                            fixed (Vector4* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    Vector4 elementValue = valuePtr[i];

                                                    floatBufferPtr[(i * 4) + 0] = elementValue.x;
                                                    floatBufferPtr[(i * 4) + 1] = elementValue.y;
                                                    floatBufferPtr[(i * 4) + 2] = elementValue.z;
                                                    floatBufferPtr[(i * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Quaternion:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            Quaternion[] value = (Quaternion[])baseValue;

                                            fixed (Quaternion* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    Quaternion elementValue = valuePtr[i];

                                                    floatBufferPtr[(i * 4) + 0] = elementValue.x;
                                                    floatBufferPtr[(i * 4) + 1] = elementValue.y;
                                                    floatBufferPtr[(i * 4) + 2] = elementValue.z;
                                                    floatBufferPtr[(i * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Rect:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            Rect[] value = (Rect[])baseValue;

                                            fixed (Rect* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    Rect elementValue = valuePtr[i];

                                                    floatBufferPtr[(i * 4) + 0] = elementValue.x;
                                                    floatBufferPtr[(i * 4) + 1] = elementValue.y;
                                                    floatBufferPtr[(i * 4) + 2] = elementValue.width;
                                                    floatBufferPtr[(i * 4) + 3] = elementValue.height;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Bounds:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            Bounds[] value = (Bounds[])baseValue;

                                            fixed (Bounds* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    Bounds elementValue = valuePtr[i];

                                                    Vector3 center = elementValue.center;
                                                    Vector3 size = elementValue.size;

                                                    floatBufferPtr[(i * 6) + 0] = center.x;
                                                    floatBufferPtr[(i * 6) + 1] = center.y;
                                                    floatBufferPtr[(i * 6) + 2] = center.z;
                                                    floatBufferPtr[(i * 6) + 3] = size.x;
                                                    floatBufferPtr[(i * 6) + 4] = size.y;
                                                    floatBufferPtr[(i * 6) + 5] = size.z;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.IntVector2:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            IntVector2[] value = (IntVector2[])baseValue;

                                            fixed (IntVector2* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    IntVector2 elementValue = valuePtr[i];

                                                    intBufferPtr[(i * 2) + 0] = elementValue.x;
                                                    intBufferPtr[(i * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.IntVector3:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            IntVector3[] value = (IntVector3[])baseValue;

                                            fixed (IntVector3* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    IntVector3 elementValue = valuePtr[i];

                                                    intBufferPtr[(i * 3) + 0] = elementValue.x;
                                                    intBufferPtr[(i * 3) + 1] = elementValue.y;
                                                    intBufferPtr[(i * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.IntVector4:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            IntVector4[] value = (IntVector4[])baseValue;

                                            fixed (IntVector4* valuePtr = value)
                                            {
                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    IntVector4 elementValue = valuePtr[i];

                                                    intBufferPtr[(i * 4) + 0] = elementValue.x;
                                                    intBufferPtr[(i * 4) + 1] = elementValue.y;
                                                    intBufferPtr[(i * 4) + 2] = elementValue.z;
                                                    intBufferPtr[(i * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    throw new UnsupportedException("elementSerializationType for array is not supported: " + elementSerializationType);
                            }
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {
                            IList baseValue = EmitHelper<IList>.CastFrom(obj, false);
                            bool hasValue = baseValue != null;
                            int length = hasValue ? baseValue.Count : -1;
                            int lengthSize = sizeof(int);

                            int elementCount = hasValue ? length : 0;
                            SerializationType elementSerializationType = simpleTypeInfo.elementSerializationType;
                            int elementSize = simpleTypeInfo.elementPrimitiveSize;

                            if (settings.variablePrefixLengthSize)
                                lengthSize = TinySerializerUtil.GetPrefixLengthSize(length);

                            dataSize = lengthSize + (elementCount * elementSize);

                            int compressedIterations = 0;
                            bool compressBoolArray = settings.compressBoolArray;
                            if (compressBoolArray && simpleTypeInfo.elementType == SerializationTypeValues.Bool)
                            {
                                compressedIterations = elementCount / 8;

                                if (elementCount % 8 != 0)
                                    compressedIterations++;

                                dataSize = lengthSize + compressedIterations;
                            }

                            EnsureBufferSize(dataSize + offset);

                            fixed (byte* bufferPtr = &buffer[offset])
                            {
                                switch (lengthSize)
                                {
                                    case 1:
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                            *sbyteBufferPtr = (sbyte)length;
                                        }
                                        break;

                                    case 3:
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                            *sbyteBufferPtr++ = -2;
                                            short* shortBufferPtr = (short*)sbyteBufferPtr;
                                            *shortBufferPtr = (short)length;
                                        }
                                        break;

                                    case 4:
                                        {
                                            int* intBufferPtr = (int*)bufferPtr;
                                            *intBufferPtr = length;
                                        }
                                        break;

                                    case 5:
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)bufferPtr;
                                            *sbyteBufferPtr++ = -4;
                                            int* intBufferPtr = (int*)sbyteBufferPtr;
                                            *intBufferPtr = length;
                                        }
                                        break;

                                    default:
                                        throw new UnsupportedException("Unsupported length size: " + lengthSize);
                                }
                            }

                            switch (elementSerializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            List<byte> value = (List<byte>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                byteBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.SByte:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            sbyte* sbyteBufferPtr = (sbyte*)byteBufferPtr;

                                            List<sbyte> value = (List<sbyte>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                sbyteBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Bool:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            List<bool> value = (List<bool>)baseValue;

                                            if (compressBoolArray)
                                            {
                                                int index = 0;
                                                for (int i = 0; i < compressedIterations; i++)
                                                {
                                                    byte result = 0;

                                                    if (value[index])
                                                        result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    if (++index < elementCount)
                                                        if (value[index])
                                                            result |= (byte)(1 << (7 - index));

                                                    byteBufferPtr[i] = result;
                                                }
                                            }
                                            else
                                            {
                                                bool* boolBufferPtr = (bool*)byteBufferPtr;

                                                for (int i = 0; i < elementCount; i++)
                                                {
                                                    boolBufferPtr[i] = value[i];
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Int16:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            short* shortBufferPtr = (short*)byteBufferPtr;

                                            List<short> value = (List<short>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                shortBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Int32:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            List<int> value = (List<int>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                intBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Int64:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            short* shortBufferPtr = (short*)byteBufferPtr;

                                            List<short> value = (List<short>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                shortBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.UInt16:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            ushort* ushortBufferPtr = (ushort*)byteBufferPtr;

                                            List<ushort> value = (List<ushort>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                ushortBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.UInt32:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            uint* uintBufferPtr = (uint*)byteBufferPtr;

                                            List<uint> value = (List<uint>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                uintBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.UInt64:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            ulong* ulongBufferPtr = (ulong*)byteBufferPtr;

                                            List<ulong> value = (List<ulong>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                ulongBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Half:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            ushort* ushortBufferPtr = (ushort*)byteBufferPtr;

                                            List<Half> value = (List<Half>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                ushortBufferPtr[i] = value[i].value;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Single:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<float> value = (List<float>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                floatBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Double:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            double* doubleBufferPtr = (double*)byteBufferPtr;

                                            List<double> value = (List<double>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                doubleBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Char:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            char* charBufferPtr = (char*)byteBufferPtr;

                                            List<char> value = (List<char>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                charBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Decimal:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            decimal* decimalBufferPtr = (decimal*)byteBufferPtr;

                                            List<decimal> value = (List<decimal>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                decimalBufferPtr[i] = value[i];
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.DateTime:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            long* longBufferPtr = (long*)byteBufferPtr;

                                            List<DateTime> value = (List<DateTime>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                longBufferPtr[i] = value[i].Ticks;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.DateTimeOffset:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            long* longBufferPtr = (long*)byteBufferPtr;

                                            List<DateTimeOffset> value = (List<DateTimeOffset>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                longBufferPtr[i] = value[i].Ticks;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.TimeSpan:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            long* longBufferPtr = (long*)byteBufferPtr;

                                            List<TimeSpan> value = (List<TimeSpan>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                longBufferPtr[i] = value[i].Ticks;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Vector2:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<Vector2> value = (List<Vector2>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                Vector2 elementValue = value[i];

                                                floatBufferPtr[(i * 2) + 0] = elementValue.x;
                                                floatBufferPtr[(i * 2) + 1] = elementValue.y;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Vector3:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<Vector3> value = (List<Vector3>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                Vector3 elementValue = value[i];

                                                floatBufferPtr[(i * 3) + 0] = elementValue.x;
                                                floatBufferPtr[(i * 3) + 1] = elementValue.y;
                                                floatBufferPtr[(i * 3) + 2] = elementValue.z;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Vector4:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<Vector4> value = (List<Vector4>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                Vector4 elementValue = value[i];

                                                floatBufferPtr[(i * 4) + 0] = elementValue.x;
                                                floatBufferPtr[(i * 4) + 1] = elementValue.y;
                                                floatBufferPtr[(i * 4) + 2] = elementValue.z;
                                                floatBufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Quaternion:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<Quaternion> value = (List<Quaternion>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                Quaternion elementValue = value[i];

                                                floatBufferPtr[(i * 4) + 0] = elementValue.x;
                                                floatBufferPtr[(i * 4) + 1] = elementValue.y;
                                                floatBufferPtr[(i * 4) + 2] = elementValue.z;
                                                floatBufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Rect:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<Rect> value = (List<Rect>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                Rect elementValue = value[i];

                                                floatBufferPtr[(i * 4) + 0] = elementValue.x;
                                                floatBufferPtr[(i * 4) + 1] = elementValue.y;
                                                floatBufferPtr[(i * 4) + 2] = elementValue.width;
                                                floatBufferPtr[(i * 4) + 3] = elementValue.height;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.Bounds:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            float* floatBufferPtr = (float*)byteBufferPtr;

                                            List<Bounds> value = (List<Bounds>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                Bounds elementValue = value[i];

                                                Vector3 center = elementValue.center;
                                                Vector3 size = elementValue.size;

                                                floatBufferPtr[(i * 6) + 0] = center.x;
                                                floatBufferPtr[(i * 6) + 1] = center.y;
                                                floatBufferPtr[(i * 6) + 2] = center.z;
                                                floatBufferPtr[(i * 6) + 3] = size.x;
                                                floatBufferPtr[(i * 6) + 4] = size.y;
                                                floatBufferPtr[(i * 6) + 5] = size.z;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.IntVector2:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            List<IntVector2> value = (List<IntVector2>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                IntVector2 elementValue = value[i];

                                                intBufferPtr[(i * 2) + 0] = elementValue.x;
                                                intBufferPtr[(i * 2) + 1] = elementValue.y;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.IntVector3:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            List<IntVector3> value = (List<IntVector3>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                IntVector3 elementValue = value[i];

                                                intBufferPtr[(i * 3) + 0] = elementValue.x;
                                                intBufferPtr[(i * 3) + 1] = elementValue.y;
                                                intBufferPtr[(i * 3) + 2] = elementValue.z;
                                            }
                                        }
                                    }
                                    break;

                                case SerializationType.IntVector4:
                                    {
                                        fixed (byte* byteBufferPtr = &buffer[offset + lengthSize])
                                        {
                                            int* intBufferPtr = (int*)byteBufferPtr;

                                            List<IntVector4> value = (List<IntVector4>)baseValue;

                                            for (int i = 0; i < elementCount; i++)
                                            {
                                                IntVector4 elementValue = value[i];

                                                intBufferPtr[(i * 4) + 0] = elementValue.x;
                                                intBufferPtr[(i * 4) + 1] = elementValue.y;
                                                intBufferPtr[(i * 4) + 2] = elementValue.z;
                                                intBufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    throw new UnsupportedException("elementSerializationType for array is not supported: " + elementSerializationType);
                            }
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            SerializationType elementSerializationType = simpleTypeInfo.elementSerializationType;
                            int elementSize = simpleTypeInfo.elementPrimitiveSize;

                            switch (elementSerializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        byte? value = EmitHelper<byte?>.CastFrom(obj, emitValueTypeCaster);
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
                                        sbyte? value = EmitHelper<sbyte?>.CastFrom(obj, emitValueTypeCaster);
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
                                        bool? value = EmitHelper<bool?>.CastFrom(obj, emitValueTypeCaster);
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
                                        short? value = EmitHelper<short?>.CastFrom(obj, emitValueTypeCaster);
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
                                        int? value = EmitHelper<int?>.CastFrom(obj, emitValueTypeCaster);
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
                                        long? value = EmitHelper<long?>.CastFrom(obj, emitValueTypeCaster);
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
                                        ushort? value = EmitHelper<ushort?>.CastFrom(obj, emitValueTypeCaster);
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
                                        uint? value = EmitHelper<uint?>.CastFrom(obj, emitValueTypeCaster);
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
                                        ulong? value = EmitHelper<ulong?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Half? value = EmitHelper<Half?>.CastFrom(obj, emitValueTypeCaster);
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
                                        float? value = EmitHelper<float?>.CastFrom(obj, emitValueTypeCaster);
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
                                        double? value = EmitHelper<double?>.CastFrom(obj, emitValueTypeCaster);
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
                                        char? value = EmitHelper<char?>.CastFrom(obj, emitValueTypeCaster);
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
                                        decimal? value = EmitHelper<decimal?>.CastFrom(obj, emitValueTypeCaster);
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
                                        DateTime? value = EmitHelper<DateTime?>.CastFrom(obj, emitValueTypeCaster);
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
                                        DateTimeOffset? value = EmitHelper<DateTimeOffset?>.CastFrom(obj, emitValueTypeCaster);
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
                                        TimeSpan? value = EmitHelper<TimeSpan?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Vector2? value = EmitHelper<Vector2?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Vector3? value = EmitHelper<Vector3?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Vector4? value = EmitHelper<Vector4?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Quaternion? value = EmitHelper<Quaternion?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Rect? value = EmitHelper<Rect?>.CastFrom(obj, emitValueTypeCaster);
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
                                        Bounds? value = EmitHelper<Bounds?>.CastFrom(obj, emitValueTypeCaster);
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
                                        IntVector2? value = EmitHelper<IntVector2?>.CastFrom(obj, emitValueTypeCaster);
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
                                        IntVector3? value = EmitHelper<IntVector3?>.CastFrom(obj, emitValueTypeCaster);
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
                                        IntVector4? value = EmitHelper<IntVector4?>.CastFrom(obj, emitValueTypeCaster);
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
                            if (!simpleTypeInfo.isValueType)
                            {
                                dataSize += SerializationTypeSizes.SBYTE;
                                EnsureBufferSize(dataSize + offset);

                                sbyte headerValue = obj != null ? (sbyte)0 : (sbyte)-1;

                                fixed (byte* byteBufferPtr = &buffer[offset])
                                {
                                    sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                                    *bufferPtr = headerValue;
                                }

                                if (headerValue == -1)
                                    break;
                            }

                            AdvancedTypeInfo advancedTypeInfo = AdvancedTypeInfo<TSource>.info;

                            if (!advancedTypeInfo.emittedFieldGetters)
                            {
                                advancedTypeInfo.EmitFieldGetters(settings.avoidCustomStructBoxing);
                            }

                            if (settings.avoidCustomStructBoxing && simpleTypeInfo.isValueType)
                            {
                                TypedReference objTypedRef = __makeref(obj);

                                for (int i = 0; i < advancedTypeInfo.fieldTypeInfos.Length; i++)
                                {
                                    AdvancedTypeInfo.FieldTypeInfo fieldTypeInfo = advancedTypeInfo.fieldTypeInfos[i];

                                    int fieldDataSize = 0;
                                    int fieldOffset = dataSize + offset;

                                    switch (fieldTypeInfo.fieldSerializationType)
                                    {
                                        case SerializationType.String:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<string>)fieldTypeInfo.getter;
                                                string value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Byte:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<byte>)fieldTypeInfo.getter;
                                                byte value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.SByte:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<sbyte>)fieldTypeInfo.getter;
                                                sbyte value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Bool:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<bool>)fieldTypeInfo.getter;
                                                bool value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Int16:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<short>)fieldTypeInfo.getter;
                                                short value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Int32:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<int>)fieldTypeInfo.getter;
                                                int value = getterDelegate.Invoke(objTypedRef);

                                                //fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;

                                                fieldDataSize = SerializationTypeSizes.INT32;

                                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                                {
                                                    int* bufferPtr = (int*)byteBufferPtr;
                                                    *bufferPtr = value;
                                                }
                                            }
                                            break;
                                        case SerializationType.Int64:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<long>)fieldTypeInfo.getter;
                                                long value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.UInt16:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<ushort>)fieldTypeInfo.getter;
                                                ushort value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.UInt32:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<uint>)fieldTypeInfo.getter;
                                                uint value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.UInt64:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<ulong>)fieldTypeInfo.getter;
                                                ulong value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Half:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Half>)fieldTypeInfo.getter;
                                                Half value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Single:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<float>)fieldTypeInfo.getter;
                                                float value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Double:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<double>)fieldTypeInfo.getter;
                                                double value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Char:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<char>)fieldTypeInfo.getter;
                                                char value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Decimal:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<decimal>)fieldTypeInfo.getter;
                                                decimal value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.DateTime:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<DateTime>)fieldTypeInfo.getter;
                                                DateTime value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.DateTimeOffset:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<DateTimeOffset>)fieldTypeInfo.getter;
                                                DateTimeOffset value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.TimeSpan:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<TimeSpan>)fieldTypeInfo.getter;
                                                TimeSpan value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Vector2:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Vector2>)fieldTypeInfo.getter;
                                                Vector2 value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Vector3:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Vector3>)fieldTypeInfo.getter;
                                                Vector3 value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Vector4:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Vector4>)fieldTypeInfo.getter;
                                                Vector4 value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Quaternion:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Quaternion>)fieldTypeInfo.getter;
                                                Quaternion value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Rect:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Rect>)fieldTypeInfo.getter;
                                                Rect value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Bounds:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Bounds>)fieldTypeInfo.getter;
                                                Bounds value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.IntVector2:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<IntVector2>)fieldTypeInfo.getter;
                                                IntVector2 value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.IntVector3:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<IntVector3>)fieldTypeInfo.getter;
                                                IntVector3 value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.IntVector4:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<IntVector4>)fieldTypeInfo.getter;
                                                IntVector4 value = getterDelegate.Invoke(objTypedRef);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.PrimitiveArray:
                                        case SerializationType.PrimitiveList:
                                        case SerializationType.ObjectArray:
                                        case SerializationType.ObjectList:
                                        case SerializationType.PrimitiveNullable:
                                        case SerializationType.ObjectNullable:
                                        case SerializationType.Object:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                                object value = getterDelegate.Invoke(obj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        default:
                                            throw new UnsupportedException("Unsupported serialization type: " + fieldTypeInfo.fieldSerializationType);
                                    }

                                    dataSize += fieldDataSize;
                                }
                            }
                            else
                            {
                                object boxedObj = obj;

                                for (int i = 0; i < advancedTypeInfo.fieldTypeInfos.Length; i++)
                                {
                                    AdvancedTypeInfo.FieldTypeInfo fieldTypeInfo = advancedTypeInfo.fieldTypeInfos[i];

                                    int fieldDataSize = 0;

                                    switch (fieldTypeInfo.fieldSerializationType)
                                    {
                                        case SerializationType.String:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, string>)fieldTypeInfo.getter;
                                                string value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Byte:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, byte>)fieldTypeInfo.getter;
                                                byte value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.SByte:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, sbyte>)fieldTypeInfo.getter;
                                                sbyte value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Bool:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, bool>)fieldTypeInfo.getter;
                                                bool value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Int16:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, short>)fieldTypeInfo.getter;
                                                short value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Int32:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, int>)fieldTypeInfo.getter;
                                                int value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Int64:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, long>)fieldTypeInfo.getter;
                                                long value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.UInt16:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, ushort>)fieldTypeInfo.getter;
                                                ushort value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.UInt32:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, uint>)fieldTypeInfo.getter;
                                                uint value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.UInt64:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, ulong>)fieldTypeInfo.getter;
                                                ulong value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Half:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Half>)fieldTypeInfo.getter;
                                                Half value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Single:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, float>)fieldTypeInfo.getter;
                                                float value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Double:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, double>)fieldTypeInfo.getter;
                                                double value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Char:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, char>)fieldTypeInfo.getter;
                                                char value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Decimal:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, decimal>)fieldTypeInfo.getter;
                                                decimal value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.DateTime:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, DateTime>)fieldTypeInfo.getter;
                                                DateTime value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.DateTimeOffset:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, DateTimeOffset>)fieldTypeInfo.getter;
                                                DateTimeOffset value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.TimeSpan:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, TimeSpan>)fieldTypeInfo.getter;
                                                TimeSpan value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Vector2:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Vector2>)fieldTypeInfo.getter;
                                                Vector2 value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Vector3:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Vector3>)fieldTypeInfo.getter;
                                                Vector3 value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Vector4:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Vector4>)fieldTypeInfo.getter;
                                                Vector4 value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Quaternion:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Quaternion>)fieldTypeInfo.getter;
                                                Quaternion value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Rect:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Rect>)fieldTypeInfo.getter;
                                                Rect value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.Bounds:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Bounds>)fieldTypeInfo.getter;
                                                Bounds value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.IntVector2:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, IntVector2>)fieldTypeInfo.getter;
                                                IntVector2 value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.IntVector3:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, IntVector3>)fieldTypeInfo.getter;
                                                IntVector3 value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.IntVector4:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, IntVector4>)fieldTypeInfo.getter;
                                                IntVector4 value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        case SerializationType.PrimitiveArray:
                                        case SerializationType.PrimitiveList:
                                        case SerializationType.ObjectArray:
                                        case SerializationType.ObjectList:
                                        case SerializationType.PrimitiveNullable:
                                        case SerializationType.ObjectNullable:
                                        case SerializationType.Object:
                                            {
                                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                                object value = getterDelegate.Invoke(boxedObj);

                                                fieldDataSize = SerializeIntoBuffer(value, offset + dataSize) - dataSize;
                                            }
                                            break;
                                        default:
                                            throw new UnsupportedException("Unsupported serialization type: " + fieldTypeInfo.fieldSerializationType);
                                    }

                                    dataSize += fieldDataSize;
                                }
                            }
                        }
                        break;
                    case SerializationType.ObjectArray:
                        {
                            throw new NotImplementedException();
                        }
                    case SerializationType.ObjectList:
                        {
                            throw new NotImplementedException();
                        }
                    case SerializationType.ObjectNullable:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        throw new UnsupportedException("Unsupported serialization type: " + simpleTypeInfo.serializationType);
                }

                return offset + dataSize;
            }
        }

        private int SerializeIntoBuffer(object boxedObj, int offset)
        {
            // TODO: Copy the generic method but use linearly search list cache for simple and advanced type info
            throw new NotImplementedException();
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
