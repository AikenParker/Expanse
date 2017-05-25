using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of RaycastHit2D related extension methods.
    /// </summary>
    public static class RaycastHit2DExt
    {
        /// <summary>
        /// Returns true if RaycastHit2D has a valid hit result.
        /// </summary>
        public static bool HasHit(this RaycastHit2D hit)
        {
            return hit.collider != null;
        }
    }
}
