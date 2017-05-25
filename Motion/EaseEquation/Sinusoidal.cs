using UnityEngine;

namespace Expanse.Motion
{
    public static class Sinusoidal
    {
        public class EaseOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                return end * Mathf.Sin(time / duration * (Mathf.PI / 2)) + start;
            }
        }

        public class EaseIn : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                return -end * Mathf.Cos(time / duration * (Mathf.PI / 2)) + end + start;
            }
        }

        public class EaseInOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                return -end / 2 * (Mathf.Cos(Mathf.PI * time / duration) - 1) + start;
            }
        }
    }
}
