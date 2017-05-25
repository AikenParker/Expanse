using UnityEngine;

namespace Expanse.Motion
{
    public static class Circular
    {
        public class EaseOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration;
                return -end * (Mathf.Sqrt(1 - time * time) - 1) + start;
            }
        }

        public class EaseIn : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration;
                time--;
                return end * Mathf.Sqrt(1 - time * time) + start;
            }
        }

        public class EaseInOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration / 2;

                if (time < 1)
                    return -end / 2 * (Mathf.Sqrt(1 - time * time) - 1) + start;

                time -= 2;
                return end / 2 * (Mathf.Sqrt(1 - time * time) + 1) + start;
            }
        }
    }
}
