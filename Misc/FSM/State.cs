namespace Expanse.Misc
{
    /// <summary>
    /// Base state class for an FSM with type T.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target object.</typeparam>
    public abstract class State<TTarget>
        where TTarget : class
    {
        private bool isInitialized;
        protected FSM<TTarget> owningFSM;
        protected TTarget target;

        /// <summary>
        /// Initializes the state by setting the target object.
        /// </summary>
        /// <remarks>
        /// Automatically called by the FSM.
        /// </remarks>
        /// <param name="owningFSM">The FSM this state is attached to.</param>
        /// <param name="target">Target object to set.</param>
        public void Initialize(FSM<TTarget> owningFSM, TTarget target)
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
