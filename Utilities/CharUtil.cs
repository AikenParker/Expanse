using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of System.Char related utility functionality.
    /// </summary>
    public static class CharUtil
    {
        /// <summary>
        /// Returns the character representation of a single digit number.
        /// </summary>
        public static char DigitToChar(int digit)
        {
            switch (digit)
            {
                case 0:
                    return '0';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                default:
                    return '\0';
            }
        }
    }
}
