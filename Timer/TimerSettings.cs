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

        public TimerSettings Randomize(float minDuration, float maxDuration)
        {
            this.isRandomized = true;
            this.minDuration = minDuration;
            this.maxDuration = maxDuration;
            return this;
        }

        public static TimerSettings GetDefault(float duration)
        {
            TimerSettings settings = new TimerSettings();
            settings.duration = duration;
            settings.isRandomized = false;
            settings.minDuration = duration;
            settings.maxDuration = duration;
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

        public static TimerSettings GetGameOneShot(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            return settings;
        }

        public static TimerSettings GetGameStandard(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.completionMode = Timer.TimerCompletionModes.STOP;
            return settings;
        }

        public static TimerSettings GetGameRepeater(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.completionMode = Timer.TimerCompletionModes.RESTART;
            return settings;
        }

        public static TimerSettings GetGameReverser(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.completionMode = Timer.TimerCompletionModes.REVERSE;
            return settings;
        }

        public static TimerSettings GetPhysicsOneShot(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.updateMode = UpdateModes.FIXED_UPDATE;
            return settings;
        }

        public static TimerSettings GetPhysicsStandard(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.completionMode = Timer.TimerCompletionModes.STOP;
            settings.updateMode = UpdateModes.FIXED_UPDATE;
            return settings;
        }

        public static TimerSettings GetPhysicsRepeater(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.completionMode = Timer.TimerCompletionModes.RESTART;
            settings.updateMode = UpdateModes.FIXED_UPDATE;
            return settings;
        }

        public static TimerSettings GetPhysicsReverser(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.completionMode = Timer.TimerCompletionModes.REVERSE;
            settings.updateMode = UpdateModes.FIXED_UPDATE;
            return settings;
        }

        public static TimerSettings GetBackgroundOneShot(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.alwaysPlay = true;
            settings.deactivateOnLoad = false;
            settings.updateMode = UpdateModes.UNSCALED_UPDATE;
            return settings;
        }

        public static TimerSettings GetBackgroundStandard(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.alwaysPlay = true;
            settings.deactivateOnLoad = false;
            settings.completionMode = Timer.TimerCompletionModes.STOP;
            settings.updateMode = UpdateModes.UNSCALED_UPDATE;
            return settings;
        }

        public static TimerSettings GetBackgroundRepeater(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.alwaysPlay = true;
            settings.deactivateOnLoad = false;
            settings.completionMode = Timer.TimerCompletionModes.RESTART;
            settings.updateMode = UpdateModes.UNSCALED_UPDATE;
            return settings;
        }

        public static TimerSettings GetBackgroundReverser(float duration)
        {
            TimerSettings settings = GetDefault(duration);
            settings.autoPlay = true;
            settings.alwaysPlay = true;
            settings.deactivateOnLoad = false;
            settings.completionMode = Timer.TimerCompletionModes.REVERSE;
            settings.updateMode = UpdateModes.UNSCALED_UPDATE;
            return settings;
        }
    }
}
