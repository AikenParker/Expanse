using System;

namespace Expanse
{
    [Serializable]
    public struct TimerSettings
    {
        public float duration;
        public bool isRandomized;
        public float minDuration;
        public float maxDuration;
        public bool autoPlay;
        public float playBackRate;
        public Timer.TimerCompletionModes completionMode;
        public UpdateModes updateMode;
        public int repeats;
        public bool deactivateOnLoad;
        public int priority;
        public bool alwaysPlay;

        public TimerSettings WithDuration(float duration)
        {
            this.isRandomized = false;
            this.duration = duration;
            return this;
        }

        public TimerSettings WithDuration(float minDuration, float maxDuration)
        {
            this.isRandomized = true;
            this.minDuration = minDuration;
            this.maxDuration = maxDuration;
            return this;
        }

        public static TimerSettings Default
        {
            get
            {
                TimerSettings settings = new TimerSettings();
                settings.duration = 1f;
                settings.isRandomized = false;
                settings.minDuration = 1f;
                settings.maxDuration = 1f;
                settings.autoPlay = false;
                settings.playBackRate = 1.0f;
                settings.completionMode = Timer.TimerCompletionModes.DEACTIVATE;
                settings.updateMode = UpdateModes.UPDATE;
                settings.repeats = -1;
                settings.deactivateOnLoad = true;
                settings.priority = 0;
                settings.alwaysPlay = false;
                return settings;
            }
        }

        public static TimerSettings GameOneShot
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                return settings;
            }
        }

        public static TimerSettings GameStandard
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.completionMode = Timer.TimerCompletionModes.STOP;
                return settings;
            }
        }

        public static TimerSettings GameRepeater
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.completionMode = Timer.TimerCompletionModes.RESTART;
                return settings;
            }
        }

        public static TimerSettings GameReverser
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.completionMode = Timer.TimerCompletionModes.REVERSE;
                return settings;
            }
        }

        public static TimerSettings PhysicsOneShot
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.updateMode = UpdateModes.FIXED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings PhysicsStandard
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.completionMode = Timer.TimerCompletionModes.STOP;
                settings.updateMode = UpdateModes.FIXED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings PhysicsRepeater
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.completionMode = Timer.TimerCompletionModes.RESTART;
                settings.updateMode = UpdateModes.FIXED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings PhysicsReverser
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.completionMode = Timer.TimerCompletionModes.REVERSE;
                settings.updateMode = UpdateModes.FIXED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings BackgroundOneShot
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.alwaysPlay = true;
                settings.deactivateOnLoad = false;
                settings.updateMode = UpdateModes.UNSCALED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings BackgroundStandard
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.alwaysPlay = true;
                settings.deactivateOnLoad = false;
                settings.completionMode = Timer.TimerCompletionModes.STOP;
                settings.updateMode = UpdateModes.UNSCALED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings BackgroundRepeater
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.alwaysPlay = true;
                settings.deactivateOnLoad = false;
                settings.completionMode = Timer.TimerCompletionModes.RESTART;
                settings.updateMode = UpdateModes.UNSCALED_UPDATE;
                return settings;
            }
        }

        public static TimerSettings BackgroundReverser
        {
            get
            {
                TimerSettings settings = TimerSettings.Default;
                settings.autoPlay = true;
                settings.alwaysPlay = true;
                settings.deactivateOnLoad = false;
                settings.completionMode = Timer.TimerCompletionModes.REVERSE;
                settings.updateMode = UpdateModes.UNSCALED_UPDATE;
                return settings;
            }
        }
    }
}
