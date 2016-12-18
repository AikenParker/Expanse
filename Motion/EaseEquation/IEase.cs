using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public interface IEase
    {
        float Update(float time, float start, float end, float duration, float param1, float param2);
    }
}
