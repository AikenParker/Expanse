using UnityEngine;
using System;
using Expanse.Extensions;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Vector3 related utility functionality.
    /// </summary>
    public static class Vector3Util
    {
        /// <summary>
        /// Creates a new Vector with specified dimensions of a set value.
        /// </summary>
        /// <param name="value">Value to set specified Vector component values to.</param>
        /// <param name="dims">Component dimensions to set the value to.</param>
        /// <returns>Returns a new Vector with set component values.</returns>
        public static Vector3 Create(float value, DimensionFlags3D dims = DimensionFlags3D.XYZ)
        {
            Vector3 vec = Vector3.zero;
            if (dims.HasFlag(DimensionFlags3D.X))
                vec.x = value;
            if (dims.HasFlag(DimensionFlags3D.Y))
                vec.y = value;
            if (dims.HasFlag(DimensionFlags3D.Z))
                vec.z = value;

            return vec;
        }

        /// <summary>
        /// Determines the angle between 2 Vectors. Zero Vector acts as the center point.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <returns>Returns the angle between the 2 Vectors.</returns>
        public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 Vectors. Vector "c" acts as the center point.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <param name="c">Center point Vector value.</param>
        /// <returns>Returns the angle between 3 Vectors.</returns>
        public static float AngleBetweenThreePoints(Vector3 a, Vector3 b, Vector3 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }

        /// <summary>
        /// Determines the signed angle between two vectors with a defined axis.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <returns>Returns the angle between the 2 Vectors.</returns>
        public static float AngleBetweenTwoPointsSigned(Vector3 a, Vector3 b, Vector3 axis)
        {
            return Mathf.Atan2(
                Vector3.Dot(axis, Vector3.Cross(a, b)),
                Vector3.Dot(a, b)) * Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// Enumeration specifying the component dimension types of a Vector3.
    /// </summary>
    public enum DimensionTypes3D
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
        Z = 3
    }

    /// <summary>
    /// Enumeration specifying the component dimension flags of a Vector3.
    /// </summary>
    [Flags]
    public enum DimensionFlags3D
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
        /// X and Z component dimensions.
        /// </summary>
        XZ = X | Z,
        /// <summary>
        /// X and Y component dimensions.
        /// </summary>
        XY = X | Y,
        /// <summary>
        /// X, Y and Z component dimensions.
        /// </summary>
        XYZ = X | Y | Z
    }
}
