using System;
using System.Collections.Generic;
using Expanse.Extensions;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Simple multi-state finite state machine.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target object.</typeparam>
    public class MultiFSM<TTarget> : FSM<TTarget>
        where TTarget : class
    {
        protected List<State<TTarget>> stateList = new List<State<TTarget>>();

        /// <summary>
        /// Creates a new FSM that will require manual updating.
        /// </summary>
        /// <param name="target">Target object this FSM is attached to.</param>
        public MultiFSM(TTarget target) : base(target) { }

        /// <summary>
        /// Creates a new FSM that will be updated by a CallBackRelay.
        /// </summary>
        /// <remarks>
        /// <para>If CBR is null the GlobalCBR is used instead.</para>
        /// <para>Target is used a attached MonoBehaviour if it is a MonoBehaviour.</para>
        /// </remarks>
        /// <param name="target">Target object this FSM is attached to.</param>
        /// <param name="cbr">CallBackRelay instance to subscribe to.</param>
        public MultiFSM(TTarget target, CallBackRelay cbr) : base(target, cbr) { }

        /// <summary>
        /// Creates a new FSM that will be updated by a CallBackRelay.
        /// </summary>
        /// <para>If CBR is null the GlobalCBR is used instead.</para>
        /// <param name="target">Target object this FSM is attached to.</param>
        /// <param name="cbr">CallBackRelay instance to subscribe to.</param>
        /// <param name="attachedMonoBehaviour">Attached MonoBehaviour instance.</param>
        public MultiFSM(TTarget target, CallBackRelay cbr, MonoBehaviour attachedMonoBehaviour)
            : base(target, cbr, attachedMonoBehaviour) { }

        /// <summary>
        /// Adds a new state to the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of the new state to add.</typeparam>
        /// <param name="onBeforeStartup">Callback invoked on new state before Startup() is called.</param>
        /// <param name="uniqueType">If true all states of that type are removed before adding.</param>
        public virtual TState AddState<TState>(Action<TState> onBeforeStartup = null, bool uniqueType = false)
            where TState : State<TTarget>, new()
        {
            if (uniqueType)
                RemoveStatesOfType<TState>();

            TState state = new TState();

            state.Initialize(this, target);

            if (onBeforeStartup != null)
                onBeforeStartup(state);

            state.Startup();

            stateList.Add(state);

            return state;
        }

        /// <summary>
        /// Removes and shutdowns a state from the FSM.
        /// </summary>
        /// <param name="removeState">State to be removed.</param>
        /// <returns>Returns true if a state was removed.</returns>
        public virtual bool RemoveState(State<TTarget> removeState)
        {
            for (int i = 0; i < stateList.Count; i++)
            {
                State<TTarget> state = stateList[i];

                if (state == removeState)
                {
                    state.Shutdown();
                    stateList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes and shutdowns the first state of type from the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of the state to be removed.</typeparam>
        /// <param name="inherited">If true base types are checked.</param>
        /// <returns>Returns true if a state was removed.</returns>
        public virtual bool RemoveStateOfType<TState>(bool inherited = true)
            where TState : State<TTarget>
        {
            Type tState = typeof(TState);

            for (int i = 0; i < stateList.Count; i++)
            {
                State<TTarget> state = stateList[i];

                if (state.GetType().IsOfType(tState, inherited))
                {
                    state.Shutdown();
                    stateList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes and shutdowns all states of type from the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of state to be removed.</typeparam>
        /// <param name="inherited">If true base types are checked.</param>
        public virtual void RemoveStatesOfType<TState>(bool inherited = true)
            where TState : State<TTarget>
        {
            Type tState = typeof(TState);

            for (int i = stateList.Count - 1; i >= 0; i--)
            {
                State<TTarget> state = stateList[i];

                if (state.GetType().IsOfType(tState, inherited))
                {
                    state.Shutdown();
                    stateList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Checks if the FSM contains a state.
        /// </summary>
        /// <param name="checkState">State to be checked.</param>
        /// <returns>Returns true if the state is in the FSM.</returns>
        public virtual bool ContainsState(State<TTarget> checkState)
        {
            for (int i = 0; i < stateList.Count; i++)
            {
                State<TTarget> state = stateList[i];

                if (state == checkState)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the FSM contains a state of a type.
        /// </summary>
        /// <typeparam name="TState">Type of state to be checked.</typeparam>
        /// <param name="inherited">If true base types are checked.</param>
        /// <returns>Returns true if the state of type is in the FSM.</returns>
        public virtual bool ContainsStateOfType<TState>(bool inherited = true)
            where TState : State<TTarget>
        {
            Type tState = typeof(TState);

            for (int i = 0; i < stateList.Count; i++)
            {
                State<TTarget> state = stateList[i];

                if (state.GetType().IsOfType(tState, inherited))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a state of type from the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of state to get.</typeparam>
        /// <returns>Returns a state of type from the FSM.</returns>
        public TState GetState<TState>()
            where TState : State<TTarget>
        {
            for (int i = 0; i < stateList.Count; i++)
            {
                State<TTarget> state = stateList[i];

                TState castedState = state as TState;

                if (castedState != null)
                    return castedState;
            }

            return null;
        }

        /// <summary>
        /// Gets all states of type from the FSM.
        /// </summary>
        /// <typeparam name="TState">Type of state to get.</typeparam>
        /// <returns>Returns all states of type from the FSM.</returns>
        public List<TState> GetStates<TState>()
            where TState : State<TTarget>
        {
            return stateList.OfTypeToList<State<TTarget>, TState>();
        }

        /// <summary>
        /// Removes and shutdowns all states.
        /// </summary>
        public virtual void ClearStates()
        {
            if (!HasState)
                return;

            for (int i = 0; i < stateList.Count; i++)
            {
                State<TTarget> state = stateList[i];

                state.Shutdown();
            }

            stateList.Clear();
        }

        /// <summary>
        /// Shutdown all states and unsubscribes from the CBR if it needs to.
        /// </summary>
        public override void Shutdown()
        {
            ClearStates();

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
                for (int i = 0; i < stateList.Count; i++)
                {
                    State<TTarget> state = stateList[i];

                    state.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Returns true if this FSM has a current state.
        /// </summary>
        public override bool HasState
        {
            get { return stateList.Count > 0; }
        }

        /// <summary>
        /// Returns the amount of states in this FSM.
        /// </summary>
        public virtual int StateCount
        {
            get { return stateList.Count; }
        }
    }
}
