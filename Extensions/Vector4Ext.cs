using UnityEngine;
using System;
using System.Collections.Generic;
using Expanse.Utilities;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Vector3 related extension methods.
    /// </summary>
    public static class Vector4Ext
    {
        /// <summary>
        /// Returns the same Vector with selected components zeroed out.
        /// </summary>
        /// <param name="source">Source Vector value to zero.</param>
        /// <param name="dims">Component dimensions to zero.</param>
        /// <returns>Returns a new Vector with zeroed out component values.</returns>
        public static Vector4 ZeroValues(this Vector4 source, DimensionFlags4D dims)
        {
            if ((dims & DimensionFlags4D.X) != 0)
                source.x = 0;
            if ((dims & DimensionFlags4D.Y) != 0)
                source.y = 0;
            if ((dims & DimensionFlags4D.Z) != 0)
                source.z = 0;
            if ((dims & DimensionFlags4D.W) != 0)
                source.w = 0;

            return source;
        }

        /// <summary>
        /// Sets selected components of a Vector to a value.
        /// </summary>
        /// <param name="source">Source Vector value to set.</param>
        /// <param name="value">Value to set to selected components on the Vector.</param>
        /// <param name="dims">Components dimensions to set.</param>
        /// <returns>Returns a new Vector with set component values.</returns>
        public static Vector4 SetValues(this Vector4 source, float value, DimensionFlags4D dims = DimensionFlags4D.XYZW)
        {
            if ((dims & DimensionFlags4D.X) != 0)
                source.x = value;
            if ((dims & DimensionFlags4D.Y) != 0)
                source.y = value;
            if ((dims & DimensionFlags4D.Z) != 0)
                source.z = value;
            if ((dims & DimensionFlags4D.W) != 0)
                source.w = value;

            return source;
        }

        /// <summary>
        /// Creates a new Vector with set X.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="x">Value to set the X component to.</param>
        /// <returns>Returns a new Vector value with X component set.</returns>
        public static Vector4 WithX(this Vector4 source, float x)
        {
            return new Vector4(x, source.y, source.z, source.w);
        }

        /// <summary>
        /// Creates a new Vector with set Y.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="y">Value to set the Y component to.</param>
        /// <returns>Returns a new Vector value with the Y component set.</returns>
        public static Vector4 WithY(this Vector4 source, float y)
        {
            return new Vector4(source.x, y, source.z, source.w);
        }

        /// <summary>
        /// Creates a new Vector with set Z.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="z">Value to set the Z component to.</param>
        /// <returns>Returns a new Vector value with the Z component set.</returns>
        public static Vector4 WithZ(this Vector4 source, float z)
        {
            return new Vector4(source.x, source.y, z, source.w);
        }

        /// <summary>
        /// Creates a new Vector with set W.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="w">Value to set the W component to.</param>
        /// <returns>Returns a new Vector value with the W component set.</returns>
        public static Vector4 WithW(this Vector4 source, float w)
        {
            return new Vector4(source.x, source.y, source.z, w);
        }

        /// <summary>
        /// Converts a Vector4 into Vector3 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to ignore when converting.</param>
        /// <returns>Returns a Vector3 from a Vector4.</returns>
        public static Vector3 ToVector3(this Vector4 source, DimensionTypes4D ignoreDim = DimensionTypes4D.W)
        {
            return Vector4Util.ToVector3(source, ignoreDim);
        }

        /// <summary>
        /// Calculates the total length of a set of Vectors by added the distance to eachother sequentially.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the total length of a list of Vectors.</returns>
        public static float CalculateTotalLength(this IList<Vector4> source)
        {
            return Vector4Util.CalculateTotalLength(source);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector4 Average(this IList<Vector4> source)
        {
            return Vector4Util.Average(source);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        /// <param name="source">List of values to get a Vector value from.</param>
        /// <param name="selector">Selects from T to get a Vector value.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector4 Average<T>(this IList<T> source, Func<T, Vector4> selector)
        {
            return Vector4Util.Average(source, selector);
        }
    }
}
