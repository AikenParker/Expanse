namespace Expanse
{
    public static class Quadratic
    {
        public class EaseOut : IEaseEquation
        {
            public float Update(float time, float start, float end, float duration)
            {
                return -end * (time /= duration) * (time - 2.0f) + start;
            }
        }

        public class EaseIn : IEaseEquation
        {
            public float Update(float time, float start, float end, float duration)
            {
                return end * (time /= duration) * time + start;
            }
        }

        public class EaseInOut : IEaseEquation
        {
            public float Update(float time, float start, float end, float duration)
            {
                if ((time /= duration / 2.0f) < 1.0f)
                    return end / 2.0f * time * time + start;

                return -end / 2.0f * ((--time) * (time - 2.0f) - 1.0f) + start;
            }
        }
    }
}
