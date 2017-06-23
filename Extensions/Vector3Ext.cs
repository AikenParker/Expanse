using UnityEngine;
using System;
using System.Collections.Generic;
using Expanse.Utilities;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Vector3 related extension methods.
    /// </summary>
    public static class Vector3Ext
    {
        /// <summary>
        /// Gets the same Vector with selected components zeroed out.
        /// </summary>
        /// <param name="source">Source Vector value to zero.</param>
        /// <param name="dims">Component dimensions to zero.</param>
        /// <returns>Returns a new Vector with zeroed out component values.</returns>
        public static Vector3 ZeroValues(this Vector3 source, DimensionFlags3D dims)
        {
            if ((dims & DimensionFlags3D.X) != 0)
                source.x = 0;
            if ((dims & DimensionFlags3D.Y) != 0)
                source.y = 0;
            if ((dims & DimensionFlags3D.Z) != 0)
                source.z = 0;

            return source;
        }

        /// <summary>
        /// Sets selected components of a Vector to a value.
        /// </summary>
        /// <param name="source">Source Vector value to set.</param>
        /// <param name="value">Value to set to selected components on the Vector.</param>
        /// <param name="dims">Components dimensions to set.</param>
        /// <returns>Returns a new Vector with set component values.</returns>
        public static Vector3 SetValues(this Vector3 source, float value, DimensionFlags3D dims = DimensionFlags3D.XYZ)
        {
            if ((dims & DimensionFlags3D.X) != 0)
                source.x = value;
            if ((dims & DimensionFlags3D.Y) != 0)
                source.y = value;
            if ((dims & DimensionFlags3D.Z) != 0)
                source.z = value;

            return source;
        }

        /// <summary>
        /// Creates a new Vector with set X.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="x">Value to set the X component to.</param>
        /// <returns>Returns a new Vector value with X component set.</returns>
        public static Vector3 WithX(this Vector3 source, float x)
        {
            return new Vector3(x, source.y, source.z);
        }

        /// <summary>
        /// Creates a new Vector with set Y.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="y">Value to set the Y component to.</param>
        /// <returns>Returns a new Vector value with the Y component set.</returns>
        public static Vector3 WithY(this Vector3 source, float y)
        {
            return new Vector3(source.x, y, source.z);
        }

        /// <summary>
        /// Creates a new Vector with set Z.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="z">Value to set the Z component to.</param>
        /// <returns>Returns a new Vector value with the Z component set.</returns>
        public static Vector3 WithZ(this Vector3 source, float z)
        {
            return new Vector3(source.x, source.y, z);
        }

        /// <summary>
        /// Converts a Vector3 into Vector2 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to ignore when converting.</param>
        /// <returns>Returns a Vector2 from a Vector3.</returns>
        public static Vector2 ToVector2(this Vector3 source, DimensionTypes3D ignoreDim = DimensionTypes3D.Z)
        {
            return Vector3Util.ToVector2(source, ignoreDim);
        }

        /// <summary>
        /// Converts a Vector3 into Vector4 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to leave zeroed when converting.</param>
        /// <returns>Returns a Vector4 from a Vector3.</returns>
        public static Vector4 ToVector4(this Vector3 source, DimensionTypes4D ignoreDim = DimensionTypes4D.W)
        {
            return Vector3Util.ToVector4(source, ignoreDim);
        }

        /// <summary>
        /// Calculates the total length of a set of Vectors by added the distance to eachother sequentially.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the total length of a list of Vectors.</returns>
        public static float CalculateTotalLength(this IList<Vector3> source)
        {
            return Vector3Util.CalculateTotalLength(source);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector3 Average(this IList<Vector3> source)
        {
            return Vector3Util.Average(source);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        /// <param name="source">List of values to get a Vector value from.</param>
        /// <param name="selector">Selects from T to get a Vector value.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector3 Average<T>(this IList<T> source, Func<T, Vector3> selector)
        {
            return Vector3Util.Average(source, selector);
        }
    }
}
