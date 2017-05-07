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

    /// <summary>
    /// A static collection of parameterless ease equation instances.
    /// </summary>
    public static class EaseEquation
    {
        public static readonly IEaseEquation DefaultEase = new Linear.EaseNone();
        public static readonly Linear.EaseNone LinearEaseNone = new Linear.EaseNone();
        public static readonly Quadratic.EaseIn QuadraticEaseIn = new Quadratic.EaseIn();
        public static readonly Quadratic.EaseOut QuadraticEaseOut = new Quadratic.EaseOut();
        public static readonly Quadratic.EaseInOut QuadraticEaseInOut = new Quadratic.EaseInOut();
    }
}
