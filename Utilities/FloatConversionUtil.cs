using System;
using UnityEngine;

namespace Expanse.Utilities
{
    public enum FloatConversionMethod
    {
        Round = 0,
        Floor,
        Ceil,
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
                    case FloatConversionMethod.Round:
                        return checked(Mathf.RoundToInt(value));
                    case FloatConversionMethod.Floor:
                        return checked(Mathf.FloorToInt(value));
                    case FloatConversionMethod.Ceil:
                        return checked(Mathf.CeilToInt(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.Round:
                        return unchecked(Mathf.RoundToInt(value));
                    case FloatConversionMethod.Floor:
                        return unchecked(Mathf.FloorToInt(value));
                    case FloatConversionMethod.Ceil:
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
                    case FloatConversionMethod.Round:
                        return checked((int)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return checked((int)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
                        return checked((int)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.Round:
                        return unchecked((int)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return unchecked((int)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
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
                    case FloatConversionMethod.Round:
                        return checked((long)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return checked((long)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
                        return checked((long)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.Round:
                        return unchecked((long)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return unchecked((long)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
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
                    case FloatConversionMethod.Round:
                        return checked((long)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return checked((long)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
                        return checked((long)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.Round:
                        return unchecked((long)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return unchecked((long)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
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
                    case FloatConversionMethod.Round:
                        return checked((short)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return checked((short)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
                        return checked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.Round:
                        return unchecked((short)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return unchecked((short)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
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
                    case FloatConversionMethod.Round:
                        return checked((short)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return checked((short)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
                        return checked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
            else
            {
                switch (floatConversionMethod)
                {
                    case FloatConversionMethod.Round:
                        return unchecked((short)Math.Round(value));
                    case FloatConversionMethod.Floor:
                        return unchecked((short)Math.Floor(value));
                    case FloatConversionMethod.Ceil:
                        return unchecked((short)Math.Ceiling(value));
                    default:
                        throw new UnexpectedException("floatConversionMethod");
                }
            }
        }
    }
}
