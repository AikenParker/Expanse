using System;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// A collection of System.Byte and System.Byte[] related utility functionality.
    /// </summary>
    public static class ByteUtil
    {
        /// <summary>
        /// Converts a primitive value type into a byte array.
        /// Avoid in production code as it boxes the value type.
        /// </summary>
        public static byte[] GetBytes<T>(T obj) where T : struct, IConvertible, IComparable, IComparable<T>
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

#if UNSAFE
        public static unsafe int GetBytes(int value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(int*)ptr = value;
            return offset + sizeof(int);
        }

        public static unsafe int GetBytes(bool value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(bool*)ptr = value;
            return offset + sizeof(bool);
        }

        public static unsafe int GetBytes(float value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(float*)ptr = value;
            return offset + sizeof(float);
        }

        public static unsafe int GetBytes(double value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(double*)ptr = value;
            return offset + sizeof(double);
        }

        public static unsafe int GetBytes(char value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(char*)ptr = value;
            return offset + sizeof(char);
        }

        public static unsafe int GetBytes(short value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(short*)ptr = value;
            return offset + sizeof(short);
        }

        public static unsafe int GetBytes(long value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(long*)ptr = value;
            return offset + sizeof(long);
        }

        public static unsafe int GetBytes(uint value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(uint*)ptr = value;
            return offset + sizeof(uint);
        }

        public static unsafe int GetBytes(ushort value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(ushort*)ptr = value;
            return offset + sizeof(ushort);
        }

        public static unsafe int GetBytes(ulong value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(ulong*)ptr = value;
            return offset + sizeof(ulong);
        }

        public static unsafe int GetBytes(byte value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(byte*)ptr = value;
            return offset + sizeof(byte);
        }

        public static unsafe int GetBytes(sbyte value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(sbyte*)ptr = value;
            return offset + sizeof(sbyte);
        }

        public static unsafe int GetBytes(decimal value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(decimal*)ptr = value;
            return offset + sizeof(decimal);
        }
#endif

        /// <summary>
        /// Converts a byte array into a primitive value type.
        /// Avoid in production code as it boxes the value type.
        /// </summary>
        public static T GetValue<T>(byte[] data) where T : struct, IConvertible, IComparable, IComparable<T>
        {
            Type type = typeof(T);

            if (type.IsAssignableTo(typeof(bool)))
                return (T)(object)BitConverter.ToBoolean(data, 0);

            if (type.IsAssignableTo(typeof(char)))
                return (T)(object)BitConverter.ToChar(data, 0);

            if (type.IsAssignableTo(typeof(double)))
                return (T)(object)BitConverter.ToDouble(data, 0);

            if (type.IsAssignableTo(typeof(float)))
                return (T)(object)BitConverter.ToSingle(data, 0);

            if (type.IsAssignableTo(typeof(int)))
                return (T)(object)BitConverter.ToInt32(data, 0);

            if (type.IsAssignableTo(typeof(long)))
                return (T)(object)BitConverter.ToInt64(data, 0);

            if (type.IsAssignableTo(typeof(short)))
                return (T)(object)BitConverter.ToInt16(data, 0);

            if (type.IsAssignableTo(typeof(uint)))
                return (T)(object)BitConverter.ToUInt32(data, 0);

            if (type.IsAssignableTo(typeof(ulong)))
                return (T)(object)BitConverter.ToUInt64(data, 0);

            if (type.IsAssignableTo(typeof(ushort)))
                return (T)(object)BitConverter.ToUInt16(data, 0);

            throw new UnsupportedException("Unable to convert data to type. " + typeof(T).ToString());
        }

        /// <summary>
        /// Gets a string composed of '0's and '1's that represent the byte binary value.
        /// </summary>
        public static string GetBinaryText(byte data, string betweenBit = "")
        {
            StringBuilder binaryStr = new StringBuilder(8 * 2);

            for (int i = 0; i < 8; i++)
            {
                binaryStr.Append(data.GetBit(i) ? "1" : "0");

                binaryStr.Append(betweenBit);
            }

            return binaryStr.ToString().Trim();
        }

        /// <summary>
        /// Gets a string composed of '0's and '1's that represent the byte array binary value.
        /// </summary>
        public static string GetBinaryText(byte[] data, string betweenBit = "", string betweenByte = " ")
        {
            StringBuilder binaryStr = new StringBuilder(data.Length * 8 * 2);

            for (int i = 0; i < data.Length * 8; i++)
            {
                binaryStr.Append(data.GetBit(i) ? "1" : "0");

                binaryStr.Append(i % 8 != 7 ? betweenBit : betweenByte);
            }

            return binaryStr.ToString().Trim();
        }
    }
}
