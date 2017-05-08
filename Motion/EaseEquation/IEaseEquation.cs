namespace Expanse
{
    public interface IEaseEquation
    {
        float Evaluate(float time, float start, float end, float duration);
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
        public static readonly Cubic.EaseIn CubicEaseIn = new Cubic.EaseIn();
        public static readonly Cubic.EaseOut CubicEaseOut = new Cubic.EaseOut();
        public static readonly Cubic.EaseInOut CubicEaseInOut = new Cubic.EaseInOut();
        public static readonly Quartic.EaseIn QuarticEaseIn = new Quartic.EaseIn();
        public static readonly Quartic.EaseOut QuarticEaseOut = new Quartic.EaseOut();
        public static readonly Quartic.EaseInOut QuarticEaseInOut = new Quartic.EaseInOut();
        public static readonly Quintic.EaseIn QuinticEaseIn = new Quintic.EaseIn();
        public static readonly Quintic.EaseOut QuinticEaseOut = new Quintic.EaseOut();
        public static readonly Quintic.EaseInOut QuinticEaseInOut = new Quintic.EaseInOut();
        public static readonly Sinusoidal.EaseIn SinusoidalEaseIn = new Sinusoidal.EaseIn();
        public static readonly Sinusoidal.EaseOut SinusoidalEaseOut = new Sinusoidal.EaseOut();
        public static readonly Sinusoidal.EaseInOut SinusoidalEaseInOut = new Sinusoidal.EaseInOut();
        public static readonly Exponential.EaseIn ExponentialEaseIn = new Exponential.EaseIn();
        public static readonly Exponential.EaseOut ExponentialEaseOut = new Exponential.EaseOut();
        public static readonly Exponential.EaseInOut ExponentialEaseInOut = new Exponential.EaseInOut();
        public static readonly Circular.EaseIn CircularEaseIn = new Circular.EaseIn();
        public static readonly Circular.EaseOut CircularEaseOut = new Circular.EaseOut();
        public static readonly Circular.EaseInOut CircularEaseInOut = new Circular.EaseInOut();
    }
}
