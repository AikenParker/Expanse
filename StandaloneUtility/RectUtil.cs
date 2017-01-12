using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Collection of Rect related utility functionality.
    /// </summary>
    public static class RectUtil
    {
        /// <summary>
        /// Returns a Rect that encapsulates a and b.
        /// </summary>
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
        /// Returns the area of a Rect.
        /// </summary>
        public static float GetArea(Rect rect)
        {
            return rect.width * rect.height;
        }

        /// <summary>
        /// Returns true if Rect A is within Rect B.
        /// </summary>
        public static bool IsWithin(Rect a, Rect b)
        {
            return a.xMin >= b.xMin && a.xMax <= b.xMax && a.yMin >= b.yMin && a.yMax <= b.yMax;
        }
    }
}