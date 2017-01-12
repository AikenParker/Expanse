using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;

namespace Expanse
{
    /// <summary>
    /// A collection of System.String related extension methods.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Adds spaces before capital letters in a string.
        /// </summary>
        public static string AddSpaces(this string source, bool preserveAcronyms = true)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            StringBuilder newText = new StringBuilder(source.Length * 2);
            newText.Append(source[0]);

            for (int i = 1; i < source.Length; i++)
            {
                if (char.IsUpper(source[i]))
                {
                    if ((source[i - 1] != ' ' && !char.IsUpper(source[i - 1])) || (preserveAcronyms && char.IsUpper(source[i - 1]) && i < source.Length - 1 && !char.IsUpper(source[i + 1])))
                        newText.Append(' ');
                }

                newText.Append(source[i]);
            }

            return newText.ToString();
        }

        /// <summary>
        /// Returns the given string in title case format.
        /// </summary>
        public static string ToTitleCase(this string source)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source.ToLower());
        }
    }
}
