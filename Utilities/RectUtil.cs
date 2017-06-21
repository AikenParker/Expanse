using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Rect related utility functionality.
    /// </summary>
    public static class RectUtil
    {
        /// <summary>
        /// Gets a Rect value that fits two other Rect values.
        /// </summary>
        /// <param name="a">First Rect value parameter.</param>
        /// <param name="b">Second Rect value parameter.</param>
        /// <returns>Returns a new Rect that can fit both A and B Rects.</returns>
        public static Rect MaxRect(Rect a, Rect b)
        {
            float xMin = Mathf.Min(a.xMin, b.xMin);
            float yMin = Mathf.Min(a.yMin, b.yMin);
            float xMax = Mathf.Max(a.xMax, b.xMax);
            float yMax = Mathf.Max(a.yMax, b.yMax);

            Rect maxRect = new Rect();

            maxRect.xMin = xMin;
            maxRect.yMin = yMin;
            maxRect.xMax = xMax;
            maxRect.yMax = yMax;

            return maxRect;
        }

        /// <summary>
        /// Calculates the area of a Rect.
        /// </summary>
        /// <param name="rect">Source Rect value.</param>
        /// <returns>Returns the area of a Rect.</returns>
        public static float GetArea(Rect rect)
        {
            return rect.width * rect.height;
        }

        /// <summary>
        /// Determines if Rect A is within Rect B.
        /// </summary>
        /// <param name="a">Within Rect to compare.</param>
        /// <param name="b">Outside Rect to compare.</param>
        /// <returns>Returns true if Rect A is within Rect B.</returns>
        public static bool IsWithin(Rect a, Rect b)
        {
            return a.xMin >= b.xMin && a.xMax <= b.xMax && a.yMin >= b.yMin && a.yMax <= b.yMax;
        }
    }
}