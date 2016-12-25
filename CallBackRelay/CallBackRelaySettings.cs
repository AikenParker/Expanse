using System;

namespace Expanse
{
    [Serializable]
    public class CallBackRelaySettings
    {
        public UpdateTypes updateType = UpdateTypes.ALL;
        public SkipTypes skipType = SkipTypes.NONE;

        [NonSerialized]
        public int frameIndex;

        public int skipFrames;
        public float skipTime;

        public int spreadCount = 1;

        public float frameBudget;

        public enum UpdateTypes
        {
            NONE = 0,
            ALL,
            SPREAD,
            BUDGET
        }

        public enum SkipTypes
        {
            NONE = 0,
            TIME,
            COUNT
        }
    }
}
