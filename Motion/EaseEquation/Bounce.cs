namespace Expanse
{
    public static class Bounce
    {
        public class EaseOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if ((time /= duration) < (1.0f / 2.75f))
                    return end * (7.5625f * time * time) + start;
                else if (time < (2.0f / 2.75f))
                    return end * (7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f) + start;
                else if (time < (2.5f / 2.75f))
                    return end * (7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f) + start;
                else
                    return end * (7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f) + start;
            }
        }

        public class EaseIn : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                return end - EaseOut(duration - time, 0.0f, end, duration, 0.0f, 0.0f) + start;
            }

            public float EaseOut(float time, float start, float end, float duration, float param1, float param2)
            {
                if ((time /= duration) < (1.0f / 2.75f))
                    return end * (7.5625f * time * time) + start;
                else if (time < (2.0f / 2.75f))
                    return end * (7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f) + start;
                else if (time < (2.5f / 2.75f))
                    return end * (7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f) + start;
                else
                    return end * (7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f) + start;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (time < duration / 2.0f)
                    return EaseIn(time * 2.0f, 0.0f, end, duration, 0.0f, 0.0f) * 0.5f + start;
                else
                    return EaseOut(time * 2.0f - duration, 0.0f, end, duration, 0.0f, 0.0f) * 0.5f + end * 0.5f + start;
            }

            public float EaseIn(float time, float start, float end, float duration, float param1, float param2)
            {
                return end - EaseOut(duration - time, 0.0f, end, duration, 0.0f, 0.0f) + start;
            }

            public float EaseOut(float time, float start, float end, float duration, float param1, float param2)
            {
                if ((time /= duration) < (1.0f / 2.75f))
                    return end * (7.5625f * time * time) + start;
                else if (time < (2.0f / 2.75f))
                    return end * (7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f) + start;
                else if (time < (2.5f / 2.75f))
                    return end * (7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f) + start;
                else
                    return end * (7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f) + start;
            }
        }
    }
}
