using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expanse.Ext;

namespace Expanse
{
    [System.Serializable]
    public class FSMPlus : IDisposable
    {
        /// <summary>
        /// Gets the abstract base type for all contained states.
        /// </summary>
        public Type BaseStateType
        {
            get
            {
                return Type.GetType(_baseStateType, true, false);
            }
        }
        [SerializeField, Popup(new string[] { PopupAttribute.STATEPLUS_TYPE_KEY }), ReadOnly(EditableInEditor = true)]
        private string _baseStateType = "StatePlus";

        /// <summary>
        /// The object this state is manipulating.
        /// </summary>
        public UnityEngine.Object Owner
        {
            get { return _owner; }
            private set { _owner = value; }
        }
        [SerializeField, HideInInspector]
        private UnityEngine.Object _owner;

        /// <summary>
        /// The currently active state in the system.
        /// </summary>
        public StatePlus CurrentState
        {
            get { return _currentState; }
            protected set { _currentState = value; }
        }
        [SerializeField]
        private StatePlus _currentState;

        /// <summary>
        /// The default state of this state machine. When there is no active state this one will automatically activate.
        /// </summary>
        public StatePlus DefaultState
        {
            get { return _defaultState; }
            set { _defaultState = value; }
        }
        [SerializeField]
        private StatePlus _defaultState;

        /// <summary>
        /// A list of non-initialized state data that may be used to load into newly pushed states.
        /// </summary>
        [SerializeField]
        protected List<StatePlus> stateOverrides = new List<StatePlus>();

        /// <summary>
        /// Initializes this state machine.
        /// </summary>
        /// <param name="owner">Object being manipulated by this state machine</param>
        public virtual void Initialize(UnityEngine.Object owner)
        {
            Initialize(owner, false, null);
        }

        /// <summary>
        /// Initializes this state machine.
        /// </summary>
        /// <param name="owner">Object being manipulated by this state machine</param>
        /// <param name="manualUpdates">Will this state machine require manual updates and disposal</param>
        public virtual void Initialize(UnityEngine.Object owner, bool manualUpdates)
        {
            Initialize(owner, manualUpdates, null);
        }

        /// <summary>
        /// Initializes this state machine.
        /// </summary>
        /// <param name="owner">Object being manipulated by this state machine</param>
        /// <param name="manualUpdates">Will this state machine require manual updates and disposal</param>
        /// <param name="defaultState">Set the default state that is activated immediately</param>
        public virtual void Initialize(UnityEngine.Object owner, bool manualUpdates, StatePlus defaultState)
        {
            if (Owner != null)
                throw new Exception("This state machine has already been initialized.");

            if (owner == null)
                throw new NullReferenceException();

            Owner = owner;

            if (!manualUpdates)
                CallBackRelay.SubscribeAll(Update, null, null, null, Dispose);

            if (defaultState != null)
                DefaultState = defaultState;

            if (CurrentState == null && DefaultState != null)
                PushDefault();
            else if (CurrentState != null && !CurrentState.IsValid)
            {
                var tempState = CurrentState;
                CurrentState = null;
                Push(tempState.GetType(), tempState);
            }
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <param name="newState">A non-initialized state</param>
        /// <param name="checkForOverride">Will the data values of this new state be overriden</param>
        /// <param name="overrideName">Specificly named override to search for</param>
        public virtual void Push(StatePlus newState, bool checkForOverride, string overrideName)
        {
            if (newState == null)
                throw new NullReferenceException();

            if (newState.IsValid)
                throw new Exception("The passed in state must be new and non-initialized.");

            if (newState == DefaultState)
                throw new Exception("Use PushDefault() instead when pushing the default state.");

            if (!BaseTypeCheck(newState))
                throw new Exception("The new state must be a sub-class of the base type: " + BaseStateType.FullName);

            if (checkForOverride)
                LoadOverride(newState, overrideName);

            PopState();

            PushState(newState);

            if (CurrentState == null && DefaultState != null)
                PushDefault();
        }

        #region PUSH OVERLOADS

        /// <summary>
        /// Pushes the default state in as current.
        /// If null the new state will be null.
        /// </summary>
        public virtual void PushDefault()
        {
            if (DefaultState != null)
            {
                StatePlus newDefault = (StatePlus)ScriptableObject.CreateInstance(DefaultState.GetType());
                newDefault.Load(DefaultState);
                Push(newDefault, false, null);
            }
            else
            {
                PopState();
                CurrentState = null;
            }
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <param name="stateType">The type of state to be created</param>
        /// <param name="overrideState">Specific override state to load from</param>
        public virtual void Push(Type stateType, StatePlus overrideState)
        {
            StatePlus newState = (StatePlus)ScriptableObject.CreateInstance(stateType);

            if (overrideState != null)
                newState.Load(overrideState);

            Push(newState, false, null);
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <param name="newState">A non-initialized state</param>
        public virtual void Push(StatePlus newState)
        {
            Push(newState, true, null);
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <param name="newState">A non-initialized state</param>
        /// <param name="checkForOverride">Will the data values of this new state be overriden</param>
        public virtual void Push(StatePlus newState, bool checkForOverride)
        {
            Push(newState, checkForOverride, null);
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <typeparam name="T">Type of the new state</typeparam>
        public virtual void Push<T>() where T : StatePlus, new()
        {
            Push<T>(true, null);
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <typeparam name="T">Type of the new state</typeparam>
        /// <param name="checkForOverride">Will the data values of this new state be overriden</param>
        public virtual void Push<T>(bool checkForOverride) where T : StatePlus, new()
        {
            Push<T>(checkForOverride, null);
        }

        /// <summary>
        /// Pushes a new state in as current.
        /// </summary>
        /// <typeparam name="T">Type of the new state</typeparam>
        /// <param name="checkForOverride">Will the data values of this new state be overriden</param>
        /// <param name="overrideName">Specificly named override to search for</param>
        public virtual void Push<T>(bool checkForOverride, string overrideName) where T : StatePlus, new()
        {
            if (typeof(T).IsAbstract)
                throw new Exception("State must not be an abstract type.");

            Push(ScriptableObject.CreateInstance<T>(), checkForOverride, overrideName);
        }

        #endregion

        /// <summary>
        /// Exits the current state and pushes the default state.
        /// </summary>
        public virtual void Pop()
        {
            PopState();
            PushDefault();
        }

        /// <summary>
        /// Invokes the update method on the current state.
        /// </summary>
        public virtual void Update()
        {
            if (CurrentState != null)
                CurrentState.Update();
        }

        /// <summary>
        /// Exits the current state.
        /// </summary>
        public virtual void Dispose()
        {
            PopState();
        }

        /// <summary>
        /// Determines if a state is a sub class of the base state type.
        /// </summary>
        protected bool BaseTypeCheck(StatePlus stateToCheck)
        {
            return stateToCheck.GetType().IsSubclassOf(BaseStateType);
        }

        /// <summary>
        /// Loads appropriate override state data into the passed state.
        /// </summary>
        protected void LoadOverride(StatePlus stateToOverride, string overrideName)
        {
            foreach (var state in stateOverrides)
            {
                if (state == null)
                    continue;

                if (state.GetType() == stateToOverride.GetType() && (overrideName == null || state.displayName == overrideName))
                {
                    stateToOverride.Load(state);
                    return;
                }
            }

            // Only throw an exception if an override name was specified and not found
            if (overrideName != null)
                throw new Exception("Unable to find appropriate state override for type: " + stateToOverride.GetType());
        }

        /// <summary>
        /// Actually exits and sets the current state to null.
        /// </summary>
        protected void PopState()
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
                ScriptableObject.Destroy(CurrentState);
                CurrentState = null;
            }
        }

        /// <summary>
        /// Actually initializes and enters a new state in as current.
        /// </summary>
        protected void PushState(StatePlus newState)
        {
            newState.Initialize(this);
            CurrentState = newState;
            newState.Enter();
        }
    }
}
 