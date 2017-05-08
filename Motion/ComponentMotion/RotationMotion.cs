using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Transform rotation towards a target rotation using quaternions.
    /// </summary>
    public class RotationMotion : QuaternionMotion
    {
        public Transform Transform { get; private set; }
        public bool UseLocal { get; set; }

        public RotationMotion() : base(1, null, null) { }
        public RotationMotion(float duration) : base(duration, null, null) { }
        public RotationMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public RotationMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public RotationMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Transform transform, Quaternion targetRotation, bool useLocal = true)
        {
            Transform = transform;
            UseLocal = useLocal;
            SetParameters(() => this.UseLocal ? this.Transform.localRotation : this.Transform.rotation, targetRotation);
        }

        public void SetParameters(Transform transform, Quaternion startRotation, Quaternion targetRotation, bool useLocal = true)
        {
            Transform = transform;
            UseLocal = useLocal;
            SetParameters(startRotation, targetRotation);
        }

        protected override void OnValueChanged()
        {
            if (Transform != null)
            {
                if (UseLocal)
                    Transform.localRotation = CurrentValue;
                else
                    Transform.rotation = CurrentValue;
            }
        }
    }
}
