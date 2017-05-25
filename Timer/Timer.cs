using System;
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

        public float Duration { get; set; }
        public float CurrentTime { get; private set; }
        public float TotalPassedTime { get; private set; }
        public bool IsRandomized { get; set; }
        public Random Randomizer { get; set; }
        public float MinDuration { get; set; }
        public float MaxDuration { get; set; }
        public bool IsPlaying { get; set; }
        public float PlaybackRate { get; set; }
        public TimerCompletionModes CompletionMode { get; set; }
        public int Repeats { get; set; }
        public bool IsUnsafe { get; private set; }
        public bool AlwaysPlay { get; set; }

        public GameObject AttachedGameObject { get; set; }
        public MonoBehaviour AttachedMonoBehaviour { get; set; }

        public event Action Deactivated;
        public event Action CompletedOrReturned;
        public event Action Completed;
        public event Action Returned;

        public int CompleteOrReturnCount { get; private set; }
        public int CompleteCount { get; private set; }
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

        public void Play()
        {
            ThrowIfInactive();

            IsPlaying = true;
        }

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
                    CBR.UnsubscribeToUpdate(this);
                    break;

                case UpdateModes.LateUpdate:
                case UpdateModes.UnscaledLateUpdate:
                    CBR.UnsubscribeToLateUpdate(this);
                    break;

                case UpdateModes.FixedUpdate:
                    CBR.UnsubscribeToFixedUpdate(this);
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
                    Duration = (Randomizer ?? RandomUtil.Instance).Float(MinDuration, MaxDuration);

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