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
    : Add reverse functionality
    */

    /// <summary>
    /// Core Expanse Motion class.
    /// </summary>
    public abstract class Motion : IComplexUpdate
    {
        public CallBackRelay CBR { get; set; }
        public UpdateModes UpdateMode { get; set; }
        public int Priority { get; set; }

        public bool IsActive { get; set; }
        public float Position { get; protected set; }

        public float Duration { get; set; }
        public float StartDelay { get; set; }
        public float EndDelay { get; set; }
        public float PlaybackRate { get; set; }

        public bool IsStarted { get; protected set; }
        public bool IsMotionStarted { get; protected set; }
        public bool IsMotionCompleted { get; protected set; }
        public bool IsCompleted { get; protected set; }

        public event Action Started;
        public event Action MotionStarted;
        public event Action MotionCompleted;
        public event Action Completed;

        public Motion()
        {
            this.CBR = CallBackRelay.GlobalCBR;
            this.UpdateMode = UpdateModes.UPDATE;
            this.Duration = 1f;
            this.PlaybackRate = 1f;
            this.IsActive = true;
        }

        public virtual void OnStart()
        {
            RaiseStarted();

            switch (UpdateMode)
            {
                case UpdateModes.UPDATE:
                case UpdateModes.UNSCALED_UPDATE:
                    CBR.SubscribeToUpdate(this);
                    break;

                case UpdateModes.LATE_UPDATE:
                case UpdateModes.UNSCALED_LATE_UPDATE:
                    CBR.SubscribeToLateUpdate(this);
                    break;

                case UpdateModes.FIXED_UPDATE:
                    CBR.SubscribeToFixedUpdate(this);
                    break;
            }
        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (!IsActive)
                return;

            float gain = deltaTime * PlaybackRate;

            float newPostion = Mathf.Clamp(Position + gain, 0, TotalDuration);
            Position = newPostion;

            OnPositionChanged();
        }

        protected abstract void OnPositionChanged();

        /// <summary>
        /// Resumes motion playback from a stop.
        /// </summary>
        public void Resume()
        {
            IsActive = true;
        }

        /// <summary>
        /// Pauses motion playback.
        /// </summary>
        public void Pause()
        {
            IsActive = false;
        }

        protected void RaiseStarted()
        {
            if (!IsStarted)
            {
                IsStarted = true;

                Started.SafeInvoke();
            }
        }

        protected void RaiseMotionStarted()
        {
            if (!IsMotionStarted)
            {
                IsMotionStarted = true;

                MotionStarted.SafeInvoke();
            }
        }

        protected void RaiseMotionCompleted()
        {
            if (!IsMotionCompleted)
            {
                IsMotionCompleted = true;

                MotionCompleted.SafeInvoke();
            }
        }

        protected void RaiseCompleted()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;

                Completed.SafeInvoke();
            }
        }

        public float TotalDuration
        {
            get { return StartDelay + Duration + EndDelay; }
        }

        /// <summary>
        /// The MonoBehaviour attached to this motion. (Usually the initiator)
        /// Note: This is not the TargetObject.
        /// </summary>
        public MonoBehaviour AttachedMonoBehaviour { get; set; }

        bool IComplexUpdate.AlwaysUpdate
        {
            get { return false; }
        }

        GameObject IUnity.gameObject
        {
            get { return this.AttachedMonoBehaviour.gameObject; }
        }

        MonoBehaviour IUnity.MonoBehaviour
        {
            get { return this.AttachedMonoBehaviour; }
        }

        bool IComplexUpdate.UnsafeUpdates
        {
            get { return false; }
        }

        bool IComplexUpdate.UnscaledDelta
        {
            get { return this.UpdateMode.EqualsAny(UpdateModes.UNSCALED_UPDATE, UpdateModes.UNSCALED_LATE_UPDATE); }
        }

        public static SequenceMotion operator +(Motion a, Motion b)
        {
            return new SequenceMotion(a, b);
        }

        public static GroupMotion operator *(Motion a, Motion b)
        {
            return new GroupMotion(a, b);
        }
    }
}
