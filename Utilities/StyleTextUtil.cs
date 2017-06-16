using System;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Unity-supported, HTML-like text style markup related utility functionality.
    /// </summary>
    public static class StyleTextUtil
    {
        public const int DEFAULT_TEXT_SIZE = 11;

        /// <summary>
        /// Opening bold tag.
        /// </summary>
        public const string BOLD_OPEN_TAG = "<b>";
        /// <summary>
        /// Closing bold tag.
        /// </summary>
        public const string BOLD_CLOSE_TAG = "</b>";
        /// <summary>
        /// Opening italics tag.
        /// </summary>
        public const string ITALICS_OPEN_TAG = "<i>";
        /// <summary>
        /// Closing italics tag.
        /// </summary>
        public const string ITALICS_CLOSE_TAG = "</i>";
        /// <summary>
        /// Opening size tag.
        /// </summary>
        public const string SIZE_OPEN_TAG = "<size={0}>";
        /// <summary>
        /// Closing size tag.
        /// </summary>
        public const string SIZE_CLOSE_TAG = "</size>";
        /// <summary>
        /// Opening color tag.
        /// </summary>
        public const string COLOR_OPEN_TAG = "<color={0}>";
        /// <summary>
        /// Closing color tag.
        /// </summary>
        public const string COLOR_CLOSE_TAG = "</color>";

        /// <summary>
        /// Surrounds string with a bold style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <returns>Returns the source string encapsulated in open and closed bold tags.</returns>
        public static string ApplyBold(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            return BOLD_OPEN_TAG + input + BOLD_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a italics style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <returns>Returns the source string encapsulated in open and closed italics tags.</returns>
        public static string ApplyItalics(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            return ITALICS_OPEN_TAG + input + ITALICS_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a size style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <param name="size">Size of the text.</param>
        /// <returns>Returns the source string encapsulated in open and closed size tags.</returns>
        public static string ApplySize(string input, int size = DEFAULT_TEXT_SIZE)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            return string.Format(SIZE_OPEN_TAG, size) + input + SIZE_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <param name="color">Color of the text.</param>
        /// <returns>Returns the source string encapsulated in open and closed color tags.</returns>
        public static string ApplyColor(string input, Color color)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            return string.Format(COLOR_OPEN_TAG, color.ToHexString(true)) + input + COLOR_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        /// <param name="source">Source string value.</param>
        /// <param name="color">Color of the text in hex value.</param>
        /// <returns>Returns the source string encapsulated in open and closed color tags.</returns>
        public static string ApplyColor(string input, string color)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            return string.Format(COLOR_OPEN_TAG, color) + input + COLOR_CLOSE_TAG;
        }
    }
}
