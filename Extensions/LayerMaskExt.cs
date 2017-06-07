using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of LayerMask related extension methods.
    /// </summary>
    public static class LayerMaskExt
    {
        /// <summary>
        /// Checks if a layerMask contains a specified layer.
        /// </summary>
        /// <param name="layerMask">LayerMask to check the layer for.</param>
        /// <param name="layer">Layer to check for on the layerMask.</param>
        /// <returns>Returns true if the layerMask contains the layer.</returns>
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        /// <summary>
        /// Checks if a layerMask contains the layer of a gameObject.
        /// </summary>
        /// <param name="layerMask">LayerMask to check the layer for.</param>
        /// <param name="gameObj">GameObject with layer to check on the layerMask.</param>
        /// <returns>Returns true if a layerMask contains the layer of a gameObject</returns>
        public static bool Contains(this LayerMask layerMask, GameObject gameObj)
        {
            return Contains(layerMask, gameObj.layer);
        }

        /// <summary>
        /// Checks if a layerMask contains a layer by name.
        /// </summary>
        /// <param name="layerMask">LayerMask to check the layer for.</param>
        /// <param name="layerName">Name of the layer to check on the layerMask.</param>
        /// <returns>Returns true if the layerMask contains the layer.</returns>
        public static bool Contains(this LayerMask layerMask, string layerName)
        {
            return Contains(layerMask, LayerMask.NameToLayer(layerName));
        }
    }
}
