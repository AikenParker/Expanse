using System.Text;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Color related extension methods.
    /// </summary>
    public static class ColorExt
    {
        /// <summary>
        /// Creates a new color with a different alpha value.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="alpha">Alpha value to be applied to the new color.</param>
        /// <returns>Returns a new color with a different alpha value.</returns>
        public static Color WithAlpha(this Color source, float alpha)
        {
            return ColorUtil.WithAlpha(source, alpha);
        }

        /// <summary>
        /// Creates a new color with a different RGB value.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="r">Red value to be applied to the new color.</param>
        /// <param name="g">Green value to be applied to the new color.</param>
        /// <param name="b">Blue value to be applied to the new color.</param>
        /// <returns>Returns a new color with different RGB values.</returns>
        public static Color WithRGB(this Color source, float r, float g, float b)
        {
            return ColorUtil.WithRGB(source, r, g, b);
        }

        /// <summary>
        /// Creates a lighter color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to lighten the color</param>
        /// <returns>Returns a lighter color from the source color.</returns>
        public static Color Lighten(this Color source, float amount)
        {
            return ColorUtil.Lighten(source, amount);
        }

        /// <summary>
        /// Creates a darker color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to darken the color</param>
        /// <returns>Returns a darker color from the source color.</returns>
        public static Color Darken(this Color source, float amount)
        {
            return ColorUtil.Darken(source, amount);
        }

        /// <summary>
        /// Creates a fully transparent color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <returns>Returns a fully transparent color from the source color.</returns>
        public static Color Transparentize(this Color source)
        {
            return ColorUtil.Transparentize(source);
        }

        /// <summary>
        /// Creates a more transparent color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to transparentize the color.</param>
        /// <returns>Returns a more transparent color from the source color.</returns>
        public static Color Transparentize(this Color source, float amount)
        {
            return ColorUtil.Transparentize(source, amount);
        }

        /// <summary>
        /// Creates a fully opaque color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <returns>Returns a fully opaque color from the source color.</returns>
        public static Color Opaquen(this Color source)
        {
            return ColorUtil.Opaquen(source);
        }

        /// <summary>
        /// Creates a more opaque color from the source color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="amount">Value from 0 to 1 describing how much to opaquen the color</param>
        /// <returns>Returns a more opaque color from the source color.</returns>
        public static Color Opaquen(this Color source, float amount)
        {
            return ColorUtil.Opaquen(source, amount);
        }

        /// <summary>
        /// Converts a color into a hexdecimal string representation of a color.
        /// </summary>
        /// <param name="source">Source color value.</param>
        /// <param name="prefixHash">If true '#' character will be prefixed to the hex string.</param>
        /// <returns>Returns the hexidecimal string representation of a color.</returns>
        public static string ToHexString(this Color source, bool prefixHash = false)
        {
            return ColorUtil.ToHexString(source, prefixHash);
        }

        /// <summary>
        /// Converts a color to greyscale.
        /// </summary>
        /// <param name="source">Source color value.</param>
        public static Color ToGrayscale(this Color source, ColorUtil.GrayscaleMethod grayscaleMethod = ColorUtil.GrayscaleMethod.Luminescence)
        {
            return ColorUtil.ToGrayscale(source, grayscaleMethod);
        }
    }
}
