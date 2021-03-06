﻿using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Single related extension methods.
    /// </summary>
    public static class FloatExt
    {
        /// <summary>
        /// Determines if two floats are equal within a tolerance.
        /// </summary>
        /// <param name="source">Source number to compare.</param>
        /// <param name="other">Number to compare source against.</param>
        /// <param name="tolerance">Equality tolerance range.</param>
        /// <returns>Returns true if the two numbers are equal given a tolerance.</returns>
        public static bool Equals(this float source, float other, float tolerance)
        {
            return Mathf.Abs(source - other) <= tolerance;
        }

        /// <summary>
        /// Determines if a float is between any two float values.
        /// </summary>
        /// <param name="source">Source number to compare.</param>
        /// <param name="a">First number to compare source against. Can be min or max.</param>
        /// <param name="b">Second number to compare source against. Can be min or max.</param>
        /// <param name="inclusive">If true the min and max numbers are included in the between range.</param>
        /// <returns>Returns true if a number is between two other numbers.</returns>
        public static bool IsBetween(this float source, float a, float b, bool inclusive = true)
        {
            if (inclusive)
                return source >= Mathf.Min(a, b) && source <= Mathf.Max(a, b);

            return source > Mathf.Min(a, b) && source < Mathf.Max(a, b);
        }

        /// <summary>
        /// Determines if a float value is not infinity or NaN.
        /// </summary>
        /// <param name="source">Source float value to check.</param>
        /// <returns>Returns true if source is not infinity or NaN.</returns>
        public static bool IsReal(this float source)
        {
            return float.IsNaN(source) == false && float.IsInfinity(source) == false;
        }

        /// <summary>
        /// Normalizes a float between a new float range.
        /// </summary>
        /// <param name="source">Source float value to normalize.</param>
        /// <param name="curMin">Current minimum source value to normalize from.</param>
        /// <param name="curMax">Current maximum source value to normalize from.</param>
        /// <param name="newMin">New minimum output value to normalize to.</param>
        /// <param name="newMax">New maximum output value to normalize to.</param>
        /// <param name="clamp">If true the output value is clamped between the newMin and newMax.</param>
        /// <returns>Returns a normalized float value.</returns>
        public static float Normalize(this float source, float curMin, float curMax, float newMin, float newMax, bool clamp = false)
        {
            float t = (source - curMin) / (curMax - curMin);

            if (clamp)
                return newMin + (newMax - newMin) * Mathf.Clamp01(t);
            else
                return newMin + (newMax - newMin) * t;
        }

        /// <summary>
        /// Normalizes a float between 0 and 1.
        /// </summary>
        /// <param name="source">Source float value to normalize.</param>
        /// <param name="curMin">Current minimum source value to normalize from.</param>
        /// <param name="curMax">Current maximum source value to normalize from.</param>
        /// <param name="clamp">If true the output value is clamped between the newMin and newMax.</param>
        /// <returns>Returns a normalized float value.</returns>
        public static float Normalize(this float source, float curMin, float curMax, bool clamp = false)
        {
            return source.Normalize(curMin, curMax, 0, 1, clamp);
        }

        /// <summary>
        /// Normalizes a float between 0 and 1. Assuming current min is 0.
        /// </summary>
        /// <param name="source">Source float value to normalize.</param>
        /// <param name="curMax">Current maximum source value to normalize from.</param>
        /// <param name="clamp">If true the output value is clamped between the newMin and newMax.</param>
        /// <returns>Returns a normalized float value.</returns>
        public static float Normalize(this float source, float curMax, bool clamp = false)
        {
            return source.Normalize(0, curMax, 0, 1, clamp);
        }

        /// <summary>
        /// Performs a modulo operation on a float.
        /// Note: '%' is a remainder operator and NOT a modulo operator.
        /// </summary>
        /// <param name="value">Float value to perform a modulo operation on.</param>
        /// <param name="mod">Modulo value.</param>
        /// <returns>Returns the modulo result from the operation.</returns>
        public static float Modulo(this float value, float mod)
        {
            return MathUtil.Modulo(value, mod);
        }

        /// <summary>
        /// Returns a float rounded to the nearest factor of a value.
        /// </summary>
        /// <param name="source">Source flaot value to round.</param>
        /// <param name="nearest">Value to round the source to.</param>
        /// <returns>Returns value rounded to the nearest arbitrary value.</returns>
        public static float RoundToNearest(this float source, float nearest)
        {
            float inverse = Mathf.Pow(nearest, -1);

            return (float)(System.Math.Round(source * inverse, System.MidpointRounding.AwayFromZero) / inverse);
        }

        /// <summary>
        /// Returns a float rounded to the nearest factor of a value with an offset.
        /// </summary>
        /// <param name="source">Source flaot value to round.</param>
        /// <param name="nearest">Value to round the source to.</param>
        /// <param name="zeroOffset">Amount to offset the nearest value by.</param>
        /// <returns>Returns value rounded to the nearest arbitrary value.</returns>
        public static float RoundToNearest(this float source, float nearest, float zeroOffset)
        {
            float roundedSouce = RoundToNearest(source, nearest);
            float offset = zeroOffset % nearest;

            return roundedSouce + offset;
        }
    }
}