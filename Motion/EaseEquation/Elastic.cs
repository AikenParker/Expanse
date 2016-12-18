using UnityEngine;

namespace Expanse
{
    public static class Elastic
    {
        public class EaseIn : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (time == 0f)
                    return start;

                if ((time /= duration) == 1f)
                    return start + end;

                if (param2 == 0f)
                    param2 = duration * 0.3f;

                float s;
                if (param1 == 0f || param1 < Mathf.Abs(end))
                {
                    param1 = end;
                    s = param2 / 4f;
                }
                else
                {
                    s = param2 / (2f * Mathf.PI) * Mathf.Asin(end / param1);
                }

                return -(param1 * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / param2)) + start;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (time == 0f)
                    return start;

                if ((time /= duration / 2f) == 2f)
                    return start + end;

                if (param2 == 0f)
                    param2 = duration * (0.3f * 1.5f);

                float s;
                if (param1 == 0f || param1 < Mathf.Abs(end))
                {
                    param1 = end;
                    s = param2 / 4f;
                }
                else
                {
                    s = param2 / (2f * Mathf.PI) * Mathf.Asin(end / param1);
                }

                if (time < 1f)
                {
                    return -0.5f * (param1 * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / param2)) + start;
                }

                return param1 * Mathf.Pow(2f, -10f * (time -= 1f)) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / param2) * 0.5f + end + start;
            }
        }

        public class EaseOut : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                if (time == 0f)
                    return start;

                if ((time /= duration) == 1f)
                    return start + end;

                if (param2 == 0f)
                    param2 = duration * 0.3f;

                float s;
                if (param1 == 0f || param1 < Mathf.Abs(end))
                {
                    param1 = end;
                    s = param2 / 4f;
                }
                else
                {
                    s = param2 / (2f * Mathf.PI) * Mathf.Asin(end / param1);
                }

                return param1 * Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * duration - s) * (2f * Mathf.PI) / param2) + end + start;
            }
        }
    }
}
