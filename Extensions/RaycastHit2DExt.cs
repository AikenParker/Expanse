using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of RaycastHit2D related extension methods.
    /// </summary>
    public static class RaycastHit2DExt
    {
        /// <summary>
        /// </summary>
        /// <param name="hit">Raycast hit information.</param>
        /// <returns>Returns true if the raycast hit contains valid hit results.</returns>
        public static bool HasHit(this RaycastHit2D hit)
        {
            return hit.collider != null;
        }
    }
}
