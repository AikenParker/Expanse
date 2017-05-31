using System;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Color related utility functionality.
    /// </summary>
    public static class ColorUtil
    {
        /// <summary>
        /// The perceived luminance of red.
        /// </summary>
        public const float R_LUMINOSITY = 0.2126f;

        /// <summary>
        /// The perceived luminance of green.
        /// </summary>
        public const float G_LUMINOSITY = 0.7152f;

        /// <summary>
        /// The perceived luminance of blue.
        /// </summary>
        public const float B_LUMINOSITY = 0.0722f;

        /// <summary>
        /// Returns a color from an RGBA uint value.
        /// </summary>
        /// <param name="value">Color in RGBA int form. (E.g 0xFFFFFFFF)</param>
        /// <returns>Color value converted from int value.</returns>
        public static Color RGBAIntToColor(uint value)
        {
            byte r = (byte)((value >> 24) & 0xFF);
            byte g = (byte)((value >> 16) & 0xFF);
            byte b = (byte)((value >> 08) & 0xFF);
            byte a = (byte)((value >> 00) & 0xFF);

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Returns a color from an RGB uint value.
        /// </summary>
        /// <param name="value">Color in RGB int form. (E.g 0xFFFFFF)</param>
        /// <param name="alpha">Alpha value in byte form. (0-255)(E.g 0xFF)</param>
        /// <returns>Color value converted from int value.</returns>
        public static Color RGBIntToColor(uint value, byte alpha = 0xFF)
        {
            byte r = (byte)((value >> 16) & 0xFF);
            byte g = (byte)((value >> 08) & 0xFF);
            byte b = (byte)((value >> 00) & 0xFF);

            return new Color32(r, g, b, alpha);
        }

        /// <summary>
        /// Returns a color from an RGB or RGBA string hex value.
        /// </summary>
        /// <param name="hexString">Color hex value in string form. (E.g "#FFFFFF" or "FFFFFF")</param>
        /// <returns>Color value from colorHex.</returns>
        public static Color HexStringToColor(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                throw new ArgumentNullException("hexString");

            int length = hexString.Length;
            int offset = hexString[0] == '#' ? 1 : 0;

            if (length - offset != 6 && length - offset != 8)
                throw new InvalidArgumentException("hexString must be either 6 (RGB) or 8 (RGBA) hex characters long");

            byte r = (byte)(CharUtil.CharToHexDigit(hexString[offset + 0]) * 0x10 + CharUtil.CharToHexDigit(hexString[offset + 1]));
            byte g = (byte)(CharUtil.CharToHexDigit(hexString[offset + 2]) * 0x10 + CharUtil.CharToHexDigit(hexString[offset + 3]));
            byte b = (byte)(CharUtil.CharToHexDigit(hexString[offset + 4]) * 0x10 + CharUtil.CharToHexDigit(hexString[offset + 5]));
            byte a = 0xFF;

            if (length - offset == 8)
                a = (byte)(CharUtil.CharToHexDigit(hexString[offset + 6]) * 0x10 + CharUtil.CharToHexDigit(hexString[offset + 7]));

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Converts a color to greyscale.
        /// </summary>
        /// <param name="color">Color to convert to grayscale.</param>
        /// <param name="grayScaleMethod">Method to use to convert color to greyscale.</param>
        /// <returns>Greyscale color of color.</returns>
        public static Color ToGrayscale(Color color, GrayscaleMethod grayScaleMethod = GrayscaleMethod.Luminescence)
        {
            float value;

            switch (grayScaleMethod)
            {
                default:
                case GrayscaleMethod.Luminescence:
                    value = (color.r * R_LUMINOSITY) +
                        (color.g * G_LUMINOSITY) +
                        (color.b * B_LUMINOSITY);

                    break;

                case GrayscaleMethod.Lightness:
                    value = (Mathf.Min(color.r, color.g, color.b) +
                        Mathf.Max(color.r, color.g, color.b)) / 2;

                    break;

                case GrayscaleMethod.Average:
                    value = (color.r + color.g + color.b) / 3;
                    break;

                case GrayscaleMethod.Unity:
                    value = color.grayscale;
                    break;
            }

            return new Color(value, value, value, color.a);
        }

        public enum GrayscaleMethod
        {
            /// <summary>
            /// Averages the perceived luminance of a color.
            /// </summary>
            Luminescence,

            /// <summary>
            /// Averages the range lightness of a color.
            /// </summary>
            Lightness,

            /// <summary>
            /// Averages the values of a color.
            /// </summary>
            Average,

            /// <summary>
            /// Uses the Unity defined Color.grayscale value.
            /// </summary>
            Unity
        }
    }
}
