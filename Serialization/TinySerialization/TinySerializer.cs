#if UNSAFE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private bool hasCustomTypeResolvers;

        public TinySerializer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            this.buffer = new byte[bufferSize];
        }

        public TinySerializer(int bufferSize, TinySerializerSettings settings) : this(bufferSize)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Adds a custom type resolver for manual serialization and deserialization of a specific type.
        /// </summary>
        /// <param name="customTypeResolver">Custom type resolver instance.</param>
        public void AddCustomTypeResolver(CustomTypeResolver customTypeResolver)
        {
            if (customTypeResolver == null)
                throw new ArgumentNullException(nameof(customTypeResolver));

            if (customTypeResolvers == null)
            {
                customTypeResolvers = new List<CustomTypeResolver>();
                hasCustomTypeResolvers = true;
            }

            customTypeResolvers.Add(customTypeResolver);
        }

        public void Prewarm<TSource>() where TSource : new()
        {
            Type tSource = typeof(TSource);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tSource);

            typeInfo.EmitDefaultConstructor<TSource>();
            typeInfo.EmitObjectDefaultConstructor();

            if (typeInfo.serializationType == SerializationType.Object)
            {
                typeInfo.InspectFields();
                typeInfo.InspectProperties();
                typeInfo.EmitFieldGetters();
                typeInfo.EmitFieldSetters();
                typeInfo.EmitPropertyGetters();
                typeInfo.EmitPropertySetters();

                if (typeInfo.isValueType)
                {
                    typeInfo.EmitFieldGettersByTypedRef();
                    typeInfo.EmitFieldSettersByTypedRef();
                    typeInfo.EmitPropertyGettersByTypedRef();
                    typeInfo.EmitPropertySettersByTypedRef();
                }

                // TODO: Prewarm types of fields and properties recursively (pass through list of already prewarmed types to avoid infinite loops)

                TinySerializerTypeInfo.GetTypeInfo(tSource);
            }
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

            int dataSize = ExperimentalSerializeIntoBuffer(obj, typeInfo, 0);

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

        // Returns size
        private int ExperimentalSerializeIntoBuffer<TSource>(TSource obj, TinySerializerTypeInfo typeInfo, int offset)
        {
            int dataSize = 0;

            switch (typeInfo.serializationType)
            {
                #region PRIMITIVE
                case SerializationType.Byte:
                    {
                        dataSize = SerializationTypeSizes.BYTE;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<byte>.CastFrom(obj);

                        fixed (byte* bufferPtr = &buffer[offset])
                        {
                            *bufferPtr = value;
                        }
                    }
                    break;
                case SerializationType.SByte:
                    {
                        dataSize = SerializationTypeSizes.SBYTE;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<sbyte>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<bool>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<short>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<int>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<long>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<ushort>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<uint>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<ulong>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Half>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<float>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<double>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<char>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<decimal>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<DateTime>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<DateTimeOffset>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<TimeSpan>.CastFrom(obj);

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
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Vector2>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            float* bufferPtr = (float*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                        }
                    }
                    break;
                case SerializationType.Vector3:
                    {
                        dataSize = SerializationTypeSizes.VECTOR3;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Vector3>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            float* bufferPtr = (float*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                            bufferPtr[2] = value.z;
                        }
                    }
                    break;
                case SerializationType.Vector4:
                    {
                        dataSize = SerializationTypeSizes.VECTOR4;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Vector4>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            float* bufferPtr = (float*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                            bufferPtr[2] = value.z;
                            bufferPtr[3] = value.w;
                        }
                    }
                    break;
                case SerializationType.Quaternion:
                    {
                        dataSize = SerializationTypeSizes.QUATERNION;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Quaternion>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            float* bufferPtr = (float*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                            bufferPtr[2] = value.z;
                            bufferPtr[3] = value.w;
                        }
                    }
                    break;
                case SerializationType.Rect:
                    {
                        dataSize = SerializationTypeSizes.RECT;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Rect>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            float* bufferPtr = (float*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                            bufferPtr[2] = value.width;
                            bufferPtr[3] = value.height;
                        }
                    }
                    break;
                case SerializationType.Bounds:
                    {
                        dataSize = SerializationTypeSizes.BOUNDS;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<Bounds>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            float* bufferPtr = (float*)byteBufferPtr;
                            Vector3 center = value.center;
                            Vector3 size = value.size;
                            bufferPtr[0] = center.x;
                            bufferPtr[1] = center.y;
                            bufferPtr[2] = center.z;
                            bufferPtr[3] = size.x;
                            bufferPtr[4] = size.y;
                            bufferPtr[5] = size.z;
                        }
                    }
                    break;
                case SerializationType.IntVector2:
                    {
                        dataSize = SerializationTypeSizes.INT_VECTOR2;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<IntVector2>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            int* bufferPtr = (int*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                        }
                    }
                    break;
                case SerializationType.IntVector3:
                    {
                        dataSize = SerializationTypeSizes.INT_VECTOR3;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<IntVector3>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            int* bufferPtr = (int*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                            bufferPtr[2] = value.z;
                        }
                    }
                    break;
                case SerializationType.IntVector4:
                    {
                        dataSize = SerializationTypeSizes.INT_VECTOR4;
                        EnsureBufferSize(offset + dataSize);

                        var value = EmitHelper<IntVector4>.CastFrom(obj);

                        fixed (byte* byteBufferPtr = &buffer[offset])
                        {
                            int* bufferPtr = (int*)byteBufferPtr;
                            bufferPtr[0] = value.x;
                            bufferPtr[1] = value.y;
                            bufferPtr[2] = value.z;
                            bufferPtr[3] = value.w;
                        }
                    }
                    break;
                #endregion
                case SerializationType.String:
                    {
                        var value = (string)(object)obj;
                        dataSize = ExperimentalSerializeStringIntoBuffer(value, offset);
                    }
                    break;
                case SerializationType.PrimitiveArray:
                    {
                        Array value = (Array)(object)obj;

                        bool isNull = value == null;
                        int arrLength = value?.Length ?? -1;
                        dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, arrLength, offset);
                        if (isNull)
                            break;

                        // TODO: Object cyclic reference

                        int arrOffset = dataSize + offset;
                        TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
                        int elementSize = elementTypeInfo.primitiveSize;
                        dataSize += elementSize * arrLength;
                        EnsureBufferSize(offset + dataSize);

                        fixed (byte* byteBufferPtr = &buffer[arrOffset])
                        {
                            switch (elementTypeInfo.serializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        var arrValue = (byte[])value;

                                        fixed (byte* valuePtr = arrValue)
                                        {
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                byteBufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.SByte:
                                    {
                                        var arrValue = (sbyte[])value;

                                        fixed (sbyte* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (sbyte*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Bool:
                                    {
                                        var arrValue = (bool[])value;

                                        fixed (bool* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (bool*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Int16:
                                    {
                                        var arrValue = (short[])value;

                                        fixed (short* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (short*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Int32:
                                    {
                                        var arrValue = (int[])value;

                                        fixed (int* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Int64:
                                    {
                                        var arrValue = (long[])value;

                                        fixed (long* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.UInt16:
                                    {
                                        var arrValue = (ushort[])value;

                                        fixed (ushort* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.UInt32:
                                    {
                                        var arrValue = (uint[])value;

                                        fixed (uint* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (uint*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.UInt64:
                                    {
                                        var arrValue = (ulong[])value;

                                        fixed (ulong* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (ulong*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Half:
                                    {
                                        var arrValue = (Half[])value;

                                        fixed (Half* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i].value;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Single:
                                    {
                                        var arrValue = (float[])value;

                                        fixed (float* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Double:
                                    {
                                        var arrValue = (double[])value;

                                        fixed (double* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (double*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Char:
                                    {
                                        var arrValue = (char[])value;

                                        fixed (char* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (char*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Decimal:
                                    {
                                        var arrValue = (decimal[])value;

                                        fixed (decimal* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (decimal*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i];
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.DateTime:
                                    {
                                        var arrValue = (DateTime[])value;

                                        fixed (DateTime* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i].Ticks;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.DateTimeOffset:
                                    {
                                        var arrValue = (DateTimeOffset[])value;

                                        fixed (DateTimeOffset* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i].Ticks;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.TimeSpan:
                                    {
                                        var arrValue = (TimeSpan[])value;

                                        fixed (TimeSpan* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                bufferPtr[i] = valuePtr[i].Ticks;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Vector2:
                                    {
                                        var arrValue = (Vector2[])value;

                                        fixed (Vector2* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
                                                bufferPtr[(i * 2) + 0] = elementValue.x;
                                                bufferPtr[(i * 2) + 1] = elementValue.y;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Vector3:
                                    {
                                        var arrValue = (Vector3[])value;

                                        fixed (Vector3* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
                                                bufferPtr[(i * 3) + 0] = elementValue.x;
                                                bufferPtr[(i * 3) + 1] = elementValue.y;
                                                bufferPtr[(i * 3) + 2] = elementValue.z;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.Vector4:
                                    {
                                        var arrValue = (Vector4[])value;

                                        fixed (Vector4* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
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
                                        var arrValue = (Quaternion[])value;

                                        fixed (Quaternion* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
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
                                        var arrValue = (Rect[])value;

                                        fixed (Rect* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
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
                                        var arrValue = (Bounds[])value;

                                        fixed (Bounds* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
                                                var center = elementValue.center;
                                                var size = elementValue.size;
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
                                        var arrValue = (IntVector2[])value;

                                        fixed (IntVector2* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
                                                bufferPtr[(i * 2) + 0] = elementValue.x;
                                                bufferPtr[(i * 2) + 1] = elementValue.y;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector3:
                                    {
                                        var arrValue = (IntVector3[])value;

                                        fixed (IntVector3* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
                                                bufferPtr[(i * 3) + 0] = elementValue.x;
                                                bufferPtr[(i * 3) + 1] = elementValue.y;
                                                bufferPtr[(i * 3) + 2] = elementValue.z;
                                            }
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector4:
                                    {
                                        var arrValue = (IntVector4[])value;

                                        fixed (IntVector4* valuePtr = arrValue)
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int i = 0; i < arrLength; i++)
                                            {
                                                var elementValue = valuePtr[i];
                                                bufferPtr[(i * 4) + 0] = elementValue.x;
                                                bufferPtr[(i * 4) + 1] = elementValue.y;
                                                bufferPtr[(i * 4) + 2] = elementValue.z;
                                                bufferPtr[(i * 4) + 3] = elementValue.w;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                            }
                        }
                    }
                    break;
                case SerializationType.PrimitiveList:
                    {
                        IList value = (IList)(object)obj;

                        bool isNull = value == null;
                        int listLength = value?.Count ?? -1;
                        dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, listLength, offset);
                        if (isNull)
                            break;

                        // TODO: Object cyclic reference

                        int listOffset = offset + dataSize;
                        TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
                        int elementSize = elementTypeInfo.primitiveSize;
                        dataSize += elementSize * listLength;
                        EnsureBufferSize(offset + dataSize);

                        fixed (byte* byteBufferPtr = &buffer[listOffset])
                        {
                            switch (elementTypeInfo.serializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        var listValue = (List<byte>)value;

                                        for (int i = 0; i < listLength; i++)
                                        {
                                            byteBufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.SByte:
                                    {
                                        var listValue = (List<sbyte>)value;

                                        var bufferPtr = (sbyte*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Bool:
                                    {
                                        var listValue = (List<bool>)value;

                                        var bufferPtr = (bool*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Int16:
                                    {
                                        var listValue = (List<short>)value;

                                        var bufferPtr = (short*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Int32:
                                    {
                                        var listValue = (List<int>)value;

                                        var bufferPtr = (int*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Int64:
                                    {
                                        var listValue = (List<long>)value;

                                        var bufferPtr = (long*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.UInt16:
                                    {
                                        var listValue = (List<ushort>)value;

                                        var bufferPtr = (ushort*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.UInt32:
                                    {
                                        var listValue = (List<uint>)value;

                                        var bufferPtr = (uint*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.UInt64:
                                    {
                                        var listValue = (List<ulong>)value;

                                        var bufferPtr = (ulong*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Half:
                                    {
                                        var listValue = (List<Half>)value;

                                        var bufferPtr = (ushort*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i].value;
                                        }
                                    }
                                    break;
                                case SerializationType.Single:
                                    {
                                        var listValue = (List<float>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Double:
                                    {
                                        var listValue = (List<double>)value;

                                        var bufferPtr = (double*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Char:
                                    {
                                        var listValue = (List<char>)value;

                                        var bufferPtr = (char*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.Decimal:
                                    {
                                        var listValue = (List<decimal>)value;

                                        var bufferPtr = (decimal*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i];
                                        }
                                    }
                                    break;
                                case SerializationType.DateTime:
                                    {
                                        var listValue = (List<DateTime>)value;

                                        var bufferPtr = (long*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i].Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTimeOffset:
                                    {
                                        var listValue = (List<DateTimeOffset>)value;

                                        var bufferPtr = (long*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i].Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.TimeSpan:
                                    {
                                        var listValue = (List<TimeSpan>)value;

                                        var bufferPtr = (long*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            bufferPtr[i] = listValue[i].Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector2:
                                    {
                                        var listValue = (List<Vector2>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 2) + 0] = elementValue.x;
                                            bufferPtr[(i * 2) + 1] = elementValue.y;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector3:
                                    {
                                        var listValue = (List<Vector3>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 3) + 0] = elementValue.x;
                                            bufferPtr[(i * 3) + 1] = elementValue.y;
                                            bufferPtr[(i * 3) + 2] = elementValue.z;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector4:
                                    {
                                        var listValue = (List<Vector4>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 4) + 0] = elementValue.x;
                                            bufferPtr[(i * 4) + 1] = elementValue.y;
                                            bufferPtr[(i * 4) + 2] = elementValue.z;
                                            bufferPtr[(i * 4) + 3] = elementValue.w;
                                        }
                                    }
                                    break;
                                case SerializationType.Quaternion:
                                    {
                                        var listValue = (List<Quaternion>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 4) + 0] = elementValue.x;
                                            bufferPtr[(i * 4) + 1] = elementValue.y;
                                            bufferPtr[(i * 4) + 2] = elementValue.z;
                                            bufferPtr[(i * 4) + 3] = elementValue.w;
                                        }
                                    }
                                    break;
                                case SerializationType.Rect:
                                    {
                                        var listValue = (List<Rect>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 4) + 0] = elementValue.x;
                                            bufferPtr[(i * 4) + 1] = elementValue.y;
                                            bufferPtr[(i * 4) + 2] = elementValue.width;
                                            bufferPtr[(i * 4) + 3] = elementValue.height;
                                        }
                                    }
                                    break;
                                case SerializationType.Bounds:
                                    {
                                        var listValue = (List<Bounds>)value;

                                        var bufferPtr = (float*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            var center = elementValue.center;
                                            var size = elementValue.size;
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
                                        var listValue = (List<IntVector2>)value;

                                        var bufferPtr = (int*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 2) + 0] = elementValue.x;
                                            bufferPtr[(i * 2) + 1] = elementValue.y;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector3:
                                    {
                                        var listValue = (List<IntVector3>)value;

                                        var bufferPtr = (int*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 3) + 0] = elementValue.x;
                                            bufferPtr[(i * 3) + 1] = elementValue.y;
                                            bufferPtr[(i * 3) + 2] = elementValue.z;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector4:
                                    {
                                        var listValue = (List<IntVector4>)value;

                                        var bufferPtr = (int*)byteBufferPtr;
                                        for (int i = 0; i < listLength; i++)
                                        {
                                            var elementValue = listValue[i];
                                            bufferPtr[(i * 4) + 0] = elementValue.x;
                                            bufferPtr[(i * 4) + 1] = elementValue.y;
                                            bufferPtr[(i * 4) + 2] = elementValue.z;
                                            bufferPtr[(i * 4) + 3] = elementValue.w;
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                            }
                        }
                    }
                    break;
                case SerializationType.PrimitiveNullable:
                    {
                        switch (typeInfo.elementTypeInfo.serializationType)
                        {
                            case SerializationType.Byte:
                                {
                                    var value = EmitHelper<byte?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.BYTE;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        *byteBufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.SByte:
                                {
                                    var value = EmitHelper<sbyte?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.SBYTE;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (sbyte*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Bool:
                                {
                                    var value = EmitHelper<bool?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.BOOL;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (bool*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Int16:
                                {
                                    var value = EmitHelper<short?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.INT16;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (short*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Int32:
                                {
                                    var value = EmitHelper<int?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.INT32;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (int*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Int64:
                                {
                                    var value = EmitHelper<long?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.INT64;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (long*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.UInt16:
                                {
                                    var value = EmitHelper<ushort?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.UINT16;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (ushort*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.UInt32:
                                {
                                    var value = EmitHelper<uint?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.UINT32;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (uint*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.UInt64:
                                {
                                    var value = EmitHelper<ulong?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.UINT64;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (ulong*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Half:
                                {
                                    var value = EmitHelper<Half?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.HALF;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (ushort*)byteBufferPtr;
                                        *bufferPtr = value.Value.value;
                                    }
                                }
                                break;
                            case SerializationType.Single:
                                {
                                    var value = EmitHelper<float?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.SINGLE;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Double:
                                {
                                    var value = EmitHelper<double?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.DOUBLE;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (double*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Char:
                                {
                                    var value = EmitHelper<char?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.CHAR;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (char*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.Decimal:
                                {
                                    var value = EmitHelper<decimal?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.DECIMAL;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (decimal*)byteBufferPtr;
                                        *bufferPtr = value.Value;
                                    }
                                }
                                break;
                            case SerializationType.DateTime:
                                {
                                    var value = EmitHelper<DateTime?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.DATE_TIME;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (long*)byteBufferPtr;
                                        *bufferPtr = value.Value.Ticks;
                                    }
                                }
                                break;
                            case SerializationType.DateTimeOffset:
                                {
                                    var value = EmitHelper<DateTimeOffset?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.DATE_TIME_OFFSET;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (long*)byteBufferPtr;
                                        *bufferPtr = value.Value.Ticks;
                                    }
                                }
                                break;
                            case SerializationType.TimeSpan:
                                {
                                    var value = EmitHelper<TimeSpan?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.TIME_SPAN;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (long*)byteBufferPtr;
                                        *bufferPtr = value.Value.Ticks;
                                    }
                                }
                                break;
                            case SerializationType.Vector2:
                                {
                                    var value = EmitHelper<Vector2?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.VECTOR2;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                    }
                                }
                                break;
                            case SerializationType.Vector3:
                                {
                                    var value = EmitHelper<Vector3?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.VECTOR3;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                        bufferPtr[2] = elementValue.z;
                                    }
                                }
                                break;
                            case SerializationType.Vector4:
                                {
                                    var value = EmitHelper<Vector4?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.VECTOR4;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                        bufferPtr[2] = elementValue.z;
                                        bufferPtr[3] = elementValue.w;
                                    }
                                }
                                break;
                            case SerializationType.Quaternion:
                                {
                                    var value = EmitHelper<Quaternion?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.QUATERNION;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                        bufferPtr[2] = elementValue.z;
                                        bufferPtr[3] = elementValue.w;
                                    }
                                }
                                break;
                            case SerializationType.Rect:
                                {
                                    var value = EmitHelper<Rect?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.RECT;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                        bufferPtr[2] = elementValue.width;
                                        bufferPtr[3] = elementValue.height;
                                    }
                                }
                                break;
                            case SerializationType.Bounds:
                                {
                                    var value = EmitHelper<Bounds?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.BOUNDS;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (float*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        var center = elementValue.center;
                                        var size = elementValue.size;
                                        bufferPtr[0] = center.x;
                                        bufferPtr[1] = center.y;
                                        bufferPtr[2] = center.z;
                                        bufferPtr[3] = size.x;
                                        bufferPtr[4] = size.y;
                                        bufferPtr[5] = size.z;
                                    }
                                }
                                break;
                            case SerializationType.IntVector2:
                                {
                                    var value = EmitHelper<IntVector2?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.INT_VECTOR2;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (int*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                    }
                                }
                                break;
                            case SerializationType.IntVector3:
                                {
                                    var value = EmitHelper<IntVector3?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.INT_VECTOR3;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (int*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                        bufferPtr[2] = elementValue.z;
                                    }
                                }
                                break;
                            case SerializationType.IntVector4:
                                {
                                    var value = EmitHelper<IntVector4?>.CastFrom(obj);
                                    bool isNull = value == null;
                                    dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                                    if (isNull)
                                        break;

                                    int valueOffset = offset + dataSize;
                                    dataSize += SerializationTypeSizes.INT_VECTOR4;
                                    EnsureBufferSize(offset + dataSize);

                                    fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                    {
                                        var bufferPtr = (int*)byteBufferPtr;
                                        var elementValue = value.Value;
                                        bufferPtr[0] = elementValue.x;
                                        bufferPtr[1] = elementValue.y;
                                        bufferPtr[2] = elementValue.z;
                                        bufferPtr[3] = elementValue.w;
                                    }
                                }
                                break;
                            default:
                                throw new InvalidOperationException("Serialiation type is not primitive: " + typeInfo.elementTypeInfo.serializationType);
                        }
                    }
                    break;
                case SerializationType.ObjectNullable:
                case SerializationType.ObjectList:
                case SerializationType.ObjectArray:
                case SerializationType.Object:
                    {
                        if (hasCustomTypeResolvers)
                        {
                            for (int i = 0; i < customTypeResolvers.Count; i++)
                            {
                                CustomTypeResolver customTypeResolver = customTypeResolvers[i];

                                if (customTypeResolver.typeHashCode == typeInfo.typeHashCode)
                                {
                                    if (typeInfo.isValueType)
                                    {
                                        TypedReference objRef = __makeref(obj);
                                        dataSize = customTypeResolver.GetSize(objRef);
                                        EnsureBufferSize(dataSize + offset);

                                        fixed (byte* byteBufferPtr = buffer)
                                        {
                                            customTypeResolver.Serialize(objRef, byteBufferPtr, offset);
                                        }
                                    }
                                    else
                                    {
                                        dataSize = customTypeResolver.GetSize(obj);
                                        EnsureBufferSize(dataSize + offset);

                                        fixed (byte* byteBufferPtr = buffer)
                                        {
                                            customTypeResolver.Serialize(obj, byteBufferPtr, offset);
                                        }
                                    }
                                    return dataSize;
                                }
                            }
                        }

                        if (typeInfo.isValueType)
                            dataSize = ExperimentalSerializeStructIntoBuffer(__makeref(obj), typeInfo, offset);
                        else
                            dataSize = ExperimentalSerializeObjectIntoBuffer(obj, typeInfo, offset);
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported serialization type: " + typeInfo.serializationType);
            }

            return dataSize;
        }

        // Returns size
        private int ExperimentalSerializeStructIntoBuffer(TypedReference objRef, TinySerializerTypeInfo typeInfo, int offset)
        {
            int dataSize = 0;

            if (!typeInfo.inspectedFields)
            {
                typeInfo.InspectFields();
            }

            if (!typeInfo.emittedFieldGettersByTypedRef)
            {
                typeInfo.EmitFieldGettersByTypedRef();
            }

            for (int i = 0; i < typeInfo.fieldTypeInfos.Length; i++)
            {
                var fieldTypeInfo = typeInfo.fieldTypeInfos[i];
                SerializationType fieldSerializationType = fieldTypeInfo.fieldTypeInfo.serializationType;

                int fieldDataSize = 0;
                int fieldOffset = offset + dataSize;

                switch (fieldSerializationType)
                {
                    #region PRIMITIVE
                    case SerializationType.Byte:
                        {
                            fieldDataSize = SerializationTypeSizes.BYTE;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<byte>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                *byteBufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.SByte:
                        {
                            fieldDataSize = SerializationTypeSizes.SBYTE;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<sbyte>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (sbyte*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Bool:
                        {
                            fieldDataSize = SerializationTypeSizes.BOOL;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<bool>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (bool*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int16:
                        {
                            fieldDataSize = SerializationTypeSizes.INT16;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<short>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (short*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int32:
                        {
                            fieldDataSize = SerializationTypeSizes.INT32;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<int>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (int*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Int64:
                        {
                            fieldDataSize = SerializationTypeSizes.INT64;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<long>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt16:
                        {
                            fieldDataSize = SerializationTypeSizes.UINT16;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<ushort>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (ushort*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt32:
                        {
                            fieldDataSize = SerializationTypeSizes.UINT32;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<uint>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (uint*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.UInt64:
                        {
                            fieldDataSize = SerializationTypeSizes.UINT64;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<ulong>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (ulong*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Half:
                        {
                            fieldDataSize = SerializationTypeSizes.HALF;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Half>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.value;
                            }
                        }
                        break;
                    case SerializationType.Single:
                        {
                            fieldDataSize = SerializationTypeSizes.SINGLE;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<float>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Double:
                        {
                            fieldDataSize = SerializationTypeSizes.DOUBLE;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<double>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (double*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Char:
                        {
                            fieldDataSize = SerializationTypeSizes.CHAR;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<char>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (char*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.Decimal:
                        {
                            fieldDataSize = SerializationTypeSizes.DECIMAL;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<decimal>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (decimal*)byteBufferPtr;
                                *bufferPtr = value;
                            }
                        }
                        break;
                    case SerializationType.DateTime:
                        {
                            fieldDataSize = SerializationTypeSizes.DATE_TIME;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<DateTime>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.DateTimeOffset:
                        {
                            fieldDataSize = SerializationTypeSizes.DATE_TIME_OFFSET;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<DateTimeOffset>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.TimeSpan:
                        {
                            fieldDataSize = SerializationTypeSizes.TIME_SPAN;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<TimeSpan>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (long*)byteBufferPtr;
                                *bufferPtr = value.Ticks;
                            }
                        }
                        break;
                    case SerializationType.Vector2:
                        {
                            fieldDataSize = SerializationTypeSizes.VECTOR2;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Vector2>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                            }
                        }
                        break;
                    case SerializationType.Vector3:
                        {
                            fieldDataSize = SerializationTypeSizes.VECTOR3;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Vector3>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                                bufferPtr[2] = value.z;
                            }
                        }
                        break;
                    case SerializationType.Vector4:
                        {
                            fieldDataSize = SerializationTypeSizes.VECTOR4;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Vector4>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                                bufferPtr[2] = value.z;
                                bufferPtr[3] = value.w;
                            }
                        }
                        break;
                    case SerializationType.Quaternion:
                        {
                            fieldDataSize = SerializationTypeSizes.QUATERNION;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Quaternion>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                                bufferPtr[2] = value.z;
                                bufferPtr[3] = value.w;
                            }
                        }
                        break;
                    case SerializationType.Rect:
                        {
                            fieldDataSize = SerializationTypeSizes.RECT;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Rect>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                                bufferPtr[2] = value.width;
                                bufferPtr[3] = value.height;
                            }
                        }
                        break;
                    case SerializationType.Bounds:
                        {
                            fieldDataSize = SerializationTypeSizes.BOUNDS;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<Bounds>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (float*)byteBufferPtr;
                                var center = value.center;
                                var size = value.size;
                                bufferPtr[0] = center.x;
                                bufferPtr[1] = center.y;
                                bufferPtr[2] = center.z;
                                bufferPtr[3] = size.x;
                                bufferPtr[4] = size.y;
                                bufferPtr[5] = size.z;
                            }
                        }
                        break;
                    case SerializationType.IntVector2:
                        {
                            fieldDataSize = SerializationTypeSizes.INT_VECTOR2;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<IntVector2>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (int*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                            }
                        }
                        break;
                    case SerializationType.IntVector3:
                        {
                            fieldDataSize = SerializationTypeSizes.INT_VECTOR3;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<IntVector3>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (int*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                                bufferPtr[2] = value.z;
                            }
                        }
                        break;
                    case SerializationType.IntVector4:
                        {
                            fieldDataSize = SerializationTypeSizes.INT_VECTOR4;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<IntVector4>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                            {
                                var bufferPtr = (int*)byteBufferPtr;
                                bufferPtr[0] = value.x;
                                bufferPtr[1] = value.y;
                                bufferPtr[2] = value.z;
                                bufferPtr[3] = value.w;
                            }
                        }
                        break;
                    #endregion
                    case SerializationType.String:
                        {
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<string>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fieldDataSize = ExperimentalSerializeStringIntoBuffer(value, fieldOffset);
                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            // TEMP: Until primitive arrays get their specific delegates
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef) as Array;

                            bool isNull = value == null;
                            int arrLength = value?.Length ?? -1;
                            fieldDataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, arrLength, fieldOffset);
                            if (isNull)
                                break;

                            // TODO: Object cyclic reference

                            int arrOffset = fieldDataSize + fieldOffset;
                            TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;
                            int elementSize = elementTypeInfo.primitiveSize;
                            fieldDataSize += elementSize * arrLength;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            fixed (byte* byteBufferPtr = &buffer[arrOffset])
                            {
                                switch (elementTypeInfo.serializationType)
                                {
                                    case SerializationType.Byte:
                                        {
                                            var arrValue = (byte[])value;

                                            fixed (byte* valuePtr = arrValue)
                                            {
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    byteBufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            var arrValue = (sbyte[])value;

                                            fixed (sbyte* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (sbyte*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Bool:
                                        {
                                            var arrValue = (bool[])value;

                                            fixed (bool* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (bool*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            var arrValue = (short[])value;

                                            fixed (short* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (short*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            var arrValue = (int[])value;

                                            fixed (int* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            var arrValue = (long[])value;

                                            fixed (long* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            var arrValue = (ushort[])value;

                                            fixed (ushort* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            var arrValue = (uint[])value;

                                            fixed (uint* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (uint*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            var arrValue = (ulong[])value;

                                            fixed (ulong* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (ulong*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            var arrValue = (Half[])value;

                                            fixed (Half* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].value;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            var arrValue = (float[])value;

                                            fixed (float* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            var arrValue = (double[])value;

                                            fixed (double* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (double*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            var arrValue = (char[])value;

                                            fixed (char* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (char*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            var arrValue = (decimal[])value;

                                            fixed (decimal* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (decimal*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            var arrValue = (DateTime[])value;

                                            fixed (DateTime* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].Ticks;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            var arrValue = (DateTimeOffset[])value;

                                            fixed (DateTimeOffset* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].Ticks;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            var arrValue = (TimeSpan[])value;

                                            fixed (TimeSpan* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].Ticks;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            var arrValue = (Vector2[])value;

                                            fixed (Vector2* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 2) + 0] = elementValue.x;
                                                    bufferPtr[(j * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            var arrValue = (Vector3[])value;

                                            fixed (Vector3* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 3) + 0] = elementValue.x;
                                                    bufferPtr[(j * 3) + 1] = elementValue.y;
                                                    bufferPtr[(j * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            var arrValue = (Vector4[])value;

                                            fixed (Vector4* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            var arrValue = (Quaternion[])value;

                                            fixed (Quaternion* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            var arrValue = (Rect[])value;

                                            fixed (Rect* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.width;
                                                    bufferPtr[(j * 4) + 3] = elementValue.height;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            var arrValue = (Bounds[])value;

                                            fixed (Bounds* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    var center = elementValue.center;
                                                    var size = elementValue.size;
                                                    bufferPtr[(j * 6) + 0] = center.x;
                                                    bufferPtr[(j * 6) + 1] = center.y;
                                                    bufferPtr[(j * 6) + 2] = center.z;
                                                    bufferPtr[(j * 6) + 3] = size.x;
                                                    bufferPtr[(j * 6) + 4] = size.y;
                                                    bufferPtr[(j * 6) + 5] = size.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            var arrValue = (IntVector2[])value;

                                            fixed (IntVector2* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 2) + 0] = elementValue.x;
                                                    bufferPtr[(j * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            var arrValue = (IntVector3[])value;

                                            fixed (IntVector3* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 3) + 0] = elementValue.x;
                                                    bufferPtr[(j * 3) + 1] = elementValue.y;
                                                    bufferPtr[(j * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            var arrValue = (IntVector4[])value;

                                            fixed (IntVector4* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                }
                            }
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {
                            // TEMP: Until primitive lists get their specific delegates
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef) as IList;

                            bool isNull = value == null;
                            int listLength = value?.Count ?? -1;
                            fieldDataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, listLength, fieldOffset);
                            if (isNull)
                                break;

                            // TODO: Object cyclic reference

                            int listOffset = fieldOffset + fieldDataSize;
                            TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;
                            int elementSize = elementTypeInfo.primitiveSize;
                            fieldDataSize += elementSize * listLength;
                            EnsureBufferSize(fieldOffset + fieldDataSize);

                            fixed (byte* byteBufferPtr = &buffer[listOffset])
                            {
                                switch (elementTypeInfo.serializationType)
                                {
                                    case SerializationType.Byte:
                                        {
                                            var listValue = (List<byte>)value;

                                            for (int j = 0; j < listLength; j++)
                                            {
                                                byteBufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            var listValue = (List<sbyte>)value;

                                            var bufferPtr = (sbyte*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Bool:
                                        {
                                            var listValue = (List<bool>)value;

                                            var bufferPtr = (bool*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            var listValue = (List<short>)value;

                                            var bufferPtr = (short*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            var listValue = (List<int>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            var listValue = (List<long>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            var listValue = (List<ushort>)value;

                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            var listValue = (List<uint>)value;

                                            var bufferPtr = (uint*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            var listValue = (List<ulong>)value;

                                            var bufferPtr = (ulong*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            var listValue = (List<Half>)value;

                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].value;
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            var listValue = (List<float>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            var listValue = (List<double>)value;

                                            var bufferPtr = (double*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            var listValue = (List<char>)value;

                                            var bufferPtr = (char*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            var listValue = (List<decimal>)value;

                                            var bufferPtr = (decimal*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            var listValue = (List<DateTime>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            var listValue = (List<DateTimeOffset>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            var listValue = (List<TimeSpan>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            var listValue = (List<Vector2>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 2) + 0] = elementValue.x;
                                                bufferPtr[(j * 2) + 1] = elementValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            var listValue = (List<Vector3>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 3) + 0] = elementValue.x;
                                                bufferPtr[(j * 3) + 1] = elementValue.y;
                                                bufferPtr[(j * 3) + 2] = elementValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            var listValue = (List<Vector4>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.z;
                                                bufferPtr[(j * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            var listValue = (List<Quaternion>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.z;
                                                bufferPtr[(j * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            var listValue = (List<Rect>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.width;
                                                bufferPtr[(j * 4) + 3] = elementValue.height;
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            var listValue = (List<Bounds>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                var center = elementValue.center;
                                                var size = elementValue.size;
                                                bufferPtr[(j * 6) + 0] = center.x;
                                                bufferPtr[(j * 6) + 1] = center.y;
                                                bufferPtr[(j * 6) + 2] = center.z;
                                                bufferPtr[(j * 6) + 3] = size.x;
                                                bufferPtr[(j * 6) + 4] = size.y;
                                                bufferPtr[(j * 6) + 5] = size.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            var listValue = (List<IntVector2>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 2) + 0] = elementValue.x;
                                                bufferPtr[(j * 2) + 1] = elementValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            var listValue = (List<IntVector3>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 3) + 0] = elementValue.x;
                                                bufferPtr[(j * 3) + 1] = elementValue.y;
                                                bufferPtr[(j * 3) + 2] = elementValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            var listValue = (List<IntVector4>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.z;
                                                bufferPtr[(j * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                }
                            }
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            // TEMP: Until primitive nullables get their specific delegates
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            bool isNull = value == null;

                            fieldDataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, fieldOffset);
                            if (isNull)
                                break;

                            int valueOffset = fieldOffset + fieldDataSize;
                            TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;

                            switch (elementTypeInfo.serializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        var trueValue = (byte)value;

                                        fieldDataSize += SerializationTypeSizes.BYTE;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            *byteBufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.SByte:
                                    {
                                        var trueValue = (sbyte)value;

                                        fieldDataSize += SerializationTypeSizes.SBYTE;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (sbyte*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Bool:
                                    {
                                        var trueValue = (bool)value;

                                        fieldDataSize += SerializationTypeSizes.BOOL;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (bool*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Int16:
                                    {
                                        var trueValue = (short)value;

                                        fieldDataSize += SerializationTypeSizes.INT16;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (short*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Int32:
                                    {
                                        var trueValue = (int)value;

                                        fieldDataSize += SerializationTypeSizes.INT32;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Int64:
                                    {
                                        var trueValue = (long)value;

                                        fieldDataSize += SerializationTypeSizes.INT64;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt16:
                                    {
                                        var trueValue = (ushort)value;

                                        fieldDataSize += SerializationTypeSizes.UINT16;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt32:
                                    {
                                        var trueValue = (uint)value;

                                        fieldDataSize += SerializationTypeSizes.UINT32;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (uint*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt64:
                                    {
                                        var trueValue = (ulong)value;

                                        fieldDataSize += SerializationTypeSizes.UINT64;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ulong*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Half:
                                    {
                                        var trueValue = (Half)value;

                                        fieldDataSize += SerializationTypeSizes.HALF;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            *bufferPtr = trueValue.value;
                                        }
                                    }
                                    break;
                                case SerializationType.Single:
                                    {
                                        var trueValue = (float)value;

                                        fieldDataSize += SerializationTypeSizes.SINGLE;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Double:
                                    {
                                        var trueValue = (double)value;

                                        fieldDataSize += SerializationTypeSizes.DOUBLE;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (double*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Char:
                                    {
                                        var trueValue = (char)value;

                                        fieldDataSize += SerializationTypeSizes.CHAR;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (char*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Decimal:
                                    {
                                        var trueValue = (decimal)value;

                                        fieldDataSize += SerializationTypeSizes.DECIMAL;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (decimal*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTime:
                                    {
                                        var trueValue = (DateTime)value;

                                        fieldDataSize += SerializationTypeSizes.DATE_TIME;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue.Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTimeOffset:
                                    {
                                        var trueValue = (DateTimeOffset)value;

                                        fieldDataSize += SerializationTypeSizes.DATE_TIME_OFFSET;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue.Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.TimeSpan:
                                    {
                                        var trueValue = (TimeSpan)value;

                                        fieldDataSize += SerializationTypeSizes.TIME_SPAN;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue.Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector2:
                                    {
                                        var trueValue = (Vector2)value;

                                        fieldDataSize += SerializationTypeSizes.VECTOR2;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector3:
                                    {
                                        var trueValue = (Vector3)value;

                                        fieldDataSize += SerializationTypeSizes.VECTOR3;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector4:
                                    {
                                        var trueValue = (Vector4)value;

                                        fieldDataSize += SerializationTypeSizes.VECTOR4;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                            bufferPtr[3] = trueValue.w;
                                        }
                                    }
                                    break;
                                case SerializationType.Quaternion:
                                    {
                                        var trueValue = (Quaternion)value;

                                        fieldDataSize += SerializationTypeSizes.QUATERNION;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                            bufferPtr[3] = trueValue.w;
                                        }
                                    }
                                    break;
                                case SerializationType.Rect:
                                    {
                                        var trueValue = (Rect)value;

                                        fieldDataSize += SerializationTypeSizes.RECT;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.width;
                                            bufferPtr[3] = trueValue.height;
                                        }
                                    }
                                    break;
                                case SerializationType.Bounds:
                                    {
                                        var trueValue = (Bounds)value;

                                        fieldDataSize += SerializationTypeSizes.BOUNDS;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            var center = trueValue.center;
                                            var size = trueValue.size;
                                            bufferPtr[0] = center.x;
                                            bufferPtr[1] = center.y;
                                            bufferPtr[2] = center.z;
                                            bufferPtr[3] = size.x;
                                            bufferPtr[4] = size.y;
                                            bufferPtr[5] = size.z;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector2:
                                    {
                                        var trueValue = (IntVector2)value;

                                        fieldDataSize += SerializationTypeSizes.INT_VECTOR2;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector3:
                                    {
                                        var trueValue = (IntVector3)value;

                                        fieldDataSize += SerializationTypeSizes.INT_VECTOR3;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector4:
                                    {
                                        var trueValue = (IntVector4)value;

                                        fieldDataSize += SerializationTypeSizes.INT_VECTOR4;
                                        EnsureBufferSize(fieldOffset + fieldDataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                            bufferPtr[3] = trueValue.w;
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                            }
                        }
                        break;
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        {
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fieldDataSize = ExperimentalSerializeObjectIntoBuffer(value, fieldTypeInfo.fieldTypeInfo, fieldOffset);
                        }
                        break;
                    default:
                        throw new UnsupportedException("Unsupported field serialization type: " + fieldSerializationType);
                }

                dataSize += fieldDataSize;
            }

            return dataSize;
        }

        // Returns size
        private int ExperimentalSerializeObjectIntoBuffer(object obj, TinySerializerTypeInfo typeInfo, int offset)
        {
            int dataSize = 0;

            if (typeInfo.serializationType == SerializationType.Object)
            {
                if (hasCustomTypeResolvers)
                {
                    for (int i = 0; i < customTypeResolvers.Count; i++)
                    {
                        CustomTypeResolver customTypeResolver = customTypeResolvers[i];

                        if (customTypeResolver.typeHashCode == typeInfo.typeHashCode)
                        {
                            dataSize = customTypeResolver.GetSize(obj);
                            EnsureBufferSize(dataSize + offset);

                            fixed (byte* byteBufferPtr = buffer)
                            {
                                customTypeResolver.Serialize(obj, byteBufferPtr, offset);
                            }

                            return dataSize;
                        }
                    }
                }

                if (!typeInfo.isValueType)
                {
                    bool isNull = obj == null;
                    dataSize += ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                    if (isNull)
                        return dataSize;

                    /* // TODO: objectRefPrefixValue if cyclic references is supported
                    if (settings.supportCyclicReferences && !typeInfo.isValueType)
                    {
                        dataSize += SerializationTypeSizes.INT64;
                        EnsureBufferSize(dataSize + offset);
                    }
                    */
                }

                if (!typeInfo.inspectedFields)
                    typeInfo.InspectFields();

                if (!typeInfo.emittedFieldGetters)
                    typeInfo.EmitFieldGetters();

                for (int i = 0; i < typeInfo.fieldTypeInfos.Length; i++)
                {
                    var fieldTypeInfo = typeInfo.fieldTypeInfos[i];
                    SerializationType fieldSerializationType = fieldTypeInfo.fieldTypeInfo.serializationType;

                    int fieldDataSize = 0;
                    int fieldOffset = offset + dataSize;

                    switch (fieldSerializationType)
                    {
                        #region PRIMITIVE
                        case SerializationType.Byte:
                            {
                                fieldDataSize = SerializationTypeSizes.BYTE;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, byte>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    *byteBufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.SByte:
                            {
                                fieldDataSize = SerializationTypeSizes.SBYTE;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, sbyte>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (sbyte*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Bool:
                            {
                                fieldDataSize = SerializationTypeSizes.BOOL;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, bool>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (bool*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Int16:
                            {
                                fieldDataSize = SerializationTypeSizes.INT16;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, short>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (short*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Int32:
                            {
                                fieldDataSize = SerializationTypeSizes.INT32;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, int>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (int*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Int64:
                            {
                                fieldDataSize = SerializationTypeSizes.INT64;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, long>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (long*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.UInt16:
                            {
                                fieldDataSize = SerializationTypeSizes.UINT16;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, ushort>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (ushort*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.UInt32:
                            {
                                fieldDataSize = SerializationTypeSizes.UINT32;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, uint>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (uint*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.UInt64:
                            {
                                fieldDataSize = SerializationTypeSizes.UINT64;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, ulong>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (ulong*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Half:
                            {
                                fieldDataSize = SerializationTypeSizes.HALF;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Half>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (ushort*)byteBufferPtr;
                                    *bufferPtr = value.value;
                                }
                            }
                            break;
                        case SerializationType.Single:
                            {
                                fieldDataSize = SerializationTypeSizes.SINGLE;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, float>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Double:
                            {
                                fieldDataSize = SerializationTypeSizes.DOUBLE;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, double>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (double*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Char:
                            {
                                fieldDataSize = SerializationTypeSizes.CHAR;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, char>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (char*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.Decimal:
                            {
                                fieldDataSize = SerializationTypeSizes.DECIMAL;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, decimal>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (decimal*)byteBufferPtr;
                                    *bufferPtr = value;
                                }
                            }
                            break;
                        case SerializationType.DateTime:
                            {
                                fieldDataSize = SerializationTypeSizes.DATE_TIME;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, DateTime>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (long*)byteBufferPtr;
                                    *bufferPtr = value.Ticks;
                                }
                            }
                            break;
                        case SerializationType.DateTimeOffset:
                            {
                                fieldDataSize = SerializationTypeSizes.DATE_TIME_OFFSET;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, DateTimeOffset>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (long*)byteBufferPtr;
                                    *bufferPtr = value.Ticks;
                                }
                            }
                            break;
                        case SerializationType.TimeSpan:
                            {
                                fieldDataSize = SerializationTypeSizes.TIME_SPAN;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, TimeSpan>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (long*)byteBufferPtr;
                                    *bufferPtr = value.Ticks;
                                }
                            }
                            break;
                        case SerializationType.Vector2:
                            {
                                fieldDataSize = SerializationTypeSizes.VECTOR2;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Vector2>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                }
                            }
                            break;
                        case SerializationType.Vector3:
                            {
                                fieldDataSize = SerializationTypeSizes.VECTOR3;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Vector3>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                    bufferPtr[2] = elementValue.z;
                                }
                            }
                            break;
                        case SerializationType.Vector4:
                            {
                                fieldDataSize = SerializationTypeSizes.VECTOR4;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Vector4>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                    bufferPtr[2] = elementValue.z;
                                    bufferPtr[3] = elementValue.w;
                                }
                            }
                            break;
                        case SerializationType.Quaternion:
                            {
                                fieldDataSize = SerializationTypeSizes.QUATERNION;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Quaternion>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                    bufferPtr[2] = elementValue.z;
                                    bufferPtr[3] = elementValue.w;
                                }
                            }
                            break;
                        case SerializationType.Rect:
                            {
                                fieldDataSize = SerializationTypeSizes.RECT;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Rect>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                    bufferPtr[2] = elementValue.width;
                                    bufferPtr[3] = elementValue.height;
                                }
                            }
                            break;
                        case SerializationType.Bounds:
                            {
                                fieldDataSize = SerializationTypeSizes.BOUNDS;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, Bounds>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (float*)byteBufferPtr;
                                    var elementValue = value;
                                    var center = elementValue.center;
                                    var size = elementValue.size;
                                    bufferPtr[0] = center.x;
                                    bufferPtr[1] = center.y;
                                    bufferPtr[2] = center.z;
                                    bufferPtr[3] = size.x;
                                    bufferPtr[4] = size.y;
                                    bufferPtr[5] = size.z;
                                }
                            }
                            break;
                        case SerializationType.IntVector2:
                            {
                                fieldDataSize = SerializationTypeSizes.INT_VECTOR2;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, IntVector2>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (int*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                }
                            }
                            break;
                        case SerializationType.IntVector3:
                            {
                                fieldDataSize = SerializationTypeSizes.INT_VECTOR3;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, IntVector3>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (int*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                    bufferPtr[2] = elementValue.z;
                                }
                            }
                            break;
                        case SerializationType.IntVector4:
                            {
                                fieldDataSize = SerializationTypeSizes.INT_VECTOR4;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, IntVector4>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fixed (byte* byteBufferPtr = &buffer[fieldOffset])
                                {
                                    var bufferPtr = (int*)byteBufferPtr;
                                    var elementValue = value;
                                    bufferPtr[0] = elementValue.x;
                                    bufferPtr[1] = elementValue.y;
                                    bufferPtr[2] = elementValue.z;
                                    bufferPtr[3] = elementValue.w;
                                }
                            }
                            break;
                        #endregion
                        case SerializationType.String:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, string>)fieldTypeInfo.getter;
                                var value = getterDelegate(obj);

                                fieldDataSize = ExperimentalSerializeStringIntoBuffer(value, fieldOffset);
                            }
                            break;
                        case SerializationType.PrimitiveArray:
                            {
                                // TEMP: Until primitive arrays get their specific delegates
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as Array;

                                bool isNull = value == null;
                                int arrLength = value?.Length ?? -1;
                                fieldDataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, arrLength, fieldOffset);
                                if (isNull)
                                    break;

                                // TODO: Object cyclic reference

                                int arrOffset = fieldDataSize + fieldOffset;
                                TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;
                                int elementSize = elementTypeInfo.primitiveSize;
                                fieldDataSize += elementSize * arrLength;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                fixed (byte* byteBufferPtr = &buffer[arrOffset])
                                {
                                    switch (elementTypeInfo.serializationType)
                                    {
                                        case SerializationType.Byte:
                                            {
                                                var arrValue = (byte[])value;

                                                fixed (byte* valuePtr = arrValue)
                                                {
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        byteBufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.SByte:
                                            {
                                                var arrValue = (sbyte[])value;

                                                fixed (sbyte* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (sbyte*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Bool:
                                            {
                                                var arrValue = (bool[])value;

                                                fixed (bool* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (bool*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Int16:
                                            {
                                                var arrValue = (short[])value;

                                                fixed (short* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (short*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Int32:
                                            {
                                                var arrValue = (int[])value;

                                                fixed (int* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (int*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Int64:
                                            {
                                                var arrValue = (long[])value;

                                                fixed (long* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (long*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.UInt16:
                                            {
                                                var arrValue = (ushort[])value;

                                                fixed (ushort* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (ushort*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.UInt32:
                                            {
                                                var arrValue = (uint[])value;

                                                fixed (uint* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (uint*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.UInt64:
                                            {
                                                var arrValue = (ulong[])value;

                                                fixed (ulong* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (ulong*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Half:
                                            {
                                                var arrValue = (Half[])value;

                                                fixed (Half* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (ushort*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j].value;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Single:
                                            {
                                                var arrValue = (float[])value;

                                                fixed (float* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Double:
                                            {
                                                var arrValue = (double[])value;

                                                fixed (double* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (double*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Char:
                                            {
                                                var arrValue = (char[])value;

                                                fixed (char* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (char*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Decimal:
                                            {
                                                var arrValue = (decimal[])value;

                                                fixed (decimal* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (decimal*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j];
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.DateTime:
                                            {
                                                var arrValue = (DateTime[])value;

                                                fixed (DateTime* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (long*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j].Ticks;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.DateTimeOffset:
                                            {
                                                var arrValue = (DateTimeOffset[])value;

                                                fixed (DateTimeOffset* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (long*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j].Ticks;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.TimeSpan:
                                            {
                                                var arrValue = (TimeSpan[])value;

                                                fixed (TimeSpan* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (long*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        bufferPtr[j] = valuePtr[j].Ticks;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Vector2:
                                            {
                                                var arrValue = (Vector2[])value;

                                                fixed (Vector2* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 2) + 0] = elementValue.x;
                                                        bufferPtr[(j * 2) + 1] = elementValue.y;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Vector3:
                                            {
                                                var arrValue = (Vector3[])value;

                                                fixed (Vector3* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 3) + 0] = elementValue.x;
                                                        bufferPtr[(j * 3) + 1] = elementValue.y;
                                                        bufferPtr[(j * 3) + 2] = elementValue.z;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Vector4:
                                            {
                                                var arrValue = (Vector4[])value;

                                                fixed (Vector4* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 4) + 0] = elementValue.x;
                                                        bufferPtr[(j * 4) + 1] = elementValue.y;
                                                        bufferPtr[(j * 4) + 2] = elementValue.z;
                                                        bufferPtr[(j * 4) + 3] = elementValue.w;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Quaternion:
                                            {
                                                var arrValue = (Quaternion[])value;

                                                fixed (Quaternion* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 4) + 0] = elementValue.x;
                                                        bufferPtr[(j * 4) + 1] = elementValue.y;
                                                        bufferPtr[(j * 4) + 2] = elementValue.z;
                                                        bufferPtr[(j * 4) + 3] = elementValue.w;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Rect:
                                            {
                                                var arrValue = (Rect[])value;

                                                fixed (Rect* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 4) + 0] = elementValue.x;
                                                        bufferPtr[(j * 4) + 1] = elementValue.y;
                                                        bufferPtr[(j * 4) + 2] = elementValue.width;
                                                        bufferPtr[(j * 4) + 3] = elementValue.height;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.Bounds:
                                            {
                                                var arrValue = (Bounds[])value;

                                                fixed (Bounds* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (float*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        var center = elementValue.center;
                                                        var size = elementValue.size;
                                                        bufferPtr[(j * 6) + 0] = center.x;
                                                        bufferPtr[(j * 6) + 1] = center.y;
                                                        bufferPtr[(j * 6) + 2] = center.z;
                                                        bufferPtr[(j * 6) + 3] = size.x;
                                                        bufferPtr[(j * 6) + 4] = size.y;
                                                        bufferPtr[(j * 6) + 5] = size.z;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.IntVector2:
                                            {
                                                var arrValue = (IntVector2[])value;

                                                fixed (IntVector2* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (int*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 2) + 0] = elementValue.x;
                                                        bufferPtr[(j * 2) + 1] = elementValue.y;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.IntVector3:
                                            {
                                                var arrValue = (IntVector3[])value;

                                                fixed (IntVector3* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (int*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 3) + 0] = elementValue.x;
                                                        bufferPtr[(j * 3) + 1] = elementValue.y;
                                                        bufferPtr[(j * 3) + 2] = elementValue.z;
                                                    }
                                                }
                                            }
                                            break;
                                        case SerializationType.IntVector4:
                                            {
                                                var arrValue = (IntVector4[])value;

                                                fixed (IntVector4* valuePtr = arrValue)
                                                {
                                                    var bufferPtr = (int*)byteBufferPtr;
                                                    for (int j = 0; j < arrLength; j++)
                                                    {
                                                        var elementValue = valuePtr[j];
                                                        bufferPtr[(j * 4) + 0] = elementValue.x;
                                                        bufferPtr[(j * 4) + 1] = elementValue.y;
                                                        bufferPtr[(j * 4) + 2] = elementValue.z;
                                                        bufferPtr[(j * 4) + 3] = elementValue.w;
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                    }
                                }
                            }
                            break;
                        case SerializationType.PrimitiveList:
                            {
                                // TEMP: Until primitive lists get their specific delegates
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as IList;

                                bool isNull = value == null;
                                int listLength = value?.Count ?? -1;
                                fieldDataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, listLength, fieldOffset);
                                if (isNull)
                                    break;

                                // TODO: Object cyclic reference

                                int listOffset = fieldOffset + fieldDataSize;
                                TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;
                                int elementSize = elementTypeInfo.primitiveSize;
                                fieldDataSize += elementSize * listLength;
                                EnsureBufferSize(fieldOffset + fieldDataSize);

                                fixed (byte* byteBufferPtr = &buffer[listOffset])
                                {
                                    switch (elementTypeInfo.serializationType)
                                    {
                                        case SerializationType.Byte:
                                            {
                                                var listValue = (List<byte>)value;

                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    byteBufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.SByte:
                                            {
                                                var listValue = (List<sbyte>)value;

                                                var bufferPtr = (sbyte*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Bool:
                                            {
                                                var listValue = (List<bool>)value;

                                                var bufferPtr = (bool*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Int16:
                                            {
                                                var listValue = (List<short>)value;

                                                var bufferPtr = (short*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Int32:
                                            {
                                                var listValue = (List<int>)value;

                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Int64:
                                            {
                                                var listValue = (List<long>)value;

                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.UInt16:
                                            {
                                                var listValue = (List<ushort>)value;

                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.UInt32:
                                            {
                                                var listValue = (List<uint>)value;

                                                var bufferPtr = (uint*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.UInt64:
                                            {
                                                var listValue = (List<ulong>)value;

                                                var bufferPtr = (ulong*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Half:
                                            {
                                                var listValue = (List<Half>)value;

                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j].value;
                                                }
                                            }
                                            break;
                                        case SerializationType.Single:
                                            {
                                                var listValue = (List<float>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Double:
                                            {
                                                var listValue = (List<double>)value;

                                                var bufferPtr = (double*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Char:
                                            {
                                                var listValue = (List<char>)value;

                                                var bufferPtr = (char*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.Decimal:
                                            {
                                                var listValue = (List<decimal>)value;

                                                var bufferPtr = (decimal*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j];
                                                }
                                            }
                                            break;
                                        case SerializationType.DateTime:
                                            {
                                                var listValue = (List<DateTime>)value;

                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j].Ticks;
                                                }
                                            }
                                            break;
                                        case SerializationType.DateTimeOffset:
                                            {
                                                var listValue = (List<DateTimeOffset>)value;

                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j].Ticks;
                                                }
                                            }
                                            break;
                                        case SerializationType.TimeSpan:
                                            {
                                                var listValue = (List<TimeSpan>)value;

                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    bufferPtr[j] = listValue[j].Ticks;
                                                }
                                            }
                                            break;
                                        case SerializationType.Vector2:
                                            {
                                                var listValue = (List<Vector2>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 2) + 0] = elementValue.x;
                                                    bufferPtr[(j * 2) + 1] = elementValue.y;
                                                }
                                            }
                                            break;
                                        case SerializationType.Vector3:
                                            {
                                                var listValue = (List<Vector3>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 3) + 0] = elementValue.x;
                                                    bufferPtr[(j * 3) + 1] = elementValue.y;
                                                    bufferPtr[(j * 3) + 2] = elementValue.z;
                                                }
                                            }
                                            break;
                                        case SerializationType.Vector4:
                                            {
                                                var listValue = (List<Vector4>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                            break;
                                        case SerializationType.Quaternion:
                                            {
                                                var listValue = (List<Quaternion>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                            break;
                                        case SerializationType.Rect:
                                            {
                                                var listValue = (List<Rect>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.width;
                                                    bufferPtr[(j * 4) + 3] = elementValue.height;
                                                }
                                            }
                                            break;
                                        case SerializationType.Bounds:
                                            {
                                                var listValue = (List<Bounds>)value;

                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    var center = elementValue.center;
                                                    var size = elementValue.size;
                                                    bufferPtr[(j * 6) + 0] = center.x;
                                                    bufferPtr[(j * 6) + 1] = center.y;
                                                    bufferPtr[(j * 6) + 2] = center.z;
                                                    bufferPtr[(j * 6) + 3] = size.x;
                                                    bufferPtr[(j * 6) + 4] = size.y;
                                                    bufferPtr[(j * 6) + 5] = size.z;
                                                }
                                            }
                                            break;
                                        case SerializationType.IntVector2:
                                            {
                                                var listValue = (List<IntVector2>)value;

                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 2) + 0] = elementValue.x;
                                                    bufferPtr[(j * 2) + 1] = elementValue.y;
                                                }
                                            }
                                            break;
                                        case SerializationType.IntVector3:
                                            {
                                                var listValue = (List<IntVector3>)value;

                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 3) + 0] = elementValue.x;
                                                    bufferPtr[(j * 3) + 1] = elementValue.y;
                                                    bufferPtr[(j * 3) + 2] = elementValue.z;
                                                }
                                            }
                                            break;
                                        case SerializationType.IntVector4:
                                            {
                                                var listValue = (List<IntVector4>)value;

                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < listLength; j++)
                                                {
                                                    var elementValue = listValue[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                            break;
                                        default:
                                            throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                    }
                                }
                            }
                            break;
                        case SerializationType.PrimitiveNullable:
                            {
                                // TEMP: Until primitive nullables get their specific delegates
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                bool isNull = value == null;

                                fieldDataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, fieldOffset);
                                if (isNull)
                                    break;

                                int valueOffset = fieldOffset + fieldDataSize;
                                TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;

                                switch (elementTypeInfo.serializationType)
                                {
                                    case SerializationType.Byte:
                                        {
                                            var trueValue = (byte)value;

                                            fieldDataSize += SerializationTypeSizes.BYTE;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                *byteBufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            var trueValue = (sbyte)value;

                                            fieldDataSize += SerializationTypeSizes.SBYTE;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (sbyte*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Bool:
                                        {
                                            var trueValue = (bool)value;

                                            fieldDataSize += SerializationTypeSizes.BOOL;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (bool*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            var trueValue = (short)value;

                                            fieldDataSize += SerializationTypeSizes.INT16;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (short*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            var trueValue = (int)value;

                                            fieldDataSize += SerializationTypeSizes.INT32;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            var trueValue = (long)value;

                                            fieldDataSize += SerializationTypeSizes.INT64;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            var trueValue = (ushort)value;

                                            fieldDataSize += SerializationTypeSizes.UINT16;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            var trueValue = (uint)value;

                                            fieldDataSize += SerializationTypeSizes.UINT32;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (uint*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            var trueValue = (ulong)value;

                                            fieldDataSize += SerializationTypeSizes.UINT64;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (ulong*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            var trueValue = (Half)value;

                                            fieldDataSize += SerializationTypeSizes.HALF;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                *bufferPtr = trueValue.value;
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            var trueValue = (float)value;

                                            fieldDataSize += SerializationTypeSizes.SINGLE;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            var trueValue = (double)value;

                                            fieldDataSize += SerializationTypeSizes.DOUBLE;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (double*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            var trueValue = (char)value;

                                            fieldDataSize += SerializationTypeSizes.CHAR;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (char*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            var trueValue = (decimal)value;

                                            fieldDataSize += SerializationTypeSizes.DECIMAL;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (decimal*)byteBufferPtr;
                                                *bufferPtr = trueValue;
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            var trueValue = (DateTime)value;

                                            fieldDataSize += SerializationTypeSizes.DATE_TIME;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                *bufferPtr = trueValue.Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            var trueValue = (DateTimeOffset)value;

                                            fieldDataSize += SerializationTypeSizes.DATE_TIME_OFFSET;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                *bufferPtr = trueValue.Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            var trueValue = (TimeSpan)value;

                                            fieldDataSize += SerializationTypeSizes.TIME_SPAN;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                *bufferPtr = trueValue.Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            var trueValue = (Vector2)value;

                                            fieldDataSize += SerializationTypeSizes.VECTOR2;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            var trueValue = (Vector3)value;

                                            fieldDataSize += SerializationTypeSizes.VECTOR3;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                                bufferPtr[2] = trueValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            var trueValue = (Vector4)value;

                                            fieldDataSize += SerializationTypeSizes.VECTOR4;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                                bufferPtr[2] = trueValue.z;
                                                bufferPtr[3] = trueValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            var trueValue = (Quaternion)value;

                                            fieldDataSize += SerializationTypeSizes.QUATERNION;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                                bufferPtr[2] = trueValue.z;
                                                bufferPtr[3] = trueValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            var trueValue = (Rect)value;

                                            fieldDataSize += SerializationTypeSizes.RECT;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                                bufferPtr[2] = trueValue.width;
                                                bufferPtr[3] = trueValue.height;
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            var trueValue = (Bounds)value;

                                            fieldDataSize += SerializationTypeSizes.BOUNDS;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                var center = trueValue.center;
                                                var size = trueValue.size;
                                                bufferPtr[0] = center.x;
                                                bufferPtr[1] = center.y;
                                                bufferPtr[2] = center.z;
                                                bufferPtr[3] = size.x;
                                                bufferPtr[4] = size.y;
                                                bufferPtr[5] = size.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            var trueValue = (IntVector2)value;

                                            fieldDataSize += SerializationTypeSizes.INT_VECTOR2;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            var trueValue = (IntVector3)value;

                                            fieldDataSize += SerializationTypeSizes.INT_VECTOR3;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                                bufferPtr[2] = trueValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            var trueValue = (IntVector4)value;

                                            fieldDataSize += SerializationTypeSizes.INT_VECTOR4;
                                            EnsureBufferSize(fieldOffset + fieldDataSize);

                                            fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                bufferPtr[0] = trueValue.x;
                                                bufferPtr[1] = trueValue.y;
                                                bufferPtr[2] = trueValue.z;
                                                bufferPtr[3] = trueValue.w;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                }
                            }
                            break;
                        case SerializationType.ObjectArray:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as Array;

                                bool isNull = value == null;
                                int arrLength = value?.Length ?? -1;
                                fieldDataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, arrLength, fieldOffset);
                                if (isNull)
                                    break;

                                // TODO: Object cyclic reference

                                int arrOffset = fieldOffset + fieldDataSize;
                                TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;

                                for (int j = 0; j < arrLength; j++)
                                {
                                    object element = value.GetValue(j);
                                    int elementDataSize = ExperimentalSerializeObjectIntoBuffer(element, elementTypeInfo, arrOffset);
                                    fieldDataSize += elementDataSize;
                                    arrOffset += elementDataSize;
                                }
                            }
                            break;
                        case SerializationType.ObjectList:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as IList;

                                bool isNull = value == null;
                                int listLength = value?.Count ?? -1;
                                fieldDataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, listLength, fieldOffset);
                                if (isNull)
                                    break;

                                // TODO: Object cyclic reference

                                int listOffset = fieldOffset + fieldDataSize;
                                TinySerializerTypeInfo elementTypeInfo = fieldTypeInfo.fieldTypeInfo.elementTypeInfo;

                                for (int j = 0; j < listLength; j++)
                                {
                                    object element = value[j];
                                    int elementDataSize = ExperimentalSerializeObjectIntoBuffer(element, elementTypeInfo, listOffset);
                                    fieldDataSize += elementDataSize;
                                    listOffset += elementDataSize;
                                }
                            }
                            break;
                        case SerializationType.ObjectNullable:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                bool isNull = value == null;

                                if (!isNull)
                                    fieldDataSize = ExperimentalSerializeObjectIntoBuffer(value, fieldTypeInfo.fieldTypeInfo.elementTypeInfo, fieldOffset);
                                else
                                    fieldDataSize = ExperimentalSerializeNullPrefixIntoBuffer(true, fieldOffset);
                            }
                            break;
                        case SerializationType.Object:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fieldDataSize = ExperimentalSerializeObjectIntoBuffer(value, fieldTypeInfo.fieldTypeInfo, fieldOffset);
                            }
                            break;
                        default:
                            throw new UnsupportedException("Unsupported field serialization type: " + fieldSerializationType);
                    }

                    dataSize += fieldDataSize;
                }
            }
            else
            {
                // Handles nested container types
                switch (typeInfo.serializationType)
                {
                    case SerializationType.String:
                        {
                            var value = obj as string;
                            dataSize = ExperimentalSerializeStringIntoBuffer(value, offset);
                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            var value = obj as Array;
                            bool isNull = value == null;
                            int arrLength = value?.Length ?? -1;
                            dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, arrLength, offset);
                            if (isNull)
                                break;

                            // TODO: Object cyclic reference

                            int arrOffset = offset + dataSize;
                            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
                            int elementSize = elementTypeInfo.primitiveSize;
                            dataSize += elementSize * arrLength;
                            EnsureBufferSize(dataSize + offset);

                            fixed (byte* byteBufferPtr = &buffer[arrOffset])
                            {
                                switch (elementTypeInfo.serializationType)
                                {
                                    case SerializationType.Byte:
                                        {
                                            var arrValue = (byte[])value;

                                            fixed (byte* valuePtr = arrValue)
                                            {
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    byteBufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            var arrValue = (sbyte[])value;

                                            fixed (sbyte* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (sbyte*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Bool:
                                        {
                                            var arrValue = (bool[])value;

                                            fixed (bool* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (bool*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            var arrValue = (short[])value;

                                            fixed (short* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (short*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            var arrValue = (int[])value;

                                            fixed (int* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            var arrValue = (long[])value;

                                            fixed (long* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            var arrValue = (ushort[])value;

                                            fixed (ushort* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            var arrValue = (uint[])value;

                                            fixed (uint* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (uint*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            var arrValue = (ulong[])value;

                                            fixed (ulong* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (ulong*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            var arrValue = (Half[])value;

                                            fixed (Half* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (ushort*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].value;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            var arrValue = (float[])value;

                                            fixed (float* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            var arrValue = (double[])value;

                                            fixed (double* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (double*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            var arrValue = (char[])value;

                                            fixed (char* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (char*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            var arrValue = (decimal[])value;

                                            fixed (decimal* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (decimal*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j];
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            var arrValue = (DateTime[])value;

                                            fixed (DateTime* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].Ticks;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            var arrValue = (DateTimeOffset[])value;

                                            fixed (DateTimeOffset* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].Ticks;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            var arrValue = (TimeSpan[])value;

                                            fixed (TimeSpan* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (long*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    bufferPtr[j] = valuePtr[j].Ticks;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            var arrValue = (Vector2[])value;

                                            fixed (Vector2* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 2) + 0] = elementValue.x;
                                                    bufferPtr[(j * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            var arrValue = (Vector3[])value;

                                            fixed (Vector3* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 3) + 0] = elementValue.x;
                                                    bufferPtr[(j * 3) + 1] = elementValue.y;
                                                    bufferPtr[(j * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            var arrValue = (Vector4[])value;

                                            fixed (Vector4* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            var arrValue = (Quaternion[])value;

                                            fixed (Quaternion* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            var arrValue = (Rect[])value;

                                            fixed (Rect* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.width;
                                                    bufferPtr[(j * 4) + 3] = elementValue.height;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            var arrValue = (Bounds[])value;

                                            fixed (Bounds* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (float*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    var center = elementValue.center;
                                                    var size = elementValue.size;
                                                    bufferPtr[(j * 6) + 0] = center.x;
                                                    bufferPtr[(j * 6) + 1] = center.y;
                                                    bufferPtr[(j * 6) + 2] = center.z;
                                                    bufferPtr[(j * 6) + 3] = size.x;
                                                    bufferPtr[(j * 6) + 4] = size.y;
                                                    bufferPtr[(j * 6) + 5] = size.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            var arrValue = (IntVector2[])value;

                                            fixed (IntVector2* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 2) + 0] = elementValue.x;
                                                    bufferPtr[(j * 2) + 1] = elementValue.y;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            var arrValue = (IntVector3[])value;

                                            fixed (IntVector3* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 3) + 0] = elementValue.x;
                                                    bufferPtr[(j * 3) + 1] = elementValue.y;
                                                    bufferPtr[(j * 3) + 2] = elementValue.z;
                                                }
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            var arrValue = (IntVector4[])value;

                                            fixed (IntVector4* valuePtr = arrValue)
                                            {
                                                var bufferPtr = (int*)byteBufferPtr;
                                                for (int j = 0; j < arrLength; j++)
                                                {
                                                    var elementValue = valuePtr[j];
                                                    bufferPtr[(j * 4) + 0] = elementValue.x;
                                                    bufferPtr[(j * 4) + 1] = elementValue.y;
                                                    bufferPtr[(j * 4) + 2] = elementValue.z;
                                                    bufferPtr[(j * 4) + 3] = elementValue.w;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                }
                            }
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {
                            var value = obj as IList;
                            bool isNull = value == null;
                            int listLength = value?.Count ?? -1;
                            dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, listLength, offset);
                            if (isNull)
                                break;

                            // TODO: Object cyclic reference

                            int listOffset = offset + dataSize;
                            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
                            int elementSize = elementTypeInfo.primitiveSize;
                            dataSize += elementSize * listLength;
                            EnsureBufferSize(dataSize + offset);

                            fixed (byte* byteBufferPtr = &buffer[listOffset])
                            {
                                switch (elementTypeInfo.serializationType)
                                {
                                    case SerializationType.Byte:
                                        {
                                            var listValue = (List<byte>)value;

                                            for (int j = 0; j < listLength; j++)
                                            {
                                                byteBufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.SByte:
                                        {
                                            var listValue = (List<sbyte>)value;

                                            var bufferPtr = (sbyte*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Bool:
                                        {
                                            var listValue = (List<bool>)value;

                                            var bufferPtr = (bool*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int16:
                                        {
                                            var listValue = (List<short>)value;

                                            var bufferPtr = (short*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int32:
                                        {
                                            var listValue = (List<int>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Int64:
                                        {
                                            var listValue = (List<long>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt16:
                                        {
                                            var listValue = (List<ushort>)value;

                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt32:
                                        {
                                            var listValue = (List<uint>)value;

                                            var bufferPtr = (uint*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.UInt64:
                                        {
                                            var listValue = (List<ulong>)value;

                                            var bufferPtr = (ulong*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Half:
                                        {
                                            var listValue = (List<Half>)value;

                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].value;
                                            }
                                        }
                                        break;
                                    case SerializationType.Single:
                                        {
                                            var listValue = (List<float>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Double:
                                        {
                                            var listValue = (List<double>)value;

                                            var bufferPtr = (double*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Char:
                                        {
                                            var listValue = (List<char>)value;

                                            var bufferPtr = (char*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.Decimal:
                                        {
                                            var listValue = (List<decimal>)value;

                                            var bufferPtr = (decimal*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j];
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTime:
                                        {
                                            var listValue = (List<DateTime>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.DateTimeOffset:
                                        {
                                            var listValue = (List<DateTimeOffset>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.TimeSpan:
                                        {
                                            var listValue = (List<TimeSpan>)value;

                                            var bufferPtr = (long*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                bufferPtr[j] = listValue[j].Ticks;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector2:
                                        {
                                            var listValue = (List<Vector2>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 2) + 0] = elementValue.x;
                                                bufferPtr[(j * 2) + 1] = elementValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector3:
                                        {
                                            var listValue = (List<Vector3>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 3) + 0] = elementValue.x;
                                                bufferPtr[(j * 3) + 1] = elementValue.y;
                                                bufferPtr[(j * 3) + 2] = elementValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.Vector4:
                                        {
                                            var listValue = (List<Vector4>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.z;
                                                bufferPtr[(j * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Quaternion:
                                        {
                                            var listValue = (List<Quaternion>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.z;
                                                bufferPtr[(j * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    case SerializationType.Rect:
                                        {
                                            var listValue = (List<Rect>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.width;
                                                bufferPtr[(j * 4) + 3] = elementValue.height;
                                            }
                                        }
                                        break;
                                    case SerializationType.Bounds:
                                        {
                                            var listValue = (List<Bounds>)value;

                                            var bufferPtr = (float*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                var center = elementValue.center;
                                                var size = elementValue.size;
                                                bufferPtr[(j * 6) + 0] = center.x;
                                                bufferPtr[(j * 6) + 1] = center.y;
                                                bufferPtr[(j * 6) + 2] = center.z;
                                                bufferPtr[(j * 6) + 3] = size.x;
                                                bufferPtr[(j * 6) + 4] = size.y;
                                                bufferPtr[(j * 6) + 5] = size.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector2:
                                        {
                                            var listValue = (List<IntVector2>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 2) + 0] = elementValue.x;
                                                bufferPtr[(j * 2) + 1] = elementValue.y;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector3:
                                        {
                                            var listValue = (List<IntVector3>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 3) + 0] = elementValue.x;
                                                bufferPtr[(j * 3) + 1] = elementValue.y;
                                                bufferPtr[(j * 3) + 2] = elementValue.z;
                                            }
                                        }
                                        break;
                                    case SerializationType.IntVector4:
                                        {
                                            var listValue = (List<IntVector4>)value;

                                            var bufferPtr = (int*)byteBufferPtr;
                                            for (int j = 0; j < listLength; j++)
                                            {
                                                var elementValue = listValue[j];
                                                bufferPtr[(j * 4) + 0] = elementValue.x;
                                                bufferPtr[(j * 4) + 1] = elementValue.y;
                                                bufferPtr[(j * 4) + 2] = elementValue.z;
                                                bufferPtr[(j * 4) + 3] = elementValue.w;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                                }
                            }
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            bool isNull = obj == null;
                            dataSize = ExperimentalSerializeNullPrefixIntoBuffer(isNull, offset);
                            if (isNull)
                                break;

                            int valueOffset = offset + dataSize;
                            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

                            switch (elementTypeInfo.serializationType)
                            {
                                case SerializationType.Byte:
                                    {
                                        var trueValue = (byte)obj;

                                        dataSize += SerializationTypeSizes.BYTE;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            *byteBufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.SByte:
                                    {
                                        var trueValue = (sbyte)obj;

                                        dataSize += SerializationTypeSizes.SBYTE;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (sbyte*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Bool:
                                    {
                                        var trueValue = (bool)obj;

                                        dataSize += SerializationTypeSizes.BOOL;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (bool*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Int16:
                                    {
                                        var trueValue = (ushort)obj;

                                        dataSize += SerializationTypeSizes.UINT16;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Int32:
                                    {
                                        var trueValue = (int)obj;

                                        dataSize += SerializationTypeSizes.INT32;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Int64:
                                    {
                                        var trueValue = (long)obj;

                                        dataSize += SerializationTypeSizes.INT64;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt16:
                                    {
                                        var trueValue = (ushort)obj;

                                        dataSize += SerializationTypeSizes.UINT16;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt32:
                                    {
                                        var trueValue = (uint)obj;

                                        dataSize += SerializationTypeSizes.UINT32;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (uint*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.UInt64:
                                    {
                                        var trueValue = (ulong)obj;

                                        dataSize += SerializationTypeSizes.UINT64;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ulong*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Half:
                                    {
                                        var trueValue = (Half)obj;

                                        dataSize += SerializationTypeSizes.HALF;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (ushort*)byteBufferPtr;
                                            *bufferPtr = trueValue.value;
                                        }
                                    }
                                    break;
                                case SerializationType.Single:
                                    {
                                        var trueValue = (float)obj;

                                        dataSize += SerializationTypeSizes.SINGLE;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Double:
                                    {
                                        var trueValue = (double)obj;

                                        dataSize += SerializationTypeSizes.DOUBLE;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (double*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Char:
                                    {
                                        var trueValue = (char)obj;

                                        dataSize += SerializationTypeSizes.CHAR;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (char*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.Decimal:
                                    {
                                        var trueValue = (decimal)obj;

                                        dataSize += SerializationTypeSizes.DECIMAL;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (decimal*)byteBufferPtr;
                                            *bufferPtr = trueValue;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTime:
                                    {
                                        var trueValue = (DateTime)obj;

                                        dataSize += SerializationTypeSizes.DATE_TIME;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue.Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.DateTimeOffset:
                                    {
                                        var trueValue = (DateTimeOffset)obj;

                                        dataSize += SerializationTypeSizes.DATE_TIME_OFFSET;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue.Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.TimeSpan:
                                    {
                                        var trueValue = (TimeSpan)obj;

                                        dataSize += SerializationTypeSizes.TIME_SPAN;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (long*)byteBufferPtr;
                                            *bufferPtr = trueValue.Ticks;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector2:
                                    {
                                        var trueValue = (Vector2)obj;

                                        dataSize += SerializationTypeSizes.VECTOR2;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector3:
                                    {
                                        var trueValue = (Vector3)obj;

                                        dataSize += SerializationTypeSizes.VECTOR3;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                        }
                                    }
                                    break;
                                case SerializationType.Vector4:
                                    {
                                        var trueValue = (Vector4)obj;

                                        dataSize += SerializationTypeSizes.VECTOR4;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                            bufferPtr[3] = trueValue.w;
                                        }
                                    }
                                    break;
                                case SerializationType.Quaternion:
                                    {
                                        var trueValue = (Quaternion)obj;

                                        dataSize += SerializationTypeSizes.QUATERNION;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                            bufferPtr[3] = trueValue.w;
                                        }
                                    }
                                    break;
                                case SerializationType.Rect:
                                    {
                                        var trueValue = (Rect)obj;

                                        dataSize += SerializationTypeSizes.RECT;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.width;
                                            bufferPtr[3] = trueValue.height;
                                        }
                                    }
                                    break;
                                case SerializationType.Bounds:
                                    {
                                        var trueValue = (Bounds)obj;

                                        dataSize += SerializationTypeSizes.BOUNDS;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (float*)byteBufferPtr;
                                            var center = trueValue.center;
                                            var size = trueValue.size;
                                            bufferPtr[0] = center.x;
                                            bufferPtr[1] = center.y;
                                            bufferPtr[2] = center.z;
                                            bufferPtr[3] = size.x;
                                            bufferPtr[4] = size.y;
                                            bufferPtr[5] = size.z;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector2:
                                    {
                                        var trueValue = (IntVector2)obj;

                                        dataSize += SerializationTypeSizes.INT_VECTOR2;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector3:
                                    {
                                        var trueValue = (IntVector3)obj;

                                        dataSize += SerializationTypeSizes.INT_VECTOR3;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                        }
                                    }
                                    break;
                                case SerializationType.IntVector4:
                                    {
                                        var trueValue = (IntVector4)obj;

                                        dataSize += SerializationTypeSizes.INT_VECTOR4;
                                        EnsureBufferSize(offset + dataSize);

                                        fixed (byte* byteBufferPtr = &buffer[valueOffset])
                                        {
                                            var bufferPtr = (int*)byteBufferPtr;
                                            bufferPtr[0] = trueValue.x;
                                            bufferPtr[1] = trueValue.y;
                                            bufferPtr[2] = trueValue.z;
                                            bufferPtr[3] = trueValue.w;
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
                            }
                        }
                        break;
                    case SerializationType.ObjectArray:
                        {
                            var value = obj as Array;
                            bool isNull = value == null;
                            int arrLength = value?.Length ?? -1;
                            dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, arrLength, offset);
                            if (isNull)
                                break;

                            // TODO: Object cyclic reference

                            int arrOffset = offset + dataSize;
                            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

                            for (int j = 0; j < arrLength; j++)
                            {
                                object element = value.GetValue(j);
                                int elementDataSize = ExperimentalSerializeObjectIntoBuffer(element, elementTypeInfo, arrOffset);
                                dataSize += elementDataSize;
                                arrOffset += elementDataSize;
                            }
                        }
                        break;
                    case SerializationType.ObjectList:
                        {
                            var value = obj as IList;
                            bool isNull = value == null;
                            int listLength = value?.Count ?? -1;
                            dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, listLength, offset);
                            if (isNull)
                                break;

                            // TODO: Object cyclic reference

                            int listOffset = offset + dataSize;
                            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

                            for (int j = 0; j < listLength; j++)
                            {
                                object element = value[j];
                                int elementDataSize = ExperimentalSerializeObjectIntoBuffer(element, elementTypeInfo, listOffset);
                                dataSize += elementDataSize;
                                listOffset += elementDataSize;
                            }
                        }
                        break;
                    case SerializationType.ObjectNullable:
                        {
                            bool isNull = obj == null;

                            if (!isNull)
                                dataSize = ExperimentalSerializeObjectIntoBuffer(obj, typeInfo.elementTypeInfo, offset);
                            else
                                dataSize = ExperimentalSerializeNullPrefixIntoBuffer(true, offset);
                        }
                        break;
                    default:
                        throw new UnexpectedException("Unexpected serialization type: " + typeInfo.serializationType);
                }
            }

            return dataSize;
        }

        // Returns size
        private int ExperimentalSerializeStringIntoBuffer(string value, int offset)
        {
            bool isNull = value == null;
            int strLength = value?.Length ?? -1;
            int dataSize = ExperimentalSerializeNullLengthPrefixIntoBuffer(isNull, strLength, offset);
            if (isNull)
                return dataSize;

            int strOffset = offset + dataSize;

            Encoding systemEncoding = null;

            switch (settings.defaultStringEncodeType)
            {
                case StringEncodeType.Char:
                    {
                        dataSize += strLength * SerializationTypeSizes.CHAR;
                        EnsureBufferSize(offset + dataSize);

                        fixed (byte* byteBufferPtr = &buffer[strOffset])
                        fixed (char* valuePtr = value)
                        {
                            var bufferPtr = (char*)byteBufferPtr;

                            for (int i = 0; i < strLength; i++)
                            {
                                bufferPtr[i] = value[i];
                            }
                        }
                    }
                    break;
                case StringEncodeType.Byte:
                    {
                        dataSize += strLength * SerializationTypeSizes.BYTE;
                        EnsureBufferSize(offset + dataSize);

                        fixed (byte* byteBufferPtr = &buffer[strOffset])
                        fixed (char* valuePtr = value)
                        {
                            for (int i = 0; i < strLength; i++)
                            {
                                byteBufferPtr[i] = unchecked((byte)value[i]);
                            }
                        }
                    }
                    break;
                case StringEncodeType.Default:
                    systemEncoding = Encoding.Default;
                    goto SystemEncoding;
                case StringEncodeType.ASCII:
                    systemEncoding = Encoding.ASCII;
                    goto SystemEncoding;
                case StringEncodeType.Unicode:
                    systemEncoding = Encoding.Unicode;
                    goto SystemEncoding;
                case StringEncodeType.UTF7:
                    systemEncoding = Encoding.UTF7;
                    goto SystemEncoding;
                case StringEncodeType.UTF8:
                    systemEncoding = Encoding.UTF8;
                    goto SystemEncoding;
                case StringEncodeType.UTF32:
                    systemEncoding = Encoding.UTF32;
                    goto SystemEncoding;
                case StringEncodeType.BigEndianUnicode:
                    systemEncoding = Encoding.BigEndianUnicode;
                    goto SystemEncoding;
                SystemEncoding:
                    {
                        fixed (char* valuePtr = value)
                        {
                            int byteCount = systemEncoding.GetByteCount(valuePtr, strLength);
                            dataSize += byteCount;
                            EnsureBufferSize(offset + dataSize);

                            fixed (byte* byteBufferPtr = &buffer[strOffset])
                            {
                                systemEncoding.GetBytes(valuePtr, strLength, byteBufferPtr, byteCount);
                            }
                        }
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported string encode type: " + settings.defaultStringEncodeType);
            }

            return dataSize;
        }

        // Returns size
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ExperimentalSerializeNullPrefixIntoBuffer(bool isNull, int offset)
        {
            EnsureBufferSize(SerializationTypeSizes.SBYTE + offset);

            sbyte nullPrefixValue = isNull ? TinySerializerConstants.NULL_VALUE : TinySerializerConstants.HAS_VALUE;

            fixed (byte* byteBufferPtr = &buffer[offset])
            {
                sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                *bufferPtr = nullPrefixValue;
            }

            return SerializationTypeSizes.SBYTE;
        }

        // Returns size
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ExperimentalSerializeNullLengthPrefixIntoBuffer(bool isNull, int length, int offset)
        {
            int dataSize;

            if (isNull)
            {
                dataSize = SerializationTypeSizes.SBYTE;
                EnsureBufferSize(dataSize + offset);

                fixed (byte* byteBufferPtr = &buffer[offset])
                {
                    sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                    *bufferPtr = TinySerializerConstants.NULL_VALUE;
                }
            }
            else
            {
                if (length < sbyte.MaxValue)
                {
                    dataSize = SerializationTypeSizes.SBYTE;
                    EnsureBufferSize(dataSize + offset);

                    fixed (byte* byteBufferPtr = &buffer[offset])
                    {
                        sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                        *bufferPtr = (sbyte)length;
                    }
                }
                else if (length < ushort.MaxValue)
                {
                    dataSize = SerializationTypeSizes.SBYTE + SerializationTypeSizes.UINT16;
                    EnsureBufferSize(dataSize + offset);

                    fixed (byte* byteBufferPtr = &buffer[offset])
                    {
                        sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                        *bufferPtr++ = TinySerializerConstants.USHORT_LENGTH;

                        ushort* lengthBufferPtr = (ushort*)bufferPtr;
                        *lengthBufferPtr = (ushort)length;
                    }
                }
                else
                {
                    dataSize = SerializationTypeSizes.SBYTE + SerializationTypeSizes.UINT32;
                    EnsureBufferSize(dataSize + offset);

                    fixed (byte* byteBufferPtr = &buffer[offset])
                    {
                        sbyte* bufferPtr = (sbyte*)byteBufferPtr;
                        *bufferPtr++ = TinySerializerConstants.USHORT_LENGTH;

                        uint* lengthBufferPtr = (uint*)bufferPtr;
                        *lengthBufferPtr = (uint)length;
                    }
                }
            }

            return dataSize;
        }

        /// <summary>
        /// Ensures that the buffer is at least of a specific size.
        /// </summary>
        /// <param name="size">Ensured buffer size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureBufferSize(int size)
        {
            if (bufferSize < size)
            {
                // TODO: Check for int overflow
                int newBufferSize = MathUtil.NextPowerOfTwo(size + 1);

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

        public TTarget Deserialize<TTarget>(byte[] data) where TTarget : new()
        {
            Type tTarget = typeof(TTarget);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tTarget);

            if (!typeInfo.emittedDefaultConstructor)
                typeInfo.EmitDefaultConstructor<TTarget>();

            var constructorDelegate = (EmitUtil.DefaultConstructorDelegate<TTarget>)typeInfo.defaultConstructor;
            TTarget obj = constructorDelegate();

            fixed (byte* dataPtr = data)
            {
                DeserializeFromData(ref obj, typeInfo, dataPtr, 0);
            }

            return obj;
        }

        public TTarget Deserialize<TTarget>(byte[] data, int offset) where TTarget : new()
        {
            Type tTarget = typeof(TTarget);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tTarget);

            if (!typeInfo.emittedDefaultConstructor)
                typeInfo.EmitDefaultConstructor<TTarget>();

            var constructorDelegate = (EmitUtil.DefaultConstructorDelegate<TTarget>)typeInfo.defaultConstructor;
            TTarget obj = constructorDelegate();

            fixed (byte* dataPtr = data)
            {
                DeserializeFromData(ref obj, typeInfo, dataPtr, offset);
            }

            return obj;
        }

        // Returns size
        public int DeserializeFromData<TTarget>(ref TTarget obj, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            int dataSize = 0;

            return dataSize;
        }

        // Returns size
        public int DeserializeStructFromData(TypedReference objRef, TinySerializer typeInfo, byte* data, int offset)
        {
            int dataSize = 0;

            return dataSize;
        }

        // Returns size
        public int DeserializeObjectFromData(ref object obj, TinySerializer typeInfo, byte* data, int offset)
        {
            int dataSize = 0;

            return dataSize;
        }

        #region OLD_SERIALIZE
        // Obsolete this when Experimental is working
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

                switch (simpleTypeInfo.serializationType)
                {
                    #region PRIMITIVE
                    case SerializationType.Byte:
                        {
                            dataSize = SerializationTypeSizes.BYTE;
                            EnsureBufferSize(dataSize + offset);

                            byte value = EmitHelper<byte>.CastFrom(obj);

                            fixed (byte* bufferPtr = &buffer[offset])
                                *bufferPtr = value;
                        }
                        break;
                    case SerializationType.SByte:
                        {
                            dataSize = SerializationTypeSizes.SBYTE;
                            EnsureBufferSize(dataSize + offset);

                            sbyte value = EmitHelper<sbyte>.CastFrom(obj);

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

                            bool value = EmitHelper<bool>.CastFrom(obj);

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

                            short value = EmitHelper<short>.CastFrom(obj);

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

                            int value = EmitHelper<int>.CastFrom(obj);

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

                            long value = EmitHelper<long>.CastFrom(obj);

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

                            ushort value = EmitHelper<ushort>.CastFrom(obj);

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

                            uint value = EmitHelper<uint>.CastFrom(obj);

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

                            ulong value = EmitHelper<ulong>.CastFrom(obj);

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

                            Half value = EmitHelper<Half>.CastFrom(obj);

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

                            float value = EmitHelper<float>.CastFrom(obj);

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

                            double value = EmitHelper<double>.CastFrom(obj);

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

                            char value = EmitHelper<char>.CastFrom(obj);

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

                            decimal value = EmitHelper<decimal>.CastFrom(obj);

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

                            DateTime value = EmitHelper<DateTime>.CastFrom(obj);

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

                            DateTimeOffset value = EmitHelper<DateTimeOffset>.CastFrom(obj);

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

                            TimeSpan value = EmitHelper<TimeSpan>.CastFrom(obj);

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

                            Vector2 value = EmitHelper<Vector2>.CastFrom(obj);

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

                            Vector3 value = EmitHelper<Vector3>.CastFrom(obj);

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

                            Vector4 value = EmitHelper<Vector4>.CastFrom(obj);

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

                            Quaternion value = EmitHelper<Quaternion>.CastFrom(obj);

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

                            Rect value = EmitHelper<Rect>.CastFrom(obj);

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

                            Bounds value = EmitHelper<Bounds>.CastFrom(obj);

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

                            IntVector2 value = EmitHelper<IntVector2>.CastFrom(obj);

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

                            IntVector3 value = EmitHelper<IntVector3>.CastFrom(obj);

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

                            IntVector4 value = EmitHelper<IntVector4>.CastFrom(obj);

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
                    #endregion
                    case SerializationType.String:
                        {
                            string value = EmitHelper<string>.CastFrom(obj);
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
                            Array baseValue = EmitHelper<Array>.CastFrom(obj);
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
                            IList baseValue = EmitHelper<IList>.CastFrom(obj);
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
                                        byte? value = EmitHelper<byte?>.CastFrom(obj);
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
                                        sbyte? value = EmitHelper<sbyte?>.CastFrom(obj);
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
                                        bool? value = EmitHelper<bool?>.CastFrom(obj);
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
                                        short? value = EmitHelper<short?>.CastFrom(obj);
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
                                        int? value = EmitHelper<int?>.CastFrom(obj);
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
                                        long? value = EmitHelper<long?>.CastFrom(obj);
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
                                        ushort? value = EmitHelper<ushort?>.CastFrom(obj);
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
                                        uint? value = EmitHelper<uint?>.CastFrom(obj);
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
                                        ulong? value = EmitHelper<ulong?>.CastFrom(obj);
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
                                        Half? value = EmitHelper<Half?>.CastFrom(obj);
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
                                        float? value = EmitHelper<float?>.CastFrom(obj);
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
                                        double? value = EmitHelper<double?>.CastFrom(obj);
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
                                        char? value = EmitHelper<char?>.CastFrom(obj);
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
                                        decimal? value = EmitHelper<decimal?>.CastFrom(obj);
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
                                        DateTime? value = EmitHelper<DateTime?>.CastFrom(obj);
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
                                        DateTimeOffset? value = EmitHelper<DateTimeOffset?>.CastFrom(obj);
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
                                        TimeSpan? value = EmitHelper<TimeSpan?>.CastFrom(obj);
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
                                        Vector2? value = EmitHelper<Vector2?>.CastFrom(obj);
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
                                        Vector3? value = EmitHelper<Vector3?>.CastFrom(obj);
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
                                        Vector4? value = EmitHelper<Vector4?>.CastFrom(obj);
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
                                        Quaternion? value = EmitHelper<Quaternion?>.CastFrom(obj);
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
                                        Rect? value = EmitHelper<Rect?>.CastFrom(obj);
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
                                        Bounds? value = EmitHelper<Bounds?>.CastFrom(obj);
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
                                        IntVector2? value = EmitHelper<IntVector2?>.CastFrom(obj);
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
                                        IntVector3? value = EmitHelper<IntVector3?>.CastFrom(obj);
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
                                        IntVector4? value = EmitHelper<IntVector4?>.CastFrom(obj);
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

        // Obsolete this when Experimental is working
        // Returns new offset
        private int SerializeIntoBuffer(object boxedObj, int offset)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
#endif
