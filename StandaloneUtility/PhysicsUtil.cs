using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
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
        /// Sets the ignore state value of all aColliders vs bColliders.
        /// </summary>
        public static void IgnoreCollision(IEnumerable<Collider> aColliders, IEnumerable<Collider> bColliders, bool ignore = true)
        {
            foreach (Collider aCol in aColliders)
                foreach (Collider bCol in bColliders)
                    Physics.IgnoreCollision(aCol, bCol, ignore);
        }
    }
}
