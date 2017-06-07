using System;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of AnimationCurve related extension methods.
    /// </summary>
    public static class AnimationCurveExt
    {
        /// <summary>
        /// Gets the duration of an animation curve.
        /// </summary>
        /// <param name="source">AnimationCurve to check the duration of.</param>
        /// <returns>Returns the duration of the animation curve in seconds.</returns>
        public static float Duration(this AnimationCurve source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            if (source.length <= 0)
                throw new InvalidArgumentException("source must have at least one keyframe");

            return source[source.length - 1].time;
        }
    }
}
