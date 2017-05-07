using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// All attached motions are played one after the other.
    /// </summary>
    public class SequenceMotion : Motion
    {
        private List<Motion> motions = new List<Motion>();

        public override float Duration
        {
            get
            {
                float sumDuration = 0f;

                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];
                    sumDuration += motion.TotalDuration;
                }

                return sumDuration;
            }
        }

        public override float TrueDuration
        {
            get
            {
                float sumTrueDuration = 0f;

                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];
                    sumTrueDuration += motion.TrueDuration;
                }

                return (sumTrueDuration + StartDelay + EndDelay) / PlaybackRate;
            }
        }

        private float motionCompletePosition;

        public SequenceMotion() : this(null, null, null, null) { }
        public SequenceMotion(Motion motionA, Motion motionB) : this(null, null, motionA, motionB) { }
        public SequenceMotion(CallBackRelay cbr, Motion motionA, Motion motionB) : this(cbr, null, motionA, motionB) { }
        public SequenceMotion(MonoBehaviour attachedMonobehaviour, Motion motionA, Motion motionB) : this(null, attachedMonobehaviour, motionA, motionB) { }
        public SequenceMotion(CallBackRelay cbr, MonoBehaviour attachedMonobehaviour, Motion motionA, Motion motionB) : base(cbr, attachedMonobehaviour)
        {
            Add(motionA);
            Add(motionB);
        }

        public void Add(Motion motion)
        {
            ThrowIfActive();

            if (motion.IsActive)
                throw new ActiveException("Cannot add active motion to sequence motion.");

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

                bool allMotionsComplete = true;

                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];

                    if (!motion.IsCompleted)
                    {
                        motion.OnUpdate(gain);

                        allMotionsComplete = false;
                        break;
                    }
                }

                // Check completed events

                if (!IsCompleted)
                {
                    if (!IsMotionCompleted && allMotionsComplete)
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
