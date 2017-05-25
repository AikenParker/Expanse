using System.Text;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.String related extension methods.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Adds spaces before capital letters in a string.
        /// E.g. "AFewWords" ➔ "A Few Words"
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
        /// E.g "Title Case"
        /// </summary>
        public static string ToTitleCase(this string source)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source.ToLower());
        }

        /// <summary>
        /// Returns the given string in pascal case format.
        /// E.g "PascalCase"
        /// </summary>
        public static string ToPascalCase(this string source)
        {
            return ToTitleCase(source).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Returns the given string in camel case format.
        /// E.g "camelCase"
        /// </summary>
        public static string ToCamelCase(this string source)
        {
            StringBuilder strBuilder = new StringBuilder(ToPascalCase(source));
            strBuilder[0] = char.ToLower(strBuilder[0]);
            return strBuilder.ToString();
        }

        /// <summary>
        /// Returns the given string in to a display format.
        /// Removes underscores, adds spaces, trims ends and formats as Title Case.
        /// E.g. " _twoWords " ➔ "Two Words"
        /// </summary>
        public static string ToDisplayString(this string source)
        {
            string str = source.Replace('_', ' ').Trim().AddSpaces().WithoutMultiWhiteSpace();
            return ToTitleCase(str);
        }

        /// <summary>
        /// Removes all white space characters until there is only one.
        /// </summary>
        public static string WithoutMultiWhiteSpace(this string source)
        {
            int sourceLength = source.Length;

            List<char> nonWhiteSpaceCharacters = new List<char>(sourceLength);
            bool lastWhiteSpace = false;

            for (int i = 0; i < sourceLength; i++)
            {
                char character = source[i];

                bool isWhiteSpace = char.IsWhiteSpace(character);

                if (!isWhiteSpace || !lastWhiteSpace)
                    nonWhiteSpaceCharacters.Add(character);

                lastWhiteSpace = isWhiteSpace;
            }

            return new string(nonWhiteSpaceCharacters.ToArray());
            //return Regex.Replace(source, @"\s+", " ");
        }

        /// <summary>
        /// Removes all white space characters.
        /// </summary>
        public static string WithoutAllWhiteSpace(this string source)
        {
            int sourceLength = source.Length;

            List<char> nonWhiteSpaceCharacters = new List<char>(sourceLength);

            for (int i = 0; i < sourceLength; i++)
            {
                char character = source[i];

                if (!char.IsWhiteSpace(character))
                    nonWhiteSpaceCharacters.Add(character);
            }

            return new string(nonWhiteSpaceCharacters.ToArray());
        }

#if UNSAFE
        /// <summary>
        /// Sets the character of index in a string.
        /// </summary>
        public static unsafe void SetCharacter(this string source, int index, char @char)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (index < 0 || index >= source.Length)
                throw new IndexOutOfRangeException("index");

            fixed (char* c = source)
            {
                c[index] = @char;
            }
        }

        /// <summary>
        /// Sets all the characters in a string to a single character.
        /// </summary>
        public static unsafe void SetAllCharacters(this string source, char @char)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            fixed (char* c = source)
            {
                for (int i = 0; i < source.Length; i++)
                    c[i] = @char;
            }
        }
#endif
    }
}
