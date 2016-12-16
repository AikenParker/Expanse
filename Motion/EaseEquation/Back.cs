using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public static class Back
    {
        public class EaseIn : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (a == 0.0f)
                    a = 1.70158f;

                return c * (t /= d) * t * ((a + 1) * t - a) + b;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (a == 0.0f)
                    a = 1.70158f;

                if ((t /= d / 2f) < 1f)
                    return c / 2f * (t * t * (((a *= (1.525f)) + 1f) * t - a)) + b;

                return c / 2f * ((t -= 2f) * t * (((a *= (1.525f)) + 1f) * t + a) + 2f) + b;
            }
        }

        public class EaseOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if (a == 0.0f)
                    a = 1.70158f;

                return c * ((t = t / d - 1f) * t * ((a + 1f) * t + a) + 1f) + b;
            }
        }
    }
}
