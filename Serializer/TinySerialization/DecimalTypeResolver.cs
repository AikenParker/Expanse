using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Expanse.TinySerialization
{
    public class DecimalTypeResolver
    {
        private DecimalResolutionType resolutionType;
        private DecimalUnion union;
        private int[] decBits;

        public DecimalTypeResolver(DecimalResolutionType resolutionType)
        {
            this.resolutionType = resolutionType;
        }

        /// <summary>
        /// Gets a decimal from a byte array at position.
        /// </summary>
        public decimal GetDecimal(ref byte[] byteData, ref int position)
        {
            switch (resolutionType)
            {
                case DecimalResolutionType.INTERNAL:
                    {
                        decBits = decBits ?? new int[4];
                        for (int j = 0; j < 4; j++)
                        {
                            decBits[j] = BitConverter.ToInt32(byteData, position);
                            position += sizeof(int);
                        }
                        return new decimal(decBits);
                    }

                case DecimalResolutionType.UNION:
                    {
                        union.int1 = BitConverter.ToInt32(byteData, position);
                        position += sizeof(int);
                        union.int2 = BitConverter.ToInt32(byteData, position);
                        position += sizeof(int);
                        union.int3 = BitConverter.ToInt32(byteData, position);
                        position += sizeof(int);
                        union.int4 = BitConverter.ToInt32(byteData, position);
                        position += sizeof(int);
                        return union.@decimal;
                    }

                default:
                    throw new UnsupportedException("resolutionType");
            }
        }

        /// <summary>
        /// Sets a decimal from a byte array at position.
        /// </summary>
        public void SetDecimal(ref byte[] byteData, ref int position, decimal value)
        {
            switch (resolutionType)
            {
                case DecimalResolutionType.INTERNAL:
                    {
                        decBits = decimal.GetBits(value);
                        for (int j = 0; j < 4; j++)
                        {
                            position = ByteUtil.GetBytes(decBits[j], byteData, position);
                        }
                    }
                    break;

                case DecimalResolutionType.UNION:
                    {
                        union.@decimal = value;
                        position = ByteUtil.GetBytes(union.int1, byteData, position);
                        position = ByteUtil.GetBytes(union.int2, byteData, position);
                        position = ByteUtil.GetBytes(union.int3, byteData, position);
                        position = ByteUtil.GetBytes(union.int4, byteData, position);
                    }
                    break;

                default:
                    throw new UnsupportedException("resolutionType");
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DecimalUnion
        {
            [FieldOffset(0)]
            public decimal @decimal;
            [FieldOffset(0)]
            public int int1;
            [FieldOffset(4)]
            public int int2;
            [FieldOffset(8)]
            public int int3;
            [FieldOffset(12)]
            public int int4;

            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return int1;
                        case 1: return int2;
                        case 2: return int3;
                        case 4: return int4;
                    }

                    throw new IndexOutOfRangeException("index");
                }

                set
                {
                    switch (index)
                    {
                        case 0: int1 = value; return;
                        case 1: int2 = value; return;
                        case 2: int3 = value; return;
                        case 4: int4 = value; return;
                        default: throw new IndexOutOfRangeException("index");
                    }
                }
            }
        }

        public enum DecimalResolutionType : byte
        {
            INTERNAL = 0,
            UNION = 1
        }
    }
}
