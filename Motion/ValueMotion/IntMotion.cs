using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// A Motion that moves a int value towards a target value.
    /// </summary>
    public class IntMotion : ValueMotion<int>
    {
        private FloatConversionMethod floatConversionMethod = FloatConversionMethod.ROUND;
        public FloatConversionMethod FloatConversionMethod
        {
            get { return floatConversionMethod; }
            set { floatConversionMethod = value; }
        }

        public IntMotion() : base(1, null, null) { }
        public IntMotion(float duration) : base(duration, null, null) { }
        public IntMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public IntMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public IntMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            CurrentValue = FloatConversionUtil.ConvertToInt(Mathf.LerpUnclamped(StartValue, TargetValue, ValueProgress), floatConversionMethod);
        }
    }
}
