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
        public static readonly IEaseEquation Default = new Linear.EaseNone();
        public static readonly Linear.EaseNone Linear = new Linear.EaseNone();
        public static readonly Quadratic.EaseIn QuadraticIn = new Quadratic.EaseIn();
        public static readonly Quadratic.EaseOut QuadraticOut = new Quadratic.EaseOut();
        public static readonly Quadratic.EaseInOut QuadraticInOut = new Quadratic.EaseInOut();
        public static readonly Cubic.EaseIn CubicIn = new Cubic.EaseIn();
        public static readonly Cubic.EaseOut CubicOut = new Cubic.EaseOut();
        public static readonly Cubic.EaseInOut CubicInOut = new Cubic.EaseInOut();
        public static readonly Quartic.EaseIn QuarticIn = new Quartic.EaseIn();
        public static readonly Quartic.EaseOut QuarticOut = new Quartic.EaseOut();
        public static readonly Quartic.EaseInOut QuarticInOut = new Quartic.EaseInOut();
        public static readonly Quintic.EaseIn QuinticIn = new Quintic.EaseIn();
        public static readonly Quintic.EaseOut QuinticOut = new Quintic.EaseOut();
        public static readonly Quintic.EaseInOut QuinticInOut = new Quintic.EaseInOut();
        public static readonly Sinusoidal.EaseIn SinusoidalIn = new Sinusoidal.EaseIn();
        public static readonly Sinusoidal.EaseOut SinusoidalOut = new Sinusoidal.EaseOut();
        public static readonly Sinusoidal.EaseInOut SinusoidalInOut = new Sinusoidal.EaseInOut();
        public static readonly Exponential.EaseIn ExponentialIn = new Exponential.EaseIn();
        public static readonly Exponential.EaseOut ExponentialOut = new Exponential.EaseOut();
        public static readonly Exponential.EaseInOut ExponentialInOut = new Exponential.EaseInOut();
        public static readonly Circular.EaseIn CircularIn = new Circular.EaseIn();
        public static readonly Circular.EaseOut CircularOut = new Circular.EaseOut();
        public static readonly Circular.EaseInOut CircularInOut = new Circular.EaseInOut();
    }
}
