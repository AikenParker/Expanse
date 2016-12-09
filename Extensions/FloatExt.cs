using UnityEngine;
using System.Collections;

namespace Expanse
{
    public static class FloatExt
    {
        /// <summary>
        /// Determines if two floats are equal within a tolerance.
        /// </summary>
        public static bool Equals(this float source, float other, float tolerance)
        {
            return Mathf.Abs(source - other) <= tolerance;
        }

        /// <summary>
        /// Determines if a float is between any two float values.
        /// </summary>
        public static bool IsBetween(this float source, float a, float b, bool inclusive = true)
        {
            if (inclusive)
                return source >= Mathf.Min(a, b) && source <= Mathf.Max(a, b);

            return source > Mathf.Min(a, b) && source < Mathf.Max(a, b);
        }

        /// <summary>
        /// Determines if a float value is not infinity or NaN.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsReal(this float source)
        {
            return float.IsNaN(source) == false && float.IsInfinity(source) == false;
        }

        /// <summary>
        /// Normalizes a float between a new float range.
        /// </summary>
        public static float Normalize(this float source, float curMin, float curMax, float newMin, float newMax, bool clamp = false)
        {
            if (clamp)
                return Mathf.Clamp(Mathf.LerpUnclamped(newMin, newMax, (source - curMin) / (curMax - curMin)), Mathf.Min(newMin, newMax), Mathf.Max(newMin, newMax));
            else
                return Mathf.LerpUnclamped(newMin, newMax, (source - curMin) / (curMax - curMin));
        }

        /// <summary>
        /// Normalizes a float between 0 and 1.
        /// </summary>
        public static float Normalize(this float source, float curMin, float curMax, bool clamp = false)
        {
            return source.Normalize(curMin, curMax, 0, 1, clamp);
        }

        /// <summary>
        /// Normalizes a float between 0 and 1. Assuming current min is 0.
        /// </summary>
        public static float Normalize(this float source, float curMax, bool clamp = false)
        {
            return source.Normalize(0, curMax, 0, 1, clamp);
        }

        /// <summary>
        /// Applies a positive modulus operation to a float.
        /// </summary>
        public static float PositiveMod(this float value, float mod)
        {
            return (value % mod + mod) % mod;
        }
    }
}