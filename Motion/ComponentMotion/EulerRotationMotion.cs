using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Transform rotation towards a target rotation using euler angles.
    /// </summary>
    public class EulerRotationMotion : Vector3Motion
    {
        public Transform Transform { get; private set; }
        public bool UseLocal { get; set; }

        public EulerRotationMotion() : base(1, null, null) { }
        public EulerRotationMotion(float duration) : base(duration, null, null) { }
        public EulerRotationMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public EulerRotationMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public EulerRotationMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Transform transform, Vector3 targetRotation, bool useLocal = true)
        {
            Transform = transform;
            UseLocal = useLocal;
            SetValues(() => this.UseLocal ? this.Transform.localEulerAngles : this.Transform.eulerAngles, targetRotation);
        }

        public void SetParameters(Transform transform, Vector3 startRotation, Vector3 targetRotation, bool useLocal = true)
        {
            Transform = transform;
            UseLocal = useLocal;
            SetValues(startRotation, targetRotation);
        }

        protected override void OnValueChanged()
        {
            if (Transform != null)
            {
                if (UseLocal)
                    Transform.localEulerAngles = CurrentValue;
                else
                    Transform.eulerAngles = CurrentValue;
            }
        }
    }
}
