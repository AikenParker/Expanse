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
        /// <param name="source">Source string value.</param>
        /// <returns>Returns the source string encapsulated in open and closed bold tags.</returns>
        public static string WithBold(this string source)
        {
            return StyleTextUtil.ApplyBold(source);
        }

        /// <summary>
        /// Surrounds string with a italics style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <returns>Returns the source string encapsulated in open and closed italics tags.</returns>
        public static string WithItalics(this string source)
        {
            return StyleTextUtil.ApplyItalics(source);
        }

        /// <summary>
        /// Surrounds string with a size style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <param name="size">Size of the text.</param>
        /// <returns>Returns the source string encapsulated in open and closed size tags.</returns>
        public static string WithSize(this string source, int size = StyleTextUtil.DEFAULT_TEXT_SIZE)
        {
            return StyleTextUtil.ApplySize(source, size);
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <param name="color">Color of the text.</param>
        /// <returns>Returns the source string encapsulated in open and closed color tags.</returns>
        public static string WithColor(this string source, Color color)
        {
            return StyleTextUtil.ApplyColor(source, color);
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <param name="color">Color of the text in hex value.</param>
        /// <returns>Returns the source string encapsulated in open and closed color tags.</returns>
        public static string WithColor(this string source, string color)
        {
            return StyleTextUtil.ApplyColor(source, color);
        }
    }
}
