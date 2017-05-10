using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A Motion that moves a DateTime value towards a target value.
    /// </summary>
    public class DateTimeMotion : ValueMotion<DateTime>
    {
        public DateTimeMotion() : base(1, null, null) { }
        public DateTimeMotion(float duration) : base(duration, null, null) { }
        public DateTimeMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public DateTimeMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public DateTimeMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            DateTime startValue = this.StartValue;
            DateTime targetValue = this.TargetValue;

            long ticks = FloatConversionUtil.ConvertToLong(Mathf.LerpUnclamped(startValue.Ticks, targetValue.Ticks, ValueProgress), FloatConversionMethod.ROUND);

            CurrentValue = new DateTime(ticks, startValue.Kind);
        }
    }
}
