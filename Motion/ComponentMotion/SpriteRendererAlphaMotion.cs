using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a SpriteRenderer alpha value towards a target alpha value.
    /// </summary>
    public class SpriteRendererAlphaMotion : FloatMotion
    {
        public SpriteRenderer SpriteRenderer { get; private set; }

        public SpriteRendererAlphaMotion() : base(1, null, null) { }
        public SpriteRendererAlphaMotion(float duration) : base(duration, null, null) { }
        public SpriteRendererAlphaMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public SpriteRendererAlphaMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public SpriteRendererAlphaMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(SpriteRenderer spriteRenderer, float targetAlpha)
        {
            SpriteRenderer = spriteRenderer;
            SetValues(() => this.SpriteRenderer.color.a, targetAlpha);
        }

        public void SetParameters(SpriteRenderer spriteRenderer, float startAlpha, float targetAlpha)
        {
            SpriteRenderer = spriteRenderer;
            SetValues(startAlpha, targetAlpha);
        }

        protected override void OnValueChanged()
        {
            if (SpriteRenderer != null)
            {
                SpriteRenderer.color = SpriteRenderer.color.WithAlpha(CurrentValue);
            }
        }
    }
}
