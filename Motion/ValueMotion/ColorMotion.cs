using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a Color towards a target value.
    /// </summary>
    public class ColorMotion : ValueMotion<Color>
    {
        public ColorMotion() : base(1, null, null) { }
        public ColorMotion(float duration) : base(duration, null, null) { }
        public ColorMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public ColorMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public ColorMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = Color.LerpUnclamped(StartValue, TargetValue, ValueProgress);
        }
    }
}
