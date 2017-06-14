using Expanse.Utilities;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.String related extension methods.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Adds spaces before capital letters in a string.
        /// <para>E.g. "AFewWords" ➔ "A Few Words"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="preserveAcronyms">If true acronyms will not be split by spaces.</param>
        /// <returns>Returns a new string with spaces added.</returns>
        public static string AddSpaces(this string source, bool preserveAcronyms = true)
        {
            return StringUtil.AddSpaces(source, preserveAcronyms);
        }

        /// <summary>
        /// Returns the given string in title case format.
        /// <para>E.g "Title Case"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in title case.</returns>
        public static string ToTitleCase(this string source)
        {
            return StringUtil.ToTitleCase(source);
        }

        /// <summary>
        /// Returns the given string in pascal case format.
        /// <para>E.g "PascalCase"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in pascal case.</returns>
        public static string ToPascalCase(this string source)
        {
            return StringUtil.ToPascalCase(source);
        }

        /// <summary>
        /// Returns the given string in camel case format.
        /// <para>E.g "camelCase"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in camel case.</returns>
        public static string ToCamelCase(this string source)
        {
            return StringUtil.ToCamelCase(source);
        }

        /// <summary>
        /// Returns the given string in to a display format.
        /// Removes underscores, adds spaces, trims ends and formats as Title Case.
        /// <para>E.g. " _twoWords " ➔ "Two Words"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in display format.</returns>
        public static string ToDisplayString(this string source)
        {
            return StringUtil.ToDisplayString(source);
        }

        /// <summary>
        /// Removes all white space characters until there is only one.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string without multiple whitespace characters in a row.</returns>
        public static string WithoutMultiWhiteSpace(this string source)
        {
            return StringUtil.WithoutMultiWhiteSpace(source);
        }

        /// <summary>
        /// Removes all white space characters.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string without any whitespace characters.</returns>
        public static string WithoutAllWhiteSpace(this string source)
        {
            return StringUtil.WithoutAllWhiteSpace(source);
        }
    }
}
