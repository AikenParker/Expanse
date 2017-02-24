using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
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
