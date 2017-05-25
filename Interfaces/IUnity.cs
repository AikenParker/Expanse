using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Base class for Expanse interfaces that are expected to extend MonoBehaviours.
    /// </summary>
    public interface IUnity
    {
        /// <summary>
        /// The GameObject this component is attached to. A component is always attached to a GameObject.
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// The MonoBehaviour this interface is attached to. This interface is always attached to a MonoBehaviour.
        /// </summary>
        MonoBehaviour MonoBehaviour { get; }
    }
}
