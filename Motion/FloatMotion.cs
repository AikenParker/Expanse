using System;

namespace Expanse
{
    public class FloatMotion : ValueMotion<float>
    {
        public FloatMotion(IEaseEquation easeEquation) : base(easeEquation)
        {

        }

        protected override void ApplyValue(float value)
        {

        }

        protected override void OnPositionChanged()
        {
            throw new NotImplementedException();
        }
    }
}
