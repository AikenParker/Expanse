using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a RectTransform size delta towards a target size.
    /// </summary>
    public class SizeDeltaMotion : Vector2Motion
    {
        public RectTransform RectTransform { get; private set; }

        public SizeDeltaMotion() : base(1, null, null) { }
        public SizeDeltaMotion(float duration) : base(duration, null, null) { }
        public SizeDeltaMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public SizeDeltaMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public SizeDeltaMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(RectTransform rectTransform, Vector2 targetSize)
        {
            RectTransform = rectTransform;
            SetValues(() => this.RectTransform.sizeDelta, targetSize);
        }

        public void SetParameters(RectTransform rectTransform, Vector2 startSize, Vector2 targetSize)
        {
            RectTransform = rectTransform;
            SetValues(startSize, targetSize);
        }

        protected override void OnValueChanged()
        {
            if (RectTransform != null)
            {
                RectTransform.sizeDelta = CurrentValue;
            }
        }
    }
}
