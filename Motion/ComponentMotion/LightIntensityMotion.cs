using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a Light intensity value towards a target value.
    /// </summary>
    public class LightIntensityMotion : FloatMotion
    {
        public Light Light { get; private set; }

        public LightIntensityMotion() : base(1, null, null) { }
        public LightIntensityMotion(float duration) : base(duration, null, null) { }
        public LightIntensityMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public LightIntensityMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public LightIntensityMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Light light, float targetIntensity)
        {
            Light = light;
            SetValues(() => this.Light.intensity, targetIntensity);
        }

        public void SetParameters(Light light, float startIntensity, float targetIntensity)
        {
            Light = light;
            SetValues(startIntensity, targetIntensity);
        }

        protected override void OnValueChanged()
        {
            if (Light != null)
            {
                Light.intensity = CurrentValue;
            }
        }
    }
}
