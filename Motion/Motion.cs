using System;
using UnityEngine;

namespace Expanse
{
    /*
    TODO:
    : Add completion mode (stop, repeat, reverse)
    : Add max repeats
    : Add statistical data
    */

    /// <summary>
    /// Core Expanse Motion class.
    /// </summary>
    public abstract class Motion : IComplexUpdate
    {
        public CallBackRelay CBR { get; private set; }
        public int Priority { get; set; }

        public bool IsActive { get; set; }
        public bool UnsafeUpdates { get; set; }
        public bool AlwaysUpdate { get; set; }
        public float Position { get; protected set; }

        public abstract float Duration { get; }
        public float StartDelay { get; set; }
        public float EndDelay { get; set; }
        public float PlaybackRate { get; set; }

        public bool IsStarted { get; private set; }
        public bool IsMotionStarted { get; private set; }
        public bool IsMotionCompleted { get; private set; }
        public bool IsCompleted { get; private set; }

        public event Action Started;
        public event Action Updated;
        public event Action MotionStarted;
        public event Action MotionCompleted;
        public event Action Completed;

        private UpdateModes updateMode = UpdateModes.UPDATE;
        public UpdateModes UpdateMode
        {
            get { return updateMode; }
            set
            {
                if (value != updateMode)
                {
                    if (IsActive)
                    {
                        UnsubscribeFromCBR();
                        updateMode = value;
                        SubsrcribeToCBR();
                    }
                    else
                    {
                        updateMode = value;
                    }
                }
            }
        }

        public Motion() : this(null, null) { }
        public Motion(CallBackRelay cbr) : this(cbr, null) { }
        public Motion(MonoBehaviour attachedMonobehaviour) : this(null, attachedMonobehaviour) { }
        public Motion(CallBackRelay cbr, MonoBehaviour attachedMonobehaviour)
        {
            this.AttachedMonoBehaviour = attachedMonobehaviour;
            this.CBR = cbr ?? CallBackRelay.GlobalCBR;
            this.UnsafeUpdates = attachedMonobehaviour == null;
            this.PlaybackRate = 1f;
        }

        protected void SubsrcribeToCBR()
        {
            switch (updateMode)
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

        protected void UnsubscribeFromCBR()
        {
            switch (updateMode)
            {
                case UpdateModes.UPDATE:
                case UpdateModes.UNSCALED_UPDATE:
                    CBR.UnsubscribeToUpdate(this);
                    break;

                case UpdateModes.LATE_UPDATE:
                case UpdateModes.UNSCALED_LATE_UPDATE:
                    CBR.UnsubscribeToLateUpdate(this);
                    break;

                case UpdateModes.FIXED_UPDATE:
                    CBR.UnsubscribeToFixedUpdate(this);
                    break;
            }
        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (!IsActive)
                return;

            // Check starting events

            if (!IsMotionStarted)
            {
                if (!IsStarted)
                {
                    OnStarted();
                }

                if (Position > StartDelay)
                {
                    OnMotionStarted();
                }
            }

            // Apply position change

            float gain = deltaTime * PlaybackRate;

            float newPostion = Mathf.Clamp(Position + gain, 0, TotalDuration);
            Position = newPostion;

            OnPositionChanged();

            OnUpdated();

            // Check completed events

            if (!IsCompleted)
            {
                if (!IsMotionCompleted && Position >= StartDelay + Duration)
                {
                    OnMotionCompleted();
                }

                if (Position >= TotalDuration)
                {
                    OnCompleted();
                }
            }
        }

        protected virtual void OnStarted()
        {
            Started.SafeInvoke();
            IsStarted = true;
        }

        protected virtual void OnMotionStarted()
        {
            MotionStarted.SafeInvoke();
            IsMotionStarted = true;
        }

        protected virtual void OnUpdated()
        {
            Updated.SafeInvoke();
        }

        protected virtual void OnMotionCompleted()
        {
            MotionCompleted.SafeInvoke();
            IsMotionCompleted = true;
        }

        protected virtual void OnCompleted()
        {
            Completed.SafeInvoke();
            IsCompleted = true;
            IsActive = false;
        }

        protected virtual void OnPositionChanged() { }

        /// <summary>
        /// Resumes motion playback from a stop.
        /// </summary>
        public void Start()
        {
            if (!IsActive)
            {
                IsActive = true;

                SubsrcribeToCBR();
            }
        }

        /// <summary>
        /// Pauses motion playback.
        /// </summary>
        public void Stop()
        {
            if (IsActive)
            {
                IsActive = false;

                UnsubscribeFromCBR();
            }
        }

        /// <summary>
        /// Motion duration including the start and end delay time.
        /// </summary>
        public float TotalDuration
        {
            get { return StartDelay + Duration + EndDelay; }
        }

        /// <summary>
        /// Total duration including the playback rate.
        /// </summary>
        public virtual float TrueDuration
        {
            get { return TotalDuration / PlaybackRate; }
        }

        /// <summary>
        /// Normalizaed value of the progress of this motion.
        /// </summary>
        public float MotionProgress
        {
            get { return Mathf.Min(Mathf.Max(Position - StartDelay, 0) / Duration, 1); }
        }

        /// <summary>
        /// Normalizaed value of the progress of this motion including delays.
        /// </summary>
        public float Progress
        {
            get { return Mathf.Min(Position / TotalDuration, 1); }
        }

        /// <summary>
        /// The MonoBehaviour attached to this motion. (Usually the initiator)
        /// Note: This is not the TargetObject.
        /// </summary>
        public MonoBehaviour AttachedMonoBehaviour { get; set; }

        GameObject IUnity.gameObject
        {
            get { return this.AttachedMonoBehaviour.gameObject; }
        }

        MonoBehaviour IUnity.MonoBehaviour
        {
            get { return this.AttachedMonoBehaviour; }
        }

        bool IComplexUpdate.UnscaledDelta
        {
            get
            {
                UpdateModes updateMode = this.UpdateMode;
                return updateMode == UpdateModes.UNSCALED_UPDATE || updateMode == UpdateModes.UNSCALED_LATE_UPDATE;
            }
        }

        public static SequenceMotion operator +(Motion a, Motion b)
        {
            return new SequenceMotion(a, b);
        }

        public static GroupMotion operator &(Motion a, Motion b)
        {
            return new GroupMotion(a, b);
        }

        protected void ThrowIfInactive()
        {
            if (!IsActive)
                throw new InactiveException("Motion is inactive");
        }

        protected void ThrowIfActive()
        {
            if (IsActive)
                throw new ActiveException("Motion is active");
        }
    }
}
