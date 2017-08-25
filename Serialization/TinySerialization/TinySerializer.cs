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
    /// <summary>
    /// WIP serializer with limited features, very fast, allocates little garbage and produces very small serialization data.
    /// </summary>
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

        /// <summary>
        /// Prewarms a specific type (and its containing types) for both serialization and deserialization.
        /// </summary>
        /// <typeparam name="TSource">Type to be prewarmed.</typeparam>
        public void Prewarm<TSource>() where TSource : new()
        {
            Type tSource = typeof(TSource);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tSource);

            if (typeInfo.serializationType == SerializationType.Object)
            {
                typeInfo.EmitDefaultConstructor();
                typeInfo.InspectFields();
                typeInfo.EmitFieldGetters();
                typeInfo.EmitFieldSetters();
                typeInfo.EmitArrayConstructor();
                typeInfo.EmitListConstructor();

                if (typeInfo.isValueType)
                {
                    typeInfo.EmitFieldGettersByTypedRef();
                }

                // TODO: Prewarm types of fields and properties recursively (pass through list of already prewarmed types to avoid infinite loops)

                TinySerializerTypeInfo.GetTypeInfo(tSource);
            }
        }

        /// <summary>
        /// Determines if a field type info should be serialized based on decrated attributes and settings.
        /// </summary>
        /// <param name="fieldTypeInfo">Field Type Info to check.</param>
        /// <returns>Returns true if the field type info should be serialized.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ShouldSerializeField(TinySerializerTypeInfo.FieldTypeInfo fieldTypeInfo)
        {
            if (settings.serializeAllFields)
                return true;

            if (fieldTypeInfo.hasSerializeAttribute)
                return true;

            if (fieldTypeInfo.hasNonSerializeAttribute)
                return false;

            if (fieldTypeInfo.hasObsoleteAttribute && !settings.serializeObsoleteFields)
                return false;

            if (fieldTypeInfo.isPrivate && !settings.serializePrivateFields)
                return false;

            return true;
        }

        #region SERIALIZATION

        /// <summary>
        /// Serializes an object into a byte array.
        /// </summary>
        /// <typeparam name="TSource">Type of the object.</typeparam>
        /// <param name="obj">Object value to serialize.</param>
        /// <returns>Returns the serialization data of the object.</returns>
        public byte[] Serialize<TSource>(TSource obj)
        {
            Type tSource = typeof(TSource);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tSource);

            int dataSize = SerializeIntoBuffer(obj, typeInfo, 0);

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

        /// <summary>
        /// Serializes an object into a specified byte array.
        /// </summary>
        /// <remarks>
        /// <see cref=">data"/> is automatically resized to fit the serialization data.
        /// </remarks>
        /// <typeparam name="TSource">Type of the object.</typeparam>
        /// <param name="obj">Object value to serialize.</param>
        /// <param name="data">Byte array to serialize the object into.</param>
        /// <returns>Returns the size of the serialization data.</returns>
        public int Serialize<TSource>(TSource obj, ref byte[] data)
        {
            Type tSource = typeof(TSource);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tSource);

            int dataSize = SerializeIntoBuffer(obj, typeInfo, 0);

            if (data == null || data.Length < dataSize)
            {
                data = new byte[dataSize];
            }

            fixed (byte* bufferPtr = buffer)
            {
                fixed (byte* dataPtr = data)
                {
                    for (int i = 0; i < dataSize; i++)
                    {
                        dataPtr[i] = bufferPtr[i];
                    }
                }

                return dataSize;
            }
        }

        /// <summary>
        /// Serializes an object into a specified byte array at an offset.
        /// </summary>
        /// <remarks>
        /// <see cref=">data"/> is automatically resized to fit the serialization data.
        /// </remarks>
        /// <typeparam name="TSource">Type of the object.</typeparam>
        /// <param name="obj">Object value to serialize.</param>
        /// <param name="data">Object value to serialize.</param>
        /// <param name="offset">Offset in the byte array to offset the serialization data.</param>
        /// <returns>Returns the size of the serialization data.</returns>
        public int Serialize<TSource>(TSource obj, ref byte[] data, int offset)
        {
            Type tSource = typeof(TSource);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tSource);

            int dataSize = SerializeIntoBuffer(obj, typeInfo, 0);
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

        /// <summary>
        /// Serializes an object of type <see cref="TSource"/> into the buffer.
        /// </summary>
        /// <typeparam name="TSource">Type of the object to serialize.</typeparam>
        /// <param name="obj">Object value to serialize.</param>
        /// <param name="typeInfo">Type info of the object.</param>
        /// <param name="offset">Offset in the buffer to serialize the value.</param>
        /// <returns>Returns the size of the object serialization.</returns>
        private int SerializeIntoBuffer<TSource>(TSource obj, TinySerializerTypeInfo typeInfo, int offset)
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
                        dataSize = SerializeStringIntoBuffer(value, offset);
                    }
                    break;
                case SerializationType.PrimitiveArray:
                    {
                        Array value = (Array)(object)obj;
                        dataSize = SerializePrimitiveArrayIntoBuffer(value, offset, typeInfo);
                    }
                    break;
                case SerializationType.PrimitiveList:
                    {
                        IList value = (IList)(object)obj;
                        dataSize = SerializePrimitiveListIntoBuffer(value, offset, typeInfo);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                                    dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
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
                            dataSize = SerializeStructIntoBuffer(__makeref(obj), typeInfo, offset);
                        else
                            dataSize = SerializeObjectIntoBuffer(obj, typeInfo, offset);
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported serialization type: " + typeInfo.serializationType);
            }

            return dataSize;
        }

        /// <summary>
        /// Serializes a typed reference into the buffer.
        /// </summary>
        /// <param name="objRef">Object reference to serialize.</param>
        /// <param name="typeInfo">Type info of the object.</param>
        /// <param name="offset">Offset in the buffer to serialize the typed reference.</param>
        /// <returns></returns>
        private int SerializeStructIntoBuffer(TypedReference objRef, TinySerializerTypeInfo typeInfo, int offset)
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

                if (!settings.serializeAllFields)
                {
                    if (!ShouldSerializeField(fieldTypeInfo))
                        continue;
                }

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

                            fieldDataSize = SerializeStringIntoBuffer(value, fieldOffset);
                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef) as Array;
                            fieldDataSize = SerializePrimitiveArrayIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef) as IList;
                            fieldDataSize = SerializePrimitiveListIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);
                            fieldDataSize = SerializePrimitiveNullableIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                        }
                        break;
                    case SerializationType.ObjectArray:
                    case SerializationType.ObjectList:
                    case SerializationType.ObjectNullable:
                    case SerializationType.Object:
                        {
                            var getterDelegate = (EmitUtil.FieldGetterDelegateByTypedRef<object>)fieldTypeInfo.getterByTypedRef;
                            var value = getterDelegate.Invoke(objRef);

                            fieldDataSize = SerializeObjectIntoBuffer(value, fieldTypeInfo.fieldTypeInfo, fieldOffset);
                        }
                        break;
                    default:
                        throw new UnsupportedException("Unsupported field serialization type: " + fieldSerializationType);
                }

                dataSize += fieldDataSize;
            }

            return dataSize;
        }

        /// <summary>
        /// Serializes an object into the buffer.
        /// </summary>
        /// <param name="obj">Object value to serialize.</param>
        /// <param name="typeInfo">Type info of the object.</param>
        /// <param name="offset">Offset in the buffer to serialize the value.</param>
        /// <returns>Returns the size of the object serialization.</returns>
        private int SerializeObjectIntoBuffer(object obj, TinySerializerTypeInfo typeInfo, int offset)
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
                    dataSize += SerializeNullPrefixIntoBuffer(isNull, offset);
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

                    if (!settings.serializeAllFields)
                    {
                        if (!ShouldSerializeField(fieldTypeInfo))
                            continue;
                    }

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

                                fieldDataSize = SerializeStringIntoBuffer(value, fieldOffset);
                            }
                            break;
                        case SerializationType.PrimitiveArray:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as Array;
                                fieldDataSize = SerializePrimitiveArrayIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                            }
                            break;
                        case SerializationType.PrimitiveList:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as IList;
                                fieldDataSize = SerializePrimitiveListIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                            }
                            break;
                        case SerializationType.PrimitiveNullable:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);
                                fieldDataSize = SerializePrimitiveNullableIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                            }
                            break;
                        case SerializationType.ObjectArray:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as Array;
                                fieldDataSize = SerializeObjectArrayIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                            }
                            break;
                        case SerializationType.ObjectList:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj) as IList;
                                fieldDataSize = SerializeObjectListIntoBuffer(value, fieldOffset, fieldTypeInfo.fieldTypeInfo);
                            }
                            break;
                        case SerializationType.ObjectNullable:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                bool isNull = value == null;

                                if (!isNull)
                                    fieldDataSize = SerializeObjectIntoBuffer(value, fieldTypeInfo.fieldTypeInfo.elementTypeInfo, fieldOffset);
                                else
                                    fieldDataSize = SerializeNullPrefixIntoBuffer(true, fieldOffset);
                            }
                            break;
                        case SerializationType.Object:
                            {
                                var getterDelegate = (EmitUtil.FieldGetterDelegate<object, object>)fieldTypeInfo.getter;
                                var value = getterDelegate.Invoke(obj);

                                fieldDataSize = SerializeObjectIntoBuffer(value, fieldTypeInfo.fieldTypeInfo, fieldOffset);
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
                            dataSize = SerializeStringIntoBuffer(value, offset);
                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            var value = obj as Array;
                            dataSize = SerializePrimitiveArrayIntoBuffer(value, offset, typeInfo);
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {
                            var value = obj as IList;
                            dataSize = SerializePrimitiveListIntoBuffer(value, offset, typeInfo);
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            dataSize = SerializePrimitiveNullableIntoBuffer(obj, offset, typeInfo);
                        }
                        break;
                    case SerializationType.ObjectArray:
                        {
                            var value = obj as Array;
                            dataSize = SerializeObjectArrayIntoBuffer(value, offset, typeInfo);
                        }
                        break;
                    case SerializationType.ObjectList:
                        {
                            var value = obj as IList;
                            dataSize = SerializeObjectListIntoBuffer(value, offset, typeInfo);
                        }
                        break;
                    case SerializationType.ObjectNullable:
                        {
                            bool isNull = obj == null;

                            if (!isNull)
                                dataSize = SerializeObjectIntoBuffer(obj, typeInfo.elementTypeInfo, offset);
                            else
                                dataSize = SerializeNullPrefixIntoBuffer(true, offset);
                        }
                        break;
                    default:
                        throw new UnexpectedException("Unexpected serialization type: " + typeInfo.serializationType);
                }
            }

            return dataSize;
        }

        /// <summary>
        /// Serializes a string value into the buffer.
        /// </summary>
        /// <param name="value">Value of the string to serialize.</param>
        /// <param name="offset">Offset in the buffer to serialize the string.</param>
        /// <returns>Returns the size of the string serialization.</returns>
        private int SerializeStringIntoBuffer(string value, int offset)
        {
            bool isNull = value == null;
            int strLength = value?.Length ?? -1;
            int dataSize = SerializeNullLengthPrefixIntoBuffer(isNull, strLength, offset);
            if (isNull)
                return dataSize;

            int strOffset = offset + dataSize;

            Encoding systemEncoding;

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
                            int byteCountPrefixSize = SerializeNullLengthPrefixIntoBuffer(false, byteCount, strOffset);

                            strOffset += byteCountPrefixSize;
                            dataSize += byteCountPrefixSize + byteCount;

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

        /// <summary>
        /// Serializes an object array into the buffer.
        /// </summary>
        /// <param name="value">Value of the object array to serialize.</param>
        /// <param name="offset">Offset in the buffer to serialize the object array.</param>
        /// <param name="typeInfo">Type info of the array value.</param>
        /// <returns>Returns the size of the object array serialization.</returns>
        private int SerializeObjectArrayIntoBuffer(Array value, int offset, TinySerializerTypeInfo typeInfo)
        {
            bool isNull = value == null;
            int arrLength = value?.Length ?? -1;
            int dataSize = SerializeNullLengthPrefixIntoBuffer(isNull, arrLength, offset);
            if (isNull)
                return dataSize;

            // TODO: Object cyclic reference

            int arrOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

            var arrayAccessInfo = typeInfo.arrayAccessInfo;
            if (!arrayAccessInfo.emittedGetterDelegate)
                arrayAccessInfo.EmitGetterDelegate();
            var arrayGetterDelegate = (EmitUtil.ArrayValueGetterDelegate<object>)arrayAccessInfo.getter;

            for (int i = 0; i < arrLength; i++)
            {
                object element = arrayGetterDelegate.Invoke(value, i);
                int elementDataSize = SerializeObjectIntoBuffer(element, elementTypeInfo, arrOffset);
                dataSize += elementDataSize;
                arrOffset += elementDataSize;
            }

            return dataSize;
        }

        /// <summary>
        /// Serializes an object list into the buffer.
        /// </summary>
        /// <param name="value">Value of the object list to serialize.</param>
        /// <param name="offset">Offset in the buffer to serialize the object list.</param>
        /// <param name="typeInfo">Type info of the list value.</param>
        /// <returns>Returns the size of the object list serialization.</returns>
        private int SerializeObjectListIntoBuffer(IList value, int offset, TinySerializerTypeInfo typeInfo)
        {
            bool isNull = value == null;
            int listLength = value?.Count ?? -1;
            int dataSize = SerializeNullLengthPrefixIntoBuffer(isNull, listLength, offset);
            if (isNull)
                return dataSize;

            // TODO: Object cyclic reference

            int listOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

            for (int j = 0; j < listLength; j++)
            {
                object element = value[j];
                int elementDataSize = SerializeObjectIntoBuffer(element, elementTypeInfo, listOffset);
                dataSize += elementDataSize;
                listOffset += elementDataSize;
            }

            return dataSize;
        }

        /// <summary>
        /// Serializes a primitive array into the buffer.
        /// </summary>
        /// <param name="value">Value of the primitive array to serialize.</param>
        /// <param name="offset">Offset in the buffer to serialize the primitive array.</param>
        /// <param name="typeInfo">Type info of the array value.</param>
        /// <returns>Returns the size of the primitive array serialization.</returns>
        private int SerializePrimitiveArrayIntoBuffer(Array value, int offset, TinySerializerTypeInfo typeInfo)
        {
            bool isNull = value == null;
            int arrLength = value?.Length ?? -1;
            int dataSize = SerializeNullLengthPrefixIntoBuffer(isNull, arrLength, offset);
            if (isNull)
                return dataSize;

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

            return dataSize;
        }

        /// <summary>
        /// Serializes a primitive list into the buffer.
        /// </summary>
        /// <param name="value">Value of the primitive list to serialize.</param>
        /// <param name="offset">Offset in the buffer to serialize the primitive list.</param>
        /// <param name="typeInfo">Type info of the list value.</param>
        /// <returns>Returns the size of the primitive list serialization.</returns>
        private int SerializePrimitiveListIntoBuffer(IList value, int offset, TinySerializerTypeInfo typeInfo)
        {
            bool isNull = value == null;
            int listLength = value?.Count ?? -1;
            int dataSize = SerializeNullLengthPrefixIntoBuffer(isNull, listLength, offset);
            if (isNull)
                return dataSize;

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

            return dataSize;
        }

        /// <summary>
        /// Serializes a primitive nullable into the buffer.
        /// </summary>
        /// <param name="value">Value of the primitive nullable to serialize.</param>
        /// <param name="offset">Offset in the buffer to serialize the primitive nullable.</param>
        /// <param name="typeInfo">Type info of the nullable value.</param>
        /// <returns>Returns the size of the primitive nullable serialization.</returns>
        private int SerializePrimitiveNullableIntoBuffer(object value, int offset, TinySerializerTypeInfo typeInfo)
        {
            bool isNull = value == null;
            int dataSize = SerializeNullPrefixIntoBuffer(isNull, offset);
            if (isNull)
                return dataSize;

            int valueOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

            switch (elementTypeInfo.serializationType)
            {
                case SerializationType.Byte:
                    {
                        var trueValue = (byte)value;

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
                        var trueValue = (sbyte)value;

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
                        var trueValue = (bool)value;

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
                        var trueValue = (short)value;

                        dataSize += SerializationTypeSizes.UINT16;
                        EnsureBufferSize(offset + dataSize);

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
                        var trueValue = (long)value;

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
                        var trueValue = (ushort)value;

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
                        var trueValue = (uint)value;

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
                        var trueValue = (ulong)value;

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
                        var trueValue = (Half)value;

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
                        var trueValue = (float)value;

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
                        var trueValue = (double)value;

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
                        var trueValue = (char)value;

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
                        var trueValue = (decimal)value;

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
                        var trueValue = (DateTime)value;

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
                        var trueValue = (DateTimeOffset)value;

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
                        var trueValue = (TimeSpan)value;

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
                        var trueValue = (Vector2)value;

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
                        var trueValue = (Vector3)value;

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
                        var trueValue = (Vector4)value;

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
                        var trueValue = (Quaternion)value;

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
                        var trueValue = (Rect)value;

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
                        var trueValue = (Bounds)value;

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
                        var trueValue = (IntVector2)value;

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
                        var trueValue = (IntVector3)value;

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
                        var trueValue = (IntVector4)value;

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

            return dataSize;
        }

        /// <summary>
        /// Serializes null value prefix into the buffer.
        /// </summary>
        /// <param name="isNull">If the value to be serialized is null.</param>
        /// <param name="offset">Offset in the buffer to serialize the value.</param>
        /// <returns>Returns the size of the prefix.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SerializeNullPrefixIntoBuffer(bool isNull, int offset)
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

        /// <summary>
        /// Serializes null value and length prefix into the buffer.
        /// </summary>
        /// <param name="isNull">If value to be serialized is null.</param>
        /// <param name="length">Length of the value to be serialized if not null.</param>
        /// <param name="offset">Offset in the buffer to serialize the value.</param>
        /// <returns>Returns the size of the prefix.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SerializeNullLengthPrefixIntoBuffer(bool isNull, int length, int offset)
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

        #endregion

        #region DESERIALIZATION

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <typeparam name="TTarget">Type of the object to deserialize.</typeparam>
        /// <param name="data">Byte array containing the serialization data of the object.</param>
        /// <returns>Returns a new instance of an object from the serialization data.</returns>
        public TTarget Deserialize<TTarget>(byte[] data)
        {
            Type tTarget = typeof(TTarget);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tTarget);

            TTarget obj;

            fixed (byte* dataPtr = data)
            {
                DeserializeFromData(out obj, typeInfo, dataPtr, 0);
            }

            return obj;
        }

        /// <summary>
        /// Deserializes an object from a byte array at an offset.
        /// </summary>
        /// <typeparam name="TTarget">Type of the object to deserialize.</typeparam>
        /// <param name="data">Byte array containing the serialization data of the object.</param>
        /// <param name="offset">Offset in the byte array where the serialization data begins.</param>
        /// <returns>Returns a new instanc eof an object from the serialization data.</returns>
        public TTarget Deserialize<TTarget>(byte[] data, int offset)
        {
            Type tTarget = typeof(TTarget);
            TinySerializerTypeInfo typeInfo = TinySerializerTypeInfo.GetTypeInfo(tTarget);

            TTarget obj;

            fixed (byte* dataPtr = data)
            {
                DeserializeFromData(out obj, typeInfo, dataPtr, offset);
            }

            return obj;
        }

        /// <summary>
        /// Deserializes an an object of type <see cref="TTarget"/> from a byte array.
        /// </summary>
        /// <typeparam name="TTarget">Target object type.</typeparam>
        /// <param name="obj">Reference to target object.</param>
        /// <param name="typeInfo">Type info for target type.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of object.</returns>
        private int DeserializeFromData<TTarget>(out TTarget obj, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            int dataSize = 0;

            switch (typeInfo.serializationType)
            {
                #region PRIMITIVE
                case SerializationType.Byte:
                    {
                        dataSize = SerializationTypeSizes.BYTE;
                        var value = data[offset];
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.SByte:
                    {
                        dataSize = SerializationTypeSizes.SBYTE;
                        var dataPtr = (sbyte*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Bool:
                    {
                        dataSize = SerializationTypeSizes.BOOL;
                        var dataPtr = (bool*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Int16:
                    {
                        dataSize = SerializationTypeSizes.INT16;
                        var dataPtr = (short*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Int32:
                    {
                        dataSize = SerializationTypeSizes.INT32;
                        var dataPtr = (int*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Int64:
                    {
                        dataSize = SerializationTypeSizes.INT64;
                        var dataPtr = (long*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.UInt16:
                    {
                        dataSize = SerializationTypeSizes.UINT16;
                        var dataPtr = (ushort*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.UInt32:
                    {
                        dataSize = SerializationTypeSizes.UINT32;
                        var dataPtr = (uint*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.UInt64:
                    {
                        dataSize = SerializationTypeSizes.UINT64;
                        var dataPtr = (ulong*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Half:
                    {
                        dataSize = SerializationTypeSizes.HALF;
                        var dataPtr = (ushort*)&data[offset];
                        var dataValue = *dataPtr;
                        var value = new Half(dataValue);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Single:
                    {
                        dataSize = SerializationTypeSizes.SINGLE;
                        var dataPtr = (float*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Double:
                    {
                        dataSize = SerializationTypeSizes.DOUBLE;
                        var dataPtr = (double*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Char:
                    {
                        dataSize = SerializationTypeSizes.CHAR;
                        var dataPtr = (char*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Decimal:
                    {
                        dataSize = SerializationTypeSizes.DECIMAL;
                        var dataPtr = (decimal*)&data[offset];
                        var value = *dataPtr;
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.DateTime:
                    {
                        dataSize = SerializationTypeSizes.DATE_TIME;
                        var dataPtr = (long*)&data[offset];
                        var dataValue = *dataPtr;
                        var value = new DateTime(dataValue);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.DateTimeOffset:
                    {
                        dataSize = SerializationTypeSizes.DATE_TIME_OFFSET;
                        var dataPtr = (long*)&data[offset];
                        var dataValue = *dataPtr;
                        var value = new DateTimeOffset(new DateTime(dataValue));
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.TimeSpan:
                    {
                        dataSize = SerializationTypeSizes.TIME_SPAN;
                        var dataPtr = (long*)&data[offset];
                        var dataValue = *dataPtr;
                        var value = new TimeSpan(dataValue);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Vector2:
                    {
                        dataSize = SerializationTypeSizes.VECTOR2;
                        var dataPtr = (float*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var value = new Vector2(x, y);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Vector3:
                    {
                        dataSize = SerializationTypeSizes.VECTOR3;
                        var dataPtr = (float*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var value = new Vector3(x, y, z);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Vector4:
                    {
                        dataSize = SerializationTypeSizes.VECTOR4;
                        var dataPtr = (float*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var w = dataPtr[3];
                        var value = new Vector4(x, y, z, w);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Quaternion:
                    {
                        dataSize = SerializationTypeSizes.QUATERNION;
                        var dataPtr = (float*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var w = dataPtr[3];
                        var value = new Quaternion(x, y, z, w);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Rect:
                    {
                        dataSize = SerializationTypeSizes.RECT;
                        var dataPtr = (float*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var width = dataPtr[2];
                        var height = dataPtr[3];
                        var value = new Rect(x, y, width, height);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.Bounds:
                    {
                        dataSize = SerializationTypeSizes.BOUNDS;
                        var dataPtr = (float*)&data[offset];
                        var centerX = dataPtr[0];
                        var centerY = dataPtr[1];
                        var centerZ = dataPtr[2];
                        var sizeX = dataPtr[3];
                        var sizeY = dataPtr[4];
                        var sizeZ = dataPtr[5];
                        var center = new Vector3(centerX, centerY, centerZ);
                        var size = new Vector3(sizeX, sizeY, sizeZ);
                        var value = new Bounds(center, size);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.IntVector2:
                    {
                        dataSize = SerializationTypeSizes.INT_VECTOR2;
                        var dataPtr = (int*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var value = new IntVector2(x, y);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.IntVector3:
                    {
                        dataSize = SerializationTypeSizes.INT_VECTOR3;
                        var dataPtr = (int*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var value = new IntVector3(x, y, z);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                case SerializationType.IntVector4:
                    {
                        dataSize = SerializationTypeSizes.INT_VECTOR4;
                        var dataPtr = (int*)&data[offset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var w = dataPtr[3];
                        var value = new IntVector4(x, y, z, w);
                        obj = EmitHelper<TTarget>.CastFrom(value);
                    }
                    break;
                #endregion
                case SerializationType.String:
                    {
                        string value;
                        dataSize = DeserializeStringFromData(out value, typeInfo, data, offset);
                        obj = (TTarget)(object)value;
                    }
                    break;
                case SerializationType.PrimitiveArray:
                    {
                        Array value;
                        dataSize = DeserializePrimitiveArrayFromData(out value, typeInfo, data, offset);
                        obj = (TTarget)(object)value;
                    }
                    break;
                case SerializationType.PrimitiveList:
                    {
                        IList value;
                        dataSize = DeserializePrimitiveListFromData(out value, typeInfo, data, offset);
                        obj = (TTarget)value;
                    }
                    break;
                case SerializationType.PrimitiveNullable:
                    {
                        bool isNull;
                        dataSize = DeserializeNullPrefixFromData(out isNull, data, offset);

                        if (isNull)
                        {
                            obj = (TTarget)(object)null;
                            break;
                        }

                        int valueOffset = offset + dataSize;
                        TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
                        dataSize += elementTypeInfo.primitiveSize;

                        switch (elementTypeInfo.serializationType)
                        {
                            case SerializationType.Byte:
                                {
                                    var dataPtr = &data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.SByte:
                                {
                                    var dataPtr = (sbyte*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Bool:
                                {
                                    var dataPtr = (bool*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Int16:
                                {
                                    var dataPtr = (short*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Int32:
                                {
                                    var dataPtr = (int*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Int64:
                                {
                                    var dataPtr = (long*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.UInt16:
                                {
                                    var dataPtr = (ushort*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.UInt32:
                                {
                                    var dataPtr = (uint*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.UInt64:
                                {
                                    var dataPtr = (ulong*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Half:
                                {
                                    var dataPtr = (ushort*)&data[valueOffset];
                                    var value = new Half(*dataPtr);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Single:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Double:
                                {
                                    var dataPtr = (double*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Char:
                                {
                                    var dataPtr = (char*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Decimal:
                                {
                                    var dataPtr = (decimal*)&data[valueOffset];
                                    var value = *dataPtr;
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.DateTime:
                                {
                                    var dataPtr = (long*)&data[valueOffset];
                                    var value = new DateTime(*dataPtr);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.DateTimeOffset:
                                {
                                    var dataPtr = (long*)&data[valueOffset];
                                    var value = new DateTimeOffset(new DateTime(*dataPtr));
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.TimeSpan:
                                {
                                    var dataPtr = (long*)&data[valueOffset];
                                    var value = new TimeSpan(*dataPtr);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Vector2:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var value = new Vector2(x, y);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Vector3:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var z = dataPtr[2];
                                    var value = new Vector3(x, y, z);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Vector4:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var z = dataPtr[2];
                                    var w = dataPtr[3];
                                    var value = new Vector4(x, y, z, w);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Quaternion:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var z = dataPtr[2];
                                    var w = dataPtr[3];
                                    var value = new Quaternion(x, y, z, w);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Rect:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var width = dataPtr[2];
                                    var height = dataPtr[3];
                                    var value = new Rect(x, y, width, height);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.Bounds:
                                {
                                    var dataPtr = (float*)&data[valueOffset];
                                    var centerX = dataPtr[0];
                                    var centerY = dataPtr[1];
                                    var centerZ = dataPtr[2];
                                    var sizeX = dataPtr[3];
                                    var sizeY = dataPtr[4];
                                    var sizeZ = dataPtr[5];
                                    var center = new Vector3(centerX, centerY, centerZ);
                                    var size = new Vector3(sizeX, sizeY, sizeZ);
                                    var value = new Bounds(center, size);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.IntVector2:
                                {
                                    var dataPtr = (int*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var value = new IntVector2(x, y);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.IntVector3:
                                {
                                    var dataPtr = (int*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var z = dataPtr[2];
                                    var value = new IntVector3(x, y, z);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
                                }
                                break;
                            case SerializationType.IntVector4:
                                {
                                    var dataPtr = (int*)&data[valueOffset];
                                    var x = dataPtr[0];
                                    var y = dataPtr[1];
                                    var z = dataPtr[2];
                                    var w = dataPtr[3];
                                    var value = new IntVector4(x, y, z, w);
                                    obj = EmitHelper<TTarget>.CastFrom(value);
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
                        object value;

                        if (hasCustomTypeResolvers)
                        {
                            for (int i = 0; i < customTypeResolvers.Count; i++)
                            {
                                CustomTypeResolver customTypeResolver = customTypeResolvers[i];

                                if (customTypeResolver.typeHashCode == typeInfo.typeHashCode)
                                {
                                    value = customTypeResolver.Deserialize(data, offset);
                                    obj = (TTarget)value;
                                    return dataSize;
                                }
                            }
                        }

                        dataSize = DeserializeObjectFromData(out value, typeInfo, data, offset);
                        obj = (TTarget)value;
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported serialization type: " + typeInfo.serializationType);
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <param name="obj">Reference to target object.</param>
        /// <param name="typeInfo">Type info for target type.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returnsReturns the serialization size of object.</returns>
        private int DeserializeObjectFromData(out object obj, TinySerializerTypeInfo typeInfo, byte* data, int offset)
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
                            obj = customTypeResolver.Deserialize(data, offset);
                            return dataSize;
                        }
                    }
                }

                if (!typeInfo.isValueType)
                {
                    bool isNull;
                    dataSize = DeserializeNullPrefixFromData(out isNull, data, offset);

                    if (isNull)
                    {
                        obj = null;
                        return dataSize;
                    }
                }

                if (!typeInfo.emittedDefaultConstructor)
                    typeInfo.EmitDefaultConstructor();

                var constructorDelegate = (EmitUtil.DefaultConstructorDelegate<object>)typeInfo.defaultConstructor;
                obj = constructorDelegate.Invoke();

                if (!typeInfo.inspectedFields)
                    typeInfo.InspectFields();

                if (!typeInfo.emittedFieldSetters)
                    typeInfo.EmitFieldSetters();

                for (int i = 0; i < typeInfo.fieldTypeInfos.Length; i++)
                {
                    var fieldTypeInfo = typeInfo.fieldTypeInfos[i];

                    if (!settings.serializeAllFields)
                    {
                        if (!ShouldSerializeField(fieldTypeInfo))
                            continue;
                    }

                    SerializationType fieldSerializationType = fieldTypeInfo.fieldTypeInfo.serializationType;

                    int fieldDataSize = 0;
                    int fieldOffset = offset + dataSize;

                    switch (fieldSerializationType)
                    {
                        #region PRIMITIVE
                        case SerializationType.Byte:
                            {
                                fieldDataSize = SerializationTypeSizes.BYTE;

                                var dataPtr = &data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, byte>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.SByte:
                            {
                                fieldDataSize = SerializationTypeSizes.SBYTE;

                                var dataPtr = (sbyte*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, sbyte>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Bool:
                            {
                                fieldDataSize = SerializationTypeSizes.BOOL;

                                var dataPtr = (bool*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, bool>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Int16:
                            {
                                fieldDataSize = SerializationTypeSizes.INT16;

                                var dataPtr = (short*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, short>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Int32:
                            {
                                fieldDataSize = SerializationTypeSizes.INT32;

                                var dataPtr = (int*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, int>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Int64:
                            {
                                fieldDataSize = SerializationTypeSizes.INT64;

                                var dataPtr = (long*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, long>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.UInt16:
                            {
                                fieldDataSize = SerializationTypeSizes.UINT16;

                                var dataPtr = (ushort*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, ushort>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.UInt32:
                            {
                                fieldDataSize = SerializationTypeSizes.UINT32;

                                var dataPtr = (uint*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, uint>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.UInt64:
                            {
                                fieldDataSize = SerializationTypeSizes.UINT64;

                                var dataPtr = (ulong*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, ulong>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Half:
                            {
                                fieldDataSize = SerializationTypeSizes.HALF;

                                var dataPtr = (ushort*)&data[fieldOffset];
                                var value = new Half(*dataPtr);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Half>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Single:
                            {
                                fieldDataSize = SerializationTypeSizes.SINGLE;

                                var dataPtr = (float*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, float>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Double:
                            {
                                fieldDataSize = SerializationTypeSizes.DOUBLE;

                                var dataPtr = (double*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, double>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Char:
                            {
                                fieldDataSize = SerializationTypeSizes.CHAR;

                                var dataPtr = (char*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, char>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Decimal:
                            {
                                fieldDataSize = SerializationTypeSizes.DECIMAL;

                                var dataPtr = (decimal*)&data[fieldOffset];
                                var value = *dataPtr;

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, decimal>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.DateTime:
                            {
                                fieldDataSize = SerializationTypeSizes.DATE_TIME;

                                var dataPtr = (long*)&data[fieldOffset];
                                var value = new DateTime(*dataPtr);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, DateTime>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.DateTimeOffset:
                            {
                                fieldDataSize = SerializationTypeSizes.DATE_TIME_OFFSET;

                                var dataPtr = (long*)&data[fieldOffset];
                                var value = new DateTimeOffset(new DateTime(*dataPtr));

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, DateTimeOffset>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.TimeSpan:
                            {
                                fieldDataSize = SerializationTypeSizes.TIME_SPAN;

                                var dataPtr = (long*)&data[fieldOffset];
                                var value = new TimeSpan(*dataPtr);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, TimeSpan>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Vector2:
                            {
                                fieldDataSize = SerializationTypeSizes.VECTOR2;

                                var dataPtr = (float*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var value = new Vector2(x, y);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Vector2>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Vector3:
                            {
                                fieldDataSize = SerializationTypeSizes.VECTOR3;

                                var dataPtr = (float*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var z = dataPtr[2];
                                var value = new Vector3(x, y, z);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Vector3>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Vector4:
                            {
                                fieldDataSize = SerializationTypeSizes.VECTOR4;

                                var dataPtr = (float*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var z = dataPtr[2];
                                var w = dataPtr[3];
                                var value = new Vector4(x, y, z, w);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Vector4>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Quaternion:
                            {
                                fieldDataSize = SerializationTypeSizes.QUATERNION;

                                var dataPtr = (float*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var z = dataPtr[2];
                                var w = dataPtr[3];
                                var value = new Quaternion(x, y, z, w);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Quaternion>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Rect:
                            {
                                fieldDataSize = SerializationTypeSizes.RECT;

                                var dataPtr = (float*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var width = dataPtr[2];
                                var height = dataPtr[3];
                                var value = new Rect(x, y, width, height);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Rect>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Bounds:
                            {
                                fieldDataSize = SerializationTypeSizes.BOUNDS;

                                var dataPtr = (float*)&data[fieldOffset];
                                var centerX = dataPtr[0];
                                var centerY = dataPtr[1];
                                var centerZ = dataPtr[2];
                                var sizeX = dataPtr[3];
                                var sizeY = dataPtr[4];
                                var sizeZ = dataPtr[5];
                                var center = new Vector3(centerX, centerY, centerZ);
                                var size = new Vector3(sizeX, sizeY, sizeZ);
                                var value = new Bounds(center, size);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, Bounds>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.IntVector2:
                            {
                                fieldDataSize = SerializationTypeSizes.INT_VECTOR2;

                                var dataPtr = (int*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var value = new IntVector2(x, y);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, IntVector2>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.IntVector3:
                            {
                                fieldDataSize = SerializationTypeSizes.INT_VECTOR3;

                                var dataPtr = (int*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var z = dataPtr[2];
                                var value = new IntVector3(x, y, z);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, IntVector3>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.IntVector4:
                            {
                                fieldDataSize = SerializationTypeSizes.INT_VECTOR4;

                                var dataPtr = (int*)&data[fieldOffset];
                                var x = dataPtr[0];
                                var y = dataPtr[1];
                                var z = dataPtr[2];
                                var w = dataPtr[3];
                                var value = new IntVector4(x, y, z, w);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, IntVector4>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        #endregion
                        case SerializationType.String:
                            {
                                string value;
                                fieldDataSize = DeserializeStringFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, string>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.PrimitiveArray:
                            {
                                Array value;
                                fieldDataSize = DeserializePrimitiveArrayFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.PrimitiveList:
                            {
                                IList value;
                                fieldDataSize = DeserializePrimitiveListFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.PrimitiveNullable:
                            {
                                object value;
                                fieldDataSize = DeserializePrimitiveNullableFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.ObjectArray:
                            {
                                Array value;
                                fieldDataSize = DeserializeObjectArrayFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.ObjectList:
                            {
                                IList value;
                                fieldDataSize = DeserializeObjectListFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.ObjectNullable:
                            {
                                bool isNull;
                                fieldDataSize = DeserializeNullPrefixFromData(out isNull, data, fieldOffset);

                                object value;

                                if (isNull)
                                {
                                    value = null;
                                }
                                else
                                {
                                    int valueOffset = fieldDataSize + fieldOffset;
                                    fieldDataSize = DeserializeObjectFromData(out value, fieldTypeInfo.fieldTypeInfo.elementTypeInfo, data, valueOffset);
                                }

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
                            }
                            break;
                        case SerializationType.Object:
                            {
                                object value;
                                fieldDataSize = DeserializeObjectFromData(out value, fieldTypeInfo.fieldTypeInfo, data, fieldOffset);

                                var setterDelegate = (EmitUtil.FieldSetterDelegate<object, object>)fieldTypeInfo.setter;
                                setterDelegate.Invoke(obj, value);
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
                            string value;
                            dataSize = DeserializeStringFromData(out value, typeInfo, data, offset);
                            obj = value;
                        }
                        break;
                    case SerializationType.PrimitiveArray:
                        {
                            Array value;
                            dataSize = DeserializePrimitiveArrayFromData(out value, typeInfo, data, offset);
                            obj = value;
                        }
                        break;
                    case SerializationType.PrimitiveList:
                        {
                            IList value;
                            dataSize = DeserializePrimitiveListFromData(out value, typeInfo, data, offset);
                            obj = value;
                        }
                        break;
                    case SerializationType.PrimitiveNullable:
                        {
                            dataSize = DeserializePrimitiveNullableFromData(out obj, typeInfo, data, offset);
                        }
                        break;
                    case SerializationType.ObjectArray:
                        {
                            Array value;
                            dataSize = DeserializeObjectArrayFromData(out value, typeInfo, data, offset);
                            obj = value;
                        }
                        break;
                    case SerializationType.ObjectList:
                        {
                            IList value;
                            dataSize = DeserializeObjectListFromData(out value, typeInfo, data, offset);
                            obj = value;
                        }
                        break;
                    case SerializationType.ObjectNullable:
                        {
                            bool isNull;
                            dataSize = DeserializeNullPrefixFromData(out isNull, data, offset);
                            int valueOffset = offset + dataSize;

                            if (!isNull)
                                dataSize += DeserializeObjectFromData(out obj, typeInfo.elementTypeInfo, data, valueOffset);
                            else
                                obj = null;
                        }
                        break;
                    default:
                        throw new UnexpectedException("Unexpected serialization type: " + typeInfo.serializationType);
                }
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes a string from a byte array.
        /// </summary>
        /// <param name="value">Reference to target string.</param>
        /// <param name="typeInfo">String type info.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of string.</returns>
        private int DeserializeStringFromData(out string value, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            bool isNull;
            int strLength;

            int dataSize = DeserializeNullLengthPrefixFromData(out isNull, out strLength, data, offset);

            if (isNull)
            {
                value = null;
                return dataSize;
            }

            int strOffset = offset + dataSize;

            Encoding systemEncoding;

            switch (settings.defaultStringEncodeType)
            {
                case StringEncodeType.Char:
                    {
                        dataSize += strLength * SerializationTypeSizes.CHAR;
                        var valuePtr = stackalloc char[strLength + 1];
                        var dataPtr = (char*)&data[strOffset];

                        for (int i = 0; i < strLength; i++)
                        {
                            valuePtr[i] = dataPtr[i];
                        }

                        value = new string(valuePtr);
                    }
                    break;
                case StringEncodeType.Byte:
                    {
                        dataSize += strLength * SerializationTypeSizes.BYTE;
                        var valuePtr = stackalloc char[strLength + 1];
                        var dataPtr = &data[strOffset];

                        for (int i = 0; i < strLength; i++)
                        {
                            valuePtr[i] = (char)dataPtr[i];
                        }

                        value = new string(valuePtr);
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
                        int byteCount;
                        int byteCountPrefixSize = DeserializeNullLengthPrefixFromData(out isNull, out byteCount, data, strOffset);

                        strOffset += byteCountPrefixSize;
                        dataSize += byteCountPrefixSize + byteCount;

                        var dataPtr = &data[strOffset];

                        value = systemEncoding.GetString(dataPtr, byteCount);
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported string encode type: " + settings.defaultStringEncodeType);
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes an object array from a byte array.
        /// </summary>
        /// <param name="array">Reference to target object array.</param>
        /// <param name="typeInfo">Object array type info.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of object array.</returns>
        private int DeserializeObjectArrayFromData(out Array array, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            bool isNull;
            int arrLength;
            int dataSize = DeserializeNullLengthPrefixFromData(out isNull, out arrLength, data, offset);

            if (isNull)
            {
                array = null;
                return dataSize;
            }

            int arrOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

            if (!elementTypeInfo.emittedArrayConstructor)
                elementTypeInfo.EmitArrayConstructor();
            var arrayConstructorDelegate = (EmitUtil.CreateNewArrayDelegate)elementTypeInfo.arrayConstructor;
            array = arrayConstructorDelegate.Invoke(arrLength);

            var arrayAccessInfo = typeInfo.arrayAccessInfo;
            if (!arrayAccessInfo.emittedSetterDelegate)
                arrayAccessInfo.EmitSetterDelegate();
            var arraySetterDelegate = (EmitUtil.ArrayValueSetterDelegate<object>)arrayAccessInfo.setter;

            for (int i = 0; i < arrLength; i++)
            {
                object element;
                int elementDataSize = DeserializeObjectFromData(out element, elementTypeInfo, data, arrOffset);
                arraySetterDelegate.Invoke(array, i, element);
                arrOffset += elementDataSize;
                dataSize += elementDataSize;
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes an object list from a byte array.
        /// </summary>
        /// <param name="list">Reference to target object list.</param>
        /// <param name="typeInfo">Object list type info.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of object list.</returns>
        private int DeserializeObjectListFromData(out IList list, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            bool isNull;
            int listLength;
            int dataSize = DeserializeNullLengthPrefixFromData(out isNull, out listLength, data, offset);

            if (isNull)
            {
                list = null;
                return dataSize;
            }

            int listOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;

            if (!elementTypeInfo.emittedListConstructor)
                elementTypeInfo.EmitListConstructor();
            var listConstructorDelegate = (EmitUtil.CreateNewListDelegate)elementTypeInfo.listConstructor;
            list = listConstructorDelegate.Invoke(listLength);

            for (int i = 0; i < listLength; i++)
            {
                object element;
                int elementDataSize = DeserializeObjectFromData(out element, elementTypeInfo, data, listOffset);
                list.Add(element);
                listOffset += elementDataSize;
                dataSize += elementDataSize;
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes a primitive array from a byte array.
        /// </summary>
        /// <param name="array">Reference to target primitive array.</param>
        /// <param name="typeInfo">Primitive array type info.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of primitive array.</returns>
        private int DeserializePrimitiveArrayFromData(out Array array, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            bool isNull;
            int arrLength;

            int dataSize = DeserializeNullLengthPrefixFromData(out isNull, out arrLength, data, offset);

            if (isNull)
            {
                array = null;
                return dataSize;
            }

            int arrOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
            dataSize += arrLength * elementTypeInfo.primitiveSize;

            switch (elementTypeInfo.serializationType)
            {
                case SerializationType.Byte:
                    {
                        var value = new byte[arrLength];
                        var dataPtr = &data[arrOffset];
                        array = value;

                        fixed (byte* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.SByte:
                    {
                        var value = new sbyte[arrLength];
                        var dataPtr = (sbyte*)&data[arrOffset];
                        array = value;

                        fixed (sbyte* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Bool:
                    {
                        var value = new bool[arrLength];
                        var dataPtr = (bool*)&data[arrOffset];
                        array = value;

                        fixed (bool* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Int16:
                    {
                        var value = new short[arrLength];
                        var dataPtr = (short*)&data[arrOffset];
                        array = value;

                        fixed (short* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Int32:
                    {
                        var value = new int[arrLength];
                        var dataPtr = (int*)&data[arrOffset];
                        array = value;

                        fixed (int* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Int64:
                    {
                        var value = new long[arrLength];
                        var dataPtr = (long*)&data[arrOffset];
                        array = value;

                        fixed (long* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.UInt16:
                    {
                        var value = new ushort[arrLength];
                        var dataPtr = (ushort*)&data[arrOffset];
                        array = value;

                        fixed (ushort* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.UInt32:
                    {
                        var value = new uint[arrLength];
                        var dataPtr = (uint*)&data[arrOffset];
                        array = value;

                        fixed (uint* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.UInt64:
                    {
                        var value = new ulong[arrLength];
                        var dataPtr = (ulong*)&data[arrOffset];
                        array = value;

                        fixed (ulong* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Half:
                    {
                        var value = new Half[arrLength];
                        var dataPtr = (ushort*)&data[arrOffset];
                        array = value;

                        fixed (Half* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = new Half(dataPtr[i]);
                            }
                        }
                    }
                    break;
                case SerializationType.Single:
                    {
                        var value = new float[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (float* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Double:
                    {
                        var value = new double[arrLength];
                        var dataPtr = (double*)&data[arrOffset];
                        array = value;

                        fixed (double* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Char:
                    {
                        var value = new char[arrLength];
                        var dataPtr = (char*)&data[arrOffset];
                        array = value;

                        fixed (char* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.Decimal:
                    {
                        var value = new decimal[arrLength];
                        var dataPtr = (decimal*)&data[arrOffset];
                        array = value;

                        fixed (decimal* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                valuePtr[i] = dataPtr[i];
                            }
                        }
                    }
                    break;
                case SerializationType.DateTime:
                    {
                        var value = new DateTime[arrLength];
                        var dataPtr = (long*)&data[arrOffset];
                        array = value;

                        fixed (DateTime* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var dataValue = dataPtr[i];
                                valuePtr[i] = new DateTime(dataValue);
                            }
                        }
                    }
                    break;
                case SerializationType.DateTimeOffset:
                    {
                        var value = new DateTimeOffset[arrLength];
                        var dataPtr = (long*)&data[arrOffset];
                        array = value;

                        fixed (DateTimeOffset* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var dataValue = new DateTime(dataPtr[i]);
                                valuePtr[i] = new DateTimeOffset(dataValue);
                            }
                        }
                    }
                    break;
                case SerializationType.TimeSpan:
                    {
                        var value = new TimeSpan[arrLength];
                        var dataPtr = (long*)&data[arrOffset];
                        array = value;

                        fixed (TimeSpan* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var dataValue = dataPtr[i];
                                valuePtr[i] = new TimeSpan(dataValue);
                            }
                        }
                    }
                    break;
                case SerializationType.Vector2:
                    {
                        var value = new Vector2[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (Vector2* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 2) + 0];
                                var y = dataPtr[(i * 2) + 1];
                                valuePtr[i] = new Vector2(x, y);
                            }
                        }
                    }
                    break;
                case SerializationType.Vector3:
                    {
                        var value = new Vector3[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (Vector3* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 3) + 0];
                                var y = dataPtr[(i * 3) + 1];
                                var z = dataPtr[(i * 3) + 2];
                                valuePtr[i] = new Vector3(x, y, z);
                            }
                        }
                    }
                    break;
                case SerializationType.Vector4:
                    {
                        var value = new Vector4[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (Vector4* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 4) + 0];
                                var y = dataPtr[(i * 4) + 1];
                                var z = dataPtr[(i * 4) + 2];
                                var w = dataPtr[(i * 4) + 3];
                                valuePtr[i] = new Vector4(x, y, z, w);
                            }
                        }
                    }
                    break;
                case SerializationType.Quaternion:
                    {
                        var value = new Quaternion[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (Quaternion* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 4) + 0];
                                var y = dataPtr[(i * 4) + 1];
                                var z = dataPtr[(i * 4) + 2];
                                var w = dataPtr[(i * 4) + 3];
                                valuePtr[i] = new Quaternion(x, y, z, w);
                            }
                        }
                    }
                    break;
                case SerializationType.Rect:
                    {
                        var value = new Rect[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (Rect* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 4) + 0];
                                var y = dataPtr[(i * 4) + 1];
                                var width = dataPtr[(i * 4) + 2];
                                var height = dataPtr[(i * 4) + 3];
                                valuePtr[i] = new Rect(x, y, width, height);
                            }
                        }
                    }
                    break;
                case SerializationType.Bounds:
                    {
                        var value = new Bounds[arrLength];
                        var dataPtr = (float*)&data[arrOffset];
                        array = value;

                        fixed (Bounds* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var centerX = dataPtr[(i * 6) + 0];
                                var centerY = dataPtr[(i * 6) + 1];
                                var centerZ = dataPtr[(i * 6) + 2];
                                var sizeX = dataPtr[(i * 6) + 3];
                                var sizeY = dataPtr[(i * 6) + 4];
                                var sizeZ = dataPtr[(i * 6) + 5];
                                var center = new Vector3(centerX, centerY, centerZ);
                                var size = new Vector3(sizeX, sizeY, sizeZ);
                                valuePtr[i] = new Bounds(center, size);
                            }
                        }
                    }
                    break;
                case SerializationType.IntVector2:
                    {
                        var value = new IntVector2[arrLength];
                        var dataPtr = (int*)&data[arrOffset];
                        array = value;

                        fixed (IntVector2* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 2) + 0];
                                var y = dataPtr[(i * 2) + 1];
                                valuePtr[i] = new IntVector2(x, y);
                            }
                        }
                    }
                    break;
                case SerializationType.IntVector3:
                    {
                        var value = new IntVector3[arrLength];
                        var dataPtr = (int*)&data[arrOffset];
                        array = value;

                        fixed (IntVector3* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 3) + 0];
                                var y = dataPtr[(i * 3) + 1];
                                var z = dataPtr[(i * 3) + 2];
                                valuePtr[i] = new IntVector3(x, y, z);
                            }
                        }
                    }
                    break;
                case SerializationType.IntVector4:
                    {
                        var value = new IntVector4[arrLength];
                        var dataPtr = (int*)&data[arrOffset];
                        array = value;

                        fixed (IntVector4* valuePtr = value)
                        {
                            for (int i = 0; i < arrLength; i++)
                            {
                                var x = dataPtr[(i * 4) + 0];
                                var y = dataPtr[(i * 4) + 1];
                                var z = dataPtr[(i * 4) + 2];
                                var w = dataPtr[(i * 4) + 3];
                                valuePtr[i] = new IntVector4(x, y, z, w);
                            }
                        }
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported primitive serialization type: " + elementTypeInfo.serializationType);
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes a primitive list from a byte array.
        /// </summary>
        /// <param name="list">Reference to target primitive list.</param>
        /// <param name="typeInfo">Primitive list type info.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of primitive list.</returns>
        private int DeserializePrimitiveListFromData(out IList list, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            bool isNull;
            int listLength;

            int dataSize = DeserializeNullLengthPrefixFromData(out isNull, out listLength, data, offset);

            if (isNull)
            {
                list = null;
                return dataSize;
            }

            int listOffset = offset + dataSize;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
            dataSize += listLength * elementTypeInfo.primitiveSize;

            switch (elementTypeInfo.serializationType)
            {
                case SerializationType.Byte:
                    {
                        var value = new List<byte>(listLength);
                        var dataPtr = &data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.SByte:
                    {
                        var value = new List<sbyte>(listLength);
                        var dataPtr = (sbyte*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Bool:
                    {
                        var value = new List<bool>(listLength);
                        var dataPtr = (bool*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Int16:
                    {
                        var value = new List<short>(listLength);
                        var dataPtr = (short*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Int32:
                    {
                        var value = new List<int>(listLength);
                        var dataPtr = (int*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Int64:
                    {
                        var value = new List<long>(listLength);
                        var dataPtr = (long*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.UInt16:
                    {
                        var value = new List<ushort>(listLength);
                        var dataPtr = (ushort*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.UInt32:
                    {
                        var value = new List<uint>(listLength);
                        var dataPtr = (uint*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.UInt64:
                    {
                        var value = new List<ulong>(listLength);
                        var dataPtr = (ulong*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Half:
                    {
                        var value = new List<Half>(listLength);
                        var dataPtr = (ushort*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var dataValue = new Half(dataPtr[i]);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Single:
                    {
                        var value = new List<float>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Double:
                    {
                        var value = new List<double>(listLength);
                        var dataPtr = (double*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Char:
                    {
                        var value = new List<char>(listLength);
                        var dataPtr = (char*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.Decimal:
                    {
                        var value = new List<decimal>(listLength);
                        var dataPtr = (decimal*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            value.Add(dataPtr[i]);
                        }
                    }
                    break;
                case SerializationType.DateTime:
                    {
                        var value = new List<DateTime>(listLength);
                        var dataPtr = (long*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var dataValue = new DateTime(dataPtr[i]);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.DateTimeOffset:
                    {
                        var value = new List<DateTimeOffset>(listLength);
                        var dataPtr = (long*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var dataValue = new DateTimeOffset(new DateTime(dataPtr[i]));
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.TimeSpan:
                    {
                        var value = new List<TimeSpan>(listLength);
                        var dataPtr = (long*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var dataValue = new TimeSpan(dataPtr[i]);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Vector2:
                    {
                        var value = new List<Vector2>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 2) + 0];
                            var y = dataPtr[(i * 2) + 1];
                            var dataValue = new Vector2(x, y);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Vector3:
                    {
                        var value = new List<Vector3>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 3) + 0];
                            var y = dataPtr[(i * 3) + 1];
                            var z = dataPtr[(i * 3) + 2];
                            var dataValue = new Vector3(x, y, z);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Vector4:
                    {
                        var value = new List<Vector4>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 4) + 0];
                            var y = dataPtr[(i * 4) + 1];
                            var z = dataPtr[(i * 4) + 2];
                            var w = dataPtr[(i * 4) + 3];
                            var dataValue = new Vector4(x, y, z, w);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Quaternion:
                    {
                        var value = new List<Quaternion>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 4) + 0];
                            var y = dataPtr[(i * 4) + 1];
                            var z = dataPtr[(i * 4) + 2];
                            var w = dataPtr[(i * 4) + 3];
                            var dataValue = new Quaternion(x, y, z, w);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Rect:
                    {
                        var value = new List<Rect>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 4) + 0];
                            var y = dataPtr[(i * 4) + 1];
                            var width = dataPtr[(i * 4) + 2];
                            var height = dataPtr[(i * 4) + 3];
                            var dataValue = new Rect(x, y, width, height);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.Bounds:
                    {
                        var value = new List<Bounds>(listLength);
                        var dataPtr = (float*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var centerX = dataPtr[(i * 6) + 0];
                            var centerY = dataPtr[(i * 6) + 1];
                            var centerZ = dataPtr[(i * 6) + 2];
                            var sizeX = dataPtr[(i * 6) + 3];
                            var sizeY = dataPtr[(i * 6) + 4];
                            var sizeZ = dataPtr[(i * 6) + 5];
                            var center = new Vector3(centerX, centerY, centerZ);
                            var size = new Vector3(sizeX, sizeY, sizeZ);
                            var dataValue = new Bounds(center, size);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.IntVector2:
                    {
                        var value = new List<IntVector2>(listLength);
                        var dataPtr = (int*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 2) + 0];
                            var y = dataPtr[(i * 2) + 1];
                            var dataValue = new IntVector2(x, y);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.IntVector3:
                    {
                        var value = new List<IntVector3>(listLength);
                        var dataPtr = (int*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 3) + 0];
                            var y = dataPtr[(i * 3) + 1];
                            var z = dataPtr[(i * 3) + 2];
                            var dataValue = new IntVector3(x, y, z);
                            value.Add(dataValue);
                        }
                    }
                    break;
                case SerializationType.IntVector4:
                    {
                        var value = new List<IntVector4>(listLength);
                        var dataPtr = (int*)&data[listOffset];
                        list = value;

                        for (int i = 0; i < listLength; i++)
                        {
                            var x = dataPtr[(i * 4) + 0];
                            var y = dataPtr[(i * 4) + 1];
                            var z = dataPtr[(i * 4) + 2];
                            var w = dataPtr[(i * 4) + 3];
                            var dataValue = new IntVector4(x, y, z, w);
                            value.Add(dataValue);
                        }
                    }
                    break;
                default:
                    throw new UnsupportedException("Unsupported primitive serialization type: " + elementTypeInfo.serializationType);
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes a primitive nullable from a byte aray.
        /// </summary>
        /// <param name="obj">Reference to target primitive nullable.</param>
        /// <param name="typeInfo">Primitive nullable type info.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of primitive nullable.</returns>
        private int DeserializePrimitiveNullableFromData(out object obj, TinySerializerTypeInfo typeInfo, byte* data, int offset)
        {
            bool isNull;
            int dataSize = DeserializeNullPrefixFromData(out isNull, data, offset);

            if (isNull)
            {
                obj = null;
                return dataSize;
            }

            int valueOffset = dataSize + offset;
            TinySerializerTypeInfo elementTypeInfo = typeInfo.elementTypeInfo;
            dataSize += elementTypeInfo.primitiveSize;

            switch (elementTypeInfo.serializationType)
            {
                case SerializationType.Byte:
                    {
                        var dataPtr = &data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.SByte:
                    {
                        var dataPtr = (sbyte*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Bool:
                    {
                        var dataPtr = (bool*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Int16:
                    {
                        var dataPtr = (short*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Int32:
                    {
                        var dataPtr = (int*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Int64:
                    {
                        var dataPtr = (long*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.UInt16:
                    {
                        var dataPtr = (ushort*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.UInt32:
                    {
                        var dataPtr = (uint*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.UInt64:
                    {
                        var dataPtr = (ulong*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Half:
                    {
                        var dataPtr = (ushort*)&data[valueOffset];
                        obj = new Half(*dataPtr);
                    }
                    break;
                case SerializationType.Single:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Double:
                    {
                        var dataPtr = (double*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Char:
                    {
                        var dataPtr = (char*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.Decimal:
                    {
                        var dataPtr = (decimal*)&data[valueOffset];
                        obj = *dataPtr;
                    }
                    break;
                case SerializationType.DateTime:
                    {
                        var dataPtr = (long*)&data[valueOffset];
                        obj = new DateTime(*dataPtr);
                    }
                    break;
                case SerializationType.DateTimeOffset:
                    {
                        var dataPtr = (long*)&data[valueOffset];
                        obj = new DateTimeOffset(new DateTime(*dataPtr));
                    }
                    break;
                case SerializationType.TimeSpan:
                    {
                        var dataPtr = (long*)&data[valueOffset];
                        obj = new TimeSpan(*dataPtr);
                    }
                    break;
                case SerializationType.Vector2:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        obj = new Vector2(x, y);
                    }
                    break;
                case SerializationType.Vector3:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        obj = new Vector3(x, y, z);
                    }
                    break;
                case SerializationType.Vector4:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var w = dataPtr[3];
                        obj = new Vector4(x, y, z, w);
                    }
                    break;
                case SerializationType.Quaternion:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var w = dataPtr[3];
                        obj = new Quaternion(x, y, z, w);
                    }
                    break;
                case SerializationType.Rect:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var width = dataPtr[2];
                        var height = dataPtr[3];
                        obj = new Rect(x, y, width, height);
                    }
                    break;
                case SerializationType.Bounds:
                    {
                        var dataPtr = (float*)&data[valueOffset];
                        var centerX = dataPtr[0];
                        var centerY = dataPtr[1];
                        var centerZ = dataPtr[2];
                        var sizeX = dataPtr[3];
                        var sizeY = dataPtr[4];
                        var sizeZ = dataPtr[5];
                        var center = new Vector3(centerX, centerY, centerZ);
                        var size = new Vector3(sizeX, sizeY, sizeZ);
                        obj = new Bounds(center, size);
                    }
                    break;
                case SerializationType.IntVector2:
                    {
                        var dataPtr = (int*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        obj = new IntVector2(x, y);
                    }
                    break;
                case SerializationType.IntVector3:
                    {
                        var dataPtr = (int*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        obj = new IntVector3(x, y, z);
                    }
                    break;
                case SerializationType.IntVector4:
                    {
                        var dataPtr = (int*)&data[valueOffset];
                        var x = dataPtr[0];
                        var y = dataPtr[1];
                        var z = dataPtr[2];
                        var w = dataPtr[3];
                        obj = new IntVector4(x, y, z, w);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Serialiation type is not primitive: " + elementTypeInfo.serializationType);
            }

            return dataSize;
        }

        /// <summary>
        /// Deserializes a null prefix value from a byte array
        /// </summary>
        /// <param name="isNull">Is true if object is null.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of the null prefix.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int DeserializeNullPrefixFromData(out bool isNull, byte* data, int offset)
        {
            var dataPtr = (sbyte*)&data[offset];
            var prefixValue = *dataPtr;

            isNull = prefixValue == TinySerializerConstants.NULL_VALUE;

            return SerializationTypeSizes.SBYTE;
        }

        /// <summary>
        /// Deserializes a null-length prefix value from a byte array.
        /// </summary>
        /// <param name="isNull">Is true if object is null.</param>
        /// <param name="length">Length prefix value.</param>
        /// <param name="data">Serialization data to deserialize the object from.</param>
        /// <param name="offset">Offset in the serialization data.</param>
        /// <returns>Returns the serialization size of the null-length prefix.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int DeserializeNullLengthPrefixFromData(out bool isNull, out int length, byte* data, int offset)
        {
            int dataSize = SerializationTypeSizes.SBYTE;

            var dataPtr = (sbyte*)&data[offset];
            var prefixValue = *dataPtr;

            isNull = false;

            switch (prefixValue)
            {
                case TinySerializerConstants.NULL_VALUE:
                    isNull = true;
                    length = -1;
                    break;
                case TinySerializerConstants.USHORT_LENGTH:
                    {
                        dataSize += SerializationTypeSizes.UINT16;
                        var lengthPtr = (ushort*)&dataPtr[1];
                        length = *lengthPtr;
                    }
                    break;
                case TinySerializerConstants.UINT_LENGTH:
                    {
                        dataSize += SerializationTypeSizes.UINT32;
                        var lengthPtr = (uint*)&dataPtr[1];
                        length = unchecked((int)*lengthPtr);
                    }
                    break;
                default:
                    length = prefixValue;
                    break;
            }

            return dataSize;
        }

        #endregion
    }
}
#endif
