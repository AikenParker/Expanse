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
    }
}
