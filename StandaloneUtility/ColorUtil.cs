using UnityEngine;

namespace Expanse
{
    public static class ColorUtil
    {
        /// <summary>
        /// Returns a color from an RGB int hex value.
        /// </summary>
        public static Color HexToColor(int colorHex)
        {
            byte b = (byte)(colorHex & 0xFF);
            byte g = (byte)((colorHex >> 8) & 0xFF);
            byte r = (byte)((colorHex >> 16) & 0xFF);

            return new Color32(r, g, b, 255);
        }

        /// <summary>
        /// Returns a color from an RGB string hex value.
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
