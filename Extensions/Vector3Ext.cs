using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// A collection of Vector3 related extension methods.
    /// </summary>
    public static class Vector3Ext
    {
        /// <summary>
        /// Returns the same Vector with selected components zeroed out.
        /// </summary>
        public static Vector3 ZeroValues(this Vector3 source, DimensionFlags3D dims)
        {
            if (dims.IsFlagSet(DimensionFlags3D.X))
                source.x = 0;
            if (dims.IsFlagSet(DimensionFlags3D.Y))
                source.y = 0;
            if (dims.IsFlagSet(DimensionFlags3D.Z))
                source.z = 0;

            return source;
        }

        /// <summary>
        /// Sets selected components of a Vector to a value.
        /// </summary>
        public static Vector3 SetValues(this Vector3 source, float value, DimensionFlags3D dims = DimensionFlags3D.XYZ)
        {
            if (dims.IsFlagSet(DimensionFlags3D.X))
                source.x = value;
            if (dims.IsFlagSet(DimensionFlags3D.Y))
                source.y = value;
            if (dims.IsFlagSet(DimensionFlags3D.Z))
                source.z = value;

            return source;
        }

        /// <summary>
        /// Creates a new Vector with set X.
        /// </summary>
        public static Vector3 WithX(this Vector3 source, float x)
        {
            return new Vector3(x, source.y, source.z);
        }

        /// <summary>
        /// Creates a new Vector with set Y.
        /// </summary>
        public static Vector3 WithY(this Vector3 source, float y)
        {
            return new Vector3(source.x, y, source.z);
        }

        /// <summary>
        /// Creates a new Vector with set Z.
        /// </summary>
        public static Vector3 WithZ(this Vector3 source, float z)
        {
            return new Vector3(source.x, source.y, z);
        }

        /// <summary>
        /// Converts a Vector3 into Vector2 by ignoring a selected dimension value.
        /// </summary>
        public static Vector2 ToVector2(this Vector3 source, DimensionTypes3D ignoreDim = DimensionTypes3D.Z)
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
        public static Vector4 ToVector4(this Vector3 source, DimensionTypes4D ignoreDim = DimensionTypes4D.W)
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
        public static float CalculateTotalLength(this IList<Vector3> source)
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
        public static Vector3 Average(this IEnumerable<Vector3> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            Vector3 sum = Vector3.zero;
            long count = 0;

            checked
            {
                foreach (Vector3 elem in source)
                {
                    sum += elem;
                    count++;
                }
            }

            if (count > 0)
                return sum / count;

            return Vector3.zero;
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        public static Vector3 Average<T>(this IEnumerable<T> source, Func<T, Vector3> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            return source.Select(selector).Average();
        }
    }
}
