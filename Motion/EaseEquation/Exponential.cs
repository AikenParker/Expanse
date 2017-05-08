using UnityEngine;

namespace Expanse
{
    public static class Exponential
    {
        public class EaseOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                return end * (-Mathf.Pow(2, -10 * time / duration) + 1) + start;
            }
        }

        public class EaseIn : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                return end * Mathf.Pow(2, 10 * (time / duration - 1)) + start;
            }
        }

        public class EaseInOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration / 2;

                if (time < 1)
                    return end / 2 * Mathf.Pow(2, 10 * (time - 1)) + start;

                time--;
                return end / 2 * (-Mathf.Pow(2, -10 * time) + 2) + start;
            }
        }
    }
}
