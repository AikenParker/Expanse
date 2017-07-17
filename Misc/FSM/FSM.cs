using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Base FSM class.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target object.</typeparam>
    public abstract class FSM<TTarget> : IUpdate
        where TTarget : class
    {
        protected readonly TTarget target;

        protected CallBackRelay cbr;
        protected MonoBehaviour attachedMonoBehaviour;

        /// <summary>
        /// Target object the FSM is attached to.
        /// </summary>
        public TTarget Target
        {
            get { return target; }
        }

        /// <summary>
        /// Creates a new FSM that will require manual updating.
        /// </summary>
        /// <param name="target">Target object this FSM is attached to.</param>
        public FSM(TTarget target)
        {
            this.target = target;
        }

        /// <summary>
        /// Creates a new FSM that will be updated by a CallBackRelay.
        /// </summary>
        /// <remarks>
        /// <para>If CBR is null the GlobalCBR is used instead.</para>
        /// <para>Target is used a attached MonoBehaviour if it is a MonoBehaviour.</para>
        /// </remarks>
        /// <param name="target">Target object this FSM is attached to.</param>
        /// <param name="cbr">CallBackRelay instance to subscribe to.</param>
        public FSM(TTarget target, CallBackRelay cbr)
        {
            this.target = target;

            cbr = cbr ?? CallBackRelay.GlobalCBR;
            this.cbr = cbr;

            attachedMonoBehaviour = target as MonoBehaviour;

            UnsafeUpdates = attachedMonoBehaviour == null;

            cbr.SubscribeToUpdate(this);
        }

        /// <summary>
        /// Creates a new FSM that will be updated by a CallBackRelay.
        /// </summary>
        /// <para>If CBR is null the GlobalCBR is used instead.</para>
        /// <param name="target">Target object this FSM is attached to.</param>
        /// <param name="cbr">CallBackRelay instance to subscribe to.</param>
        /// <param name="attachedMonoBehaviour">Attached MonoBehaviour instance.</param>
        public FSM(TTarget target, CallBackRelay cbr, MonoBehaviour attachedMonoBehaviour)
        {
            this.target = target;

            cbr = cbr ?? CallBackRelay.GlobalCBR;
            this.cbr = cbr;

            this.attachedMonoBehaviour = attachedMonoBehaviour;

            UnsafeUpdates = attachedMonoBehaviour == null;

            cbr.SubscribeToUpdate(this);
        }

        /// <summary>
        /// Shutdowns the current state and unsubscribes from the CBR if it needs to.
        /// </summary>
        public virtual void Shutdown()
        {
            if (cbr != null)
            {
                cbr.Unsubscribe(this);
            }
        }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        /// <param name="deltaTime">Time between this update and the last update.</param>
        public abstract void OnUpdate(float deltaTime);

        /// <summary>
        /// Returns true if this FSM has a target.
        /// </summary>
        public bool HasTarget
        {
            get { return target != null; }
        }

        /// <summary>
        /// Returns true if this FSM has a current state.
        /// </summary>
        public abstract bool HasState { get; }

        /// <summary>
        /// Will this update even if the attached MonoBehaviour is destroyed or non-existent?
        /// <para>Only matters if attached to a CBR.</para>
        /// </summary>
        public bool UnsafeUpdates { get; set; }
        /// <summary>
        /// Will this update even if the attached MonoBehaviour is disabled or inactive?
        /// <para>Only matters if attached to a CBR.</para>
        /// </summary>
        public bool AlwaysUpdate { get; set; }
        /// <summary>
        /// Will the update use unscaled deltaTime?
        /// <para>Only matters if attached to a CBR.</para>
        /// </summary>
        public bool UnscaledDelta { get; set; }
        /// <summary>
        /// Attached MonoBehaviour object to this FSM.
        /// <para>Only matters if attached to a CBR.</para>
        /// </summary>
        MonoBehaviour IUnity.MonoBehaviour { get { return attachedMonoBehaviour; } }
        /// <summary>
        /// Attached GameObject to this FSM.
        /// <para>Only matters if attached to a CBR.</para>
        /// </summary>
        GameObject IUnity.gameObject
        {
            get
            {
                if (attachedMonoBehaviour != null)
                    return attachedMonoBehaviour.gameObject;

                else return null;
            }
        }
    }
}
