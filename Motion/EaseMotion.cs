using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public abstract class EaseMotion<T> : ValueMotion<T>
    {
        protected IEase easeEquation;

        public float EaseParam1 { get; set; }
        public float EaseParam2 { get; set; }

        public void SetEaseEquation<U>() where U : IEase, new()
        {
            easeEquation = new U();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsActive)
                return;

            float processedDelta = deltaTime * PlaybackRate;

            float rawCurrentTime = CurrentTime + processedDelta;

            CurrentTime = Mathf.Clamp(rawCurrentTime, 0f, Duration);

            float value = easeEquation.Update(CurrentTime, 0f, 1f, Duration, EaseParam1, EaseParam2);

            ApplyValue(value);

            //bool hasCompleted = (PlaybackRate > 0 && rawCurrentTime >= Duration)
            //    || (PlaybackRate < 0 && rawCurrentTime <= 0f);
        }
    }
}
