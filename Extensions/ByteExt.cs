namespace Expanse
{
    /// <summary>
    /// A collection of byte and byte[] related extension methods.
    /// Note: All index parameters are zero-based.
    /// </summary>
    public static class ByteExt
    {
        /// <summary>
        /// Get a specific bool in a byte.
        /// </summary>
        public static bool GetBit(this byte data, int bitIndex)
        {
            int mask = 1 << bitIndex;

            return (data & mask) != 0;
        }

        /// <summary>
        /// Get a specific bit in a byte array.
        /// </summary>
        public static bool GetBit(this byte[] data, int bitIndex)
        {
            return GetBit(data[bitIndex / 8], bitIndex % 8);
        }
        
        /// <summary>
        /// Get a specific bit in a byte array.
        /// </summary>
        public static bool GetBit(this byte[] data, int byteIndex, int bitIndex)
        {
            return GetBit(data[byteIndex], bitIndex);
        }

        /// <summary>
        /// Sets a specific bit value in a byte.
        /// </summary>
        public static byte SetBit(this byte data, int bitIndex, bool value)
        {
            if (value)
                data |= (byte)(1 << bitIndex);
            else
                data &= (byte)~(1 << bitIndex);

            return data;
        }

        /// <summary>
        /// Sets a specific bit value in a byte array.
        /// </summary>
        public static void SetBit(this byte[] data, int bitIndex, bool value)
        {
            int byteIndex = bitIndex / 8;

            byte @byte = SetBit(data[byteIndex], bitIndex % 8, value);

            data[byteIndex] = @byte;
        }

        /// <summary>
        /// Sets a specific bit value in a byte array.
        /// </summary>
        public static void SetBit(this byte[] data, int byteIndex, int bitIndex, bool value)
        {
            SetBit(data[byteIndex], bitIndex, value);
        }

        /// <summary>
        /// Inverts a specific bit value in a byte.
        /// </summary>
        public static byte InvertBit(this byte data, int bitIndex)
        {
            data ^= (byte)(1 << bitIndex);

            return data;
        }

        /// <summary>
        /// Inverts a specific bit value in a byte array.
        /// </summary>
        public static void InvertBit(this byte[] data, int bitIndex)
        {
            int byteIndex = bitIndex / 8;

            byte @byte = InvertBit(data[byteIndex], bitIndex % 8);

            data[byteIndex] = @byte;
        }

        /// <summary>
        /// Inverts a specific bit value in a byte array.
        /// </summary>
        public static void InvertBit(this byte[] data, int byteIndex, int bitIndex)
        {
            InvertBit(data[byteIndex], bitIndex);
        }

        /// <summary>
        /// Sets all bits in a specific byte to a value.
        /// </summary>
        public static void SetByte(this byte[] data, int byteIndex, bool value)
        {
            data[byteIndex] = (byte)(value ? 0xFF : 0x00);
        }

        /// <summary>
        /// Inverts all bits in a byte.
        /// </summary>
        public static byte InvertByte(this byte data)
        {
            return (byte)~data;
        }

        /// <summary>
        /// Inverts all bits in a specific byte;
        /// </summary>
        public static void InvertByte(this byte[] data, int byteIndex)
        {
            data[byteIndex] = InvertByte(data[byteIndex]);
        }

        /// <summary>
        /// Sets all bits within a specific range to value.
        /// </summary>
        public static void SetBitRange(this byte[] data, bool value, int startBitIndex, int endBitIndex)
        {
            for (int i = startBitIndex; i < endBitIndex; i++)
            {
                data.SetBit(i, value);
            }
        }

        /// <summary>
        /// Inverts all bits within a specific range.
        /// </summary>
        public static void InvertBitRange(this byte[] data, int startBitIndex, int endBitIndex)
        {
            for (int i = startBitIndex; i < endBitIndex; i++)
            {
                data.InvertBit(i);
            }
        }
    }
}
