namespace Expanse
{
    public static class Back
    {
        public abstract class BackBase : IEaseEquation
        {
            public float ParamA { get; set; }

            public abstract float Evaluate(float time, float start, float end, float duration);
        }

        public class EaseIn : BackBase
        {
            public override float Evaluate(float time, float start, float end, float duration)
            {
                if (ParamA == 0.0f)
                    ParamA = 1.70158f;

                return end * (time /= duration) * time * ((ParamA + 1) * time - ParamA) + start;
            }
        }

        public class EaseInOut : BackBase
        {
            public override float Evaluate(float time, float start, float end, float duration)
            {
                if (ParamA == 0.0f)
                    ParamA = 1.70158f;

                if ((time /= duration / 2f) < 1f)
                    return end / 2f * (time * time * (((ParamA *= (1.525f)) + 1f) * time - ParamA)) + start;

                return end / 2f * ((time -= 2f) * time * (((ParamA *= (1.525f)) + 1f) * time + ParamA) + 2f) + start;
            }
        }

        public class EaseOut : BackBase
        {
            public override float Evaluate(float time, float start, float end, float duration)
            {
                if (ParamA == 0.0f)
                    ParamA = 1.70158f;

                return end * ((time = time / duration - 1f) * time * ((ParamA + 1f) * time + ParamA) + 1f) + start;
            }
        }
    }
}
