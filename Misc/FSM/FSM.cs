using System;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Simple single-state finite state machine.
    /// </summary>
    /// <typeparam name="T">Type of the target object.</typeparam>
    public class FSM<T> : IUpdate where T : class
    {
        protected readonly T target;
        protected State state;

        protected CallBackRelay cbr;
        protected MonoBehaviour attachedMonoBehaviour;

        /// <summary>
        /// Target object the FSM is attached to.
        /// </summary>
        public T Target
        {
            get { return target; }
        }

        /// <summary>
        /// Creates a new FSM that will require manual updating.
        /// </summary>
        /// <param name="target">Target object this FSM is attached to.</param>
        public FSM(T target)
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
        public FSM(T target, CallBackRelay cbr)
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
        public FSM(T target, CallBackRelay cbr, MonoBehaviour attachedMonoBehaviour)
        {
            this.target = target;

            cbr = cbr ?? CallBackRelay.GlobalCBR;
            this.cbr = cbr;

            this.attachedMonoBehaviour = attachedMonoBehaviour;

            UnsafeUpdates = attachedMonoBehaviour == null;

            cbr.SubscribeToUpdate(this);
        }

        /// <summary>
        /// Sets the current state of the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of the new state to add.</typeparam>
        /// <param name="onBeforeStartup">Callback invoked on new state before Startup() is called.</param>
        public virtual void SetState<TState>(Action<TState> onBeforeStartup = null)
            where TState : State, new()
        {
            RemoveState();

            TState state = new TState();

            state.Initialize(this, target);

            if (onBeforeStartup != null)
                onBeforeStartup(state);

            state.Startup();

            this.state = state;
        }

        /// <summary>
        /// Removes and shutdowns the current state.
        /// </summary>
        public virtual void RemoveState()
        {
            if (!HasState)
                return;

            state.Shutdown();

            state = null;
        }

        /// <summary>
        /// Shutdowns the current state and unsubscribes from the CBR if it needs to.
        /// </summary>
        public void Shutdown()
        {
            RemoveState();

            if (cbr != null)
            {
                cbr.Unsubscribe(this);
            }
        }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        /// <param name="deltaTime">Time between this update and the last update.</param>
        public virtual void OnUpdate(float deltaTime)
        {
            if (HasState)
            {
                state.Update(deltaTime);
            }
        }

        /// <summary>
        /// Returns true if this FSM has a target.
        /// </summary>
        public bool HasTarget
        {
            get { return target != null; }
        }

        /// <summary>
        /// Returns true if this FSM as a current state.
        /// </summary>
        public bool HasState
        {
            get { return state != null; }
        }

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

        /// <summary>
        /// Base state class for an FSM with type T.
        /// </summary>
        public abstract class State
        {
            private bool isInitialized;
            protected FSM<T> owningFSM;
            protected T target;

            /// <summary>
            /// Initializes the state by setting the target object.
            /// </summary>
            /// <remarks>
            /// Automatically called by the FSM.
            /// </remarks>
            /// <param name="owningFSM">The FSM this state is attached to.</param>
            /// <param name="target">Target object to set.</param>
            public void Initialize(FSM<T> owningFSM, T target)
            {
                if (isInitialized)
                    return;

                isInitialized = true;

                this.owningFSM = owningFSM;
                this.target = target;
            }

            /// <summary>
            /// Called once when the state is added by the FSM.
            /// </summary>
            /// <remarks>
            /// Automatically called by the FSM.
            /// </remarks>
            public virtual void Startup() { }

            /// <summary>
            /// Called whenever the FSM is updated.
            /// </summary>
            /// <remarks>
            /// Automatically called by the FSM.
            /// </remarks>
            /// <param name="deltaTime">Time between this update and the last update.</param>
            public virtual void Update(float deltaTime) { }

            /// <summary>
            /// Called once when the state is removed by the FSM.
            /// </summary>
            /// <remarks>
            /// Automatically called by the FSM.
            /// </remarks>
            public virtual void Shutdown() { }
        }
    }
}
