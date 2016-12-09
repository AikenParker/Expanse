using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public static class AnimationCurveExt
    {
        /// <summary>
        /// Returns the length of the animation curve in seconds.
        /// </summary>
        public static float Duration(this AnimationCurve source)
        {
            if (source.IsNullOrEmpty() || source.length <= 0)
                throw new NullReferenceException();

            return source[source.length - 1].time;
        }
    }
}
