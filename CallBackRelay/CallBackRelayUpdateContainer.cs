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
                return this.GetIsUnscaled() ? this.UnscaledDeltaTime : this.DeltaTime;
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
                case CallBackRelaySettings.SkipTypes.COUNT:
                    return (settings.frameIndex % (settings.skipFrames + 1)) != 0;

                case CallBackRelaySettings.SkipTypes.TIME:
                    return TrueDeltaTime < settings.skipTime;

                case CallBackRelaySettings.SkipTypes.NONE:
                    return false;

                default:
                    throw new UnexpectedException();
            }
        }

        public bool GetIsUnscaled()
        {
            if (updateObj == null)
                throw new ArgumentNullException("updateObj");

            IComplexUpdate complexUpdateObj = updateObj as IComplexUpdate;

            if (complexUpdateObj != null)
                return complexUpdateObj.UnscaledDelta;
            else return false;
        }

        public UpdateResult GetUpdateResult()
        {
            if (updateObj == null)
                return UpdateResult.REMOVE;

            IComplexUpdate complexUpdateObj = updateObj as IComplexUpdate;

            if (complexUpdateObj != null)
            {
                bool unsafeUpdates = complexUpdateObj.UnsafeUpdates;

                if (unsafeUpdates)
                    return UpdateResult.SUCCESS;
            }

            if (!updateObj.MonoBehaviour)
                return UpdateResult.REMOVE;

            bool activeOrEnabled = updateObj.MonoBehaviour.isActiveAndEnabled;

            if (complexUpdateObj != null)
            {
                if (!complexUpdateObj.AlwaysUpdate && !activeOrEnabled)
                    return UpdateResult.FAIL;
            }
            else
            {
                if (!activeOrEnabled)
                    return UpdateResult.FAIL;
            }

            return UpdateResult.SUCCESS;
        }

        public enum UpdateResult
        {
            SUCCESS = 0,
            FAIL = 1,
            REMOVE = 2
        }
    }
}
