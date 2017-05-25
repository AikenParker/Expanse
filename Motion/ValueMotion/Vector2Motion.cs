using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a Vector2 towards a target value.
    /// </summary>
    public class Vector2Motion : ValueMotion<Vector2>
    {
        public Vector2Motion() : base(1, null, null) { }
        public Vector2Motion(float duration) : base(duration, null, null) { }
        public Vector2Motion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public Vector2Motion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public Vector2Motion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = Vector2.LerpUnclamped(StartValue, TargetValue, ValueProgress);
        }
    }
}
