using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of math related utility functionality.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// Determines if 2 angles are equal to eachother within a value of tolerance.
        /// </summary>
        public static bool CompareAngleWithTolerance(float a, float b, float tolerance)
        {
            float angleA = a.PositiveMod(360);
            float angleB = b.PositiveMod(360);
            return Mathf.Abs(angleA - angleB) <= tolerance;
        }
    }
}