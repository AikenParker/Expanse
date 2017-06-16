using System.Collections.Generic;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of physics related utility functionality.
    /// </summary>
    public static class PhysicsUtil
    {
        /// <summary>
        /// Multi-use collider array buffer for use in non-allocating physics methods.
        /// Note: Iterate over the non-alloc result count instead of colliderBuffer.Length
        /// </summary>
        public static Collider[] colliderBuffer = new Collider[50];

        /// <summary>
        /// Multi-use raycasthit array buffer for use in non-allocating physics methods.
        /// Note: Iterate over the non-alloc result count instead of raycastHitBuffer.Length
        /// </summary>
        public static RaycastHit[] raycastHitBuffer = new RaycastHit[50];

        /// <summary>
        /// Sets the ignore status between all colliders in a collection.
        /// </summary>
        /// <param name="colliders">Source collider collection.</param>
        /// <param name="ignore">If true all colliders will ignore eachother.</param>
        public static void IgnoreCollision(IEnumerable<Collider> colliders, bool ignore = true)
        {
            foreach (Collider aCol in colliders)
                foreach (Collider bCol in colliders)
                {
                    if (aCol == bCol)
                        break;

                    Physics.IgnoreCollision(aCol, bCol, ignore);
                }
        }

        /// <summary>
        /// Sets the ignore state value between a single collider and a collection of colliders.
        /// Note: Does not affect ignore status between colliders in the collection.
        /// </summary>
        /// <param name="collider">Source collider component.</param>
        /// <param name="targetColliders">Collection of other target colliders.</param>
        /// <param name="ignore">If true the source collider will ignore all target colliders.</param>
        public static void IgnoreCollision(Collider collider, IEnumerable<Collider> targetColliders, bool ignore = true)
        {
            foreach (Collider otherCol in targetColliders)
                Physics.IgnoreCollision(collider, otherCol, ignore);
        }

        /// <summary>
        /// Sets the ignore state value between two collections of colliders.
        /// Note: Does not affect ignore status between colliders in the same collection.
        /// </summary>
        /// <param name="aColliders">First collection of colliders.</param>
        /// <param name="bColliders">Second collection of colliders</param>
        /// <param name="ignore">If true all colliders in collection A will ignore all colliders in collection B.</param>
        public static void IgnoreCollision(IEnumerable<Collider> aColliders, IEnumerable<Collider> bColliders, bool ignore = true)
        {
            foreach (Collider aCol in aColliders)
                foreach (Collider bCol in bColliders)
                    Physics.IgnoreCollision(aCol, bCol, ignore);
        }
    }
}
