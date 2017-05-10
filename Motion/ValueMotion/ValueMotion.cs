using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A motion that moves a value of any type towards a target value.
    /// </summary>
    public abstract class ValueMotion<T> : Motion where T : struct
    {
        public T StartValue { get; protected set; }
        public T TargetValue { get; protected set; }

        private T currentValue;
        public T CurrentValue
        {
            get { return currentValue; }
            protected set
            {
                currentValue = value;
                OnValueChanged();
                ValueChanged.SafeInvoke(currentValue);
            }
        }

        public event Action<T> ValueChanged;

        public float ValueProgress { get; private set; }

        public IEaseEquation EaseEquation { get; set; }

        private Func<T> startValueGetter;
        private Func<T> targetValueGetter;

        private float duration;
        public override float Duration
        {
            get { return duration; }
        }

        public ValueMotion() : this(1, null, null) { }
        public ValueMotion(float duration) : this(duration, null, null) { }
        public ValueMotion(float duration, CallBackRelay cbr) : this(duration, cbr, null) { }
        public ValueMotion(float duration, MonoBehaviour attachedMonobehaviour) : this(duration, null, attachedMonobehaviour) { }
        public ValueMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(cbr, attachedMonobehaviour)
        {
            this.duration = duration;

            this.EaseEquation = Expanse.EaseEquation.Default;
        }

        /// <summary>
        /// Set base parameters with constant start and target values.
        /// </summary>
        public virtual void SetValues(T startValue, T targetValue)
        {
            this.StartValue = startValue;
            this.TargetValue = targetValue;
            this.startValueGetter = null;
            this.targetValueGetter = null;
        }

        /// <summary>
        /// Set base parameters with a variable start value (Evaluated when motion starts) and constant target value.
        /// </summary>
        public virtual void SetValues(Func<T> startValueGetter, T targetValue)
        {
            this.startValueGetter = startValueGetter;
            this.TargetValue = targetValue;
            this.targetValueGetter = null;
        }

        /// <summary>
        /// Set base parameters with constant start value and variable target value. (Evaluated when motion starts)
        /// </summary>
        public virtual void SetValues(T startValue, Func<T> targetValueGetter)
        {
            this.StartValue = startValue;
            this.targetValueGetter = targetValueGetter;
            this.startValueGetter = null;
        }

        /// <summary>
        /// Set base parameters with variable start and target value. (Evaluated when motion starts)
        /// </summary>
        public virtual void SetValues(Func<T> startValueGetter, Func<T> targetValueGetter)
        {
            this.startValueGetter = startValueGetter;
            this.targetValueGetter = targetValueGetter;
        }

        public virtual void SetEaseEquation<U>() where U : IEaseEquation, new()
        {
            this.EaseEquation = new U();
        }

        public virtual void SetEaseEquation(IEaseEquation easeEquation)
        {
            this.EaseEquation = easeEquation;
        }

        protected override void OnMotionStarted()
        {
            base.OnMotionStarted();

            ValueProgress = 0;

            if (this.startValueGetter != null)
            {
                StartValue = this.startValueGetter();
            }

            if (this.targetValueGetter != null)
            {
                TargetValue = this.targetValueGetter();
            }

            OnProgressChanged();
        }

        protected override void OnPositionChanged()
        {
            if (this.IsMotionStarted && !this.IsMotionCompleted)
            {
                ValueProgress = this.EaseEquation.Evaluate(this.MotionProgress, 0, 1, 1);
                OnProgressChanged();
            }
        }

        protected virtual void OnValueChanged() { }

        protected override void OnMotionCompleted()
        {
            ValueProgress = 1;
            OnProgressChanged();

            base.OnMotionCompleted();
        }

        protected abstract void OnProgressChanged();
    }
}
