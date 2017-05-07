using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Transform scale towards a target scale.
    /// </summary>
    public class ScaleMotion : Vector3Motion
    {
        public Transform Transform { get; private set; }

        public ScaleMotion() : base(1, null, null) { }
        public ScaleMotion(float duration) : base(duration, null, null) { }
        public ScaleMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public ScaleMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public ScaleMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Transform transform, Vector3 targetScale)
        {
            Transform = transform;
            SetParameters(() => this.Transform.localScale, targetScale);
        }

        public void SetParameters(Transform transform, Vector3 startScale, Vector3 targetScale)
        {
            Transform = transform;
            SetParameters(startScale, targetScale);
        }

        protected override void OnValueChanged()
        {
            if (Transform != null)
            {
                Transform.localScale = CurrentValue;
            }
        }
    }
}
