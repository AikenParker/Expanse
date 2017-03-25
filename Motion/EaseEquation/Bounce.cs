namespace Expanse
{
    public static class Bounce
    {
        public abstract class BounceBase : IEaseEquation
        {
            public float ParamA { get; set; }
            public float ParamB { get; set; }

            public abstract float Update(float time, float start, float end, float duration);
        }

        public class EaseOut : BounceBase
        {
            public override float Update(float time, float start, float end, float duration)
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

        public class EaseIn : BounceBase
        {
            public override float Update(float time, float start, float end, float duration)
            {
                return end - EaseOut(duration - time, 0.0f, end, duration) + start;
            }

            private float EaseOut(float time, float start, float end, float duration)
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

        public class EaseInOut : BounceBase
        {
            public override float Update(float time, float start, float end, float duration)
            {
                if (time < duration / 2.0f)
                    return EaseIn(time * 2.0f, 0.0f, end, duration) * 0.5f + start;
                else
                    return EaseOut(time * 2.0f - duration, 0.0f, end, duration) * 0.5f + end * 0.5f + start;
            }

            private float EaseIn(float time, float start, float end, float duration)
            {
                return end - EaseOut(duration - time, 0.0f, end, duration) + start;
            }

            private float EaseOut(float time, float start, float end, float duration)
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
