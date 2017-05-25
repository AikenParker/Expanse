using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace Expanse.TinySerialization
{
    /// <summary>
    /// Very fast and low GC Alloc serializer but very limited.
    /// </summary>
    public class TinySerializer : IByteSerializer
    {
        private SerializationInfo serializationInfo;

        private List<TypeCacheInfo> typeCacheInfoList;
        private List<FieldCacheInfo> fieldCacheInfoList;
        private TypeCacheInfo lastTypeCacheInfo;

        private BasicPrimitiveTypeResolver basicPrimitiveTypeResolver;
        private DateTimeTypeResolver dateTimeTypeResolver;
        private DecimalTypeResolver decimalTypeResolver;
        private StringTypeResolver stringTypeResolver;

        public TinySerializer() : this(SerializationInfo.Default) { }

        public TinySerializer(SerializationInfo serializationInfo)
        {
            this.serializationInfo = serializationInfo;

            basicPrimitiveTypeResolver = new BasicPrimitiveTypeResolver();
            dateTimeTypeResolver = new DateTimeTypeResolver();
            decimalTypeResolver = new DecimalTypeResolver(serializationInfo.DecimalResolutionType);
            stringTypeResolver = new StringTypeResolver(serializationInfo.StringResolutionType);
        }

        public void ClearCache()
        {
            lastTypeCacheInfo = null;

            if (typeCacheInfoList != null)
            {
                typeCacheInfoList.Clear();
            }
        }

        public T Deserialize<T>(byte[] byteData) where T : new()
        {
            TypeCacheInfo typeCacheInfo = GetTypeCacheInfo(typeof(T));
            T obj = typeCacheInfo.GetDefaultConstructedInstance<T>();
            FieldCacheInfo[] fieldCacheInfoList = typeCacheInfo.fields;
            int fieldLength = fieldCacheInfoList.Length;

            if (fieldLength == 0)
                return obj;

            int dataLength = byteData.Length;
            int position = 0;

            for (int i = 0; i < fieldLength; i++)
            {
                FieldCacheInfo fci = fieldCacheInfoList[i];

                // --SUPPORTED-TYPE-SWITCH--
                switch (fci.type)
                {
                    case SupportedFieldType.Int:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetInt(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Bool:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetBool(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Float:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetFloat(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Double:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetDouble(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Char:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetChar(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.String:
                        fci.SetValue(obj, stringTypeResolver.GetString(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.DateTime:
                        fci.SetValue(obj, dateTimeTypeResolver.GetDateTime(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Short:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetShort(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Long:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetLong(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.UInt:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetUint(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.UShort:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetUshort(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.ULong:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetUlong(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Byte:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetByte(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.SByte:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetSbyte(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.Decimal:
                        fci.SetValue(obj, decimalTypeResolver.GetDecimal(ref byteData, ref position));
                        continue;
                }
            }

            return obj;
        }

        public byte[] Serialize<T>(T obj)
        {
            TypeCacheInfo typeCacheInfo = GetTypeCacheInfo(typeof(T));
            FieldCacheInfo[] fieldCacheInfoList = typeCacheInfo.fields;
            int fieldLength = fieldCacheInfoList.Length;

            if (fieldLength == 0)
                return new byte[0];

            int byteLength = typeCacheInfo.CalculateSize(obj);
            byte[] byteData = new byte[byteLength];
            int position = 0;

            for (int i = 0; i < fieldLength; i++)
            {
                FieldCacheInfo fci = fieldCacheInfoList[i];

                // --SUPPORTED-TYPE-SWITCH--
                switch (fci.type)
                {
                    case SupportedFieldType.Int:
                        basicPrimitiveTypeResolver.SetInt(ref byteData, ref position, fci.GetValue<T, int>(obj));
                        continue;
                    case SupportedFieldType.Bool:
                        basicPrimitiveTypeResolver.SetBool(ref byteData, ref position, fci.GetValue<T, bool>(obj));
                        continue;
                    case SupportedFieldType.Float:
                        basicPrimitiveTypeResolver.SetFloat(ref byteData, ref position, fci.GetValue<T, float>(obj));
                        continue;
                    case SupportedFieldType.Double:
                        basicPrimitiveTypeResolver.SetDouble(ref byteData, ref position, fci.GetValue<T, double>(obj));
                        continue;
                    case SupportedFieldType.Char:
                        basicPrimitiveTypeResolver.SetChar(ref byteData, ref position, fci.GetValue<T, char>(obj));
                        continue;
                    case SupportedFieldType.String:
                        stringTypeResolver.SetString(ref byteData, ref position, fci.GetValue<T, string>(obj));
                        continue;
                    case SupportedFieldType.DateTime:
                        dateTimeTypeResolver.SetDateTime(ref byteData, ref position, fci.GetValue<T, DateTime>(obj));
                        continue;
                    case SupportedFieldType.Short:
                        basicPrimitiveTypeResolver.SetShort(ref byteData, ref position, fci.GetValue<T, short>(obj));
                        continue;
                    case SupportedFieldType.Long:
                        basicPrimitiveTypeResolver.SetLong(ref byteData, ref position, fci.GetValue<T, long>(obj));
                        continue;
                    case SupportedFieldType.UInt:
                        basicPrimitiveTypeResolver.SetUint(ref byteData, ref position, fci.GetValue<T, uint>(obj));
                        continue;
                    case SupportedFieldType.UShort:
                        basicPrimitiveTypeResolver.SetUshort(ref byteData, ref position, fci.GetValue<T, ushort>(obj));
                        continue;
                    case SupportedFieldType.ULong:
                        basicPrimitiveTypeResolver.SetUlong(ref byteData, ref position, fci.GetValue<T, ulong>(obj));
                        continue;
                    case SupportedFieldType.Byte:
                        basicPrimitiveTypeResolver.SetByte(ref byteData, ref position, fci.GetValue<T, byte>(obj));
                        continue;
                    case SupportedFieldType.SByte:
                        basicPrimitiveTypeResolver.SetSbyte(ref byteData, ref position, fci.GetValue<T, sbyte>(obj));
                        continue;
                    case SupportedFieldType.Decimal:
                        decimalTypeResolver.SetDecimal(ref byteData, ref position, fci.GetValue<T, decimal>(obj));
                        continue;
                }
            }

            return byteData;
        }

        private TypeCacheInfo GetTypeCacheInfo(Type type)
        {
            if (lastTypeCacheInfo != null && type == lastTypeCacheInfo.type)
                return lastTypeCacheInfo;

            if (typeCacheInfoList == null)
                typeCacheInfoList = new List<TypeCacheInfo>();

            TypeCacheInfo typeCacheInfo = null;

            for (int i = 0; i < typeCacheInfoList.Count; i++)
            {
                TypeCacheInfo tci = typeCacheInfoList[i];
                if (tci.type == type)
                {
                    typeCacheInfo = tci;
                    break;
                }
            }

            if (typeCacheInfo == null)
            {
                FieldInfo[] allFieldInfo = type.GetFields(serializationInfo.BindingFlags);
                int fieldInfoCount = allFieldInfo.Length;

                fieldCacheInfoList = fieldCacheInfoList ?? new List<FieldCacheInfo>(fieldInfoCount);
                fieldCacheInfoList.Capacity = fieldInfoCount;

                int fieldListCacheCount = fieldCacheInfoList.Count;
                int supportedFieldListCount = 0;

                for (int i = 0; i < fieldInfoCount; i++)
                {
                    FieldInfo fieldInfo = allFieldInfo[i];
                    SupportedFieldType fieldType = FieldCacheInfo.GetFieldType(fieldInfo);

                    if (fieldType != SupportedFieldType.None)
                    {
                        supportedFieldListCount++;
                        FieldCacheInfo field = new FieldCacheInfo(this, fieldInfo, fieldType);

                        if (i < fieldListCacheCount)
                            fieldCacheInfoList[i] = field;
                        else
                            fieldCacheInfoList.Add(field);
                    }
                }

                FieldCacheInfo[] fields = new FieldCacheInfo[supportedFieldListCount];
                for (int i = 0; i < fields.Length; i++)
                {
                    fields[i] = fieldCacheInfoList[i];
                }

                typeCacheInfo = new TypeCacheInfo(this, type, fields);

                typeCacheInfoList.Add(typeCacheInfo);
            }

            lastTypeCacheInfo = typeCacheInfo;
            return typeCacheInfo;
        }

        public SerializationInfo SerializationInfo
        {
            get { return serializationInfo; }
        }
    }
}
