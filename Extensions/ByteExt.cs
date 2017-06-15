namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Byte and System.Byte[] related extension methods.
    /// Note: All index parameters are zero-based.
    /// </summary>
    public static class ByteExt
    {
        /// <summary>
        /// Get a specific bool in a byte.
        /// </summary>
        /// <param name="source">Source byte value.</param>
        /// <param name="bitIndex">Index of the bit to get the value of.</param>
        /// <returns>Returns true if the bit in the byte of index is set.</returns>
        public static bool GetBit(this byte source, int bitIndex)
        {
            int mask = 1 << bitIndex;

            return (source & mask) != 0;
        }

        /// <summary>
        /// Get a specific bit in a byte array.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="bitIndex">Index of the bit to get the value of.</param>
        /// <returns>Returns true if the bit in the byte array of index is set.</returns>
        public static bool GetBit(this byte[] source, int bitIndex)
        {
            return GetBit(source[bitIndex / 8], bitIndex % 8);
        }

        /// <summary>
        /// Get a specific bit in a byte array.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="byteIndex">Index of the byte in the array to check.</param>
        /// <param name="bitIndex">Index of the bit to get the value of in the byte.</param>
        /// <returns>Returns true if the bit in the byte array of index is set.</returns>
        public static bool GetBit(this byte[] source, int byteIndex, int bitIndex)
        {
            return GetBit(source[byteIndex], bitIndex);
        }

        /// <summary>
        /// Sets a specific bit value in a byte.
        /// </summary>
        /// <param name="source">Source byte value.</param>
        /// <param name="bitIndex">Index of the bit to set the value of.</param>
        /// <param name="value">Value to set the bit with.</param>
        /// <returns>Returns a new byte with specified bit of index in byte set with value.</returns>
        public static byte SetBit(this byte source, int bitIndex, bool value)
        {
            if (value)
                source |= (byte)(1 << bitIndex);
            else
                source &= (byte)~(1 << bitIndex);

            return source;
        }

        /// <summary>
        /// Sets a specific bit value in a byte array.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="bitIndex">Index of the bit to set the value of.</param>
        /// <param name="value">Value to set the bit with.</param>
        public static void SetBit(this byte[] source, int bitIndex, bool value)
        {
            int byteIndex = bitIndex / 8;

            byte @byte = SetBit(source[byteIndex], bitIndex % 8, value);

            source[byteIndex] = @byte;
        }

        /// <summary>
        /// Sets a specific bit value in a byte array.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="byteIndex">Index of the byte in the array to check.</param>
        /// <param name="bitIndex">Index of the bit to set the value of.</param>
        /// <param name="value">Value to set the bit with.</param>
        public static void SetBit(this byte[] source, int byteIndex, int bitIndex, bool value)
        {
            SetBit(source[byteIndex], bitIndex, value);
        }

        /// <summary>
        /// Inverts a specific bit value in a byte.
        /// </summary>
        /// <param name="source">Source byte value.</param>
        /// <param name="bitIndex">Index of the bit to invert the value of.</param>
        /// <returns>Returns a new byte with the bit of index with an inverted value.</returns>
        public static byte InvertBit(this byte source, int bitIndex)
        {
            source ^= (byte)(1 << bitIndex);

            return source;
        }

        /// <summary>
        /// Inverts a specific bit value in a byte array.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="bitIndex">Bit of the index to invert the value of.</param>
        public static void InvertBit(this byte[] source, int bitIndex)
        {
            int byteIndex = bitIndex / 8;

            byte @byte = InvertBit(source[byteIndex], bitIndex % 8);

            source[byteIndex] = @byte;
        }

        /// <summary>
        /// Inverts a specific bit value in a byte array.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="byteIndex">Index of the byte in the array to check.</param>
        /// <param name="bitIndex">Index of the bit to invert the value of.</param>
        public static void InvertBit(this byte[] source, int byteIndex, int bitIndex)
        {
            InvertBit(source[byteIndex], bitIndex);
        }

        /// <summary>
        /// Sets all bits in a specific byte to a value.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="byteIndex">Index f the byte in the array to set the value of.</param>
        /// <param name="value">Value to set the byte with.</param>
        public static void SetByte(this byte[] source, int byteIndex, bool value)
        {
            source[byteIndex] = (byte)(value ? 0xFF : 0x00);
        }

        /// <summary>
        /// Inverts all bits in a byte.
        /// </summary>
        /// <param name="source">Source byte value.</param>
        /// <returns>Returns a new byte with inverted values.</returns>
        public static byte InvertByte(this byte source)
        {
            return (byte)~source;
        }

        /// <summary>
        /// Inverts all bits in a specific byte;
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="byteIndex">Index of the byte in the array to invert the value of.</param>
        public static void InvertByte(this byte[] source, int byteIndex)
        {
            source[byteIndex] = InvertByte(source[byteIndex]);
        }

        /// <summary>
        /// Sets all bits within a specific range to value.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="value">Value to set the bit range with.</param>
        /// <param name="startBitIndex">Starting index of the bit range (Inclusive)</param>
        /// <param name="endBitIndex">Ending index of the bit range (Exclusive)</param>
        public static void SetBitRange(this byte[] source, bool value, int startBitIndex, int endBitIndex)
        {
            for (int i = startBitIndex; i < endBitIndex; i++)
            {
                source.SetBit(i, value);
            }
        }

        /// <summary>
        /// Inverts all bits within a specific range.
        /// </summary>
        /// <param name="source">Source byte array value.</param>
        /// <param name="startBitIndex">Starting index of the bit range (Inclusive)</param>
        /// <param name="endBitIndex">Ending index of the bit range (Exclusive)</param>
        public static void InvertBitRange(this byte[] source, int startBitIndex, int endBitIndex)
        {
            for (int i = startBitIndex; i < endBitIndex; i++)
            {
                source.InvertBit(i);
            }
        }
    }
}
