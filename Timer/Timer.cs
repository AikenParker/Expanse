using System;
using Expanse.Random;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Time related utility class.
    /// </summary>
    public class Timer : IUpdate, IDisposable
    {
        protected Timer() { }

        private Timer(MonoBehaviour attachedMonoBahviour, CallBackRelay CBR)
        {
            if (CBR == null)
                throw new ArgumentNullException("CBR");

            this.callBackRelay = CBR;
            this.callBackRelay.Destroyed += this.Deactivate;

            if (attachedMonoBahviour != null)
            {
                this.AttachedGameObject = attachedMonoBahviour.gameObject;
                this.AttachedMonoBehaviour = attachedMonoBahviour;
            }
            else this.IsUnsafe = true;

            this.Subscribe(callBackRelay, UpdateMode);
            this.isActive = true;
        }

        #region STATIC CONSTRUCTORS

        /// <summary>
        /// Creates an unsafe, global, one-shot timer.
        /// </summary>
        public static Timer Create(float duration, Action onComplete = null)
        {
            Timer newTimer = new Timer(null, CallBackRelay.GlobalCBR);

            newTimer.ApplySettings(TimerSettings.GameOneShot.WithDuration(duration));
            newTimer.Completed += onComplete;

            return newTimer;
        }

        /// <summary>
        /// Creates an unsafe, global timer.
        /// </summary>
        public static Timer Create(TimerSettings settings, Action onComplete = null)
        {
            Timer newTimer = new Timer(null, CallBackRelay.GlobalCBR);

            newTimer.ApplySettings(settings);
            if (settings.completionMode != TimerCompletionModes.Reverse)
                newTimer.Completed += onComplete;
            else
                newTimer.CompletedOrReturned += onComplete;

            return newTimer;
        }

        /// <summary>
        /// Creates a global, one-shot timer.
        /// </summary>
        public static Timer Create(MonoBehaviour monoBehaviour, float duration, Action onComplete = null)
        {
            Timer newTimer = new Timer(monoBehaviour, CallBackRelay.GlobalCBR);

            newTimer.ApplySettings(TimerSettings.GameOneShot.WithDuration(duration));
            newTimer.Completed += onComplete;

            return newTimer;
        }

        /// <summary>
        /// Creates a global timer.
        /// </summary>
        public static Timer Create(MonoBehaviour monoBehaviour, TimerSettings settings, Action onComplete = null)
        {
            Timer newTimer = new Timer(monoBehaviour, CallBackRelay.GlobalCBR);

            newTimer.ApplySettings(settings);
            if (settings.completionMode != TimerCompletionModes.Reverse)
                newTimer.Completed += onComplete;
            else
                newTimer.CompletedOrReturned += onComplete;

            return newTimer;
        }

        /// <summary>
        /// Creates a one-shot timer.
        /// </summary>
        public static Timer Create(MonoBehaviour monoBehaviour, CallBackRelay CBR, float duration, Action onComplete = null)
        {
            Timer newTimer = new Timer(monoBehaviour, CBR);

            newTimer.ApplySettings(TimerSettings.GameOneShot.WithDuration(duration));
            newTimer.Completed += onComplete;

            return newTimer;
        }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        public static Timer Create(MonoBehaviour monoBehaviour, CallBackRelay CBR, TimerSettings settings, Action onComplete = null)
        {
            Timer newTimer = new Timer(monoBehaviour, CBR);

            newTimer.ApplySettings(settings);
            if (settings.completionMode != TimerCompletionModes.Reverse)
                newTimer.Completed += onComplete;
            else
                newTimer.CompletedOrReturned += onComplete;

            return newTimer;
        }

        #endregion

        private bool isActive;
        private UpdateModes updateMode;
        private CallBackRelay callBackRelay;
        private bool deactivateOnLoad;

        /// <summary>
        /// Duration of the timer in seconds.
        /// </summary>
        public float Duration { get; set; }
        /// <summary>
        /// Time passed in seconds for this iteration of this timer.
        /// </summary>
        public float CurrentTime { get; private set; }
        /// <summary>
        /// Total time passed in seconds for the entire lifespan of this timer.
        /// </summary>
        public float TotalPassedTime { get; private set; }
        /// <summary>
        /// If true the duration is randomized between every iteration of this timer.
        /// </summary>
        public bool IsRandomized { get; set; }
        /// <summary>
        /// RNG responsible for providing the randomized duration.
        /// </summary>
        public RNG Randomizer { get; set; }
        /// <summary>
        /// Minimum duration time in seconds this timer can be if randomized.
        /// </summary>
        public float MinDuration { get; set; }
        /// <summary>
        /// Maximum duration time in seconds this timer can be if randomized.
        /// </summary>
        public float MaxDuration { get; set; }
        /// <summary>
        /// Is true if this timer is not paused and currently playing.
        /// </summary>
        public bool IsPlaying { get; set; }
        /// <summary>
        /// Multiplicative factor at which this timer passes time.
        /// </summary>
        public float PlaybackRate { get; set; }
        /// <summary>
        /// Defines the behaviour to perform upon this timer completing.
        /// </summary>
        public TimerCompletionModes CompletionMode { get; set; }
        /// <summary>
        /// Amount of remaining iterations this timer will perform.
        /// </summary>
        public int Repeats { get; set; }
        /// <summary>
        /// If true the timer will continue even if the attached game object and MonoBehaviour is destroyed.
        /// </summary>
        public bool IsUnsafe { get; private set; }
        /// <summary>
        /// If true the timer will continue even if the attached game object or MonoBehaviour is disabled.
        /// </summary>
        public bool AlwaysPlay { get; set; }

        /// <summary>
        /// The attached game object for this timer.
        /// <para>Usually the game object responsible for creating this timer.</para>
        /// </summary>
        public GameObject AttachedGameObject { get; set; }
        /// <summary>
        /// The attached MonoBehaviour for this timer.
        /// <para>Usually the MonoBehaviour responsible for creating this timer.</para>
        /// </summary>
        public MonoBehaviour AttachedMonoBehaviour { get; set; }

        /// <summary>
        /// Event invoked when this timer is deactivated for any reason.
        /// </summary>
        public event Action Deactivated;
        /// <summary>
        /// Event invoked when this timer completes or returns to the start. (If reversing)
        /// </summary>
        public event Action CompletedOrReturned;
        /// <summary>
        /// Event invoked when this timer completes.
        /// </summary>
        public event Action Completed;
        /// <summary>
        /// Event invoked when this timer returns to the start. (If reversing)
        /// </summary>
        public event Action Returned;

        /// <summary>
        /// Amount of times this timer has completed or returned.
        /// </summary>
        public int CompleteOrReturnCount { get; private set; }
        /// <summary>
        /// Amount of times this timer has completed.
        /// </summary>
        public int CompleteCount { get; private set; }
        /// <summary>
        /// Amount of times this timer has returned.
        /// </summary>
        public int ReturnCount { get; private set; }

        void IUpdate.OnUpdate(float deltaTime)
        {
            ThrowIfInactive();

            if (!IsPlaying)
                return;

            float processedDeltaTime = deltaTime * PlaybackRate;

            this.TotalPassedTime += Mathf.Abs(processedDeltaTime);

            float rawNewTime = this.CurrentTime + processedDeltaTime;

            float leftOverTime;

            if (!this.IsReversing)
                leftOverTime = rawNewTime - this.Duration;
            else
                leftOverTime = rawNewTime * -1;

            if (leftOverTime < 0)
            {
                this.CurrentTime = rawNewTime;
            }
            else
            {
                if (rawNewTime >= Duration)
                {
                    OnCompleted();

                    HandleEnd(leftOverTime);

                    if (Repeats > 0)
                        Repeats--;
                }
                else if (rawNewTime <= 0)
                {
                    OnReturned();
                    HandleEnd(leftOverTime);
                }
                else throw new UnexpectedException("Timer value errors");
            }
        }

        /// <summary>
        /// Sets the current state of this timer to playing.
        /// </summary>
        public void Play()
        {
            ThrowIfInactive();

            IsPlaying = true;
        }

        /// <summary>
        /// Sets the current state of this timer to not playing.
        /// </summary>
        public void Stop()
        {
            ThrowIfInactive();

            IsPlaying = false;
        }

        public void Reverse()
        {
            ThrowIfInactive();

            PlaybackRate *= -1;
        }

        public void Reset()
        {
            ThrowIfInactive();

            IsPlaying = false;
            IsReversing = false;
            CurrentTime = 0f;
        }

        public void Restart()
        {
            ThrowIfInactive();

            IsPlaying = true;
            IsReversing = false;
            CurrentTime = 0f;
        }

        public void End()
        {
            ThrowIfInactive();

            CurrentTime = CurrentEndTime;
        }

        public void ApplySettings(TimerSettings settings)
        {
            ThrowIfInactive();

            this.Duration = settings.duration;
            this.IsRandomized = settings.isRandomized;
            this.MinDuration = settings.minDuration;
            this.MaxDuration = settings.maxDuration;
            this.IsPlaying = settings.autoPlay;
            this.PlaybackRate = settings.playBackRate;
            this.CompletionMode = settings.completionMode;
            this.Repeats = settings.repeats;
            this.AlwaysPlay = settings.alwaysPlay;
            this.UpdateMode = settings.updateMode;
            this.DeactivateOnLevelChange = settings.deactivateOnLoad;

            if (this.IsRandomized)
                this.Duration = NextDuration;
        }

        public void Reactivate()
        {
            if (IsActive)
            {
                Debug.LogWarning("Timer is already active", AttachedMonoBehaviour);
                return;
            }

            this.Subscribe(CallBackRelay, UpdateMode);

            CallBackRelay.Destroyed += Deactivate;
            if (this.DeactivateOnLevelChange)
                CallBackRelay.LevelChanged += Deactivate;

            this.isActive = true;
        }

        public void Deactivate()
        {
            ThrowIfInactive();

            this.Stop();
            this.isActive = false;

            this.Unsubscribe(CallBackRelay, UpdateMode);

            CallBackRelay.Destroyed -= Deactivate;
            if (this.DeactivateOnLevelChange)
                CallBackRelay.LevelChanged -= Deactivate;

            if (Deactivated != null)
                Deactivated();
        }

        private void ThrowIfInactive()
        {
            if (!IsActive)
                throw new InactiveException("Timer is inactive");
        }

        private void OnUpdateModeChanged(UpdateModes prevMode, UpdateModes newMode)
        {
            this.Unsubscribe(CallBackRelay, prevMode);
            this.Subscribe(CallBackRelay, newMode);
        }

        private void OnCallBackRelayChanged(CallBackRelay prevCBR, CallBackRelay newCBR)
        {
            if (prevCBR != null)
            {
                prevCBR.Destroyed -= this.Deactivate;

                if (DeactivateOnLevelChange)
                    prevCBR.LevelChanged -= this.Deactivate;

                this.Unsubscribe(prevCBR, UpdateMode);
            }

            newCBR.Destroyed += this.Deactivate;

            if (DeactivateOnLevelChange)
                newCBR.LevelChanged += this.Deactivate;

            this.Subscribe(newCBR, UpdateMode);
        }

        private void OnDeactivateOnLoadChanged()
        {
            if (DeactivateOnLevelChange)
                CallBackRelay.LevelChanged += this.Deactivate;
            else
                CallBackRelay.LevelChanged -= this.Deactivate;
        }

        private void Unsubscribe(CallBackRelay CBR, UpdateModes mode)
        {
            switch (mode)
            {
                case UpdateModes.Update:
                case UpdateModes.UnscaledUpdate:
                    CBR.UnsubscribeFromUpdate(this);
                    break;

                case UpdateModes.LateUpdate:
                case UpdateModes.UnscaledLateUpdate:
                    CBR.UnsubscribeFromLateUpdate(this);
                    break;

                case UpdateModes.FixedUpdate:
                    CBR.UnsubscribeFromFixedUpdate(this);
                    break;
            }
        }

        private void Subscribe(CallBackRelay CBR, UpdateModes mode)
        {
            switch (mode)
            {
                case UpdateModes.Update:
                case UpdateModes.UnscaledUpdate:
                    CBR.SubscribeToUpdate(this);
                    break;

                case UpdateModes.LateUpdate:
                case UpdateModes.UnscaledLateUpdate:
                    CBR.SubscribeToLateUpdate(this);
                    break;

                case UpdateModes.FixedUpdate:
                    CBR.SubscribeToFixedUpdate(this);
                    break;
            }
        }

        private void OnCompleted()
        {
            CompleteOrReturnCount++;
            CompleteCount++;

            if (CompletedOrReturned != null)
                CompletedOrReturned();

            if (Completed != null)
                Completed();
        }

        private void OnReturned()
        {
            CompleteOrReturnCount++;
            ReturnCount++;

            if (CompletedOrReturned != null)
                CompletedOrReturned();

            if (Returned != null)
                Returned();
        }

        private void HandleEnd(float leftOverTime)
        {
            this.Duration = NextDuration;

            switch (CompletionMode)
            {
                case TimerCompletionModes.Deactivate:
                    this.CurrentTime = CurrentEndTime;
                    this.Deactivate();
                    break;

                case TimerCompletionModes.Stop:
                    this.CurrentTime = CurrentEndTime;
                    this.Stop();
                    break;

                case TimerCompletionModes.Restart:
                    if (Repeats == 0)
                    {
                        this.CurrentTime = CurrentEndTime;
                        this.Deactivate();
                    }
                    else
                    {
                        this.Restart();
                        this.CurrentTime = leftOverTime;
                    }
                    break;

                case TimerCompletionModes.Reverse:
                    if (Repeats == 0 && !IsReversing)
                    {
                        this.CurrentTime = CurrentEndTime;
                        this.Deactivate();
                    }
                    else
                    {
                        this.Reverse();
                        this.CurrentTime = !IsReversing ? Duration - leftOverTime : leftOverTime;
                    }
                    break;
            }
        }

        public bool IsActive
        {
            get { return isActive; }
        }

        public UpdateModes UpdateMode
        {
            get { return updateMode; }
            set
            {
                ThrowIfInactive();

                if (value != updateMode)
                {
                    UpdateModes prevUpdateMode = updateMode;
                    updateMode = value;
                    OnUpdateModeChanged(prevUpdateMode, updateMode);
                }
            }
        }

        public CallBackRelay CallBackRelay
        {
            get { return callBackRelay; }
            set
            {
                ThrowIfInactive();

                if (value != callBackRelay)
                {
                    CallBackRelay prevCallBackRelay = callBackRelay;
                    callBackRelay = value;
                    OnCallBackRelayChanged(prevCallBackRelay, callBackRelay);
                }
            }
        }

        public bool DeactivateOnLevelChange
        {
            get { return deactivateOnLoad; }
            set
            {
                ThrowIfInactive();

                if (deactivateOnLoad != value)
                {
                    deactivateOnLoad = value;
                    OnDeactivateOnLoadChanged();
                }
            }
        }

        public float TrueDuration
        {
            get
            {
                return Duration / (Mathf.Abs(PlaybackRate) * (UnscaledDelta ? 1f : TimeManager.TimeScale));
            }
        }

        public float Percentage
        {
            get { return CurrentTime / Duration; }
        }

        public bool IsReversing
        {
            get { return PlaybackRate < 0f; }
            set
            {
                if (value != IsReversing)
                    this.Reverse();
            }
        }

        public float CurrentEndTime
        {
            get { return !IsReversing ? Duration : 0f; }
        }

        public float RemainingTime
        {
            get { return !IsReversing ? Duration - CurrentTime : CurrentTime; }
        }

        public float TotalRemainingTime
        {
            get
            {
                switch (CompletionMode)
                {
                    case TimerCompletionModes.Restart:
                    case TimerCompletionModes.Reverse:
                        if (Repeats < 0)
                            return float.PositiveInfinity;
                        else
                            return ((Repeats - 1) * Duration) + RemainingTime;

                    case TimerCompletionModes.Stop:
                    case TimerCompletionModes.Deactivate:
                    default:
                        return RemainingTime;
                }
            }
        }

        private float NextDuration
        {
            get
            {
                if (IsRandomized)
                    Duration = (Randomizer ?? Random.RandomUtil.Instance).Float(MinDuration, MaxDuration);

                return Duration;
            }
        }

        public void Dispose()
        {
            if (this.IsActive)
                this.Deactivate();
        }

        GameObject IUnity.gameObject
        {
            get
            {
                return this.AttachedGameObject;
            }
        }

        MonoBehaviour IUnity.MonoBehaviour
        {
            get
            {
                return this.AttachedMonoBehaviour;
            }
        }

        bool IUpdate.UnsafeUpdates
        {
            get
            {
                return this.IsUnsafe;
            }
        }

        bool IUpdate.AlwaysUpdate
        {
            get
            {
                return this.AlwaysPlay;
            }
        }

        public bool UnscaledDelta
        {
            get
            {
                UpdateModes updateMode = this.UpdateMode;
                return updateMode == UpdateModes.UnscaledUpdate || updateMode == UpdateModes.UnscaledLateUpdate;
            }
        }

        public enum TimerCompletionModes
        {
            Deactivate = 0,
            Stop = 1,
            Restart = 2,
            Reverse = 3
        }
    }

    public enum UpdateModes
    {
        Update = 0,
        UnscaledUpdate = 1,
        FixedUpdate = 2,
        LateUpdate = 3,
        UnscaledLateUpdate = 4,
    }
}