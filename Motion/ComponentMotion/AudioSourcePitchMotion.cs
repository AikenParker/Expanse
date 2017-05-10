using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an AudioSource pitch value towards a target pitch.
    /// </summary>
    public class AudioSourcePitchMotion : FloatMotion
    {
        public AudioSource AudioSource { get; private set; }

        public AudioSourcePitchMotion() : base(1, null, null) { }
        public AudioSourcePitchMotion(float duration) : base(duration, null, null) { }
        public AudioSourcePitchMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public AudioSourcePitchMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public AudioSourcePitchMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(AudioSource audioSource, float targetPitch)
        {
            AudioSource = audioSource;
            SetValues(() => this.AudioSource.pitch, targetPitch);
        }

        public void SetParameters(AudioSource audioSource, float startPitch, float targetPitch)
        {
            AudioSource = audioSource;
            SetValues(startPitch, targetPitch);
        }

        protected override void OnValueChanged()
        {
            if (AudioSource != null)
            {
                AudioSource.pitch = CurrentValue;
            }
        }
    }
}
