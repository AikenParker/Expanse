using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public class FloatMotion : Motion<float>
    {
        public FloatMotion(float startValue, float targetValue)
            : base(startValue, targetValue)
        {

        }
    }
}
