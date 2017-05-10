using System;
using UnityEngine;

namespace Expanse
{
    public enum FloatConversionMethod
    {
        ROUND = 0,
        FLOOR,
        CEIL,
    }

    /// <summary>
    /// A collection of float conversion related utility functionality.
    /// </summary>
    public static class FloatConversionUtil
    {
        /// <summary>
        /// Converts a float value to an int using a float conversion method.
        /// </summary>
        public static int ConvertToInt(float value, FloatConversionMethod floatConversionMethod)
        {
            switch (floatConversionMethod)
            {
                case FloatConversionMethod.ROUND:
                    return Mathf.RoundToInt(value);
                case FloatConversionMethod.FLOOR:
                    return Mathf.FloorToInt(value);
                case FloatConversionMethod.CEIL:
                    return Mathf.CeilToInt(value);
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }

        /// <summary>
        /// Converts a float value to an long using a float conversion method.
        /// </summary>
        public static long ConvertToLong(float value, FloatConversionMethod floatConversionMethod)
        {
            switch (floatConversionMethod)
            {
                case FloatConversionMethod.ROUND:
                    return (long)Math.Round(value);
                case FloatConversionMethod.FLOOR:
                    return (long)Math.Floor(value);
                case FloatConversionMethod.CEIL:
                    return (long)Math.Ceiling(value);
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }
    }
}
