using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

public static class VectorUtil
{
    /// <summary>
    /// 
    /// </summary>
    public static Vector3 Create(float value, Vector3Dim dims = Vector3Dim.XYZ)
    {
        Vector3 vec = Vector3.zero;
        if (dims.IsFlagSet(Vector3Dim.X))
            vec.x = value;
        if (dims.IsFlagSet(Vector3Dim.Y))
            vec.y = value;
        if (dims.IsFlagSet(Vector3Dim.Z))
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
    /// Determines the angle between 2 Vector3s.
    /// </summary>
    public static float AngleBetweenTwoPoints(Vector3 vecA, Vector3 vecB)
    {
        return Mathf.Acos(Vector3.Dot(vecA, vecB) / (vecA.magnitude * vecB.magnitude));
    }

    /// <summary>
    /// Determines the angle between 3 Vector3s.
    /// </summary>
    public static float AngleBetweenThreePoints(Vector3 vecA, Vector3 vecB, Vector3 vecC)
    {
        return AngleBetweenTwoPoints(vecA - vecC, vecB - vecC);
    }
}

[Flags]
public enum Vector3Dim
{
    NONE = 0,
    X = 1 << 0,
    Y = 1 << 1,
    Z = 1 << 2,
    XZ = X | Z,
    XY = X | Y,
    XYZ = X | Y | Z
}