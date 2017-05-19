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
                    case SupportedFieldType.INT:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetInt(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.BOOL:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetBool(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.FLOAT:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetFloat(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.DOUBLE:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetDouble(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.CHAR:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetChar(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.STRING:
                        fci.SetValue(obj, stringTypeResolver.GetString(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.DATE_TIME:
                        fci.SetValue(obj, dateTimeTypeResolver.GetDateTime(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.SHORT:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetShort(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.LONG:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetLong(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.UINT:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetUint(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.USHORT:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetUshort(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.ULONG:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetUlong(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.BYTE:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetByte(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.SBYTE:
                        fci.SetValue(obj, basicPrimitiveTypeResolver.GetSbyte(ref byteData, ref position));
                        continue;
                    case SupportedFieldType.DECIMAL:
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
                    case SupportedFieldType.INT:
                        basicPrimitiveTypeResolver.SetInt(ref byteData, ref position, fci.GetValue<T, int>(obj));
                        continue;
                    case SupportedFieldType.BOOL:
                        basicPrimitiveTypeResolver.SetBool(ref byteData, ref position, fci.GetValue<T, bool>(obj));
                        continue;
                    case SupportedFieldType.FLOAT:
                        basicPrimitiveTypeResolver.SetFloat(ref byteData, ref position, fci.GetValue<T, float>(obj));
                        continue;
                    case SupportedFieldType.DOUBLE:
                        basicPrimitiveTypeResolver.SetDouble(ref byteData, ref position, fci.GetValue<T, double>(obj));
                        continue;
                    case SupportedFieldType.CHAR:
                        basicPrimitiveTypeResolver.SetChar(ref byteData, ref position, fci.GetValue<T, char>(obj));
                        continue;
                    case SupportedFieldType.STRING:
                        stringTypeResolver.SetString(ref byteData, ref position, fci.GetValue<T, string>(obj));
                        continue;
                    case SupportedFieldType.DATE_TIME:
                        dateTimeTypeResolver.SetDateTime(ref byteData, ref position, fci.GetValue<T, DateTime>(obj));
                        continue;
                    case SupportedFieldType.SHORT:
                        basicPrimitiveTypeResolver.SetShort(ref byteData, ref position, fci.GetValue<T, short>(obj));
                        continue;
                    case SupportedFieldType.LONG:
                        basicPrimitiveTypeResolver.SetLong(ref byteData, ref position, fci.GetValue<T, long>(obj));
                        continue;
                    case SupportedFieldType.UINT:
                        basicPrimitiveTypeResolver.SetUint(ref byteData, ref position, fci.GetValue<T, uint>(obj));
                        continue;
                    case SupportedFieldType.USHORT:
                        basicPrimitiveTypeResolver.SetUshort(ref byteData, ref position, fci.GetValue<T, ushort>(obj));
                        continue;
                    case SupportedFieldType.ULONG:
                        basicPrimitiveTypeResolver.SetUlong(ref byteData, ref position, fci.GetValue<T, ulong>(obj));
                        continue;
                    case SupportedFieldType.BYTE:
                        basicPrimitiveTypeResolver.SetByte(ref byteData, ref position, fci.GetValue<T, byte>(obj));
                        continue;
                    case SupportedFieldType.SBYTE:
                        basicPrimitiveTypeResolver.SetSbyte(ref byteData, ref position, fci.GetValue<T, sbyte>(obj));
                        continue;
                    case SupportedFieldType.DECIMAL:
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

                    if (fieldType != SupportedFieldType.NONE)
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
