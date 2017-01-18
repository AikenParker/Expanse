using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// A collection of Vector3 related extension methods.
    /// </summary>
    public static class Vector4Ext
    {
        /// <summary>
        /// Returns the same Vector with selected components zeroed out.
        /// </summary>
        public static Vector4 ZeroValues(this Vector4 source, DimensionFlags4D dims)
        {
            if (dims.IsFlagSet(DimensionFlags4D.X))
                source.x = 0;
            if (dims.IsFlagSet(DimensionFlags4D.Y))
                source.y = 0;
            if (dims.IsFlagSet(DimensionFlags4D.Z))
                source.z = 0;
            if (dims.IsFlagSet(DimensionFlags4D.W))
                source.w = 0;

            return source;
        }

        /// <summary>
        /// Sets selected components of a Vector to a value.
        /// </summary>
        public static Vector4 SetValues(this Vector4 source, float value, DimensionFlags4D dims = DimensionFlags4D.XYZW)
        {
            if (dims.IsFlagSet(DimensionFlags4D.X))
                source.x = value;
            if (dims.IsFlagSet(DimensionFlags4D.Y))
                source.y = value;
            if (dims.IsFlagSet(DimensionFlags4D.Z))
                source.z = value;
            if (dims.IsFlagSet(DimensionFlags4D.W))
                source.w = value;

            return source;
        }

        /// <summary>
        /// Creates a new Vector with set X.
        /// </summary>
        public static Vector4 WithX(this Vector4 source, float x)
        {
            return new Vector4(x, source.y, source.z, source.w);
        }

        /// <summary>
        /// Creates a new Vector with set Y.
        /// </summary>
        public static Vector4 WithY(this Vector4 source, float y)
        {
            return new Vector4(source.x, y, source.z, source.w);
        }

        /// <summary>
        /// Creates a new Vector with set Z.
        /// </summary>
        public static Vector4 WithZ(this Vector4 source, float z)
        {
            return new Vector4(source.x, source.y, z, source.w);
        }

        /// <summary>
        /// Creates a new Vector with set W.
        /// </summary>
        public static Vector4 WithW(this Vector4 source, float w)
        {
            return new Vector4(source.x, source.y, source.z, w);
        }

        /// <summary>
        /// Converts a Vector4 into Vector3 by ignoring a selected dimension value.
        /// </summary>
        public static Vector3 ToVector3(this Vector4 source, DimensionTypes4D ignoreDim = DimensionTypes4D.W)
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
        public static float CalculateTotalLength(this IList<Vector4> source)
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
        public static Vector4 Average(this IEnumerable<Vector4> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            Vector4 sum = Vector4.zero;
            long count = 0;

            checked
            {
                foreach (Vector4 elem in source)
                {
                    sum += elem;
                    count++;
                }
            }

            if (count > 0)
                return sum / count;

            return Vector4.zero;
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        public static Vector4 Average<T>(this IEnumerable<T> source, Func<T, Vector4> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            return source.Select(selector).Average();
        }
    }
}
