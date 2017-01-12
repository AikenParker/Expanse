using UnityEngine;
using System;

namespace Expanse
{
    /// <summary>
    /// A collection of vector related utility functionality.
    /// </summary>
    public static class VectorUtil
    {
        /// <summary>
        /// Creates a new vector with specified dimensions of a set value.
        /// </summary>
        public static Vector3 Create(float value, DimensionFlags3D dims = DimensionFlags3D.XYZ)
        {
            Vector3 vec = Vector3.zero;
            if (dims.IsFlagSet(DimensionFlags3D.X))
                vec.x = value;
            if (dims.IsFlagSet(DimensionFlags3D.Y))
                vec.y = value;
            if (dims.IsFlagSet(DimensionFlags3D.Z))
                vec.z = value;
            return vec;
        }

        /// <summary>
        /// Gets a point dist units from A, in direction B.
        /// </summary>
        public static Vector3 GetPointAtDistAlongLine(Vector3 pointA, Vector3 pointB, float dist, bool fromA)
        {
            float vx = pointB.x - pointA.x;
            float vy = pointB.y - pointA.y;
            float vz = pointB.z - pointA.z;

            float mag = Mathf.Sqrt(vx * vx + vy * vy + vz * vz); // length

            vx /= mag;
            vy /= mag;
            vz /= mag;

            Vector3 point = new Vector3();
            if (fromA)
            {
                point.x = pointA.x + vx * (mag + dist);
                point.y = pointA.y + vy * (mag + dist);
                point.z = pointA.z + vz * (mag + dist);
            }
            else
            {
                point.x = pointB.x + vx * (mag + dist);
                point.y = pointB.y + vy * (mag + dist);
                point.z = pointB.z + vz * (mag + dist);
            }
            return point;
        }

        /// <summary>
        /// Determines the angle between 2 vectors. Zero vector acts as the center point.
        /// </summary>
        public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude));
        }

        /// <summary>
        /// Determines the angle between 3 vectors. Vector "c" acts as the center point.
        /// </summary>
        public static float AngleBetweenThreePoints(Vector3 a, Vector3 b, Vector3 c)
        {
            return AngleBetweenTwoPoints(a - c, b - c);
        }
    }

    public enum DimensionTypes3D
    {
        X = 1,
        Y = 2,
        Z = 3
    }

    [Flags]
    public enum DimensionFlags3D
    {
        NONE = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        XZ = X | Z,
        XY = X | Y,
        XYZ = X | Y | Z
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

    public enum DimensionTypes4D
    {
        X = 1,
        Y = 2,
        Z = 3,
        W = 4
    }

    [Flags]
    public enum DimensionFlags4D
    {
        NONE = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        W = 1 << 3,
        XYZ = X | Y | Z,
        WYZW = X | Y | Z | W
    }
}
