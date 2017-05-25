using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves an AudioSource volume value towards a target volume.
    /// </summary>
    public class AudioSourceVolumeMotion : FloatMotion
    {
        public AudioSource AudioSource { get; private set; }

        public AudioSourceVolumeMotion() : base(1, null, null) { }
        public AudioSourceVolumeMotion(float duration) : base(duration, null, null) { }
        public AudioSourceVolumeMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public AudioSourceVolumeMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public AudioSourceVolumeMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(AudioSource audioSource, float targetVolume)
        {
            AudioSource = audioSource;
            SetValues(() => this.AudioSource.volume, targetVolume);
        }

        public void SetParameters(AudioSource audioSource, float startVolume, float targetVolume)
        {
            AudioSource = audioSource;
            SetValues(startVolume, targetVolume);
        }

        protected override void OnValueChanged()
        {
            if (AudioSource != null)
            {
                AudioSource.volume = CurrentValue;
            }
        }
    }
}
