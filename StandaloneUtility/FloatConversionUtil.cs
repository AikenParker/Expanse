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
        public static int ConvertToInt(float value, FloatConversionMethod floatConversionMethod, bool @checked = true)
        {
            if (@checked)
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return checked(Mathf.RoundToInt(value));
                    case FloatConversionMethod.FLOOR:
                        return checked(Mathf.FloorToInt(value));
                    case FloatConversionMethod.CEIL:
                        return checked(Mathf.CeilToInt(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return unchecked(Mathf.RoundToInt(value));
                    case FloatConversionMethod.FLOOR:
                        return unchecked(Mathf.FloorToInt(value));
                    case FloatConversionMethod.CEIL:
                        return unchecked(Mathf.CeilToInt(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }

        /// <summary>
        /// Converts a double value to an int using a float conversion method.
        /// </summary>
        public static int ConvertToInt(double value, FloatConversionMethod floatConversionMethod, bool @checked = true)
        {
            if (@checked)
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return checked((int)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return checked((int)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return checked((int)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return unchecked((int)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return unchecked((int)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return unchecked((int)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }

        /// <summary>
        /// Converts a float value to a long using a float conversion method.
        /// </summary>
        public static long ConvertToLong(float value, FloatConversionMethod floatConversionMethod, bool @checked = true)
        {
            if (@checked)
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return checked((long)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return checked((long)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return checked((long)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return unchecked((long)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return unchecked((long)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return unchecked((long)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }

        /// <summary>
        /// Converts a double value to a long using a float conversion method.
        /// </summary>
        public static long ConvertToLong(double value, FloatConversionMethod floatConversionMethod, bool @checked = true)
        {
            if (@checked)
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return checked((long)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return checked((long)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return checked((long)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return unchecked((long)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return unchecked((long)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return unchecked((long)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }

        /// <summary>
        /// Converts a float value to a short using a float conversion method.
        /// </summary>
        public static short ConvertToShort(float value, FloatConversionMethod floatConversionMethod, bool @checked = true)
        {
            if (@checked)
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return checked((short)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return checked((short)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return checked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return unchecked((short)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return unchecked((short)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return unchecked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }

        /// <summary>
        /// Converts a double value to a short using a float conversion method.
        /// </summary>
        public static short ConvertToShort(double value, FloatConversionMethod floatConversionMethod, bool @checked = true)
        {
            if (@checked)
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return checked((short)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return checked((short)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return checked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.ROUND:
                        return unchecked((short)Math.Round(value));
                    case FloatConversionMethod.FLOOR:
                        return unchecked((short)Math.Floor(value));
                    case FloatConversionMethod.CEIL:
                        return unchecked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }
    }
}
