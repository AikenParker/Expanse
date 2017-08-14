using System;
using Expanse.Extensions;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of System.Byte and System.Byte[] related utility functionality.
    /// </summary>
    public static class ByteUtil
    {
        /// <summary>
        /// Converts a primitive value type into a byte array.
        /// </summary>
        /// <typeparam name="T">Type of the value to get the bytes of.</typeparam>
        /// <param name="value">Value to get bytes of.</param>
        /// <returns>Returns a new byte array representing the value given.</returns>
        public static byte[] GetBytes<T>(T value) where T : struct, IConvertible, IComparable, IComparable<T>
        {
            if (value is bool)
                return BitConverter.GetBytes(EmitHelper<bool>.CastFrom(value, true));

            if (value is char)
                return BitConverter.GetBytes(EmitHelper<char>.CastFrom(value, true));

            if (value is double)
                return BitConverter.GetBytes(EmitHelper<double>.CastFrom(value, true));

            if (value is float)
                return BitConverter.GetBytes(EmitHelper<float>.CastFrom(value, true));

            if (value is int)
                return BitConverter.GetBytes(EmitHelper<int>.CastFrom(value, true));

            if (value is long)
                return BitConverter.GetBytes(EmitHelper<long>.CastFrom(value, true));

            if (value is short)
                return BitConverter.GetBytes(EmitHelper<short>.CastFrom(value, true));

            if (value is uint)
                return BitConverter.GetBytes(EmitHelper<uint>.CastFrom(value, true));

            if (value is ulong)
                return BitConverter.GetBytes(EmitHelper<ulong>.CastFrom(value, true));

            if (value is ushort)
                return BitConverter.GetBytes(EmitHelper<ushort>.CastFrom(value, true));

            throw new UnsupportedException("Unable to convert type to bytes. " + typeof(T).ToString());
        }

        /// <summary>
        /// Converts a primitive value type into a byte array.
        /// </summary>
        /// <typeparam name="T">Type of the value to get the bytes of.</typeparam>
        /// <param name="value">Value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static int GetBytes<T>(T value, byte[] array, int offset) where T : struct, IConvertible, IComparable, IComparable<T>
        {
            byte[] data = GetBytes(value);
            int length = data.Length;

            for (int i = 0; i < length; i++)
            {
                array[i + offset] = data[i];
            }

            return offset + length;
        }

#if UNSAFE
        /// <summary>
        /// Gets the bytes of an int and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Int value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(int value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(int*)ptr = value;
            return offset + sizeof(int);
        }

        /// <summary>
        /// Gets the bytes of a bool and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Bool value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(bool value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(bool*)ptr = value;
            return offset + sizeof(bool);
        }

        /// <summary>
        /// Gets the bytes of a float and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Float value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(float value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(float*)ptr = value;
            return offset + sizeof(float);
        }

        /// <summary>
        /// Gets the bytes of a double and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Double value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(double value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(double*)ptr = value;
            return offset + sizeof(double);
        }

        /// <summary>
        /// Gets the bytes of a char and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Char value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(char value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(char*)ptr = value;
            return offset + sizeof(char);
        }

        /// <summary>
        /// Gets the bytes of a short and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Short value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(short value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(short*)ptr = value;
            return offset + sizeof(short);
        }

        /// <summary>
        /// Gets the bytes of a long and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Long value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(long value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(long*)ptr = value;
            return offset + sizeof(long);
        }

        /// <summary>
        /// Gets the bytes of a uint and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Uint value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(uint value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(uint*)ptr = value;
            return offset + sizeof(uint);
        }

        /// <summary>
        /// Gets the bytes of a ushort and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Ushort value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(ushort value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(ushort*)ptr = value;
            return offset + sizeof(ushort);
        }

        /// <summary>
        /// Gets the bytes of a ulong and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Ulong value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(ulong value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(ulong*)ptr = value;
            return offset + sizeof(ulong);
        }

        /// <summary>
        /// Gets the bytes of a byte and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Byte value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(byte value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(byte*)ptr = value;
            return offset + sizeof(byte);
        }

        /// <summary>
        /// Gets the bytes of an sbyte and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Sbyte value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
        public static unsafe int GetBytes(sbyte value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(sbyte*)ptr = value;
            return offset + sizeof(sbyte);
        }

        /// <summary>
        /// Gets the bytes of a decimal and sets it into a byte array at offset.
        /// Returns the new position (offset + length).
        /// </summary>
        /// <param name="value">Decimal value to get bytes of.</param>
        /// <param name="array">Byte array to write the bytes into.</param>
        /// <param name="offset">Index in the byte array to start writin gbytes into.</param>
        /// <returns>Returns the position in the byte array where the value data ends.</returns>
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
        /// <typeparam name="T">Type of the value to get.</typeparam>
        /// <param name="data">Byte array to get the value from.</param>
        /// <param name="offset">Offset in the byte array to get the value from.</param>
        /// <returns>Returns the value convertered from the byte array.</returns>
        public static T GetValue<T>(byte[] data, int offset = 0) where T : struct, IConvertible, IComparable, IComparable<T>
        {
            Type type = typeof(T);

            if (type.IsAssignableTo(typeof(bool)))
                return (T)(object)BitConverter.ToBoolean(data, offset);

            if (type.IsAssignableTo(typeof(char)))
                return (T)(object)BitConverter.ToChar(data, offset);

            if (type.IsAssignableTo(typeof(double)))
                return (T)(object)BitConverter.ToDouble(data, offset);

            if (type.IsAssignableTo(typeof(float)))
                return (T)(object)BitConverter.ToSingle(data, offset);

            if (type.IsAssignableTo(typeof(int)))
                return (T)(object)BitConverter.ToInt32(data, offset);

            if (type.IsAssignableTo(typeof(long)))
                return (T)(object)BitConverter.ToInt64(data, offset);

            if (type.IsAssignableTo(typeof(short)))
                return (T)(object)BitConverter.ToInt16(data, offset);

            if (type.IsAssignableTo(typeof(uint)))
                return (T)(object)BitConverter.ToUInt32(data, offset);

            if (type.IsAssignableTo(typeof(ulong)))
                return (T)(object)BitConverter.ToUInt64(data, offset);

            if (type.IsAssignableTo(typeof(ushort)))
                return (T)(object)BitConverter.ToUInt16(data, offset);

            throw new UnsupportedException("Unable to convert data to type. " + typeof(T).ToString());
        }

        /// <summary>
        /// Gets a string composed of '0's and '1's that represent the byte binary value.
        /// </summary>
        /// <param name="data">Byte value to get the binary text from.</param>
        /// <param name="betweenBit">String to insert between each bit character.</param>
        /// <returns>Returns a binary string representing a byte.</returns>
        public static string GetBinaryText(byte data, string betweenBit = "")
        {
            // TODO: Allow specification of Endian type.

            int betweenBitLength = string.IsNullOrEmpty(betweenBit) ? 0 : betweenBit.Length;

            char[] binaryCharArr = new char[8 + (7 * betweenBitLength)];

            int position = 0;

            for (int i = 0; i < 8; i++)
            {
                binaryCharArr[position++] = data.GetBit(i) ? '1' : '0';

                if (betweenBitLength > 0 && i <= 7)
                {
                    for (int j = 0; j < betweenBitLength; j++)
                    {
                        binaryCharArr[position++] = betweenBit[j];
                    }
                }
            }

            return new string(binaryCharArr);
        }

        /// <summary>
        /// Gets a string composed of '0's and '1's that represent the byte array binary value.
        /// </summary>
        /// <param name="data">Byte value to get the binary text from.</param>
        /// <param name="betweenBit">String to insert between each bit character.</param>
        /// <param name="betweenByte">String to insert between each byte representation.</param>
        /// <returns>Returns a binary string representing a byte.</returns>
        public static string GetBinaryText(byte[] data, string betweenBit = "", string betweenByte = " ")
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // TODO: Allow specification of Endian type.

            int dataLength = data.Length;

            if (dataLength == 0)
                return string.Empty;

            int betweenBitLength = string.IsNullOrEmpty(betweenBit) ? 0 : betweenBit.Length;
            int betweenByteLength = string.IsNullOrEmpty(betweenByte) ? 0 : betweenByte.Length;

            char[] binaryCharArr = new char[(dataLength * 8) + (betweenBitLength * 7 * dataLength) + (betweenByteLength * (dataLength - 1))];

            int position = 0;

            for (int i = 0; i < dataLength; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    binaryCharArr[position++] = data.GetBit((i * 8) + j) ? '1' : '0';

                    if (betweenBitLength > 0 && j <= 7)
                    {
                        for (int k = 0; k < betweenBitLength; k++)
                        {
                            binaryCharArr[position++] = betweenBit[k];
                        }
                    }
                }

                if (i != dataLength - 1)
                {
                    for (int j = 0; j < betweenByteLength; j++)
                    {
                        binaryCharArr[position++] = betweenByte[j];
                    }
                }
            }

            return new string(binaryCharArr);
        }
    }
}
