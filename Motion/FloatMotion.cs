using System;
using UnityEngine;

namespace Expanse
{
    public class FloatMotion : ValueMotion<float>
    {
        public FloatMotion() : base(1, null, null) { }
        public FloatMotion(float duration) : base(duration, null, null) { }
        public FloatMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public FloatMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public FloatMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = Mathf.Lerp(StartValue, TargetValue, ValueProgress);
        }
    }
}
