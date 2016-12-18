namespace Expanse
{
    public static class Linear
    {
        public class EaseNone : IEase
        {
            public float Update(float time, float start, float end, float duration, float param1, float param2)
            {
                return end * time / duration + start;
            }
        }
    }
}
