namespace Expanse
{
    /// <summary>
    /// Implemented by all Expanse Motions
    /// </summary>
    public interface IMotion : IComplexUpdate
    {
        float CurrentTime { get; }
        bool IsActive { get; set; }

        float Duration { get; }
        float StartDelay { get; set; }
        float EndDelay { get; set; }
        float PlaybackRate { get; set; }

        bool IsStarted { get; }
        bool IsMotionStarted { get; }
        bool IsMotionCompleted { get; }
        bool IsCompleted { get; }

        void OnStart();

        void Resume();
        void Pause();
    }
}
