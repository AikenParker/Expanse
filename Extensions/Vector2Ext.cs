using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// A collection of Vector2 related extension methods.
    /// </summary>
    public static class Vector2Ext
    {
        /// <summary>
        /// Returns the same Vector with selected components zeroed out.
        /// </summary>
        public static Vector2 ZeroValues(this Vector2 source, DimensionFlags2D dims)
        {
            if (dims.HasFlag(DimensionFlags2D.X))
                source.x = 0;
            if (dims.HasFlag(DimensionFlags2D.Y))
                source.y = 0;

            return source;
        }

        /// <summary>
        /// Sets selected components of a Vector to a value.
        /// </summary>
        public static Vector2 SetValues(this Vector2 source, float value, DimensionFlags2D dims = DimensionFlags2D.XY)
        {
            if (dims.HasFlag(DimensionFlags2D.X))
                source.x = value;
            if (dims.HasFlag(DimensionFlags2D.Y))
                source.y = value;

            return source;
        }

        /// <summary>
        /// Creates a new Vector with set X.
        /// </summary>
        public static Vector2 WithX(this Vector2 source, float x)
        {
            return new Vector2(x, source.y);
        }

        /// <summary>
        /// Creates a new Vector with set Y.
        /// </summary>
        public static Vector2 WithY(this Vector2 source, float y)
        {
            return new Vector2(source.x, y);
        }

        /// <summary>
        /// Converts a Vector2 into Vector3 by ignoring a selected dimension value.
        /// </summary>
        public static Vector3 ToVector3(this Vector2 source, DimensionTypes3D ignoreDim = DimensionTypes3D.Z)
        {
            switch (ignoreDim)
            {
                case DimensionTypes3D.Z:
                    return new Vector3(source.x, source.y, 0f);
                case DimensionTypes3D.Y:
                    return new Vector3(source.x, 0f, source.y);
                case DimensionTypes3D.X:
                    return new Vector3(0f, source.y, source.x);
                default:
                    throw new InvalidArgumentException("ignoreDim");
            }
        }

        /// <summary>
        /// Calculates the total length of a set of Vectors by added the distance to eachother sequentially.
        /// </summary>
        public static float CalculateTotalLength(this IList<Vector2> source)
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
        public static Vector2 Average(this IEnumerable<Vector2> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            Vector2 sum = Vector2.zero;
            long count = 0;

            checked
            {
                foreach (Vector2 elem in source)
                {
                    sum += elem;
                    count++;
                }
            }

            if (count > 0)
                return sum / count;

            return Vector2.zero;
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        public static Vector2 Average<T>(this IEnumerable<T> source, Func<T, Vector2> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            return source.Select(selector).Average();
        }
    }
}
