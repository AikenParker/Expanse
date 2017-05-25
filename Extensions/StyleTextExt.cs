using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Unity-supported, HTML-like text style markup related extension methods.
    /// </summary>
    public static class StyleTextExt
    {
        /// <summary>
        /// Surrounds string with a bold style markup.
        /// </summary>
        public static string WithBold(this string source)
        {
            return StyleTextUtil.ApplyBold(source);
        }

        /// <summary>
        /// Surrounds string with a italics style markup.
        /// </summary>
        public static string WithItalics(this string source)
        {
            return StyleTextUtil.ApplyItalics(source);
        }

        /// <summary>
        /// Surrounds string with a size style markup.
        /// </summary>
        public static string WithSize(this string source, int size = StyleTextUtil.DEFAULT_TEXT_SIZE)
        {
            return StyleTextUtil.ApplySize(source, size);
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        public static string WithColor(this string source, Color color)
        {
            return StyleTextUtil.ApplyColor(source, color);
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        public static string WithColor(this string source, string color)
        {
            return StyleTextUtil.ApplyColor(source, color);
        }
    }
}
