using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a Quaternion towards a target value.
    /// </summary>
    public class QuaternionMotion : ValueMotion<Quaternion>
    {
        public bool UseSlerp { get; set; }

        public QuaternionMotion() : base(1, null, null) { }
        public QuaternionMotion(float duration) : base(duration, null, null) { }
        public QuaternionMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public QuaternionMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public QuaternionMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            if (UseSlerp)
                CurrentValue = Quaternion.SlerpUnclamped(StartValue, TargetValue, ValueProgress);
            else
                CurrentValue = Quaternion.LerpUnclamped(StartValue, TargetValue, ValueProgress);
        }
    }
}