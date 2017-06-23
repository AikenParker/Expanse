using UnityEngine;
using System;
using Expanse.Extensions;
using System.Collections.Generic;

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
            if ((dims & DimensionFlags4D.X) != 0)
                vec.x = value;
            if ((dims & DimensionFlags4D.Y) != 0)
                vec.y = value;
            if ((dims & DimensionFlags4D.Z) != 0)
                vec.z = value;
            if ((dims & DimensionFlags4D.W) != 0)
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

        /// <summary>
        /// Converts a Vector4 into Vector3 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to ignore when converting.</param>
        /// <returns>Returns a Vector3 from a Vector4.</returns>
        public static Vector3 ToVector3(Vector4 source, DimensionTypes4D ignoreDim = DimensionTypes4D.W)
        {
            switch (ignoreDim)
            {
                case DimensionTypes4D.W:
                    return new Vector3(source.x, source.y, source.z);
                case DimensionTypes4D.Z:
                    return new Vector3(source.x, source.y, source.w);
                case DimensionTypes4D.Y:
                    return new Vector3(source.x, source.w, source.z);
                case DimensionTypes4D.X:
                    return new Vector3(source.w, source.y, source.z);
                default:
                    throw new InvalidArgumentException("ignoreDim");
            }
        }

        /// <summary>
        /// Calculates the total length of a set of Vectors by added the distance to eachother sequentially.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the total length of a list of Vectors.</returns>
        public static float CalculateTotalLength(IList<Vector4> source)
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
        public static Vector4 Average(IList<Vector4> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            float x = 0, y = 0, z = 0, w = 0;

            for (int i = 0; i < source.Count; i++)
            {
                Vector4 vec = source[i];

                x += vec.x;
                y += vec.y;
                z += vec.z;
                z += vec.w;
            }

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        /// <param name="source">List of values to get a Vector value from.</param>
        /// <param name="selector">Selects from T to get a Vector value.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector4 Average<T>(IList<T> source, Func<T, Vector4> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            float x = 0, y = 0, z = 0, w = 0;

            for (int i = 0; i < source.Count; i++)
            {
                Vector4 vec = selector(source[i]);

                x += vec.x;
                y += vec.y;
                z += vec.z;
                w += vec.w;
            }

            return new Vector4(x, y, z, w);
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
