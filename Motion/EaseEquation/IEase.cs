using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public interface IEase
    {
        float Update(float t, float b, float c, float d, float a, float p);
    }
}
