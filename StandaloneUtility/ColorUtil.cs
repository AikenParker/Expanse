using System.Globalization;
using UnityEngine;

namespace Expanse
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
        /// Returns a color from an RGB uint hex value.
        /// Note: Does not support RGBA
        /// </summary>
        public static Color HexToColor(uint colorHex)
        {
            byte r = (byte)((colorHex >> 0x10) & 0xFF);
            byte g = (byte)((colorHex >> 0x8) & 0xFF);
            byte b = (byte)(colorHex & 0xFF);

            return new Color32(r, g, b, 0xFF);
        }

        /// <summary>
        /// Returns a color from an RGB or RGBA string hex value.
        /// </summary>
        public static Color HexToColor(string colorHex)
        {
            if (colorHex[0] == '#')
                colorHex = colorHex.Remove(0, 1);

            if (colorHex.Length != 6 || colorHex.Length != 8)
                throw new InvalidArgumentException("colorHex must be either 6 (RGB) or 8 (RGBA) hex characters long");

            byte r = byte.Parse(colorHex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(colorHex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(colorHex.Substring(4, 2), NumberStyles.HexNumber);
            byte a = colorHex.Length > 6 ? byte.Parse(colorHex.Substring(6, 2), NumberStyles.HexNumber) : (byte)0xFF;

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Converts a color to greyscale.
        /// </summary>
        public static Color ToGrayscale(Color color, GrayscaleMethod grayScaleMethod = GrayscaleMethod.LUMINESCENCE)
        {
            float value;

            switch (grayScaleMethod)
            {
                default:
                case GrayscaleMethod.LUMINESCENCE:
                    value = (color.r * R_LUMINOSITY) +
                        (color.g * G_LUMINOSITY) +
                        (color.b * B_LUMINOSITY);

                    break;

                case GrayscaleMethod.LIGHTNESS:
                    value = (Mathf.Min(color.r, color.g, color.b) +
                        Mathf.Max(color.r, color.g, color.b)) / 2;

                    break;

                case GrayscaleMethod.AVERAGE:
                    value = (color.r + color.g + color.b) / 3;
                    break;

                case GrayscaleMethod.UNITY:
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
            LUMINESCENCE,

            /// <summary>
            /// Averages the range lightness of a color.
            /// </summary>
            LIGHTNESS,

            /// <summary>
            /// Averages the values of a color.
            /// </summary>
            AVERAGE,

            /// <summary>
            /// Uses the Unity defined Color.grayscale value.
            /// </summary>
            UNITY
        }
    }
}
