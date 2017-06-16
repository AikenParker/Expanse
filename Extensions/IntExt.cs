using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Int32 related extension methods.
    /// </summary>
    public static class IntExt
    {
        /// <summary>
        /// Determines if two ints are equal within a tolerance.
        /// </summary>
        /// <param name="source">Source number to compare.</param>
        /// <param name="other">Number to compare source against.</param>
        /// <param name="tolerance">Equality tolerance range.</param>
        /// <returns>Returns true if the two numbers are equal given a tolerance.</returns>
        public static bool Equals(this int source, int other, int tolerance)
        {
            return Mathf.Abs(source - other) <= tolerance;
        }

        /// <summary>
        /// Determines if an int is between any two int values.
        /// </summary>
        /// <param name="source">Source number to compare.</param>
        /// <param name="a">First number to compare source against. Can be min or max.</param>
        /// <param name="b">Second number to compare source against. Can be min or max.</param>
        /// <param name="inclusive">If true the min and max numbers are included in the between range.</param>
        /// <returns>Returns true if a number is between two other numbers.</returns>
        public static bool IsBetween(this int source, int a, int b, bool inclusive = true)
        {
            if (inclusive)
                return source >= Mathf.Min(a, b) && source <= Mathf.Max(a, b);

            return source > Mathf.Min(a, b) && source < Mathf.Max(a, b);
        }

        /// <summary>
        /// Performs a modulo operation on a int.
        /// Note: '%' is a remainder operator and NOT a modulo operator.
        /// </summary>
        /// <param name="value">Int value to perform a modulo operation on.</param>
        /// <param name="mod">Modulo value.</param>
        /// <returns>Returns the modulo result from the operation.</returns>
        public static int Modulo(this int value, int mod)
        {
            return MathUtil.Modulo(value, mod);
        }

        /// <summary>
        /// Reverses the digit order of an integer.
        /// </summary>
        /// <param name="num">Source number to reverse.</param>
        /// <returns>Returns a new int with the digits in reverse order to num.</returns>
        public static int Reverse(this int num)
        {
            int result = 0;

            while (num > 0)
            {
                result = result * 10 + num % 10;
                num /= 10;
            }

            return result;
        }
    }
}
