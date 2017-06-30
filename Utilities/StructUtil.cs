using System;
using System.Runtime.InteropServices;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Struct related utility functionality.
    /// </summary>
    public static class StructUtil
    {
        /// <summary>
        /// Structure containing two values of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of structure to contain.</typeparam>
        public struct DoubleStruct<T> where T : struct
        {
            /// <summary>
            /// First contained value.
            /// </summary>
            public T First;
            /// <summary>
            /// Second contained value.
            /// </summary>
            public T Second;

            /// <summary>
            /// Default DoubleStruct value for type T.
            /// </summary>
            public static readonly DoubleStruct<T> Value = default(DoubleStruct<T>);
        }

#if UNSAFE
        /// <summary>
        /// Determines the size of any structure type.
        /// </summary>
        /// <typeparam name="T">Type of struct to be size checked.</typeparam>
        /// <returns>Returns the size of any struct.</returns>
        public unsafe static int SizeOf<T>() where T : struct
        {
            Type type = typeof(T);

            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return sizeof(bool);
                case TypeCode.Char:
                    return sizeof(char);
                case TypeCode.SByte:
                    return sizeof(sbyte);
                case TypeCode.Byte:
                    return sizeof(byte);
                case TypeCode.Int16:
                    return sizeof(short);
                case TypeCode.UInt16:
                    return sizeof(ushort);
                case TypeCode.Int32:
                    return sizeof(int);
                case TypeCode.UInt32:
                    return sizeof(uint);
                case TypeCode.Int64:
                    return sizeof(long);
                case TypeCode.UInt64:
                    return sizeof(ulong);
                case TypeCode.Single:
                    return sizeof(float);
                case TypeCode.Double:
                    return sizeof(double);
                case TypeCode.Decimal:
                    return sizeof(decimal);
                default:
                    T[] tArray = new T[2];
                    GCHandle tArrayPinned = GCHandle.Alloc(tArray, GCHandleType.Pinned);
                    try
                    {
                        TypedReference tRef0 = __makeref(tArray[0]);
                        TypedReference tRef1 = __makeref(tArray[1]);

                        IntPtr ptrToT0 = *((IntPtr*)&tRef0);
                        IntPtr ptrToT1 = *((IntPtr*)&tRef1);

                        return (int)(((byte*)ptrToT1) - ((byte*)ptrToT0));
                    }
                    finally
                    {
                        tArrayPinned.Free();
                    }

                    // No allocation alternative
                    /*
                    var doubleStruct = DoubleStruct<T>.Value;

                    var tRef0 = __makeref(doubleStruct.First);
                    var tRef1 = __makeref(doubleStruct.Second);

                    IntPtr ptrToT0 = *((IntPtr*)&tRef0);
                    IntPtr ptrToT1 = *((IntPtr*)&tRef1);

                    return (int)(((byte*)ptrToT1) - ((byte*)ptrToT0));
                    */
            }
        }

        /// <summary>
        /// Reinterprets the bytes of a struct type directly into another.
        /// </summary>
        /// <typeparam name="TInput">Input type struct.</typeparam>
        /// <typeparam name="TOutput">Output type struct.</typeparam>
        /// <param name="value">Value of type TInput to be reinterpreted.</param>
        /// <param name="sizeBytes">Size in bytes of both struct types.</param>
        /// <returns>Returns a TOutput struct value with the same bytes as value.</returns>
        public unsafe static TOutput Reinterpret<TInput, TOutput>(TInput value, int sizeBytes)
            where TInput : struct
            where TOutput : struct
        {
            TOutput result = default(TOutput);

            TypedReference resultRef = __makeref(result);
            byte* resultPtr = (byte*)*((IntPtr*)&resultRef);

            TypedReference curValueRef = __makeref(value);
            byte* curValuePtr = (byte*)*((IntPtr*)&curValueRef);

            for (int i = 0; i < sizeBytes; ++i)
            {
                resultPtr[i] = curValuePtr[i];
            }

            return result;
        }
#endif
    }
}
