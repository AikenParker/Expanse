using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace Expanse
{
    /// <summary>
    /// Time related utility class.
    /// </summary>
    [System.Serializable]
    public class Timer : IDisposable
    {
        // Static reference to all timers (Timers not in here do not function)
        protected static List<Timer> AllTimers = new List<Timer>(50);
        private static bool IsSubscribed { get; set; }      // Has a callback relay subscription been made?

        // Timer properties
        public float Length { get; private set; }           // How long will the timer take. (Seconds)
        public float Value { get; private set; }            // Current time value of this timer. (Seconds)
        public bool IsPlaying { get; set; }                 // Is the timer currently ticking down and playing?
        public bool AutoRestart { get; set; }               // Will the timer reset automatically once finished?
        public bool DisposeOnCompletion { get; set; }       // Will the timer automatically dispose itself after completion?
        public bool DisposeOnLoad { get; set; }             // Will the timer automatically dispose itself on load?
        public UpdateModes UpdateMode { get; set; }         // Determines which delta value to affect the timer.
        public int MaxRepeats { get; set; }                 // Maximum amount of times the timer can end before self disposing. (Default(0) = Infinite)
        public bool IsRandomized { get; private set; }      // Does this timer have a randomized length?
        public float MinLength { get; set; }                // Maximum length while randomized. (Seconds)
        public float MaxLength { get; set; }                // Minimum length while randomized. (Seconds)
        private float GetNewLength
        {
            get
            {
                Length = IsRandomized ? UnityEngine.Random.Range(MinLength, MaxLength) : Length;
                return Length;
            }
        }                       // Returns new length value. Where the new randomized length is calculated.

        // Quick-Info properties
        public float Percentage                             // Returns value between 0 and 1 showing how much of the timer has elapsed.
        { get { return Value / Length; } }
        public float TimeRemaining                          // Returns the time left in seconds before the timer completes.
        { get { return Value; } }
        public float TimeElapsed                            // Returns the time in seconds that have elapsed since the start of the timer.
        { get { return Length - Value; } }
        public uint ElapsedCount { get; private set; }      // Amount of times this timer has elapsed (Calling Elapsed action)

        // Events
        public event Action Elapsed;
        public event Action<float> Updated;

        #region Constructors

        // Do not use parameterless constructor.
        protected Timer() { }

        /// <summary>
        /// Creates a simple one-time use timer.
        /// </summary>
        protected Timer(float length, Action action = null)
        {
            Length = length;
            Value = Length;

            TimerPlusSetup((int)Presets.OneTimeUse, action);
        }

        /// <summary>
        /// Creates a simple specified preset timer.
        /// </summary>
        protected Timer(float length, Presets preset, Action action = null)
        {
            Length = length;
            Value = Length;

            TimerPlusSetup((int)preset, action);
        }

        /// <summary>
        /// Creates a timer with specified custom settings.
        /// </summary>
        protected Timer(float length, bool startPlaying, bool autoRestart, bool disposeOnCompletion, bool disposeOnLoad, UpdateModes updateMode, Action action = null)
        {
            Length = length;
            Value = Length;

            int preset = 0;
            preset += startPlaying ? 1 : 0;
            preset += autoRestart ? 10 : 0;
            preset += disposeOnCompletion ? 100 : 0;
            preset += disposeOnLoad ? 1000 : 0;
            preset += (int)updateMode * 10000;

            TimerPlusSetup((int)preset, action);
        }

        /// <summary>
        /// Creates a randomized simple one-time use timer.
        /// </summary>
        protected Timer(float minLength, float maxLength, Action action = null)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            IsRandomized = true;
            Value = GetNewLength;

            TimerPlusSetup((int)Presets.OneTimeUse, action);
        }

        /// <summary>
        /// Creates a randomized simple specified preset timer.
        /// </summary>
        protected Timer(float minLength, float maxLength, Presets preset, Action action = null)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            IsRandomized = true;
            Value = GetNewLength;

            TimerPlusSetup((int)preset, action);
        }

        /// <summary>
        /// Creates a randomized timer with all customized settings.
        /// </summary>
        protected Timer(float minLength, float maxLength, bool startPlaying, bool autoRestart, bool disposeOnCompletion, bool disposeOnLoad, UpdateModes updateMode, Action action = null)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            IsRandomized = true;
            Value = GetNewLength;

            int preset = 0;
            preset += startPlaying ? 1 : 0;
            preset += autoRestart ? 10 : 0;
            preset += disposeOnCompletion ? 100 : 0;
            preset += disposeOnLoad ? 1000 : 0;
            preset += (int)updateMode * 10000;

            TimerPlusSetup((int)preset, action);
        }

        // Applies the initial timer settings.
        protected void TimerPlusSetup(int settings, Action action)
        {
            // Add to all timer list
            AllTimers.Add(this);

            // Subscribe to update events etc.
            if (!IsSubscribed)
            {
                CallBackRelay.SubscribeAll(UpdateAll, FixedUpdateAll, null, DisposeAllOnLoad, DisposeAll);
                IsSubscribed = true;
            }

            // Apply settings from int
            string str = settings.ToString();
            IsPlaying = str.SafeGet(str.Length - 1, '0') == '1';
            AutoRestart = str.SafeGet(str.Length - 2, '0') == '1';
            DisposeOnCompletion = str.SafeGet(str.Length - 3, '0') == '1';
            DisposeOnLoad = str.SafeGet(str.Length - 4, '0') == '1';
            UpdateMode = (UpdateModes)int.Parse(str.SafeGet(str.Length - 5, '0').ToString());

            // Add to elapsed action
            if (!action.IsNullOrEmpty())
                Elapsed += action;
        }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Creates a simple one-time use timer.
        /// </summary>
        public static Timer Create(float length, Action action = null)
        {
            return new Timer(length, action);
        }

        /// <summary>
        /// Creates a simple specified preset timer.
        /// </summary>
        public static Timer Create(float length, Presets preset, Action action = null)
        {
            return new Timer(length, preset, action);
        }

        /// <summary>
        /// Creates a timer with all customized settings.
        /// </summary>
        public static Timer Create(float length, bool startPlaying, bool autoRestart, bool disposeOnCompletion, bool disposeOnLoad, UpdateModes updateMode, Action action = null)
        {
            return new Timer(length, startPlaying, autoRestart, disposeOnCompletion, disposeOnLoad, updateMode, action);
        }

        /// <summary>
        /// Creates a randomized simple one-time use timer.
        /// </summary>
        public static Timer CreateRandom(float minLength, float maxLength, Action action = null)
        {
            return new Timer(minLength, maxLength, action);
        }

        /// <summary>
        /// Creates a randomized simple specified preset timer.
        /// </summary>
        public static Timer CreateRandom(float minLength, float maxLength, Presets mode, Action action = null)
        {
            return new Timer(minLength, maxLength, mode, action);
        }

        /// <summary>
        /// Creates a randomized timer with all customized settings.
        /// </summary>
        public static Timer CreateRandom(float minLength, float maxLength, bool startPlaying, bool autoRestart, bool disposeOnCompletion, bool disposeOnLoad, UpdateModes updateMode, Action action = null)
        {
            return new Timer(minLength, maxLength, startPlaying, autoRestart, disposeOnCompletion, disposeOnLoad, updateMode, action);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Updates all non-fixed scaled or unscaled update timers.
        /// </summary>
        private static void UpdateAll()
        {
            for (int i = AllTimers.Count - 1; i >= 0; --i)
            {
                if (AllTimers[i].UpdateMode == UpdateModes.ScaledUpdate)
                    AllTimers[i].Update(Time.deltaTime);
                else if (AllTimers[i].UpdateMode == UpdateModes.UnscaledUpdate)
                    AllTimers[i].Update(Time.unscaledDeltaTime);
            }
        }

        /// <summary>
        /// Updates all fixed update timers.
        /// </summary>
        private static void FixedUpdateAll()
        {
            for (int i = AllTimers.Count - 1; i >= 0; --i)
            {
                if (AllTimers[i].UpdateMode == UpdateModes.FixedUpdate)
                    AllTimers[i].Update(Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Dispose all timers that should be disabled on load.
        /// </summary>
        private static void DisposeAllOnLoad()
        {
            for (int i = AllTimers.Count - 1; i >= 0; --i)
            {
                if (AllTimers[i].DisposeOnLoad)
                    AllTimers[i].Dispose();
            }
        }

        /// <summary>
        /// Disposes all currently active timers.
        /// </summary>
        private static void DisposeAll()
        {
            for (int i = AllTimers.Count - 1; i >= 0; --i)
            {
                AllTimers[i].Dispose();
            }
        }

        /// <summary>
        /// Removes a timer from the list permanently. Causing it to stop running.
        /// </summary>
        private static void Remove(Timer TimerInstance)
        {
            if (AllTimers.Contains(TimerInstance))
            {
                AllTimers.Remove(TimerInstance);
            }
        }

        /// <summary>
        /// Returns the count of all currently working timers.
        /// </summary>
        public static int TimerCount()
        {
            return AllTimers.Count;
        }

        /// <summary>
        /// Returns the count of all currently working timers that meet the specified predicate.
        /// </summary>
        public static int TimerCount(Predicate<Timer> match)
        {
            return AllTimers.FindAll(match).Count;
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Stops the timer and sets the timer value back.
        /// </summary>
        public void Reset()
        {
            IsPlaying = false;
            Value = GetNewLength;
        }

        /// <summary>
        /// Resets the timer's value and starts the timer up.
        /// </summary>
        public void Restart()
        {
            Value = GetNewLength;
            IsPlaying = true;
        }

        /// <summary>
        /// Toggles the playing state of this timer.
        /// </summary>
        public void Toggle()
        {
            IsPlaying = !IsPlaying;
        }

        /// <summary>
        /// Sets the IsPlaying value to true.
        /// </summary>
        public void Start()
        {
            IsPlaying = true;
        }

        /// <summary>
        /// Sets the IsPlaying value to false.
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// Sets to timer value to zero.
        /// </summary>
        public void End()
        {
            Value = 0;
        }

        /// <summary>
        /// Converts this timer into a randomized timer. (Reset recommended after this)
        /// </summary>
        public void Randomize(float minLength, float maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            IsRandomized = true;
        }

        /// <summary>
        /// Converts this timer to a non-randomized one. (Reset recommended after this)
        /// </summary>
        public void UnRandomize(float length)
        {
            Length = length;
            IsRandomized = false;
        }

        /// <summary>
        /// Changes the length of the timer and also adjusts the value if it is currently above the new length.
        /// </summary>
        public void ModifyLength(float newLength)
        {
            Length = newLength;
        }

        /// <summary>
        /// Changes the length of the timer and also adjusts the value if it is currently above the new length with the option to scale it.
        /// </summary>
        /// <param name="newLength">New length of this timer.</param>
        /// <param name="scaleValue">Scale the current value to the new length?</param>
        public void ModifyLength(float newLength, bool scaleValue)
        {
            float NewValue;

            if (scaleValue)
                NewValue = newLength * Percentage;
            else
                NewValue = Mathf.Min(Value, Length);

            Length = newLength;
            Value = NewValue;
        }

        /// <summary>
        /// Changes the min and max length in a randomized timer.
        /// </summary>
        public void ModifyLength(float newMinLength, float newMaxLength)
        {
            MinLength = newMinLength;
            MaxLength = newMaxLength;
        }

        /// <summary>
        /// Changes the timers current value regardless of play state.
        /// </summary>
        /// <param name="valueModification">How much is added onto the current value.</param>
        /// <param name="allowOverload">If the new value should be allowed to surpass max length.</param>
        public void ModifyValue(float valueModification, bool allowOverload = false)
        {
            if (allowOverload)
                Value = Mathf.Max(0, Value + valueModification);
            else
                Value = Mathf.Clamp(Value + valueModification, 0, Length);
        }

        /// <summary>
        /// Directly manipulates the timer value.
        /// </summary>
        public void SetValue(float newValue, bool allowOverload = false)
        {
            if (allowOverload)
                Value = Mathf.Max(0, newValue);
            else
                Value = Mathf.Clamp(newValue, 0, Length);
        }

        #endregion

        #region Non-public

        // Updates a timer's value.
        private void Update(float delta)
        {
            if (IsPlaying)
            {
                Value -= delta;

                if (Updated != null)
                    Updated(Percentage);
            }
            else
                return;

            if (Value <= 0)
            {
                OnTimerElapsed(Mathf.Abs(Value));
            }
        }

        // Called privately. Handles event invocation and reseting.
        protected virtual void OnTimerElapsed(float leftOver)
        {
            ElapsedCount++;
            if (Elapsed != null)
                Elapsed.Invoke();

            if (MaxRepeats > 0 && ElapsedCount >= MaxRepeats) // Check if max repeats is met
            {
                IsPlaying = false;
                Value = 0;
                this.Dispose();
            }
            else if (AutoRestart) // Begin automatic restart
            {
                Value = GetNewLength - leftOver;
            }
            else // End timer
            {
                IsPlaying = false;
                Value = 0;

                if (DisposeOnCompletion)
                {
                    this.Dispose();
                }
            }
        }

        #endregion

        #region Nested

        // Defines how this timer will be used
        public enum Presets
        {
            /// <summary>
            /// IsPlaying = false,
            /// AutoRestart = false,
            /// DisposeOnCompletion = false,
            /// DisposeOnLoad = true,
            /// UpdateMode = Scaled delta.
            /// </summary>
            Standard = 01000,

            /// <summary>
            /// IsPlaying = true,
            /// AutoRestart = false,
            /// DisposeOnCompletion = true,
            /// DisposeOnLoad = true,
            /// UpdateMode = Scaled delta.
            /// </summary>
            OneTimeUse = 01101,

            /// <summary>
            /// IsPlaying = false;
            /// AutoRestart = true;
            /// DisposeOnCompletion = false;
            /// DisposeOnLoad = true;
            /// UpdateMode = Scaled delta;
            /// </summary>
            Repeater = 01010,

            /// <summary>
            /// IsPlaying = false,
            /// AutoRestart = false,
            /// DisposeOnCompletion = false,
            /// DisposeOnLoad = false,
            /// UpdateMode = Unscaled delta.
            /// </summary>
            BackgroundStandard = 10000,

            /// <summary>
            /// IsPlaying = true,
            /// AutoRestart = false,
            /// DisposeOnCompletion = true,
            /// DisposeOnLoad = false,
            /// UpdateMode = Unscaled delta.
            /// </summary>
            BackgroundOneTimeUse = 10101,

            /// <summary>
            /// IsPlaying = false;
            /// AutoRestart = true;
            /// DisposeOnCompletion = false;
            /// DisposeOnLoad = false;
            /// UpdateMode = Unscaled delta;
            /// </summary>
            BackgroundRepeater = 10010,

            /// <summary>
            /// IsPlaying = false,
            /// AutoRestart = false,
            /// DisposeOnCompletion = false,
            /// DisposeOnLoad = true,
            /// UpdateMode = Fixed delta.
            /// </summary>
            PhysicsStandard = 21000,

            /// <summary>
            /// IsPlaying = true,
            /// AutoRestart = false,
            /// DisposeOnCompletion = true,
            /// DisposeOnLoad = true,
            /// UpdateMode = Fixed delta.
            /// </summary>
            PhysicsOneTimeUse = 21101,

            /// <summary>
            /// IsPlaying = false;
            /// AutoRestart = true;
            /// DisposeOnCompletion = false;
            /// DisposeOnLoad = true;
            /// UpdateMode = Fixed delta;
            /// </summary>
            PhysicsRepeater = 21010
        };

        // Defines valid delta values.
        public enum UpdateModes { ScaledUpdate = 0, UnscaledUpdate = 1, FixedUpdate = 2 };

        #endregion

        #region Disposal

        public bool IsDisposed { get; private set; }
        private SafeHandle Handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// Not usually necessary but is often used to null reference a timer.
        /// E.g. OnDisposed += () timer = null;
        /// </summary>
        public event Action OnDisposed;

        /// <summary>
        /// Call to free resources if this timer will not be needed again.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent costly deconstructor
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (IsDisposed)
                return;

            // Allow host reference cleanup
            if (OnDisposed != null)
                OnDisposed();

            // Free managed objects
            if (Disposing)
            {
                if (Handle != null)
                    Handle.Dispose();
                Elapsed = null;
                Updated = null;
                OnDisposed = null;
            }

            // Free unmanaged objects
            Stop();
            Remove(this);

            IsDisposed = true;
        }

        ~Timer()
        {
            Dispose(false);
        }

        #endregion
    }
}