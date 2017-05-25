using UnityEngine;
using System;
using Expanse.Extensions;

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
        public static Vector2 Create(float value, DimensionFlags2D dims = DimensionFlags2D.XY)
        {
            Vector2 vec = Vector2.zero;
            if (dims.HasFlag(DimensionFlags2D.X))
                vec.x = value;
            if (dims.HasFlag(DimensionFlags2D.Y))
                vec.y = value;

            return vec;
        }

        /// <summary>
        /// Determines the angle between 2 Vectors. Zero Vector acts as the center point.
        /// </summary>
        public static float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
        {
            return Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 Vectors. Vector "c" acts as the center point.
        /// </summary>
        public static float AngleBetweenThreePoints(Vector2 a, Vector2 b, Vector2 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }

        /// <summary>
        /// Rotates a point around a center point by an angle.
        /// </summary>
        /// <param name="angle">In degrees</param>
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
        /// Determines the signed angle between two vectors.
        /// </summary>
        public static float AngleSigned(Vector2 a, Vector2 b)
        {
            Vector3 v1 = a.ToVector3();
            Vector3 v2 = b.ToVector3();

            return Mathf.Atan2(Vector3.Dot(Vector3.forward, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Creates a vector from a degree angle.
        /// </summary>
        public static Vector2 DegreeToVector2(float degree, Vector2 zeroAngle)
        {
            float offsetAngle = Vector2.Angle(Vector2.right, zeroAngle.normalized);

            float radian = (degree + offsetAngle) * Mathf.Deg2Rad;

            return new Vector2(-Mathf.Cos(radian), Mathf.Sin(radian));
        }

        /// <summary>
        /// Creates a vector from a degree angle with Vector2.Right as a zero degree angle.
        /// </summary>
        public static Vector2 DegreeToVector2(float degree)
        {
            return DegreeToVector2(degree, Vector2.right);
        }
    }

    public enum DimensionTypes2D
    {
        X = 1,
        Y = 2
    }

    [Flags]
    public enum DimensionFlags2D
    {
        NONE = 0,
        X = 1 << 0,
        Y = 1 << 1,
        XY = X | Y,
    }
}
