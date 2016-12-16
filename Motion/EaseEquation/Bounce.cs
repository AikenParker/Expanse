using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public static class Bounce
    {
        public class EaseOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if ((t /= d) < (1.0f / 2.75f))
                    return c * (7.5625f * t * t) + b;
                else if (t < (2.0f / 2.75f))
                    return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
                else if (t < (2.5f / 2.75f))
                    return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + b;
                else
                    return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + b;
            }
        }

        public class EaseIn : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                return c - EaseOut(d - t, 0.0f, c, d, 0.0f, 0.0f) + b;
            }

            public float EaseOut(float t, float b, float c, float d, float a, float p)
            {
                if ((t /= d) < (1.0f / 2.75f))
                    return c * (7.5625f * t * t) + b;
                else if (t < (2.0f / 2.75f))
                    return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
                else if (t < (2.5f / 2.75f))
                    return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + b;
                else
                    return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + b;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (t < d / 2.0f)
                    return EaseIn(t * 2.0f, 0.0f, c, d, 0.0f, 0.0f) * 0.5f + b;
                else
                    return EaseOut(t * 2.0f - d, 0.0f, c, d, 0.0f, 0.0f) * 0.5f + c * 0.5f + b;
            }

            public float EaseIn(float t, float b, float c, float d, float a, float p)
            {
                return c - EaseOut(d - t, 0.0f, c, d, 0.0f, 0.0f) + b;
            }

            public float EaseOut(float t, float b, float c, float d, float a, float p)
            {
                if ((t /= d) < (1.0f / 2.75f))
                    return c * (7.5625f * t * t) + b;
                else if (t < (2.0f / 2.75f))
                    return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
                else if (t < (2.5f / 2.75f))
                    return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + b;
                else
                    return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + b;
            }
        }
    }
}
