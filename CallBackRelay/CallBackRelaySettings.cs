using System;

namespace Expanse
{
    [Serializable]
    public class CallBackRelaySettings
    {
        public UpdateTypes updateType = UpdateTypes.All;
        public SkipTypes skipType = SkipTypes.None;

        [NonSerialized]
        public int frameIndex;

        public int skipFrames;
        public float skipTime;

        public int spreadCount = 1;

        public float frameBudget;

        public enum UpdateTypes
        {
            None = 0,

            /// <summary>
            /// Updates all objects.
            /// </summary>
            All,

            /// <summary>
            /// Updates a set amount of objects.
            /// </summary>
            Spread,

            /// <summary>
            /// Updates within a set amount of time.
            /// </summary>
            Budget
        }

        public enum SkipTypes
        {
            /// <summary>
            /// Do not skip any updates.
            /// </summary>
            None = 0,

            /// <summary>
            /// Skip updates within a set amount of time.
            /// </summary>
            Time,

            /// <summary>
            /// Skip updates after a set amount of frames.
            /// </summary>
            Count
        }
    }
}
