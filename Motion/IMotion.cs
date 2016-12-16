using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// INCOMPLETE
    /// </summary>
    public interface IMotion : IComplexUpdate
    {
        float StartDelay { get; set; }
        float EndDelay { get; set; }
        float Duration { get; set; }

        Action Completed { get; set; }
        Action Started { get; set; }
    }
}
