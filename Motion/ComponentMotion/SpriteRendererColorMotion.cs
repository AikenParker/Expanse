using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves an SpriteRenderer color value towards a target color.
    /// </summary>
    public class SpriteRendererColorMotion : ColorMotion
    {
        public SpriteRenderer SpriteRenderer { get; private set; }

        public SpriteRendererColorMotion() : base(1, null, null) { }
        public SpriteRendererColorMotion(float duration) : base(duration, null, null) { }
        public SpriteRendererColorMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public SpriteRendererColorMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public SpriteRendererColorMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(SpriteRenderer spriteRenderer, Color targetColor)
        {
            SpriteRenderer = spriteRenderer;
            SetValues(() => this.SpriteRenderer.color, targetColor);
        }

        public void SetParameters(SpriteRenderer spriteRenderer, Color startColor, Color targetColor)
        {
            SpriteRenderer = spriteRenderer;
            SetValues(startColor, targetColor);
        }

        protected override void OnValueChanged()
        {
            if (SpriteRenderer != null)
            {
                SpriteRenderer.color = CurrentValue;
            }
        }
    }
}
