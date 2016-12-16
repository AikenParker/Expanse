using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// INCOMPLETE
    /// </summary>
    public abstract class Motion<T> : IMotion
    {
        public UpdateModes UpdateMode { get; protected set; }
        public int Priority { get; set; }

        GameObject attachedGameObject;
        MonoBehaviour attachedMonoBehaviour;

        public T StartValue { get; private set; }
        public T TargetValue { get; private set; }
        public float StartDelay { get; set; }
        public float EndDelay { get; set; }
        public float Duration { get; set; }

        public event Action Completed;
        public event Action Started;

        protected IEase easeEquation;

        protected Motion(T startValue, T targetValue)
        {
            this.StartValue = startValue;
            this.TargetValue = targetValue;

            SetEaseEquation<Linear.EaseNone>();
        }

        public void SetEaseEquation<U>()where U : IEase, new()
        {
            easeEquation = new U();
        }

        public void Update()
        {

        }

        void IUpdate.OnUpdate(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public MonoBehaviour AttachedMonoBehaviour
        {
            get { return attachedMonoBehaviour; }
            protected set
            {
                attachedMonoBehaviour = value;
                attachedGameObject = value.gameObject;
            }
        }

        bool IComplexUpdate.AlwaysUpdate
        {
            get { return false; }
        }

        GameObject IUnity.gameObject
        {
            get { return this.attachedGameObject; }
        }

        MonoBehaviour IUnity.MonoBehaviour
        {
            get { return this.attachedMonoBehaviour; }
        }

        bool IComplexUpdate.UnsafeUpdates
        {
            get { return false; }
        }

        bool IComplexUpdate.UnscaledDelta
        {
            get { return this.UpdateMode.EqualsAny(UpdateModes.UNSCALED_UPDATE, UpdateModes.UNSCALED_LATE_UPDATE); }
        }

        Action IMotion.Completed
        {
            get { return Completed; }

            set { Completed += value; }
        }

        Action IMotion.Started
        {
            get { return Started; }

            set { Started += value; }
        }
    }
}
