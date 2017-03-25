using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    public class GroupMotion : Motion
    {
        public GroupMotion(params Motion[] motions) : base()
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
