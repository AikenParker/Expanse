using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A Motion that moves a DateTime value towards a target value.
    /// </summary>
    public class DateTimeMotion : ValueMotion<DateTime>
    {
        DateTimeKind DateTimeKind { get; set; }

        public DateTimeMotion() : base(1, null, null) { }
        public DateTimeMotion(float duration) : base(duration, null, null) { }
        public DateTimeMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public DateTimeMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public DateTimeMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public override void SetParameters(DateTime startValue, DateTime targetValue)
        {
            base.SetParameters(startValue, targetValue);

            DateTimeKind = startValue.Kind;
        }

        public override void SetParameters(Func<DateTime> getter, DateTime targetValue)
        {
            base.SetParameters(getter, targetValue);

            DateTimeKind = targetValue.Kind;
        }

        protected override void OnProgressChanged()
        {
            long ticks = FloatConversionUtil.ConvertToLong(Mathf.LerpUnclamped(StartValue.Ticks, TargetValue.Ticks, ValueProgress), FloatConversionMethod.ROUND);

            CurrentValue = new DateTime(ticks, DateTimeKind);
        }
    }
}
