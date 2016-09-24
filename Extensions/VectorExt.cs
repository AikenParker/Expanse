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
        /// <summary>
        /// 
        /// </summary>
        public static Vector3 ZeroValues(this Vector3 source, Vector3Dim dims)
        {
            if (dims.IsFlagSet(Vector3Dim.X))
                source.x = 0;
            if (dims.IsFlagSet(Vector3Dim.Y))
                source.y = 0;
            if (dims.IsFlagSet(Vector3Dim.Z))
                source.z = 0;
            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 SetValues(this Vector3 source, float value, Vector3Dim dims = Vector3Dim.XYZ)
        {
            if (dims.IsFlagSet(Vector3Dim.X))
                source.x = value;
            if (dims.IsFlagSet(Vector3Dim.Y))
                source.y = value;
            if (dims.IsFlagSet(Vector3Dim.Z))
                source.z = value;
            return source;
        }

        public static Vector2 ToVector2(this Vector3 source, Vector3Dim ignoreDim = Vector3Dim.Z)
        {
            if (ignoreDim.IsFlagSet(Vector3Dim.Z))
                return new Vector2(source.x, source.y);
            else if (ignoreDim.IsFlagSet(Vector3Dim.Y))
                return new Vector2(source.x, source.z);
            else if (ignoreDim.IsFlagSet(Vector3Dim.X))
                return new Vector2(source.y, source.z);
            else
                throw new ArgumentException("ignoreDim");
        }
    }
}
