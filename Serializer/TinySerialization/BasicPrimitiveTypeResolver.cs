using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse.TinySerialization
{
    public class BasicPrimitiveTypeResolver
    {
        /// <summary>
        /// Gets an int from a byte array at position.
        /// </summary>
        public int GetInt(ref byte[] byteData, ref int position)
        {
            int @int = BitConverter.ToInt32(byteData, position);
            position += sizeof(int);
            return @int;
        }

        /// <summary>
        /// Gets a bool from a byte array at position.
        /// </summary>
        public bool GetBool(ref byte[] byteData, ref int position)
        {
            bool @bool = BitConverter.ToBoolean(byteData, position);
            position += sizeof(bool);
            return @bool;
        }

        /// <summary>
        /// Gets a float from a byte array at position.
        /// </summary>
        public float GetFloat(ref byte[] byteData, ref int position)
        {
            float @float = BitConverter.ToSingle(byteData, position);
            position += sizeof(float);
            return @float;
        }

        /// <summary>
        /// Gets a double from a byte array at position.
        /// </summary>
        public double GetDouble(ref byte[] byteData, ref int position)
        {
            double @double = BitConverter.ToDouble(byteData, position);
            position += sizeof(double);
            return @double;
        }

        /// <summary>
        /// Gets a char from a byte array at position.
        /// </summary>
        public char GetChar(ref byte[] byteData, ref int position)
        {
            char @char = BitConverter.ToChar(byteData, position);
            position += sizeof(char);
            return @char;
        }

        /// <summary>
        /// Gets a short from a byte array at position.
        /// </summary>
        public short GetShort(ref byte[] byteData, ref int position)
        {
            short @short = BitConverter.ToInt16(byteData, position);
            position += sizeof(short);
            return @short;
        }

        /// <summary>
        /// Gets a long from a byte array at position.
        /// </summary>
        public long GetLong(ref byte[] byteData, ref int position)
        {
            long @long = BitConverter.ToInt64(byteData, position);
            position += sizeof(long);
            return @long;
        }

        /// <summary>
        /// Gets a uint from a byte array at position.
        /// </summary>
        public uint GetUint(ref byte[] byteData, ref int position)
        {
            uint @uint = BitConverter.ToUInt32(byteData, position);
            position += sizeof(uint);
            return @uint;
        }

        /// <summary>
        /// Gets a ushort from a byte array at position.
        /// </summary>
        public ushort GetUshort(ref byte[] byteData, ref int position)
        {
            ushort @ushort = BitConverter.ToUInt16(byteData, position);
            position += sizeof(ushort);
            return @ushort;
        }

        /// <summary>
        /// Gets a ulong from a byte array at position.
        /// </summary>
        public ulong GetUlong(ref byte[] byteData, ref int position)
        {
            ulong @ulong = BitConverter.ToUInt64(byteData, position);
            position += sizeof(ulong);
            return @ulong;
        }

        /// <summary>
        /// Gets a byte from a byte array at position.
        /// </summary>
        public byte GetByte(ref byte[] byteData, ref int position)
        {
            byte @byte = byteData[position];
            position += sizeof(byte);
            return @byte;
        }

        /// <summary>
        /// Gets an sbyte from a byte array at position.
        /// </summary>
        public sbyte GetSbyte(ref byte[] byteData, ref int position)
        {
            sbyte @sbyte = (sbyte)byteData[position];
            position += sizeof(sbyte);
            return @sbyte;
        }

        /// <summary>
        /// Sets an int in a byte array at position.
        /// </summary>
        public void SetInt(ref byte[] byteData, ref int position, int value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a bool in a byte array at position.
        /// </summary>
        public void SetBool(ref byte[] byteData, ref int position, bool value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a float in a byte array at position.
        /// </summary>
        public void SetFloat(ref byte[] byteData, ref int position, float value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a double in a byte array at position.
        /// </summary>
        public void SetDouble(ref byte[] byteData, ref int position, double value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a char in a byte array at position.
        /// </summary>
        public void SetChar(ref byte[] byteData, ref int position, char value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a short in a byte array at position.
        /// </summary>
        public void SetShort(ref byte[] byteData, ref int position, short value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a long in a byte array at position.
        /// </summary>
        public void SetLong(ref byte[] byteData, ref int position, long value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a uint in a byte array at position.
        /// </summary>
        public void SetUint(ref byte[] byteData, ref int position, uint value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a ushort in a byte array at position.
        /// </summary>
        public void SetUshort(ref byte[] byteData, ref int position, ushort value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a ulong in a byte array at position.
        /// </summary>
        public void SetUlong(ref byte[] byteData, ref int position, ulong value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets a byte in a byte array at position.
        /// </summary>
        public void SetByte(ref byte[] byteData, ref int position, byte value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }

        /// <summary>
        /// Sets an sbyte in a byte array at position.
        /// </summary>
        public void SetSbyte(ref byte[] byteData, ref int position, sbyte value)
        {
            position = ByteUtil.GetBytes(value, byteData, position);
        }
    }
}
