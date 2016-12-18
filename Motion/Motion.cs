using System;
using UnityEngine;

namespace Expanse
{
    /*
    TODO:
    : Add dynamic CBR support
    : Add start/end delays
    : Add completion mode
    : Add max repeats
    : Add statistical data
    : Add more events 

    KNOWN ISSUES:
    : IsComplete is invalid if the motion is reset after completion
    : Reset and Restart completion modes do not carry over leftover time
    */

    /// <summary>
    /// Core Expanse Motion class.
    /// </summary>
    public abstract class Motion<T> : IMotion
    {
        public bool isActive;

        public CallBackRelay CBR { get; private set; }

        public UpdateModes UpdateMode { get; protected set; }
        public int Priority { get; set; }

        GameObject attachedGameObject;
        MonoBehaviour attachedMonoBehaviour;

        public float Duration { get; set; }
        public float PlaybackRate { get; set; }
        public MotionCompletionModes CompletionMode { get; set; }

        public bool IsPlaying { get; set; }
        public float CurrentTime { get; private set; }
        public bool IsComplete { get; private set; }

        public event Action Completed;

        protected IEase easeEquation;

        public float EaseParam1 { get; set; }
        public float EaseParam2 { get; set; }

        protected Motion(CallBackRelay callBackRelay, bool autoStart)
        {
            this.Duration = 1f;
            this.CurrentTime = 0f;

            this.PlaybackRate = 1f;
            this.IsPlaying = autoStart;

            SetEaseEquation<Linear.EaseNone>();

            if (callBackRelay != null)
            {
                this.CBR = callBackRelay;
                this.CBR.SubscribeToUpdate(this);
                this.isActive = true;
            }
        }

        public void SetEaseEquation<U>()where U : IEase, new()
        {
            ThrowIfInactive();

            easeEquation = new U();
        }

        public void OnUpdate(float deltaTime)
        {
            ThrowIfInactive();

            if (!IsPlaying)
                return;

            float processedDelta = deltaTime * PlaybackRate;

            float rawCurrentTime = CurrentTime + processedDelta;

            CurrentTime = Mathf.Clamp(rawCurrentTime, 0f, Duration);

            float value = easeEquation.Update(CurrentTime, 0f, 1f, Duration, EaseParam1, EaseParam2);

            ApplyValue(value);

            bool hasCompleted = (PlaybackRate > 0 && rawCurrentTime >= Duration)
                || (PlaybackRate < 0 && rawCurrentTime <= 0f);

            if (hasCompleted)
            {
                if (Completed != null)
                    Completed.Invoke();

                switch (CompletionMode)
                {
                    case MotionCompletionModes.STOP:
                        Stop();
                        IsComplete = true;
                        break;

                    case MotionCompletionModes.RESTART:
                        Restart();
                        break;

                    case MotionCompletionModes.REVERSE:
                        Reverse();
                        break;

                    case MotionCompletionModes.DEACTIVATE:
                        Stop();
                        IsComplete = true;
                        isActive = false;
                        CBR.UnsubscribeToUpdate(this);
                        break;
                }
            }
        }

        protected abstract void ApplyValue(float value);

        /// <summary>
        /// Resumes motion playback from a stop.
        /// </summary>
        public void Play()
        {
            ThrowIfInactive();

            IsPlaying = true;
        }

        /// <summary>
        /// Pauses motion playback.
        /// </summary>
        public void Stop()
        {
            ThrowIfInactive();

            IsPlaying = false;
        }

        /// <summary>
        /// Resets current time and stops playback.
        /// </summary>
        public void Reset()
        {
            ThrowIfInactive();

            CurrentTime = 0f;
            IsPlaying = false;
        }

        /// <summary>
        /// Resets current time and resumes playback.
        /// </summary>
        public void Restart()
        {
            ThrowIfInactive();

            CurrentTime = 0f;
            IsPlaying = true;
        }

        /// <summary>
        /// Reverses the playback rate.
        /// </summary>
        public void Reverse()
        {
            PlaybackRate *= -1;
        }

        /// <summary>
        /// The MonoBehaviour attached to this motion. (Usually the initiator)
        /// Note: This is not the TargetObject.
        /// </summary>
        public MonoBehaviour AttachedMonoBehaviour
        {
            get { return attachedMonoBehaviour; }
            protected set
            {
                ThrowIfInactive();

                attachedMonoBehaviour = value;
                attachedGameObject = value.gameObject;
            }
        }

        /// <summary>
        /// Returns the normalized current time between 0 and 1.
        /// </summary>
        public float NormalizedTime
        {
            get { return CurrentTime.Normalize(Duration, true); }
        }

        public bool IsActive
        {
            get { return isActive; }
        }

        bool IComplexUpdate.AlwaysUpdate
        {
            get { return false; }
        }

        GameObject IUnity.gameObject
        {
            get { return this.attachedGameObject; }
        }

        MonoBehaviour IUnity.MonoBehaviour
        {
            get { return this.attachedMonoBehaviour; }
        }

        bool IComplexUpdate.UnsafeUpdates
        {
            get { return false; }
        }

        bool IComplexUpdate.UnscaledDelta
        {
            get { return this.UpdateMode.EqualsAny(UpdateModes.UNSCALED_UPDATE, UpdateModes.UNSCALED_LATE_UPDATE); }
        }

        protected void ThrowIfInactive()
        {
            if (!IsActive)
                throw new InactiveException("Motion is inactive");
        }

        public enum MotionCompletionModes
        {
            DEACTIVATE = 0,
            STOP = 1,
            RESTART = 2,
            REVERSE = 3
        }
    }
}
