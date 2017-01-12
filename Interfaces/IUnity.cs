using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Base class for Unity interfaces. Allows for conversion between interface types and Unity types.
    /// </summary>
    public interface IUnity
    {
        /// <summary>
        /// The game object this component is attached to. A component is always attached to a game object.
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// The mono behavior this interface is attached to. This interface is always attached to a mono behavior.
        /// </summary>
        MonoBehaviour MonoBehaviour { get; }
    }
}
