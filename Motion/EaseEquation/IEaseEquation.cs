using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public interface IEaseEquation
    {
        float Update(float time, float start, float end, float duration);
    }
}
