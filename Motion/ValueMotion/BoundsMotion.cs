using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A Motion that moves a Bounds value towards a target value.
    /// </summary>
    public class BoundsMotion : ValueMotion<Bounds>
    {
        public BoundsMotion() : base(1, null, null) { }
        public BoundsMotion(float duration) : base(duration, null, null) { }
        public BoundsMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public BoundsMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public BoundsMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            Bounds startValue = this.StartValue;
            Bounds targetValue = this.TargetValue;

            float valueProgress = this.ValueProgress;

            Vector3 center = Vector2.Lerp(startValue.center, targetValue.center, valueProgress);
            Vector3 size = Vector2.Lerp(startValue.size, targetValue.size, valueProgress);

            CurrentValue = new Bounds(center, size);
        }
    }
}
