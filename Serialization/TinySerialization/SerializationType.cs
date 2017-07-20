﻿namespace Expanse.Serialization.TinySerialization
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
}
