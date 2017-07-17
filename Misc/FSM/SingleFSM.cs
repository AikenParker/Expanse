using System;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Simple single-state finite state machine.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target object.</typeparam>
    public class SingleFSM<TTarget> : FSM<TTarget>
        where TTarget : class
    {
        protected State<TTarget> state;

        /// <summary>
        /// Creates a new FSM that will require manual updating.
        /// </summary>
        /// <param name="target">Target object this FSM is attached to.</param>
        public SingleFSM(TTarget target) : base(target) { }

        /// <summary>
        /// Creates a new FSM that will be updated by a CallBackRelay.
        /// </summary>
        /// <remarks>
        /// <para>If CBR is null the GlobalCBR is used instead.</para>
        /// <para>Target is used a attached MonoBehaviour if it is a MonoBehaviour.</para>
        /// </remarks>
        /// <param name="target">Target object this FSM is attached to.</param>
        /// <param name="cbr">CallBackRelay instance to subscribe to.</param>
        public SingleFSM(TTarget target, CallBackRelay cbr) : base(target, cbr) { }

        /// <summary>
        /// Creates a new FSM that will be updated by a CallBackRelay.
        /// </summary>
        /// <para>If CBR is null the GlobalCBR is used instead.</para>
        /// <param name="target">Target object this FSM is attached to.</param>
        /// <param name="cbr">CallBackRelay instance to subscribe to.</param>
        /// <param name="attachedMonoBehaviour">Attached MonoBehaviour instance.</param>
        public SingleFSM(TTarget target, CallBackRelay cbr, MonoBehaviour attachedMonoBehaviour)
            : base(target, cbr, attachedMonoBehaviour) { }

        /// <summary>
        /// Sets the current state of the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of the new state to add.</typeparam>
        /// <param name="onBeforeStartup">Callback invoked on new state before Startup() is called.</param>
        public virtual void SetState<TState>(Action<TState> onBeforeStartup = null)
            where TState : State<TTarget>, new()
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
        public override void Shutdown()
        {
            RemoveState();

            base.Shutdown();
        }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        /// <param name="deltaTime">Time between this update and the last update.</param>
        public override void OnUpdate(float deltaTime)
        {
            if (HasState)
            {
                state.Update(deltaTime);
            }
        }

        /// <summary>
        /// Returns true if this FSM has a current state.
        /// </summary>
        public override bool HasState
        {
            get { return state != null; }
        }
    }
}
