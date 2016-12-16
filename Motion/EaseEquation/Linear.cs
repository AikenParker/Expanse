using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public static class Linear
    {
        public class EaseNone : IEase
        {
            public float Update(float t, float b, float c, float d, float a, float p)
            {
                return c * t / d + b;
            }
        }
    }
}
