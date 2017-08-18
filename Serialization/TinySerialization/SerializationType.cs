#if UNSAFE

using System;
using System.Collections.Generic;
using Expanse.Misc;
using UnityEngine;

namespace Expanse.Serialization.TinySerialization
{
    public enum SerializationType : byte
    {
        None = 0,
        Object,
        String,
        Byte,
        SByte,
        Bool,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        Half,
        Single,
        Double,
        Char,
        Decimal,
        DateTime,
        DateTimeOffset,
        TimeSpan,
        Vector2,
        Vector3,
        Vector4,
        Quaternion,
        Rect,
        Bounds,
        IntVector2,
        IntVector3,
        IntVector4,
        PrimitiveArray,
        PrimitiveList,
        ObjectArray,
        ObjectList,
        PrimitiveNullable,
        ObjectNullable
    }

    public static class SerializationTypeValues
    {
        public static readonly Type String;
        public static readonly Type Byte;
        public static readonly Type SByte;
        public static readonly Type Bool;
        public static readonly Type Int16;
        public static readonly Type Int32;
        public static readonly Type Int64;
        public static readonly Type UInt16;
        public static readonly Type UInt32;
        public static readonly Type UInt64;
        public static readonly Type Half;
        public static readonly Type Single;
        public static readonly Type Double;
        public static readonly Type Char;
        public static readonly Type Decimal;
        public static readonly Type DateTime;
        public static readonly Type DateTimeOffset;
        public static readonly Type TimeSpan;
        public static readonly Type Vector2;
        public static readonly Type Vector3;
        public static readonly Type Vector4;
        public static readonly Type Quaternion;
        public static readonly Type Rect;
        public static readonly Type Bounds;
        public static readonly Type IntVector2;
        public static readonly Type IntVector3;
        public static readonly Type IntVector4;
        public static readonly Type UnboundList;
        public static readonly Type UnboundNullable;

        static SerializationTypeValues()
        {
            String = typeof(string);
            Byte = typeof(byte);
            SByte = typeof(sbyte);
            Bool = typeof(bool);
            Int16 = typeof(short);
            Int32 = typeof(int);
            Int64 = typeof(long);
            UInt16 = typeof(ushort);
            UInt32 = typeof(uint);
            UInt64 = typeof(ulong);
            Half = typeof(Half);
            Single = typeof(float);
            Double = typeof(double);
            Char = typeof(char);
            Decimal = typeof(decimal);
            DateTime = typeof(DateTime);
            DateTimeOffset = typeof(DateTimeOffset);
            TimeSpan = typeof(TimeSpan);
            Vector2 = typeof(Vector2);
            Vector3 = typeof(Vector3);
            Vector4 = typeof(Vector4);
            Quaternion = typeof(Quaternion);
            Rect = typeof(Rect);
            Bounds = typeof(Bounds);
            IntVector2 = typeof(IntVector2);
            IntVector3 = typeof(IntVector3);
            IntVector4 = typeof(IntVector4);
            UnboundList = typeof(List<>);
            UnboundNullable = typeof(Nullable<>);
        }
    }

    public static class SerializationTypeSizes
    {
        public const int BYTE = sizeof(byte);
        public const int SBYTE = sizeof(sbyte);
        public const int BOOL = sizeof(bool);
        public const int INT16 = sizeof(short);
        public const int INT32 = sizeof(int);
        public const int INT64 = sizeof(long);
        public const int UINT16 = sizeof(ushort);
        public const int UINT32 = sizeof(uint);
        public const int UINT64 = sizeof(ulong);
        public const int HALF = sizeof(ushort);
        public const int SINGLE = sizeof(float);
        public const int DOUBLE = sizeof(double);
        public const int CHAR = sizeof(char);
        public const int DECIMAL = sizeof(decimal);
        public const int DATE_TIME = sizeof(long);
        public const int DATE_TIME_OFFSET = sizeof(long);
        public const int TIME_SPAN = sizeof(long);
        public const int VECTOR2 = sizeof(float) * 2;
        public const int VECTOR3 = sizeof(float) * 3;
        public const int VECTOR4 = sizeof(float) * 4;
        public const int QUATERNION = sizeof(float) * 4;
        public const int RECT = sizeof(float) * 4;
        public const int BOUNDS = sizeof(float) * 6;
        public const int INT_VECTOR2 = sizeof(int) * 2;
        public const int INT_VECTOR3 = sizeof(int) * 3;
        public const int INT_VECTOR4 = sizeof(int) * 4;
    }

    public static class SerializationTypeHashCodes
    {
        public static readonly int String;
        public static readonly int Byte;
        public static readonly int SByte;
        public static readonly int Bool;
        public static readonly int Int16;
        public static readonly int Int32;
        public static readonly int Int64;
        public static readonly int UInt16;
        public static readonly int UInt32;
        public static readonly int UInt64;
        public static readonly int Half;
        public static readonly int Single;
        public static readonly int Double;
        public static readonly int Char;
        public static readonly int Decimal;
        public static readonly int DateTime;
        public static readonly int DateTimeOffset;
        public static readonly int TimeSpan;
        public static readonly int Vector2;
        public static readonly int Vector3;
        public static readonly int Vector4;
        public static readonly int Quaternion;
        public static readonly int Rect;
        public static readonly int Bounds;
        public static readonly int IntVector2;
        public static readonly int IntVector3;
        public static readonly int IntVector4;
        public static readonly int UnboundList;
        public static readonly int UnboundNullable;

        static SerializationTypeHashCodes()
        {
            String = SerializationTypeValues.String.TypeHandle.Value.ToInt32();
            Byte = SerializationTypeValues.Byte.TypeHandle.Value.ToInt32();
            SByte = SerializationTypeValues.SByte.TypeHandle.Value.ToInt32();
            Bool = SerializationTypeValues.Bool.TypeHandle.Value.ToInt32();
            Int16 = SerializationTypeValues.Int16.TypeHandle.Value.ToInt32();
            Int32 = SerializationTypeValues.Int32.TypeHandle.Value.ToInt32();
            Int64 = SerializationTypeValues.Int64.TypeHandle.Value.ToInt32();
            UInt16 = SerializationTypeValues.UInt16.TypeHandle.Value.ToInt32();
            UInt32 = SerializationTypeValues.UInt32.TypeHandle.Value.ToInt32();
            UInt64 = SerializationTypeValues.UInt64.TypeHandle.Value.ToInt32();
            Half = SerializationTypeValues.Half.TypeHandle.Value.ToInt32();
            Single = SerializationTypeValues.Single.TypeHandle.Value.ToInt32();
            Double = SerializationTypeValues.Double.TypeHandle.Value.ToInt32();
            Char = SerializationTypeValues.Char.TypeHandle.Value.ToInt32();
            Decimal = SerializationTypeValues.Decimal.TypeHandle.Value.ToInt32();
            DateTime = SerializationTypeValues.DateTime.TypeHandle.Value.ToInt32();
            DateTimeOffset = SerializationTypeValues.DateTimeOffset.TypeHandle.Value.ToInt32();
            TimeSpan = SerializationTypeValues.TimeSpan.TypeHandle.Value.ToInt32();
            Vector2 = SerializationTypeValues.Vector2.TypeHandle.Value.ToInt32();
            Vector3 = SerializationTypeValues.Vector3.TypeHandle.Value.ToInt32();
            Vector4 = SerializationTypeValues.Vector4.TypeHandle.Value.ToInt32();
            Quaternion = SerializationTypeValues.Quaternion.TypeHandle.Value.ToInt32();
            Rect = SerializationTypeValues.Rect.TypeHandle.Value.ToInt32();
            Bounds = SerializationTypeValues.Bounds.TypeHandle.Value.ToInt32();
            IntVector2 = SerializationTypeValues.IntVector2.TypeHandle.Value.ToInt32();
            IntVector3 = SerializationTypeValues.IntVector3.TypeHandle.Value.ToInt32();
            IntVector4 = SerializationTypeValues.IntVector4.TypeHandle.Value.ToInt32();
            UnboundList = SerializationTypeValues.UnboundList.TypeHandle.Value.ToInt32();
            UnboundNullable = SerializationTypeValues.UnboundNullable.TypeHandle.Value.ToInt32();
        }
    }
}
#endif
