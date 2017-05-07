using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// All attached motions are played at the same time.
    /// </summary>
    public class GroupMotion : Motion
    {
        private List<Motion> motions = new List<Motion>();

        public override float Duration
        {
            get
            {
                float maxDuration = 0f;

                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];
                    float motionDuration = motion.TotalDuration;

                    if (motionDuration > maxDuration)
                        maxDuration = motionDuration;
                }

                return maxDuration;
            }
        }

        public override float TrueDuration
        {
            get
            {
                float maxTrueDuration = 0f;

                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];
                    float motionDuration = motion.TrueDuration;

                    if (motionDuration > maxTrueDuration)
                        maxTrueDuration = motionDuration;
                }

                return (maxTrueDuration + StartDelay + EndDelay) / PlaybackRate;
            }
        }

        private float motionCompletePosition;

        public GroupMotion() : this(null, null, null, null) { }
        public GroupMotion(Motion motionA, Motion motionB) : this(null, null, motionA, motionB) { }
        public GroupMotion(CallBackRelay cbr, Motion motionA, Motion motionB) : this(cbr, null, motionA, motionB) { }
        public GroupMotion(MonoBehaviour attachedMonobehaviour, Motion motionA, Motion motionB) : this(null, attachedMonobehaviour, motionA, motionB) { }
        public GroupMotion(CallBackRelay cbr, MonoBehaviour attachedMonobehaviour, Motion motionA, Motion motionB) : base(cbr, attachedMonobehaviour)
        {
            Add(motionA);
            Add(motionB);
        }

        public void Add(Motion motion)
        {
            ThrowIfActive();

            if (motion.IsActive)
                throw new ActiveException("Cannot add active motion to group motion.");

            motion.IsActive = true;
            motions.Add(motion);
        }

        public bool Remove(Motion motion)
        {
            ThrowIfActive();

            motion.IsActive = false;
            return motions.Remove(motion);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsActive)
                return;

            // Check starting events

            if (!IsMotionStarted)
            {
                if (!IsStarted)
                {
                    OnStarted();
                }

                if (Position > StartDelay)
                {
                    OnMotionStarted();
                }
            }

            // Apply position change

            float gain = deltaTime * PlaybackRate;

            float newPostion = Mathf.Max(Position + gain, 0);
            Position = newPostion;

            OnUpdated();

            if (IsMotionStarted)
            {
                OnMotionUpdated();

                // Update child motions

                int completeCount = 0;

                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];

                    if (!motion.IsCompleted)
                        motion.OnUpdate(gain);

                    if (motion.IsCompleted)
                        completeCount++;
                }

                // Check completed events

                if (!IsCompleted)
                {
                    if (!IsMotionCompleted && completeCount == motions.Count)
                    {
                        OnMotionCompleted();
                    }

                    if (IsMotionCompleted && Position >= motionCompletePosition + EndDelay)
                    {
                        OnCompleted();
                    }
                }
            }
        }

        protected override void OnMotionCompleted()
        {
            base.OnMotionCompleted();

            motionCompletePosition = Position;
        }
    }
}
