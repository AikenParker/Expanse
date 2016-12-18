namespace Expanse
{
    public static class Back
    {
        public class EaseIn : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (param1 == 0.0f)
                    param1 = 1.70158f;

                return end * (time /= duration) * time * ((param1 + 1) * time - param1) + start;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (param1 == 0.0f)
                    param1 = 1.70158f;

                if ((time /= duration / 2f) < 1f)
                    return end / 2f * (time * time * (((param1 *= (1.525f)) + 1f) * time - param1)) + start;

                return end / 2f * ((time -= 2f) * time * (((param1 *= (1.525f)) + 1f) * time + param1) + 2f) + start;
            }
        }

        public class EaseOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (param1 == 0.0f)
                    param1 = 1.70158f;

                return end * ((time = time / duration - 1f) * time * ((param1 + 1f) * time + param1) + 1f) + start;
            }
        }
    }
}
