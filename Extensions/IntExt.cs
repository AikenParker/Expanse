using UnityEngine;
using System.Collections;

namespace Expanse
{
    public static class IntExt
    {
        /// <summary>
        /// Determines if two ints are equal within a tolerance.
        /// </summary>
        public static bool Equals(this int source, int other, int tolerance)
        {
            return Mathf.Abs(source - other) <= tolerance;
        }

        /// <summary>
        /// Determines if an int is between any two int values.
        /// </summary>
        public static bool IsBetween(this int source, int a, int b, bool inclusive = true)
        {
            if (inclusive)
                return source >= Mathf.Min(a, b) && source <= Mathf.Max(a, b);

            return source > Mathf.Min(a, b) && source < Mathf.Max(a, b);
        }

        /// <summary>
        /// Applies a negative modulus operation to a float.
        /// </summary>
        public static float PositiveMod(this int value, int mod)
        {
            return (value % mod + mod) % mod;
        }

        /// <summary>
        /// Reverses the digit order of an integer.
        /// </summary>
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
