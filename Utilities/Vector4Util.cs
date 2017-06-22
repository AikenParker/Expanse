using UnityEngine;
using System;
using Expanse.Extensions;

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
        /// <param name="value">Value to set specified Vector component values to.</param>
        /// <param name="dims">Component dimensions to set the value to.</param>
        /// <returns>Returns a new Vector with set component values.</returns>
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
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <returns>Returns the angle between the 2 Vectors.</returns>
        public static float AngleBetweenTwoPoints(Vector4 a, Vector4 b)
        {
            return Mathf.Acos(Vector4.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 Vectors. Vector "c" acts as the center point.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <param name="c">Center point Vector value.</param>
        /// <returns>Returns the angle between 3 Vectors.</returns>
        public static float AngleBetweenThreePoints(Vector4 a, Vector4 b, Vector4 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }
    }

    /// <summary>
    /// Enumeration specifying the component dimension types of a Vector4.
    /// </summary>
    public enum DimensionTypes4D
    {
        /// <summary>
        /// X component dimension.
        /// </summary>
        X = 1,
        /// <summary>
        /// Y component dimension.
        /// </summary>
        Y = 2,
        /// <summary>
        /// Z component dimension.
        /// </summary>
        Z = 3,
        /// <summary>
        /// W component dimension.
        /// </summary>
        W = 4
    }

    /// <summary>
    /// Enumeration specifying the component dimension flags of a Vector4.
    /// </summary>
    [Flags]
    public enum DimensionFlags4D
    {
        /// <summary>
        /// No component dimension.
        /// </summary>
        None = 0,
        /// <summary>
        /// X component dimension.
        /// </summary>
        X = 1 << 0,
        /// <summary>
        /// Y component dimension.
        /// </summary>
        Y = 1 << 1,
        /// <summary>
        /// Z component dimension.
        /// </summary>
        Z = 1 << 2,
        /// <summary>
        /// W component dimension.
        /// </summary>
        W = 1 << 3,
        /// <summary>
        /// X, Y and Z component dimensions.
        /// </summary>
        XYZ = X | Y | Z,
        /// <summary>
        /// X, Y, Z and W component dimensions.
        /// </summary>
        XYZW = X | Y | Z | W
    }
}
