namespace Expanse.Motion
{
    public static class Linear
    {
        public class EaseNone : IEaseEquation
        {
            public float Evaluate(float time, float start, float end, float duration)
            {
                return end * time / duration + start;
            }
        }
    }
}
