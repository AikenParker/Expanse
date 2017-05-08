namespace Expanse
{
    public static class Quintic
    {
        public class EaseOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration;
                time--;
                return -end * (time * time * time * time * time - 1) + start;
            }
        }

        public class EaseIn : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration;
                return end * time * time * time * time * time + start;
            }
        }

        public class EaseInOut : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                time /= duration / 2;

                if (time < 1)
                    return end / 2 * time * time * time * time * time + start;

                time -= 2;
                return -end / 2 * (time * time * time * time * time - 2) + start;
            }
        }
    }
}
