using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public static class Elastic
    {
        public class EaseIn : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (t == 0f)
                    return b;

                if ((t /= d) == 1f)
                    return b + c;

                if (p == 0f)
                    p = d * 0.3f;

                float s;
                if (a == 0f || a < Mathf.Abs(c))
                {
                    a = c;
                    s = p / 4f;
                }
                else
                {
                    s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
                }

                return -(a * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p)) + b;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (t == 0f)
                    return b;

                if ((t /= d / 2f) == 2f)
                    return b + c;

                if (p == 0f)
                    p = d * (0.3f * 1.5f);

                float s;
                if (a == 0f || a < Mathf.Abs(c))
                {
                    a = c;
                    s = p / 4f;
                }
                else
                {
                    s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
                }

                if (t < 1f)
                {
                    return -0.5f * (a * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p)) + b;
                }

                return a * Mathf.Pow(2f, -10f * (t -= 1f)) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p) * 0.5f + c + b;
            }
        }

        public class EaseOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (t == 0f)
                    return b;

                if ((t /= d) == 1f)
                    return b + c;

                if (p == 0f)
                    p = d * 0.3f;

                float s;
                if (a == 0f || a < Mathf.Abs(c))
                {
                    a = c;
                    s = p / 4f;
                }
                else
                {
                    s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
                }

                return a * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p) + c + b;
            }
        }
    }
}
