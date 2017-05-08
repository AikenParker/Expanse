using UnityEngine;
using UnityEngine.UI;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Light color value towards a target color.
    /// </summary>
    public class LightColorMotion : ColorMotion
    {
        public Light Light { get; private set; }

        public LightColorMotion() : base(1, null, null) { }
        public LightColorMotion(float duration) : base(duration, null, null) { }
        public LightColorMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public LightColorMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public LightColorMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Light light, Color targetColor)
        {
            Light = light;
            SetParameters(() => this.Light.color, targetColor);
        }

        public void SetParameters(Light light, Color startColor, Color targetColor)
        {
            Light = light;
            SetParameters(startColor, targetColor);
        }

        protected override void OnValueChanged()
        {
            if (Light != null)
            {
                Light.color = CurrentValue;
            }
        }
    }
}
