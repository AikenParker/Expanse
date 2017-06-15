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
        /// Creates a new color with a different alpha value.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="alpha">Alpha value to be applied to the new color.</param>
        /// <returns>Returns a new color with a different alpha value.</returns>
        public static Color WithAlpha(Color source, float alpha)
        {
            return new Color(source.r, source.g, source.b, alpha);
        }

        /// <summary>
        /// Creates a new color with a different RGB value.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="r">Red value to be applied to the new color.</param>
        /// <param name="g">Green value to be applied to the new color.</param>
        /// <param name="b">Blue value to be applied to the new color.</param>
        /// <returns>Returns a new color with different RGB values.</returns>
        public static Color WithRGB(Color source, float r, float g, float b)
        {
            return new Color(r, g, b, source.a);
        }

        /// <summary>
        /// Creates a lighter color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to lighten the color</param>
        /// <returns>Returns a lighter color from the source color.</returns>
        public static Color Lighten(Color source, float amount)
        {
            return Color.Lerp(source, Color.white.WithAlpha(source.a), amount);
        }

        /// <summary>
        /// Creates a darker color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to darken the color</param>
        /// <returns>Returns a darker color from the source color.</returns>
        public static Color Darken(Color source, float amount)
        {
            return Color.Lerp(source, Color.black.WithAlpha(source.a), amount);
        }

        /// <summary>
        /// Creates a fully transparent color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <returns>Returns a fully transparent color from the source color.</returns>
        public static Color Transparentize(Color source)
        {
            return source.WithAlpha(0f);
        }

        /// <summary>
        /// Creates a more transparent color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to transparentize the color.</param>
        /// <returns>Returns a more transparent color from the source color.</returns>
        public static Color Transparentize(Color source, float amount)
        {
            return source.WithAlpha(Mathf.Lerp(source.a, 0f, amount));
        }

        /// <summary>
        /// Creates a fully opaque color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <returns>Returns a fully opaque color from the source color.</returns>
        public static Color Opaquen(Color source)
        {
            return source.WithAlpha(1f);
        }

        /// <summary>
        /// Creates a more opaque color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to opaquen the color</param>
        /// <returns>Returns a more opaque color from the source color.</returns>
        public static Color Opaquen(Color source, float amount)
        {
            return source.WithAlpha(Mathf.Lerp(source.a, 1f, amount));
        }

        /// <summary>
        /// Returns a color from an RGBA uint value.
        /// </summary>
        /// <param name="value">Color in RGBA int form. (E.g 0xFFFFFFFF)</param>
        /// <returns>Color value converted from int value.</returns>
        public static Color FromRGBA(uint value)
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
        public static Color FromRGB(uint value, byte alpha = 0xFF)
        {
            byte r = (byte)((value >> 16) & 0xFF);
            byte g = (byte)((value >> 08) & 0xFF);
            byte b = (byte)((value >> 00) & 0xFF);

            return new Color32(r, g, b, alpha);
        }

        /// <summary>
        /// Converts a color into a hexdecimal string representation of a color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="prefixHash">If true '#' character will be prefixed to the hex string.</param>
        /// <returns>Returns the hexidecimal string representation of a color.</returns>
        public static string ToHexString(Color source, bool prefixHash = false)
        {
            Color32 source32 = source;
            string hexString = prefixHash ? "#000000" : "000000";
            int position = prefixHash ? 1 : 0;

            string rHex = source32.r.ToString("X2");
            string gHex = source32.g.ToString("X2");
            string bHex = source32.b.ToString("X2");

            StringUtil.SetSubString(hexString, rHex, position);
            position += 2;
            StringUtil.SetSubString(hexString, gHex, position);
            position += 2;
            StringUtil.SetSubString(hexString, bHex, position);

            return hexString;
        }

        /// <summary>
        /// Returns a color from an RGB or RGBA string hex value.
        /// </summary>
        /// <param name="hexString">Color hex value in string form. (E.g "#FFFFFF" or "FFFFFF")</param>
        /// <returns>Color value from colorHex.</returns>
        public static Color FromHexString(string hexString)
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
