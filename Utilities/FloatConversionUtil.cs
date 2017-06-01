using System;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Method types used to convert floating-point values into integer values.
    /// </summary>
    public enum FloatConversionMethod : byte
    {
        /// <summary>
        /// Rounds to the closest integer.
        /// </summary>
        Round = 0,

        /// <summary>
        /// Floors to the closest integer.
        /// </summary>
        Floor,

        /// <summary>
        /// Ceilings to the closest integer.
        /// </summary>
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
        /// <param name="value">Float value to convert into an int.</param>
        /// <param name="method">Conversion method to use to convert the float into an int.</param>
        /// <param name="checked">If true range checking will be used and exceptions may be thrown.</param>
        /// <returns>Returns an int that was converted from a float.</returns>
        public static int ConvertToInt(float value, FloatConversionMethod method, bool @checked = true)
        {
            switch (method)
            {
                case FloatConversionMethod.Round:
                    return @checked ? checked(Mathf.RoundToInt(value)) : unchecked(Mathf.RoundToInt(value));
                case FloatConversionMethod.Floor:
                    return @checked ? checked(Mathf.FloorToInt(value)) : unchecked(Mathf.FloorToInt(value));
                case FloatConversionMethod.Ceil:
                    return @checked ? checked(Mathf.CeilToInt(value)) : unchecked(Mathf.CeilToInt(value));
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }

        /// <summary>
        /// Converts a double value to an int using a float conversion method.
        /// </summary>
        /// <param name="value">Double value to convert into an int.</param>
        /// <param name="method">Conversion method to use to convert the double into an int.</param>
        /// <param name="checked">If true range checking will be used and exceptions may be thrown.</param>
        /// <returns>Returns an int that was converted from a double.</returns>
        public static int ConvertToInt(double value, FloatConversionMethod method, bool @checked = true)
        {
            switch (method)
            {
                case FloatConversionMethod.Round:
                    return @checked ? checked((int)Math.Round(value)) : unchecked((int)Math.Round(value)); ;
                case FloatConversionMethod.Floor:
                    return @checked ? checked((int)Math.Floor(value)) : unchecked((int)Math.Floor(value)); ;
                case FloatConversionMethod.Ceil:
                    return @checked ? checked((int)Math.Ceiling(value)) : unchecked((int)Math.Ceiling(value)); ;
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }

        /// <summary>
        /// Converts a float value to a long using a float conversion method.
        /// </summary>
        /// <param name="value">Float value to convert into a long.</param>
        /// <param name="method">Conversion method to use to convert the float into a long.</param>
        /// <param name="checked">If true range checking will be used and exceptions may be thrown.</param>
        /// <returns>Returns a long that was converted from a float.</returns>
        public static long ConvertToLong(float value, FloatConversionMethod method, bool @checked = true)
        {
            switch (method)
            {
                case FloatConversionMethod.Round:
                    return @checked ? checked((long)Math.Round(value)) : unchecked((long)Math.Round(value));
                case FloatConversionMethod.Floor:
                    return @checked ? checked((long)Math.Floor(value)) : unchecked((long)Math.Floor(value));
                case FloatConversionMethod.Ceil:
                    return @checked ? checked((long)Math.Ceiling(value)) : unchecked((long)Math.Ceiling(value));
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }

        /// <summary>
        /// Converts a double value to a long using a float conversion method.
        /// </summary>
        /// <param name="value">Double value to convert into a long.</param>
        /// <param name="method">Conversion method to use to convert the double into a long.</param>
        /// <param name="checked">If true range checking will be used and exceptions may be thrown.</param>
        /// <returns>Returns a long that was converted from a double.</returns>
        public static long ConvertToLong(double value, FloatConversionMethod method, bool @checked = true)
        {
            switch (method)
            {
                case FloatConversionMethod.Round:
                    return @checked ? checked((long)Math.Round(value)) : unchecked((long)Math.Round(value));
                case FloatConversionMethod.Floor:
                    return @checked ? checked((long)Math.Floor(value)) : unchecked((long)Math.Floor(value));
                case FloatConversionMethod.Ceil:
                    return @checked ? checked((long)Math.Ceiling(value)) : unchecked((long)Math.Ceiling(value));
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }

        /// <summary>
        /// Converts a float value to a short using a float conversion method.
        /// </summary>
        /// <param name="value">Float value to convert into a short.</param>
        /// <param name="method">Conversion method to use to convert the float into a short.</param>
        /// <param name="checked">If true range checking will be used and exceptions may be thrown.</param>
        /// <returns>Returns a short that was converted from a float.</returns>
        public static short ConvertToShort(float value, FloatConversionMethod method, bool @checked = true)
        {
            switch (method)
            {
                case FloatConversionMethod.Round:
                    return @checked ? checked((short)Math.Round(value)) : unchecked((short)Math.Round(value));
                case FloatConversionMethod.Floor:
                    return @checked ? checked((short)Math.Floor(value)) : unchecked((short)Math.Floor(value));
                case FloatConversionMethod.Ceil:
                    return @checked ? checked((short)Math.Ceiling(value)) : unchecked((short)Math.Ceiling(value));
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }

        /// <summary>
        /// Converts a double value to a short using a float conversion method.
        /// </summary>
        /// <param name="value">Double value to convert into a short.</param>
        /// <param name="method">Conversion method to use to convert the double into a short.</param>
        /// <param name="checked">If true range checking will be used and exceptions may be thrown.</param>
        /// <returns>Returns a short that was converted from a double.</returns>
        public static short ConvertToShort(double value, FloatConversionMethod method, bool @checked = true)
        {
            switch (method)
            {
                case FloatConversionMethod.Round:
                    return @checked ? checked((short)Math.Round(value)) : unchecked((short)Math.Round(value));
                case FloatConversionMethod.Floor:
                    return @checked ? checked((short)Math.Floor(value)) : unchecked((short)Math.Floor(value));
                case FloatConversionMethod.Ceil:
                    return @checked ? checked((short)Math.Ceiling(value)) : unchecked((short)Math.Ceiling(value));
                default:
                    throw new UnexpectedException("floatConversionMethod");
            }
        }
    }
}
