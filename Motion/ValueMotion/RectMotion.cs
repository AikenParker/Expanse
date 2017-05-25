using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// A Motion that moves a Rect value towards a target value.
    /// </summary>
    public class RectMotion : ValueMotion<Rect>
    {
        public RectMotion() : base(1, null, null) { }
        public RectMotion(float duration) : base(duration, null, null) { }
        public RectMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public RectMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public RectMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        protected override void OnProgressChanged()
        {
            Rect startValue = this.StartValue;
            Rect targetValue = this.TargetValue;

            float valueProgress = this.ValueProgress;

            Vector2 position = Vector2.Lerp(startValue.position, targetValue.position, valueProgress);
            Vector2 size = Vector2.Lerp(startValue.size, targetValue.size, valueProgress);

            CurrentValue = new Rect(position, size);
        }
    }
}
