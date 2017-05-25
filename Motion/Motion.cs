using System;
using Expanse.Extensions;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Motion
{
    /*
    TODO:
    : Add CompletionMode (stop, repeat, reverse) (consider an option to be able to repeat/reverse just the motion and delays seperately)
    : Add InitialRepeats, RepeatsRemaining
    : Add Duration/TrueDuration, TotalDuration/TrueTotalDuration, TotalDurationWithRepeats/TrueTotalDurationWithRepeats
    : Add TimeRemaining/TrueTimeRemaining, TotalTimeRemaining/TrueTotalTimeRemaining, TotalTimeRemainingWithRepeats/TrueTotalTimeRemainingWithRepeats
    : Add MotionUtil (e.g. bounce, pulse, shake)
    : Add all SetParameter overloads to component motions.
    : Add comments to public API
    : Add comments everywhere
    : Add statistical data (TimeElapsed, UpdateCount, etc.)
    : Optimize
    */

    /// <summary>
    /// Core Expanse Motion class.
    /// </summary>
    public abstract class Motion : IUpdate
    {
        private CallBackRelay cbr;
        /// <summary>
        /// The CallBackRelay that this motion will subscribe to when active to get Update callbacks.
        /// Note: This is null if the motion is in a Group or Sequence Motion.
        /// </summary>
        public CallBackRelay CBR
        {
            get { return cbr; }
            set
            {
                if (cbr != value)
                {
                    if (cbr != null && IsActive)
                        UnsubscribeFromCBR();

                    cbr = value;

                    if (cbr != null && IsActive)
                        SubsrcribeToCBR();
                }
            }
        }

        /// <summary>
        /// The activity state of this motion. If true something should be updating this motion. (Usually the CBR)
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Will this motion update even if the attached MonoBehaviour is destroyed or non-existent.
        /// </summary>
        public bool UnsafeUpdates { get; set; }
        /// <summary>
        /// Will this motion update even if the attached MonoBehaviour is disabled or inactive.
        /// </summary>
        public bool AlwaysUpdate { get; set; }
        /// <summary>
        /// The playback position of this motion in seconds. This also includes time passed during the start and end delays.
        /// </summary>
        public float Position { get; protected set; }

        /// <summary>
        /// The duration of the motion itself. Not including start or end delays.
        /// </summary>
        public abstract float Duration { get; }
        /// <summary>
        /// The amount of time that will pass before the motion begins.
        /// </summary>
        public float StartDelay { get; set; }
        /// <summary>
        /// The amount of time that will pass after the motion completes but before the Completed callback is invoked.
        /// </summary>
        public float EndDelay { get; set; }
        /// <summary>
        /// The speed multiplier applied to the playback rate of this motion. (2 = twice as fast)
        /// </summary>
        public float PlaybackRate { get; set; }

        /// <summary>
        /// If true the motion has recieved its first update. (Start delay begins)
        /// </summary>
        public bool IsStarted { get; private set; }
        /// <summary>
        /// If true the motion itself has begun its first update. (Start delay finished)
        /// </summary>
        public bool IsMotionStarted { get; private set; }
        /// <summary>
        /// If true the motion itself has just completed its last update. (End delay starts)
        /// </summary>
        public bool IsMotionCompleted { get; private set; }
        /// <summary>
        /// If true the motion has recieved its last update. (End delay finished)
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Called on the first update when the start delay begins.
        /// </summary>
        public event Action Started;
        /// <summary>
        /// Called after the start delay is completed and the motion begins.
        /// </summary>
        public event Action MotionStarted;
        /// <summary>
        /// Called every time the motion updates. (Including during the start and end delays)
        /// </summary>
        public event Action Updated;
        /// <summary>
        /// Called every time the motion updates. (NOT during the start and end delays)
        /// </summary>
        public event Action MotionUpdated;
        /// <summary>
        /// Called when the motion is complete and the end delay begins.
        /// </summary>
        public event Action MotionCompleted;
        /// <summary>
        /// Called when the end delay is complete.
        /// </summary>
        public event Action Completed;

        private UpdateModes updateMode = UpdateModes.Update;
        /// <summary>
        /// Defines which update loop this motion will subscribe to.
        /// All callbacks are invoked in the context of this loop.
        /// Note: All child motions of a Sequence or Group inherit the update mode of its parent.
        /// </summary>
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

        /// <summary>
        /// Creates an unsafe and globally updating motion.
        /// </summary>
        public Motion() : this(null, null) { }
        /// <summary>
        /// Creates an unsafe motion.
        /// </summary>
        public Motion(CallBackRelay cbr) : this(cbr, null) { }
        /// <summary>
        /// Creates a globally updating motion.
        /// </summary>
        public Motion(MonoBehaviour attachedMonobehaviour) : this(null, attachedMonobehaviour) { }
        /// <summary>
        /// Creates a motion.
        /// </summary>
        public Motion(CallBackRelay cbr, MonoBehaviour attachedMonobehaviour)
        {
            this.AttachedMonoBehaviour = attachedMonobehaviour;
            this.cbr = cbr ?? CallBackRelay.GlobalCBR;
            this.UnsafeUpdates = attachedMonobehaviour == null;
            this.PlaybackRate = 1f;
        }

        protected void SubsrcribeToCBR()
        {
            if (cbr == null)
                return;

            switch (updateMode)
            {
                case UpdateModes.Update:
                case UpdateModes.UnscaledUpdate:
                    cbr.SubscribeToUpdate(this);
                    break;

                case UpdateModes.LateUpdate:
                case UpdateModes.UnscaledLateUpdate:
                    cbr.SubscribeToLateUpdate(this);
                    break;

                case UpdateModes.FixedUpdate:
                    cbr.SubscribeToFixedUpdate(this);
                    break;
            }
        }

        protected void UnsubscribeFromCBR()
        {
            if (cbr == null)
                return;

            switch (updateMode)
            {
                case UpdateModes.Update:
                case UpdateModes.UnscaledUpdate:
                    cbr.UnsubscribeToUpdate(this);
                    break;

                case UpdateModes.LateUpdate:
                case UpdateModes.UnscaledLateUpdate:
                    cbr.UnsubscribeToLateUpdate(this);
                    break;

                case UpdateModes.FixedUpdate:
                    cbr.UnsubscribeToFixedUpdate(this);
                    break;
            }
        }

        /// <summary>
        /// Updates the state of this motion. Should only be called by a CBR or Group/Sequence motion.
        /// </summary>
        public virtual void OnUpdate(float deltaTime)
        {
            if (!IsActive)
                return;

            float position = this.Position;
            float startDelay = this.StartDelay;
            float endDelay = this.EndDelay;
            float duration = this.Duration;
            float totalDuration = duration + startDelay + endDelay;

            // Check starting events

            if (!IsMotionStarted)
            {
                if (!IsStarted)
                {
                    OnStarted();
                }

                if (position > startDelay)
                {
                    OnMotionStarted();
                }
            }

            // Apply position change

            float gain = deltaTime * PlaybackRate;
            float newPostion = Mathf.Clamp(position + gain, 0, totalDuration);

            Position = newPostion;

            OnPositionChanged();

            OnUpdated();

            if (IsMotionStarted)
            {
                OnMotionUpdated();

                // Check completed events

                if (!IsCompleted)
                {
                    if (!IsMotionCompleted && newPostion >= startDelay + duration)
                    {
                        OnMotionCompleted();
                    }

                    if (newPostion >= totalDuration)
                    {
                        OnCompleted();
                    }
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

            if (IsStarted)
            {
                IsMotionStarted = true;
            }
        }

        protected virtual void OnUpdated()
        {
            Updated.SafeInvoke();
        }

        protected virtual void OnMotionUpdated()
        {
            MotionUpdated.SafeInvoke();
        }

        protected virtual void OnMotionCompleted()
        {
            MotionCompleted.SafeInvoke();

            if (IsMotionStarted)
            {
                IsMotionCompleted = true;
            }
        }

        protected virtual void OnCompleted()
        {
            Completed.SafeInvoke();

            if (IsMotionCompleted)
            {
                IsCompleted = true;
                Stop();
            }
        }

        protected virtual void OnPositionChanged() { }

        /// <summary>
        /// Begins/resumes motion playback.
        /// </summary>
        public virtual void Start()
        {
            if (!IsActive)
            {
                IsActive = true;

                SubsrcribeToCBR();
            }
        }

        /// <summary>
        /// Ends/pauses motion playback.
        /// </summary>
        public virtual void Stop()
        {
            if (IsActive)
            {
                IsActive = false;

                UnsubscribeFromCBR();
            }
        }

        /// <summary>
        /// Resets the state of the motion and stops playback.
        /// </summary>
        public virtual void Reset()
        {
            Stop();

            Position = 0;
            IsStarted = false;
            IsMotionStarted = false;
            IsMotionCompleted = false;
            IsCompleted = false;
        }

        /// <summary>
        /// Resets the state of the motion and starts playback.
        /// </summary>
        public virtual void Restart()
        {
            Reset();
            Start();
        }

        /// <summary>
        /// Resets the state of the motion and stops playback to after the start delay.
        /// </summary>
        public virtual void ResetToMotion()
        {
            Stop();

            Position = StartDelay;
            IsStarted = true;
            IsMotionStarted = false;
            IsMotionCompleted = false;
            IsCompleted = false;
        }

        /// <summary>
        /// Resets the state of the motion and starts playback to after the start delay.
        /// </summary>
        public virtual void RestartToMotion()
        {
            ResetToMotion();
            Start();
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

        bool IUpdate.UnscaledDelta
        {
            get
            {
                UpdateModes updateMode = this.UpdateMode;
                return updateMode == UpdateModes.UnscaledUpdate || updateMode == UpdateModes.UnscaledLateUpdate;
            }
        }

        /// <summary>
        /// Adds 2 motions together into a Sequence.
        /// </summary>
        public static SequenceMotion operator +(Motion a, Motion b)
        {
            return new SequenceMotion(a, b);
        }

        /// <summary>
        /// Adds 2 motions together into a Group.
        /// </summary>
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
