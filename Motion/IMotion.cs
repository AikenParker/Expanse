namespace Expanse
{
    /// <summary>
    /// Implemented by all Expanse Motions
    /// </summary>
    public interface IMotion : IComplexUpdate
    {
        float Duration { get; set; }

        void Play();
        void Stop();
        void Reset();
        void Restart();
        void Reverse();
    }
}
