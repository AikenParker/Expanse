namespace Expanse
{
    public interface IUpdate : IUnity
    {
        /// <summary>
        /// Invoked by an object managing the updates for this interface.
        /// </summary>
        /// <param name="deltaTime">Time in seconds since the last time this was called.</param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// Will this update even if the attached MonoBehaviour is destroyed or non-existent?
        /// </summary>
        bool UnsafeUpdates { get; }

        /// <summary>
        /// Will this update even if the attached MonoBehaviour is disabled or inactive?
        /// </summary>
        bool AlwaysUpdate { get; }

        /// <summary>
        /// Will the update use unscaled deltaTime?
        /// </summary>
        bool UnscaledDelta { get; }
    }
}