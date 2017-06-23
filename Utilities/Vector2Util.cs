using UnityEngine;
using System;
using Expanse.Extensions;
using System.Collections.Generic;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Vector2 related utility functionality.
    /// </summary>
    public static class Vector2Util
    {
        /// <summary>
        /// Creates a new Vector with specified dimensions of a set value.
        /// </summary>
        /// <param name="value">Value to set specified Vector component values to.</param>
        /// <param name="dims">Component dimensions to set the value to.</param>
        /// <returns>Returns a new Vector with set component values.</returns>
        public static Vector2 Create(float value, DimensionFlags2D dims = DimensionFlags2D.XY)
        {
            Vector2 vec = default(Vector2);

            if ((dims & DimensionFlags2D.X) != 0)
                vec.x = value;
            if ((dims & DimensionFlags2D.Y) != 0)
                vec.y = value;

            return vec;
        }

        /// <summary>
        /// Determines the angle between 2 Vectors. Zero Vector acts as the center point.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <returns>Returns the angle between the 2 Vectors.</returns>
        public static float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
        {
            return Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 Vectors. Vector "c" acts as the center point.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <param name="c">Center point Vector value.</param>
        /// <returns>Returns the angle between 3 Vectors.</returns>
        public static float AngleBetweenThreePoints(Vector2 a, Vector2 b, Vector2 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }

        /// <summary>
        /// Rotates a point around a center point by an angle.
        /// </summary>
        /// <param name="point">Vector value to rotate around the center.</param>
        /// <param name="center">Vector value that the point rotates around.</param>
        /// <param name="angle">Degrees of rotation.</param>
        public static Vector2 RotateAroundCenter(Vector2 point, Vector2 center, float angle)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float cosTheta = Mathf.Cos(angleRad);
            float sinTheta = Mathf.Sin(angleRad);

            float x = cosTheta * (point.x - center.x) - sinTheta * (point.y - center.y) + center.x;
            float y = sinTheta * (point.x - center.x) + cosTheta * (point.y - center.y) + center.y;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Determines the signed angle between two Vectors.
        /// </summary>
        /// <param name="a">First Vector value to compare.</param>
        /// <param name="b">Second Vector value to compare.</param>
        /// <returns>Returns the angle between the 2 Vectors.</returns>
        public static float AngleBetweenTwoPointsSigned(Vector2 a, Vector2 b)
        {
            Vector3 v1 = a.ToVector3();
            Vector3 v2 = b.ToVector3();

            return Mathf.Atan2(Vector3.Dot(Vector3.forward, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Creates a Vector from a degree angle.
        /// </summary>
        /// <param name="degree">Degree of rotation of the new Vector.</param>
        /// <param name="zeroAngle">Vector value acting as 0 degrees rotation.</param>
        /// <returns>Returns a new Vector with a degree of rotation.</returns>
        public static Vector2 DegreeToVector2(float degree, Vector2 zeroAngle)
        {
            float offsetAngle = Vector2.Angle(Vector2.right, zeroAngle.normalized);

            float radian = (degree + offsetAngle) * Mathf.Deg2Rad;

            return new Vector2(-Mathf.Cos(radian), Mathf.Sin(radian));
        }

        /// <summary>
        /// Creates a Vector from a degree angle with Vector2.Right as a zero degree angle.
        /// </summary>
        /// <param name="degree">Degree of rotation of the new Vector.</param>
        /// <returns>Returns a new Vector with a degree of rotation.</returns>
        public static Vector2 DegreeToVector2(float degree)
        {
            return DegreeToVector2(degree, Vector2.right);
        }

        /// <summary>
        /// Converts a Vector2 into Vector3 by ignoring a selected dimension value.
        /// </summary>
        /// <param name="source">Source Vector value.</param>
        /// <param name="ignoreDim">Component dimension to leave zeroed when converting.</param>
        /// <returns>Returns a Vector3 from a Vector2.</returns>
        public static Vector3 ToVector3(Vector2 source, DimensionTypes3D ignoreDim = DimensionTypes3D.Z)
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
        /// <param name="source">List of Vector values.</param>
        /// <returns>Returns the total length of a list of Vectors.</returns>
        public static float CalculateTotalLength(IList<Vector2> source)
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
        public static Vector2 Average(IList<Vector2> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            float x = 0, y = 0;

            for (int i = 0; i < source.Count; i++)
            {
                Vector2 vec = source[i];

                x += vec.x;
                y += vec.y;
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Calculates the average Vector from a set of Vectors selected from another set.
        /// </summary>
        /// <param name="source">List of values to get a Vector value from.</param>
        /// <param name="selector">Selects from T to get a Vector value.</param>
        /// <returns>Returns the average Vector value.</returns>
        public static Vector2 Average<T>(IList<T> source, Func<T, Vector2> selector)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (selector == null)
                throw new NullReferenceException("selector");

            float x = 0, y = 0;

            for (int i = 0; i < source.Count; i++)
            {
                Vector2 vec = selector(source[i]);

                x += vec.x;
                y += vec.y;
            }

            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Enumeration specifying the component dimension types of a Vector2.
    /// </summary>
    public enum DimensionTypes2D : byte
    {
        /// <summary>
        /// X component dimension.
        /// </summary>
        X = 1,
        /// <summary>
        /// Y component dimension.
        /// </summary>
        Y = 2
    }

    /// <summary>
    /// Enumeration specifying the component dimension flags of a Vector2.
    /// </summary>
    [Flags]
    public enum DimensionFlags2D : byte
    {
        /// <summary>
        /// No component dimensions.
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
        /// X and Y component dimensions.
        /// </summary>
        XY = X | Y,
    }
}
