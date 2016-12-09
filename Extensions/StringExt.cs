using UnityEngine;
using System.Collections;
using System.Text;

namespace Expanse
{
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
    }
}
