using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A Motion that moves a float value towards a target value.
    /// </summary>
    public class FloatMotion : ValueMotion<float>
    {
        public FloatMotion() : base(1, null, null) { }
        public FloatMotion(float duration) : base(duration, null, null) { }
        public FloatMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public FloatMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public FloatMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = Mathf.LerpUnclamped(StartValue, TargetValue, ValueProgress);
        }
    }
}
