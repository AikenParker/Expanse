using UnityEngine;
using System;
using Expanse.Extensions;
using System.Collections.Generic;

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
            if ((dims & DimensionFlags3D.X) != 0)
                vec.x = value;
            if ((dims & DimensionFlags3D.Y) != 0)
                vec.y = value;
            if ((dims & DimensionFlags3D.Z) != 0)
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

        /// <summary>
        /// Converts a Vector3 into Vector2 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to ignore when converting.</param>
        /// <returns>Returns a Vector2 from a Vector3.</returns>
        public static Vector2 ToVector2(Vector3 source, DimensionTypes3D ignoreDim = DimensionTypes3D.Z)
        {
            switch (ignoreDim)
            {
                case DimensionTypes3D.Z:
                    return new Vector2(source.x, source.y);
                case DimensionTypes3D.Y:
                    return new Vector2(source.x, source.z);
                case DimensionTypes3D.X:
                    return new Vector2(source.y, source.z);
                default:
                    throw new InvalidArgumentException("ignoreDim");
            }
        }

        /// <summary>
        /// Converts a Vector3 into Vector4 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to leave zeroed when converting.</param>
        /// <returns>Returns a Vector4 from a Vector3.</returns>
        public static Vector4 ToVector4(Vector3 source, DimensionTypes4D ignoreDim = DimensionTypes4D.W)
        {
            switch (ignoreDim)
            {
                case DimensionTypes4D.W:
                    return new Vector4(source.x, source.y, source.z, 0f);
                case DimensionTypes4D.Z:
                    return new Vector4(source.x, source.y, 0f, source.z);
                case DimensionTypes4D.Y:
                    return new Vector4(source.x, 0f, source.z, source.y);
                case DimensionTypes4D.X:
                    return new Vector4(0f, source.y, source.z, source.x);
                default:
                    throw new InvalidArgumentException("ignoreDim");
            }
        }

        /// <summary>
        /// Calculates the total length of a set of Vectors by added the distance to eachother sequentially.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the total length of a list of Vectors.</returns>
        public static float CalculateTotalLength(IList<Vector3> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            float totalLength = 0f;

            for (int i = 1; i < source.Count; i++)
            {
                totalLength += (source[i - 1] - source[i]).magnitude;
            }

            return totalLength;
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector3 Average(IList<Vector3> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            float x = 0, y = 0, z = 0;

            for (int i = 0; i < source.Count; i++)
            {
                Vector3 vec = source[i];

                x += vec.x;
                y += vec.y;
                z += vec.z;
            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        /// <param name="source">List of values to get a Vector value from.</param>
        /// <param name="selector">Selects from T to get a Vector value.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector3 Average<T>(IList<T> source, Func<T, Vector3> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            float x = 0, y = 0, z = 0;

            for (int i = 0; i < source.Count; i++)
            {
                Vector3 vec = selector(source[i]);

                x += vec.x;
                y += vec.y;
                z += vec.z;
            }

            return new Vector3(x, y, z);
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
