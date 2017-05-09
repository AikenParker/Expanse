using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a RectTransform anchored position towards a target position.
    /// </summary>
    public class AnchoredPositionMotion : Vector2Motion
    {
        public RectTransform RectTransform { get; private set; }

        public AnchoredPositionMotion() : base(1, null, null) { }
        public AnchoredPositionMotion(float duration) : base(duration, null, null) { }
        public AnchoredPositionMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public AnchoredPositionMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public AnchoredPositionMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(RectTransform rectTransform, Vector2 targetAnchoredPosition)
        {
            RectTransform = rectTransform;
            SetParameters(() => this.RectTransform.anchoredPosition, targetAnchoredPosition);
        }

        public void SetParameters(RectTransform rectTransform, Vector2 startAnchoredPosition, Vector2 targetAnchoredPosition)
        {
            RectTransform = rectTransform;
            SetParameters(startAnchoredPosition, targetAnchoredPosition);
        }

        protected override void OnValueChanged()
        {
            if (RectTransform != null)
            {
                RectTransform.anchoredPosition = CurrentValue;
            }
        }
    }
}
