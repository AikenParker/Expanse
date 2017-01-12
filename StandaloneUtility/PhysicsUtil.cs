using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Collection of physics related utility functionality.
    /// </summary>
    public static class PhysicsUtil
    {
        /// <summary>
        /// Single-use collider array buffer for use in non-allocating physics methods.
        /// </summary>
        public static Collider[] colliderBuffer = new Collider[50];

        /// <summary>
        /// Single-use raycasthit array buffer for use in non-allocating physics methods.
        /// </summary>
        public static RaycastHit[] raycastHitBuffer = new RaycastHit[50];

        /// <summary>
        /// Sets the ignore status between all colliders in a collection.
        /// </summary>
        public static void IgnoreCollision(IEnumerable<Collider> colliders, bool ignore = true)
        {
            foreach (Collider aCol in colliders)
                foreach (Collider bCol in colliders)
                {
                    if (aCol.Equals(bCol))
                        break;

                    Physics.IgnoreCollision(aCol, bCol, ignore);
                }
        }

        /// <summary>
        /// Sets the ignore state value between a single collider and a collection of colliders.
        /// Note: Does not affect ignore status between colliders in the collection.
        /// </summary>
        public static void IgnoreCollision(Collider collider, IEnumerable<Collider> otherColliders, bool ignore = true)
        {
            foreach (Collider otherCol in otherColliders)
                Physics.IgnoreCollision(collider, otherCol, ignore);
        }

        /// <summary>
        /// Sets the ignore state value between two collections of colliders.
        /// Note: Does not affect ignore status between colliders in the same collection.
        /// </summary>
        public static void IgnoreCollision(IEnumerable<Collider> aColliders, IEnumerable<Collider> bColliders, bool ignore = true)
        {
            foreach (Collider aCol in aColliders)
                foreach (Collider bCol in bColliders)
                    Physics.IgnoreCollision(aCol, bCol, ignore);
        }
    }
}
