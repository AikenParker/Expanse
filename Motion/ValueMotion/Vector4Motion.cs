using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a Vector4 towards a target value.
    /// </summary>
    public class Vector4Motion : ValueMotion<Vector4>
    {
        public Vector4Motion() : base(1, null, null) { }
        public Vector4Motion(float duration) : base(duration, null, null) { }
        public Vector4Motion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public Vector4Motion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public Vector4Motion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = Vector4.LerpUnclamped(StartValue, TargetValue, ValueProgress);
        }
    }
}
