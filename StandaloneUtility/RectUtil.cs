using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Collection of Rect related utility functionality.
    /// </summary>
    public static class RectUtil
    {
        /// <summary>
        /// Returns a rect that encapsulates a and b.
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
    }
}