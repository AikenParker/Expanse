using System;
using UnityEngine;

namespace Expanse.RefUtility
{
    /// <summary>
    /// Collection of Vector3 utility methods that make extensive use of passing by reference purely for the performance benefit.
    /// </summary>
    public static class Vector3RefUtil
    {
        /// <summary>
        /// Result is set equal the addition of Vector "a" and "b".
        /// </summary>
#if NET_4_6_
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void Add(ref Vector3 a, ref Vector3 b, ref Vector3 result)
        {
            result.x = a.x + b.x;
            result.y = a.y + b.y;
            result.z = a.z + b.z;
        }

        /// <summary>
        /// Vector "a" is set to equal itself plus "b".
        /// </summary>
#if NET_4_6_
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void AddOn(ref Vector3 a, ref Vector3 b)
        {
            a.x += b.x;
            a.y += b.y;
            a.z += b.z;
        }

        /// <summary>
        /// Result is set to equal Vector "a" multiplied by a float.
        /// </summary>
#if NET_4_6_
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void Multiply(ref Vector3 a, ref float multiplier, ref Vector3 result)
        {
            result.x = a.x * multiplier;
            result.y = a.y * multiplier;
            result.z = a.z * multiplier;
        }

        /// <summary>
        /// Vector "a" is set to itself multiplied by a float.
        /// </summary>
#if NET_4_6_
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void MultiplyOn(ref Vector3 a, ref float multiplier)
        {
            a.x *= multiplier;
            a.y *= multiplier;
            a.z *= multiplier;
        }
    }
}
