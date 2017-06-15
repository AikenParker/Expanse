using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Expanse.Extensions;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of System.String related utility functionality.
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// Adds spaces before capital letters in a string.
        /// <para>E.g. "AFewWords" ➔ "A Few Words"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="preserveAcronyms">If true acronyms will not be split by spaces.</param>
        /// <returns>Returns a new string with spaces added.</returns>
        public static string AddSpaces(string source, bool preserveAcronyms = true)
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
        /// <para>E.g "Title Case"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in title case.</returns>
        public static string ToTitleCase(string source)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source.ToLower());
        }

        /// <summary>
        /// Returns the given string in pascal case format.
        /// <para>E.g "PascalCase"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in pascal case.</returns>
        public static string ToPascalCase(string source)
        {
            return ToTitleCase(source).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Returns the given string in camel case format.
        /// <para>E.g "camelCase"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in camel case.</returns>
        public static string ToCamelCase(string source)
        {
            StringBuilder strBuilder = new StringBuilder(ToPascalCase(source));
            strBuilder[0] = char.ToLower(strBuilder[0]);
            return strBuilder.ToString();
        }

        /// <summary>
        /// Returns the given string in to a display format.
        /// Removes underscores, adds spaces, trims ends and formats as Title Case.
        /// <para>E.g. " _twoWords " ➔ "Two Words"</para>
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string in display format.</returns>
        public static string ToDisplayString(string source)
        {
            string str = source.Replace('_', ' ').Trim().AddSpaces().WithoutMultiWhiteSpace();
            return ToTitleCase(str);
        }

        /// <summary>
        /// Removes all white space characters until there is only one.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string without multiple whitespace characters in a row.</returns>
        public static string WithoutMultiWhiteSpace(string source)
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
        }

        /// <summary>
        /// Removes all white space characters.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <returns>Returns a new string without any whitespace characters.</returns>
        public static string WithoutAllWhiteSpace(string source)
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
        /// Replaces all instances of character in a string with another character.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="oldChar">Character to be replaced.</param>
        /// <param name="newChar">Replacing character.</param>
        public static unsafe void ReplaceCharacter(string source, char oldChar, char newChar)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            int sourceLength = source.Length;

            fixed (char* s = source)
            {
                for (int i = 0; i < sourceLength; i++)
                {
                    if (s[i] == oldChar)
                        s[i] = newChar;
                }
            }
        }

        /// <summary>
        /// Replaces the first instance of character in a string with another character.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="oldChar">Character to be replaced.</param>
        /// <param name="newChar">Replacing character.</param>
        public static unsafe void ReplaceFirstCharacter(string source, char oldChar, char newChar)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            int sourceLength = source.Length;

            fixed (char* s = source)
            {
                for (int i = 0; i < sourceLength; i++)
                {
                    if (s[i] == oldChar)
                    {
                        s[i] = newChar;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Replaces all instance of the substring in a string with another substring of the same length.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="oldSubstring">Substring to be replaced.</param>
        /// <param name="newSubstring">Replacing substring.</param>
        public static unsafe void ReplaceSubstring(string source, string oldSubstring, string newSubstring)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(oldSubstring))
                throw new ArgumentNullException("oldSubstring");

            if (string.IsNullOrEmpty(newSubstring))
                throw new ArgumentNullException("newSubstring");

            int sourceLength = source.Length;
            int oldSubLength = oldSubstring.Length;
            int newSubLength = newSubstring.Length;

            if (oldSubLength != newSubLength)
                throw new InvalidArgumentException("newSubstring must have the same length as oldSubstring");

            if (oldSubLength > sourceLength || oldSubLength == 0)
                return;

            if (oldSubLength == 1)
            {
                ReplaceCharacter(source, oldSubstring[0], newSubstring[0]);
                return;
            }

            fixed (char* s = source, o = oldSubstring, n = newSubstring)
            {
                for (int i = 0; i < sourceLength - (oldSubLength - 1); i++)
                {
                    if (s[i] == o[0] && s[i + oldSubLength - 1] == o[oldSubLength - 1])
                    {
                        bool match = true;

                        for (int j = 1; j < oldSubLength - 1; j++)
                        {
                            if (s[i + j] != o[j])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            for (int j = 0; j < oldSubLength; j++)
                            {
                                s[i + j] = n[j];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replaces the first instance of a substring in a string with another substring of the same length.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="oldSubstring">Substring to be replaced.</param>
        /// <param name="newSubstring">Replacing substring.</param>
        public static unsafe void ReplaceFirstSubstring(string source, string oldSubstring, string newSubstring)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(oldSubstring))
                throw new ArgumentNullException("oldSubstring");

            if (string.IsNullOrEmpty(newSubstring))
                throw new ArgumentNullException("newSubstring");

            int sourceLength = source.Length;
            int oldSubLength = oldSubstring.Length;
            int newSubLength = newSubstring.Length;

            if (oldSubLength != newSubLength)
                throw new InvalidArgumentException("newSubstring must have the same length as oldSubstring");

            if (oldSubLength > sourceLength || oldSubLength == 0)
                return;

            if (oldSubLength == 1)
            {
                ReplaceFirstCharacter(source, oldSubstring[0], newSubstring[0]);
                return;
            }

            fixed (char* s = source, o = oldSubstring, n = newSubstring)
            {
                for (int i = 0; i < sourceLength - oldSubLength; i++)
                {
                    if (s[i] == o[0] && s[i + oldSubLength - 1] == o[oldSubLength - 1])
                    {
                        bool match = true;

                        for (int j = 1; j < oldSubLength - 1; j++)
                        {
                            if (s[i + j] != o[j])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            for (int j = 0; j < oldSubLength; j++)
                            {
                                s[i + j] = n[j];
                            }

                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the character of index in a string.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="index">Index of the character to replace.</param>
        /// <param name="char">Character to replace with.</param>
        public static unsafe void ReplaceCharacterByIndex(string source, int index, char @char)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (index < 0 || index >= source.Length)
                throw new IndexOutOfRangeException("index");

            fixed (char* s = source)
            {
                s[index] = @char;
            }
        }

        /// <summary>
        /// Sets all the characters in a string to a single character.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="char">Character to replace with.</param>
        public static unsafe void ReplaceAllCharacters(string source, char @char)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            fixed (char* s = source)
            {
                for (int i = 0; i < source.Length; i++)
                    s[i] = @char;
            }
        }

        /// <summary>
        /// Sets a substring in a string at a position.
        /// </summary>
        /// <param name="source">Source string value to modify.</param>
        /// <param name="subString">Sub string to set into the source string.</param>
        /// <param name="position">Index where the sub string will be placed into on the source string.</param>
        public static unsafe void SetSubString(string source, string subString, int position)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(subString))
                throw new ArgumentNullException("subString");

            int sourceLength = source.Length;
            int subStringLength = subString.Length;

            if (position < 0)
                throw new InvalidArgumentException("position must be greater than zero.");

            if (sourceLength < subStringLength + position)
                throw new InvalidArgumentException("subString at postion cannot fit into source.");

            fixed (char* s = source)
            {
                for (int i = 0; i < subStringLength; i++)
                    s[i + position] = subString[i];
            }
        }
#endif
    }
}
