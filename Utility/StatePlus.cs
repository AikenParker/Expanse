using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Base class for states run by the FSMPlus state machine.
    /// </summary>
    /// <typeparam name="T">Owning object type</typeparam>
    public abstract class StatePlus : ScriptableObject
    {
        /// <summary>
        /// The display name of this state.
        /// </summary>
        public string displayName = "Unnamed";

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
        /// The state machine that is controlling this state.
        /// </summary>
        public FSMPlus OwningFSM
        {
            get { return _owningFSM; }
            private set { _owningFSM = value; }
        }
        [SerializeField, HideInInspector]
        private FSMPlus _owningFSM;

        /// <summary>
        /// Check to ensure that the owner and owning FSM exist and that their owner reference is the same
        /// </summary>
        public bool IsValid
        {
            get { return Owner != null && OwningFSM != null && Owner == OwningFSM.Owner; }
        }

        /// <summary>
        /// Initializes this state.
        /// </summary>
        /// <param name="owningFSM">Controlling state machine</param>
        public void Initialize(FSMPlus owningFSM)
        {
            if (IsValid)
                throw new Exception("This state has already been initialized.");

            if (owningFSM == null)
                throw new NullReferenceException();

            OwningFSM = owningFSM;
            Owner = owningFSM.Owner;
        }

        /// <summary>
        /// Called once when first pushed into the state machine.
        /// </summary>
        public virtual void Enter()
        {
            throw new Exception("Invocation of base.Enter is unnecessary.");
        }

        /// <summary>
        /// Called once every frame after Enter() and before Exit().
        /// </summary>
        public virtual void Update()
        {
            throw new Exception("Invocation of base.Update is unnecessary.");
        }

        /// <summary>
        /// Called once when finally popped out of the state machine, or if the state machine is disposed.
        /// </summary>
        public virtual void Exit()
        {
            throw new Exception("Invocation of base.Exit is unnecessary.");
        }

        /// <summary>
        /// Load in values from serialized object. Call base to load the name.
        /// </summary>
        public virtual void Load(StatePlus loadFrom)
        {
            this.displayName = loadFrom.displayName;
        }
    }
}