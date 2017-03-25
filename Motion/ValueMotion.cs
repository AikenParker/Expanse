using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public abstract class ValueMotion<T> : Motion
    {
        public IEaseEquation EaseEquation { get; set; }

        public ValueMotion(IEaseEquation easeEquation) : base()
        {
            this.EaseEquation = easeEquation;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsActive)
                return;
        }

        protected abstract void ApplyValue(float value);
    }
}
