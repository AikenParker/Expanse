using System;
using System.Collections;
using System.Text;

namespace Expanse
{
    public static class ByteUtil
    {
        public static byte[] GetBytes<T>(T obj)
        {
            if (obj is bool)
                return BitConverter.GetBytes((bool)(object)obj);

            if (obj is char)
                return BitConverter.GetBytes((char)(object)obj);

            if (obj is double)
                return BitConverter.GetBytes((double)(object)obj);

            if (obj is float)
                return BitConverter.GetBytes((float)(object)obj);

            if (obj is int)
                return BitConverter.GetBytes((int)(object)obj);

            if (obj is long)
                return BitConverter.GetBytes((long)(object)obj);

            if (obj is short)
                return BitConverter.GetBytes((short)(object)obj);

            if (obj is uint)
                return BitConverter.GetBytes((uint)(object)obj);

            if (obj is ulong)
                return BitConverter.GetBytes((ulong)(object)obj);

            if (obj is ushort)
                return BitConverter.GetBytes((ushort)(object)obj);

            throw new UnsupportedException("Unable to convert type to bytes. " + typeof(T).ToString());
        }

        public static string GetBinaryText(byte[] data, string betweenBit = "", string betweenByte = " ")
        {
            BitArray bitArray = new BitArray(data);

            StringBuilder binaryStr = new StringBuilder(data.Length * 8 * 2);

            for (int i = 0; i < bitArray.Count; i++)
            {
                binaryStr.Append(bitArray.Get(i) ? "1" : "0");

                binaryStr.Append(i % 8 != 7 ? betweenBit : betweenByte);
            }

            return binaryStr.ToString().Trim();
        }
    }
}
