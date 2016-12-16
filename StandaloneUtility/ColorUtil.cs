using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class ColorUtil
    {
        /// <summary>
        /// Returns a color from a string hex value.
        /// </summary>
        public static Color HexToColor(string colorHex)
        {
            if (colorHex[0] == '#')
                colorHex = colorHex.Remove(0, 1);

            byte r = byte.Parse(colorHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(colorHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(colorHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, 255);
        }
    }
}
