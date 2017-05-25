using System;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse
{
    public class CallBackRelayUpdateContainer
    {
        public readonly IUpdate updateObj;

        public float LastFrameTime { get; private set; }
        public float LastUnscaledFrameTime { get; private set; }

        public CallBackRelayUpdateContainer(IUpdate updateObj)
        {
            this.updateObj = updateObj;

            this.LastFrameTime = Time.time;
            this.LastUnscaledFrameTime = Time.unscaledTime;
        }

        public float DeltaTime
        {
            get
            {
                return Time.time - LastFrameTime;
            }
        }

        public float UnscaledDeltaTime
        {
            get
            {
                return Time.unscaledTime - LastUnscaledFrameTime;
            }
        }

        public float TrueDeltaTime
        {
            get
            {
                return updateObj.UnscaledDelta ? this.UnscaledDeltaTime : this.DeltaTime;
            }
        }

        public bool TryUpdate(CallBackRelaySettings settings)
        {
            bool isSkipFrame = GetIsSkipFrame(settings);

            if (!isSkipFrame)
            {
                updateObj.OnUpdate(TrueDeltaTime);

                LastFrameTime = Time.time;
                LastUnscaledFrameTime = Time.unscaledTime;
            }

            return !isSkipFrame;
        }

        private bool GetIsSkipFrame(CallBackRelaySettings settings)
        {
            switch (settings.skipType)
            {
                case CallBackRelaySettings.SkipTypes.Count:
                    return (settings.frameIndex % (settings.skipFrames + 1)) != 0;

                case CallBackRelaySettings.SkipTypes.Time:
                    return TrueDeltaTime < settings.skipTime;

                case CallBackRelaySettings.SkipTypes.None:
                    return false;

                default:
                    throw new UnexpectedException();
            }
        }

        public UpdateResult GetUpdateResult()
        {
            if (updateObj == null)
                return UpdateResult.Remove;

            if (updateObj.UnsafeUpdates)
                return UpdateResult.Success;

            if (updateObj.MonoBehaviour == null)
                return UpdateResult.Remove;

            if (!updateObj.AlwaysUpdate && !updateObj.MonoBehaviour.isActiveAndEnabled)
                return UpdateResult.Fail;

            return UpdateResult.Success;
        }

        public enum UpdateResult
        {
            Success = 0,
            Fail = 1,
            Remove = 2
        }
    }
}
