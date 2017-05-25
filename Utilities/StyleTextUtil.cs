using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Unity-supported, HTML-like text style markup related utility functionality.
    /// </summary>
    public static class StyleTextUtil
    {
        internal const int DEFAULT_TEXT_SIZE = 11;

        private const string BOLD_OPEN_TAG = "<b>";
        private const string BOLD_CLOSE_TAG = "</b>";
        private const string ITALICS_OPEN_TAG = "<i>";
        private const string ITALICS_CLOSE_TAG = "</i>";
        private const string SIZE_OPEN_TAG = "<size={0}>";
        private const string SIZE_CLOSE_TAG = "</size>";
        private const string COLOR_OPEN_TAG = "<color=#{0}>";
        private const string COLOR_CLOSE_TAG = "</color>";

        /// <summary>
        /// Surrounds string with a bold style markup.
        /// </summary>
        public static string ApplyBold(string input)
        {
            return BOLD_OPEN_TAG + input + BOLD_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a italics style markup.
        /// </summary>
        public static string ApplyItalics(string input)
        {
            return ITALICS_OPEN_TAG + input + ITALICS_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a size style markup.
        /// </summary>
        public static string ApplySize(string input, int size = DEFAULT_TEXT_SIZE)
        {
            return string.Format(SIZE_OPEN_TAG, size) + input + SIZE_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        public static string ApplyColor(string input, Color color)
        {
            return string.Format(COLOR_OPEN_TAG, color.ToHexString()) + input + COLOR_CLOSE_TAG;
        }

        /// <summary>
        /// Surrounds string with a color style markup.
        /// </summary>
        public static string ApplyColor(string input, string color)
        {
            return string.Format(COLOR_OPEN_TAG, color) + input + COLOR_CLOSE_TAG;
        }
    }
}
