using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public class SequenceMotion : Motion
    {
        public SequenceMotion(params Motion[] motions) : base()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            throw new NotImplementedException();
        }

        protected override void OnPositionChanged()
        {
            throw new NotImplementedException();
        }
    }
}
