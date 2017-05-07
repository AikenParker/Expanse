using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public abstract class ValueMotion<T> : Motion where T : struct
    {
        public T StartValue { get; private set; }
        public T TargetValue { get; private set; }
        public T CurrentValue { get; protected set; }

        public float ValueProgress { get; private set; }

        public IEaseEquation EaseEquation { get; set; }

        private Func<T> getter;

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

            this.EaseEquation = new Linear.EaseNone();
        }

        public virtual void SetParameters(Func<T> getter, T targetValue)
        {
            this.getter = getter;
            this.TargetValue = targetValue;
        }

        public virtual void SetParameters(T startValue, T targetValue)
        {
            this.StartValue = startValue;
            this.TargetValue = targetValue;
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

            if (this.getter != null)
            {
                StartValue = this.getter();
            }

            OnProgressChanged();
        }

        protected override void OnPositionChanged()
        {
            if (this.IsMotionStarted && !this.IsMotionCompleted)
            {
                ValueProgress = this.EaseEquation.Update(this.MotionProgress, 0, 1, 1);
                OnProgressChanged();
            }
        }

        protected override void OnMotionCompleted()
        {
            ValueProgress = 1;
            OnProgressChanged();

            base.OnMotionCompleted();
        }

        protected abstract void OnProgressChanged();
    }
}
