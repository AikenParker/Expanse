using UnityEngine;

namespace Expanse
{
    public static class MathUtil
    {
        public static bool CheckValueWithTolerance(float a, float b, float tolerance)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        public static bool CheckAngleWithTolerance(float a, float b, float tolerance)
        {
            float angleA = a.PositiveMod(360);
            float angleB = b.PositiveMod(360);
            return Mathf.Abs(angleA - angleB) <= tolerance;
        }

        public static float PositiveMod(this float value, float mod)
        {
            return (value % mod + mod) % mod;
        }

        public static float PositiveMod(this int value, int mod)
        {
            return (value % mod + mod) % mod;
        }
    }
}