using UnityEngine;
using System;

namespace Expanse
{
    /// <summary>
    /// A collection of Vector3 related utility functionality.
    /// </summary>
    public static class Vector3Util
    {
        /// <summary>
        /// Creates a new Vector with specified dimensions of a set value.
        /// </summary>
        public static Vector3 Create(float value, DimensionFlags3D dims = DimensionFlags3D.XYZ)
        {
            Vector3 vec = Vector3.zero;
            if (dims.IsFlagSet(DimensionFlags3D.X))
                vec.x = value;
            if (dims.IsFlagSet(DimensionFlags3D.Y))
                vec.y = value;
            if (dims.IsFlagSet(DimensionFlags3D.Z))
                vec.z = value;

            return vec;
        }

        /// <summary>
        /// Determines the angle between 2 Vectors. Zero Vector acts as the center point.
        /// </summary>
        public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 Vectors. Vector "c" acts as the center point.
        /// </summary>
        public static float AngleBetweenThreePoints(Vector3 a, Vector3 b, Vector3 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }
    }

    public enum DimensionTypes3D
    {
        X = 1,
        Y = 2,
        Z = 3
    }

    [Flags]
    public enum DimensionFlags3D
    {
        NONE = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        XZ = X | Z,
        XY = X | Y,
        XYZ = X | Y | Z
    }
}
