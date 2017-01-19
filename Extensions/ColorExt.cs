using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of Color related extension methods.
    /// </summary>
    public static class ColorExt
    {
        /// <summary>
        /// Creates a new color with a different alpha value.
        /// </summary>
        public static Color WithAlpha(this Color source, float alpha)
        {
            return new Color(source.r, source.g, source.b, alpha);
        }

        /// <summary>
        /// Creates a new color with a different RGB value.
        /// </summary>
        public static Color WithRGB(this Color source, float r, float g, float b)
        {
            return new Color(r, g, b, source.a);
        }

        /// <summary>
        /// Creates a lighter color from source.
        /// </summary>
        /// <param name="amount">Value from 0 to 1 describing how much to lighten the color</param>
        public static Color Lighten(this Color source, float amount)
        {
            return Color.Lerp(source, Color.white.WithAlpha(source.a), amount);
        }

        /// <summary>
        /// Creates a darker color from source.
        /// </summary>
        /// <param name="amount">Value from 0 to 1 describing how much to darken the color</param>
        public static Color Darken(this Color source, float amount)
        {
            return Color.Lerp(source, Color.white.WithAlpha(source.a), amount);
        }

        /// <summary>
        /// Creates a fully transparent color from source.
        /// </summary>
        public static Color Transparentize(this Color source)
        {
            return source.WithAlpha(0f);
        }

        /// <summary>
        /// Creates a more transparent color from source.
        /// </summary>
        /// <param name="amount">Value from 0 to 1 describing how much to transparentize the color</param>
        public static Color Transparentize(this Color source, float amount)
        {
            return source.WithAlpha(Mathf.Lerp(source.a, 0f, amount));
        }

        /// <summary>
        /// Creates a fully opaque color from source.
        /// </summary>
        public static Color Opaquen(this Color source)
        {
            return source.WithAlpha(1f);
        }

        /// <summary>
        /// Creates a more opaque color from source.
        /// </summary>
        /// <param name="amount">Value from 0 to 1 describing how much to opaquen the color</param>
        public static Color Opaquen(this Color source, float amount)
        {
            return source.WithAlpha(Mathf.Lerp(source.a, 1f, amount));
        }

        /// <summary>
        /// Returns the hexidecimal string of a color.
        /// </summary>
        public static string ToHexString(this Color source, bool prefixHash = false)
        {
            Color32 source32 = source;

            StringBuilder strBuilder = new StringBuilder(prefixHash ? 4 : 3);

            if (prefixHash)
                strBuilder.Append('#');

            strBuilder.Append(source32.r.ToString("X2"));
            strBuilder.Append(source32.g.ToString("X2"));
            strBuilder.Append(source32.b.ToString("X2"));

            return strBuilder.ToString();
        }

        /// <summary>
        /// Converts a color to greyscale.
        /// </summary>
        public static Color ToGrayscale(this Color source, ColorUtil.GrayscaleMethod grayscaleMethod = ColorUtil.GrayscaleMethod.LUMINESCENCE)
        {
            return ColorUtil.ToGrayscale(source, grayscaleMethod);
        }
    }
}
