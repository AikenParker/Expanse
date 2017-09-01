using System;
using System.Collections.Generic;
using Expanse.Extensions;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of math related utility functionality.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// Performs a modulo operation on a float.
        /// Note: '%' is a remainder operator and NOT a modulo operator.
        /// </summary>
        /// <param name="value">Float value to perform a modulo operation on.</param>
        /// <param name="mod">Modulo value.</param>
        /// <returns>Returns the modulo result from the operation.</returns>
        public static float Modulo(float value, float mod)
        {
            return (value % mod + mod) % mod;
        }

        /// <summary>
        /// Performs a modulo operation on a int.
        /// Note: '%' is a remainder operator and NOT a modulo operator.
        /// </summary>
        /// <param name="value">Int value to perform a modulo operation on.</param>
        /// <param name="mod">Modulo value.</param>
        /// <returns>Returns the modulo result from the operation.</returns>
        public static int Modulo(int value, int mod)
        {
            return (value % mod + mod) % mod;
        }

        /// <summary>
        /// Determines if an int value is a power of two.
        /// </summary>
        /// <param name="value">Int value to check.</param>
        /// <returns>Returns true if int value is power of two.</returns>
        public static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Calculates the next power of two value.
        /// </summary>
        /// <param name="value">Source int value.</param>
        /// <returns>Returns the next power of two value.</returns>
        public static int NextPowerOfTwo(int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }

        /// <summary>
        /// Calculates the absolute value of an int without branching.
        /// </summary>
        /// <param name="value">Source int value.</param>
        /// <returns>Returns the absolute value of an int.</returns>
        public static int Abs(int value)
        {
            int mask = value >> sizeof(int) * 8 - 1;
            return (value + mask) ^ mask;
        }

        /// <summary>
        /// Determines if 2 angles are equal to eachother within a value of tolerance.
        /// </summary>
        /// <param name="angleA">First angle to compare.</param>
        /// <param name="angleB">Seconds angle to compare.</param>
        /// <param name="tolerance">Degree of angle tolerance to be considered equal.</param>
        /// <returns>Returns true if both angles are equal within a specified tolerance.</returns>
        public static bool CompareAngleWithTolerance(float angleA, float angleB, float tolerance)
        {
            float a = angleA.Modulo(360);
            float b = angleB.Modulo(360);
            return Mathf.Abs(a - b) <= tolerance;
        }

        /// <summary>
        /// Calculates the standard deviation of a list of floats.
        /// </summary>
        /// <param name="values">List of float values.</param>
        /// <returns>Returns the standard deviation of a list of float values.</returns>
        public static float CalculateStandardDeviation(this IList<float> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            int count = values.Count;

            double sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }
            double average = sum / count;

            double sumPow = 0;
            for (int i = 0; i < count; i++)
            {
                sumPow += Math.Pow(values[i] - average, 2);
            }
            double averagePow = sumPow / count;

            return (float)Math.Sqrt(averagePow);
        }
    }
}