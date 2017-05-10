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

        /// <summary>
        /// Adds a new motion into the sequence.
        /// </summary>
        public void Add(Motion motion)
        {
            ThrowIfActive();

            if (motion.IsActive)
                throw new ActiveException("Cannot add active motion to sequence motion.");

            if (motions.ContainsObject(motion))
                throw new ArgumentException("motion is already in sequence motion.");

            motion.CBR = null;
            motion.IsActive = IsActive;

            motions.Add(motion);
        }

        /// <summary>
        /// Removes a motion from the sequence.
        /// </summary>
        public bool Remove(Motion motion)
        {
            ThrowIfActive();

            motion.IsActive = false;
            motion.CBR = CallBackRelay.GlobalCBR;

            return motions.Remove(motion);
        }

        /// <summary>
        /// Begins/resumes motion playback.
        /// </summary>
        public override void Start()
        {
            base.Start();

            for (int i = 0; i < motions.Count; i++)
            {
                Motion motion = motions[i];
                motion.Start();
            }
        }

        /// <summary>
        /// Ends/pauses motion playback.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            for (int i = 0; i < motions.Count; i++)
            {
                Motion motion = motions[i];
                motion.Stop();
            }
        }

        /// <summary>
        /// Resets the state of the motion and starts playback.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < motions.Count; i++)
            {
                Motion motion = motions[i];
                motion.Reset();
            }
        }

        /// <summary>
        /// Resets the state of the motion and stops playback to after the start delay.
        /// </summary>
        public override void Restart()
        {
            base.Restart();

            for (int i = 0; i < motions.Count; i++)
            {
                Motion motion = motions[i];
                motion.Restart();
            }
        }

        /// <summary>
        /// Resets the state of the motion and starts playback to after the start delay.
        /// </summary>
        public override void ResetToMotion()
        {
            base.ResetToMotion();

            for (int i = 0; i < motions.Count; i++)
            {
                Motion motion = motions[i];
                motion.ResetToMotion();
            }
        }

        /// <summary>
        /// Resets the state of the motion and starts playback to after the start delay.
        /// </summary>
        public override void RestartToMotion()
        {
            base.RestartToMotion();

            for (int i = 0; i < motions.Count; i++)
            {
                Motion motion = motions[i];
                motion.RestartToMotion();
            }
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
