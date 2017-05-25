using UnityEngine;

namespace Expanse.Motion
{
    public static class Elastic
    {
        public abstract class ElasticBase : IEaseEquation
        {
            public float ParamA { get; set; }
            public float ParamB { get; set; }

            public abstract float Evaluate(float time, float start, float end, float duration);
        }

        public class EaseIn : ElasticBase
        {
            public override float Evaluate(float time, float start, float end, float duration)
            {
                if (time == 0f)
                    return start;

                if ((time /= duration) == 1f)
                    return start + end;

                if (ParamB == 0f)
                    ParamB = duration * 0.3f;

                float s;
                if (ParamA == 0f || ParamA < Mathf.Abs(end))
                {
                    ParamA = end;
                    s = ParamB / 4f;
                }
                else
                {
                    s = ParamB / (2f * Mathf.PI) * Mathf.Asin(end / ParamA);
                }

                return -(ParamA * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / ParamB)) + start;
            }
        }

        public class EaseInOut : ElasticBase
        {
            public override float Evaluate(float time, float start, float end, float duration)
            {
                if (time == 0f)
                    return start;

                if ((time /= duration / 2f) == 2f)
                    return start + end;

                if (ParamB == 0f)
                    ParamB = duration * (0.3f * 1.5f);

                float s;
                if (ParamA == 0f || ParamA < Mathf.Abs(end))
                {
                    ParamA = end;
                    s = ParamB / 4f;
                }
                else
                {
                    s = ParamB / (2f * Mathf.PI) * Mathf.Asin(end / ParamA);
                }

                if (time < 1f)
                {
                    return -0.5f * (ParamA * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / ParamB)) + start;
                }

                return ParamA * Mathf.Pow(2f, -10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / ParamB) * 0.5f + end + start;
            }
        }

        public class EaseOut : ElasticBase
        {
            public override float Evaluate(float time, float start, float end, float duration)
            {
                if (time == 0f)
                    return start;

                if ((time /= duration) == 1f)
                    return start + end;

                if (ParamB == 0f)
                    ParamB = duration * 0.3f;

                float s;
                if (ParamA == 0f || ParamA < Mathf.Abs(end))
                {
                    ParamA = end;
                    s = ParamB / 4f;
                }
                else
                {
                    s = ParamB / (2f * Mathf.PI) * Mathf.Asin(end / ParamA);
                }

                return ParamA * Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / ParamB) + end + start;
            }
        }
    }
}
