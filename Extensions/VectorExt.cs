using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// Vector specific extension methods.
    /// </summary>
    public static class VectorExt
    {
        public static Vector3 ZeroValues(this Vector3 source, DimensionFlags dims)
        {
            if (dims.IsFlagSet(DimensionFlags.X))
                source.x = 0;
            if (dims.IsFlagSet(DimensionFlags.Y))
                source.y = 0;
            if (dims.IsFlagSet(DimensionFlags.Z))
                source.z = 0;
            return source;
        }

        public static Vector3 SetValues(this Vector3 source, float value, DimensionFlags dims = DimensionFlags.XYZ)
        {
            if (dims.IsFlagSet(DimensionFlags.X))
                source.x = value;
            if (dims.IsFlagSet(DimensionFlags.Y))
                source.y = value;
            if (dims.IsFlagSet(DimensionFlags.Z))
                source.z = value;
            return source;
        }

        public static Vector3 WithX(this Vector3 source, float x)
        {
            return new Vector3(x, source.y, source.z);
        }

        public static Vector3 WithY(this Vector3 source, float y)
        {
            return new Vector3(source.x, y, source.z);
        }

        public static Vector3 WithZ(this Vector3 source, float z)
        {
            return new Vector3(source.x, source.y, z);
        }

        public static Vector2 WithX(this Vector2 source, float x)
        {
            return new Vector2(x, source.y);
        }

        public static Vector2 WithY(this Vector2 source, float y)
        {
            return new Vector2(source.x, y);
        }

        public static Vector2 ToVector2(this Vector3 source, DimensionTypes ignoreDim = DimensionTypes.Z)
        {
            switch (ignoreDim)
            {
                case DimensionTypes.Z:
                    return new Vector2(source.x, source.y);
                case DimensionTypes.Y:
                    return new Vector2(source.x, source.z);
                case DimensionTypes.X:
                    return new Vector2(source.y, source.z);
                default:
                    throw new InvalidArgumentException("ignoreDim");
            }
        }

        public static Vector3 ToVector3(this Vector2 source, DimensionTypes ignoreDim = DimensionTypes.Z)
        {
            switch (ignoreDim)
            {
                case DimensionTypes.Z:
                    return new Vector3(source.x, source.y, 0f);
                case DimensionTypes.Y:
                    return new Vector3(source.x, 0f, source.y);
                case DimensionTypes.X:
                    return new Vector3(0f, source.y, source.x);
                default:
                    throw new InvalidArgumentException("ignoreDim");
            }
        }

        /// <summary>
        /// Rotates a point around a center point by an angle.
        /// </summary>
        /// <param name="angle">In degrees</param>
        public static Vector2 RotateAroundCenter(this Vector2 point, Vector2 center, float angle)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float cosTheta = Mathf.Cos(angleRad);
            float sinTheta = Mathf.Sin(angleRad);

            float x = cosTheta * (point.x - center.x) - sinTheta * (point.y - center.y) + center.x;
            float y = sinTheta * (point.x - center.x) + cosTheta * (point.y - center.y) + center.y;

            return new Vector2(x, y);
        }

        public static float TotalLength(this IList<Vector3> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            float totalLength = 0f;

            for (int i = 1; i < source.Count; i++)
            {
                totalLength += Vector3.Distance(source[i - 1], source[i]);
            }

            return totalLength;
        }

        public static float TotalLength(this IList<Vector2> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            float totalLength = 0f;

            for (int i = 1; i < source.Count; i++)
            {
                totalLength += Vector2.Distance(source[i - 1], source[i]);
            }

            return totalLength;
        }

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

        public static Vector3 Average<T>(this IEnumerable<T> source, Func<T, Vector3> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            return source.Select(selector).Average();
        }

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
