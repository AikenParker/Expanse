using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public abstract class ValueMotion<T> : Motion
    {
        protected abstract void ApplyValue(float value);
    }
}
