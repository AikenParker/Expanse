using UnityEngine;
using System;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Vector4 related utility functionality.
    /// </summary>
    public static class Vector4Util
    {
        /// <summary>
        /// Creates a new Vector with specified dimensions of a set value.
        /// </summary>
        public static Vector4 Create(float value, DimensionFlags4D dims = DimensionFlags4D.XYZW)
        {
            Vector4 vec = Vector4.zero;
            if (dims.HasFlag(DimensionFlags4D.X))
                vec.x = value;
            if (dims.HasFlag(DimensionFlags4D.Y))
                vec.y = value;
            if (dims.HasFlag(DimensionFlags4D.Z))
                vec.z = value;
            if (dims.HasFlag(DimensionFlags4D.W))
                vec.w = value;

            return vec;
        }

        /// <summary>
        /// Determines the angle between 2 Vectors. Zero Vector acts as the center point.
        /// </summary>
        public static float AngleBetweenTwoPoints(Vector4 a, Vector4 b)
        {
            return Mathf.Acos(Vector4.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 Vectors. Vector "c" acts as the center point.
        /// </summary>
        public static float AngleBetweenThreePoints(Vector4 a, Vector4 b, Vector4 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }
    }

    public enum DimensionTypes4D
    {
        X = 1,
        Y = 2,
        Z = 3,
        W = 4
    }

    [Flags]
    public enum DimensionFlags4D
    {
        NONE = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        W = 1 << 3,
        XYZ = X | Y | Z,
        XYZW = X | Y | Z | W
    }
}
