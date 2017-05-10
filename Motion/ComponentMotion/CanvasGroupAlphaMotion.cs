using UnityEngine;
using UnityEngine.UI;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an CanvasGroup alpha value towards a target alpha value.
    /// </summary>
    public class CanvasGroupAlphaMotion : FloatMotion
    {
        public CanvasGroup CanvasGroup { get; private set; }

        public CanvasGroupAlphaMotion() : base(1, null, null) { }
        public CanvasGroupAlphaMotion(float duration) : base(duration, null, null) { }
        public CanvasGroupAlphaMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public CanvasGroupAlphaMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public CanvasGroupAlphaMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(CanvasGroup canvasGroup, float targetAlpha)
        {
            CanvasGroup = canvasGroup;
            SetValues(() => this.CanvasGroup.alpha, targetAlpha);
        }

        public void SetParameters(CanvasGroup canvasGroup, float startAlpha, float targetAlpha)
        {
            CanvasGroup = canvasGroup;
            SetValues(startAlpha, targetAlpha);
        }

        protected override void OnValueChanged()
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = CurrentValue;
            }
        }
    }
}
