using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Vector3 towards a target value.
    /// </summary>
    public class Vector3Motion : ValueMotion<Vector3>
    {
        public Vector3Motion() : base(1, null, null) { }
        public Vector3Motion(float duration) : base(duration, null, null) { }
        public Vector3Motion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public Vector3Motion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public Vector3Motion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = Vector3.LerpUnclamped(StartValue, TargetValue, ValueProgress);
        }
    }
}
