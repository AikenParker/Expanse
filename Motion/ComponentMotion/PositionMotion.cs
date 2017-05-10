using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Transform position towards a target position.
    /// </summary>
    public class PositionMotion : Vector3Motion
    {
        public Transform Transform { get; private set; }
        public bool UseLocal { get; set; }

        public PositionMotion() : base(1, null, null) { }
        public PositionMotion(float duration) : base(duration, null, null) { }
        public PositionMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public PositionMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public PositionMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Transform transform, Vector3 targetPosition, bool useLocal = true)
        {
            Transform = transform;
            UseLocal = useLocal;
            SetValues(() => this.UseLocal ? this.Transform.localPosition : this.Transform.position, targetPosition);
        }

        public void SetParameters(Transform transform, Vector3 startPosition, Vector3 targetPosition, bool useLocal = true)
        {
            Transform = transform;
            UseLocal = useLocal;
            SetValues(startPosition, targetPosition);
        }

        protected override void OnValueChanged()
        {
            if (Transform != null)
            {
                if (UseLocal)
                    Transform.localPosition = CurrentValue;
                else
                    Transform.position = CurrentValue;
            }
        }
    }
}
