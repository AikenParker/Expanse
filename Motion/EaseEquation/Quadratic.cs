using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public static class Quadratic
    {
        public class EaseOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                return -c * (t /= d) * (t - 2.0f) + b;
            }
        }

        public class EaseIn : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                return c * (t /= d) * t + b;
            }
        }

        public class EaseInOut : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                if ((t /= d / 2.0f) < 1.0f)
                    return c / 2.0f * t * t + b;

                return -c / 2.0f * ((--t) * (t - 2.0f) - 1.0f) + b;
            }
        }
    }
}
