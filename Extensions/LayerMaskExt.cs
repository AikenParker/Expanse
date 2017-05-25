using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of LayerMask related extension methods.
    /// </summary>
    public static class LayerMaskExt
    {
        /// <summary>
        /// Returns true if a layermask contains a layer.
        /// </summary>
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        /// <summary>
        /// Returns true if a layermask contains a game object layer.
        /// </summary>
        public static bool Contains(this LayerMask layerMask, GameObject gameObj)
        {
            return Contains(layerMask, gameObj.layer);
        }

        /// <summary>
        /// Returns true if a layermask contains a layer.
        /// </summary>
        public static bool Contains(this LayerMask layerMask, string layerName)
        {
            return Contains(layerMask, LayerMask.NameToLayer(layerName));
        }
    }
}
