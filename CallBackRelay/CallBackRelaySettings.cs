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

            /// <summary>
            /// Updates all objects.
            /// </summary>
            ALL,

            /// <summary>
            /// Updates a set amount of objects.
            /// </summary>
            SPREAD,

            /// <summary>
            /// Updates within a set amount of time.
            /// </summary>
            BUDGET
        }

        public enum SkipTypes
        {
            /// <summary>
            /// Do not skip any updates.
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Skip updates within a set amount of time.
            /// </summary>
            TIME,

            /// <summary>
            /// Skip updates after a set amount of frames.
            /// </summary>
            COUNT
        }
    }
}
