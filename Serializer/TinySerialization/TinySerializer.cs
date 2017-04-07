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
        private BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        private List<TypeCacheInfo> typeCacheInfoList;
        private TypeCacheInfo lastTypeCacheInfo;
        private char[] charData;

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

                switch (fci.type)
                {
                    case SupportedFieldType.INT:
                        fci.SetValue(obj, BitConverter.ToInt32(byteData, position));
                        position += sizeof(int);
                        continue;
                    case SupportedFieldType.BOOL:
                        fci.SetValue(obj, BitConverter.ToBoolean(byteData, position));
                        position += sizeof(bool);
                        continue;
                    case SupportedFieldType.FLOAT:
                        fci.SetValue(obj, BitConverter.ToSingle(byteData, position));
                        position += sizeof(float);
                        continue;
                    case SupportedFieldType.DOUBLE:
                        fci.SetValue(obj, BitConverter.ToDouble(byteData, position));
                        position += sizeof(double);
                        continue;
                    case SupportedFieldType.CHAR:
                        fci.SetValue(obj, BitConverter.ToChar(byteData, position));
                        position += sizeof(char);
                        continue;
                    case SupportedFieldType.STRING:
                        int stringLength = 0;
                        {
                            int stringPosition = sizeof(char) - 1;
                            while (position + stringPosition < dataLength)
                            {
                                if (byteData[position + stringPosition] == 0 && byteData[position + stringPosition - 1] == 0)
                                    break;

                                stringPosition += sizeof(char);
                                stringLength++;
                            }
                        }

                        if (charData == null || charData.Length < stringLength)
                            charData = new char[stringLength];

                        for (int j = 0; j < stringLength; j++)
                        {
                            byte byte1 = byteData[position + (j * sizeof(char))];
                            byte byte2 = byteData[position + (j * sizeof(char) + 1)];
                            charData[j] = (char)(byte1 + byte2);
                        }
                        fci.SetValue(obj, new string(charData, 0, stringLength));
                        position += stringLength * sizeof(char) + sizeof(char);
                        continue;
                    case SupportedFieldType.DATE_TIME:
                        long ticks = BitConverter.ToInt64(byteData, position);
                        fci.SetValue(obj, new DateTime(ticks));
                        position += sizeof(long);
                        continue;
                    case SupportedFieldType.SHORT:
                        fci.SetValue(obj, BitConverter.ToInt16(byteData, position));
                        position += sizeof(short);
                        continue;
                    case SupportedFieldType.LONG:
                        fci.SetValue(obj, BitConverter.ToInt64(byteData, position));
                        position += sizeof(long);
                        continue;
                    case SupportedFieldType.UINT:
                        fci.SetValue(obj, BitConverter.ToUInt32(byteData, position));
                        position += sizeof(uint);
                        continue;
                    case SupportedFieldType.USHORT:
                        fci.SetValue(obj, BitConverter.ToUInt16(byteData, position));
                        position += sizeof(ushort);
                        continue;
                    case SupportedFieldType.ULONG:
                        fci.SetValue(obj, BitConverter.ToUInt64(byteData, position));
                        position += sizeof(ulong);
                        continue;
                    case SupportedFieldType.BYTE:
                        fci.SetValue(obj, byteData[position]);
                        position += sizeof(byte);
                        continue;
                    case SupportedFieldType.SBYTE:
                        fci.SetValue(obj, (sbyte)byteData[position]);
                        position += sizeof(sbyte);
                        continue;
                    case SupportedFieldType.DECIMAL:
                        int[] decBits = new int[4];
                        for (int j = 0; j < 4; j++)
                        {
                            decBits[j] = BitConverter.ToInt32(byteData, position);
                            position += sizeof(int);
                        }
                        decimal decValue = new decimal(decBits);
                        fci.SetValue(obj, decValue);
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

                switch (fci.type)
                {
                    case SupportedFieldType.INT:
                        position = ByteUtil.GetBytes(fci.GetValue<T, int>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.BOOL:
                        position = ByteUtil.GetBytes(fci.GetValue<T, bool>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.FLOAT:
                        position = ByteUtil.GetBytes(fci.GetValue<T, float>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.DOUBLE:
                        position = ByteUtil.GetBytes(fci.GetValue<T, double>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.CHAR:
                        position = ByteUtil.GetBytes(fci.GetValue<T, char>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.STRING:
                        string stringVal = fci.GetValue<T, string>(obj);
                        int stringLength = stringVal.Length;
                        for (int j = 0; j < stringLength; j++)
                        {
                            position = ByteUtil.GetBytes(stringVal[j], byteData, position);
                        }
                        for (int j = 0; j < sizeof(char); j++)
                        {
                            byteData[position++] = 0;
                        }
                        continue;
                    case SupportedFieldType.DATE_TIME:
                        position = ByteUtil.GetBytes(fci.GetValue<T, DateTime>(obj).Ticks, byteData, position);
                        continue;
                    case SupportedFieldType.SHORT:
                        position = ByteUtil.GetBytes(fci.GetValue<T, short>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.LONG:
                        position = ByteUtil.GetBytes(fci.GetValue<T, long>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.UINT:
                        position = ByteUtil.GetBytes(fci.GetValue<T, uint>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.USHORT:
                        position = ByteUtil.GetBytes(fci.GetValue<T, ushort>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.ULONG:
                        position = ByteUtil.GetBytes(fci.GetValue<T, ulong>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.BYTE:
                        position = ByteUtil.GetBytes(fci.GetValue<T, byte>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.SBYTE:
                        position = ByteUtil.GetBytes(fci.GetValue<T, sbyte>(obj), byteData, position);
                        continue;
                    case SupportedFieldType.DECIMAL:
                        decimal decValue = fci.GetValue<T, decimal>(obj);
                        int[] decBits = decimal.GetBits(decValue);
                        for (int j = 0; j < 4; j++)
                        {
                            position = ByteUtil.GetBytes(decBits[j], byteData, position);
                        }
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
                FieldCacheInfo[] fields = type.GetFields(bindingFlags).SelectWhereToList(x => new FieldCacheInfo(x), x => x.type != SupportedFieldType.NONE).ToArray();

                typeCacheInfo = new TypeCacheInfo(type, fields);

                typeCacheInfoList.Add(typeCacheInfo);
            }

            lastTypeCacheInfo = typeCacheInfo;
            return typeCacheInfo;
        }

        public BindingFlags BindingFlags
        {
            get { return bindingFlags; }
            set { bindingFlags = value; }
        }
    }
}
