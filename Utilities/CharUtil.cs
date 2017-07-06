using System;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of System.Char related utility functionality.
    /// </summary>
    public static class CharUtil
    {
        /// <summary>
        /// Converts a digit into a character.
        /// </summary>
        /// <param name="digit">Digit to convert to char.</param>
        /// <returns>Char representation of the digit.</returns>
        public static char DigitToChar(int digit)
        {
            switch (digit)
            {
                case 0: return '0';
                case 1: return '1';
                case 2: return '2';
                case 3: return '3';
                case 4: return '4';
                case 5: return '5';
                case 6: return '6';
                case 7: return '7';
                case 8: return '8';
                case 9: return '9';
                default:
                    throw new InvalidArgumentException("digit");
            }
        }

        /// <summary>
        /// Converts a character into a numerical value.
        /// </summary>
        /// <param name="char">The character to convert to a numerical value.</param>
        /// <returns>Returns the numerical value of a the character.</returns>
        public static int CharToDigit(char @char)
        {
            switch (@char)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                default:
                    throw new InvalidArgumentException("@char");
            }
        }

        /// <summary>
        /// Converts a hex digit into a character.
        /// </summary>
        /// <param name="digit">Digit to convert to char.</param>
        /// <param name="uppercase">Use uppercase or lowercase for alpha character representations.</param>
        /// <returns>Char representation of the hex digit.</returns>
        public static char HexDigitToChar(int digit, bool uppercase = true)
        {
            switch (digit)
            {
                case 0: return '0';
                case 1: return '1';
                case 2: return '2';
                case 3: return '3';
                case 4: return '4';
                case 5: return '5';
                case 6: return '6';
                case 7: return '7';
                case 8: return '8';
                case 9: return '9';
                case 0xA: return uppercase ? 'A' : 'a';
                case 0xB: return uppercase ? 'B' : 'b';
                case 0xC: return uppercase ? 'C' : 'c';
                case 0xD: return uppercase ? 'D' : 'd';
                case 0xE: return uppercase ? 'E' : 'e';
                case 0xF: return uppercase ? 'F' : 'f';
                default:
                    throw new InvalidArgumentException("digit");
            }
        }

        /// <summary>
        /// Converts a character into a numerical hex value.
        /// </summary>
        /// <param name="char">The character to convert to a numerical hex value.</param>
        /// <returns>Returns the numerical hex value of a the character.</returns>
        public static int CharToHexDigit(char @char)
        {
            switch (@char)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a':
                case 'A': return 0xA;
                case 'b':
                case 'B': return 0xB;
                case 'c':
                case 'C': return 0xC;
                case 'd':
                case 'D': return 0xD;
                case 'e':
                case 'E': return 0xE;
                case 'f':
                case 'F': return 0xF;
                default:
                    throw new InvalidArgumentException("@char");
            }
        }
    }
}
