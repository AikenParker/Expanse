namespace Expanse
{
    public static class Quadratic
    {
        public class EaseOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                return -end * (time /= duration) * (time - 2.0f) + start;
            }
        }

        public class EaseIn : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                return end * (time /= duration) * time + start;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if ((time /= duration / 2.0f) < 1.0f)
                    return end / 2.0f * time * time + start;

                return -end / 2.0f * ((--time) * (time - 2.0f) - 1.0f) + start;
            }
        }
    }
}
