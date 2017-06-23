using UnityEngine;
using System;
using System.Collections.Generic;
using Expanse.Utilities;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Vector2 related extension methods.
    /// </summary>
    public static class Vector2Ext
    {
        /// <summary>
        /// Returns the same Vector with selected components zeroed out.
        /// </summary>
        /// <param name="source">Source Vector value to zero.</param>
        /// <param name="dims">Component dimensions to zero.</param>
        /// <returns>Returns a new Vector with zeroed out component values.</returns>
        public static Vector2 ZeroValues(this Vector2 source, DimensionFlags2D dims)
        {
            if ((dims & DimensionFlags2D.X) != 0)
                source.x = 0;
            if ((dims & DimensionFlags2D.Y) != 0)
                source.y = 0;

            return source;
        }

        /// <summary>
        /// Sets selected components of a Vector to a value.
        /// </summary>
        /// <param name="source">Source Vector value to set.</param>
        /// <param name="value">Value to set to selected components on the Vector.</param>
        /// <param name="dims">Components dimensions to set.</param>
        /// <returns>Returns a new Vector with set component values.</returns>
        public static Vector2 SetValues(this Vector2 source, float value, DimensionFlags2D dims = DimensionFlags2D.XY)
        {
            if ((dims & DimensionFlags2D.X) != 0)
                source.x = value;
            if ((dims & DimensionFlags2D.Y) != 0)
                source.y = value;

            return source;
        }

        /// <summary>
        /// Creates a new Vector with set X.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="x">Value to set the X component to.</param>
        /// <returns>Returns a new Vector value with X component set.</returns>
        public static Vector2 WithX(this Vector2 source, float x)
        {
            return new Vector2(x, source.y);
        }

        /// <summary>
        /// Creates a new Vector with set Y.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="y">Value to set the Y component to.</param>
        /// <returns>Returns a new Vector value with the Y component set.</returns>
        public static Vector2 WithY(this Vector2 source, float y)
        {
            return new Vector2(source.x, y);
        }

        /// <summary>
        /// Converts a Vector2 into Vector3 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to leave zeroed when converting.</param>
        /// <returns>Returns a Vector3 from a Vector2.</returns>
        public static Vector3 ToVector3(this Vector2 source, DimensionTypes3D ignoreDim = DimensionTypes3D.Z)
        {
            return Vector2Util.ToVector3(source, ignoreDim);
        }

        /// <summary>
        /// Calculates the total length of a set of Vectors by added the distance to eachother sequentially.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the total length of a list of Vectors.</returns>
        public static float CalculateTotalLength(this IList<Vector2> source)
        {
            return Vector2Util.CalculateTotalLength(source);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors.
        /// </summary>
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector2 Average(this IList<Vector2> source)
        {
            return Vector2Util.Average(source);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        /// <param name="source">List of values to get a Vector value from.</param>
        /// <param name="selector">Selects from T to get a Vector value.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector2 Average<T>(this IList<T> source, Func<T, Vector2> selector)
        {
            return Vector2Util.Average(source, selector);
        }
    }
}
