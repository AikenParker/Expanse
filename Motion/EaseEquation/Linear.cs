namespace Expanse
{
    public static class Linear
    {
        public class EaseNone : IEaseEquation
        {
            public float Update(float time, float start, float end, float duration)
            {
                return end * time / duration + start;
            }
        }
    }
}
