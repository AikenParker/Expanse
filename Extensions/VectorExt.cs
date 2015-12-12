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
    namespace Ext
    {
        /// <summary>
        /// Vector specific extension methods.
        /// </summary>
        public static class VectorExt
        {
            /// <summary>
            /// Returns a vector 2 from a vector 3 ignoring the Y axis.
            /// </summary>
            public static Vector2 IgnoreY(this Vector3 source)
            {
                return new Vector2(source.x, source.z);
            }

            /// <summary>
            /// Zeros a vector 3's Y value.
            /// </summary>
            public static Vector3 IgnoreY(this Vector3 source, float defaultY = 0)
            {
                return new Vector3(source.x, defaultY, source.z);
            }

            /// <summary>
            /// Determines the angle between 2 Vector3s.
            /// </summary>
            public static float AngleBetweenTwoPoints(this Vector3 vecA, Vector3 vecB)
            {
                return Mathf.Acos(Vector3.Dot(vecA, vecB) / (vecA.magnitude * vecB.magnitude));
            }

            /// <summary>
            /// Determines the angle between 3 Vector3s.
            /// </summary>
            public static float AngleBetweenThreePoints(this Vector3 vecA, Vector3 vecB, Vector3 vecC)
            {
                return AngleBetweenTwoPoints(vecA - vecC, vecB - vecC);
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
        }
    }
}
