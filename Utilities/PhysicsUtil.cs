using System;
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
        /// <para>Note: Iterate over the non-alloc result count instead of colliderBuffer.Length</para>
        /// </summary>
        public static Collider[] colliderBuffer = new Collider[20];

        /// <summary>
        /// Multi-use raycasthit array buffer for use in non-allocating physics methods.
        /// <para>Note: Iterate over the non-alloc result count instead of raycastHitBuffer.Length</para>
        /// </summary>
        public static RaycastHit[] raycastHitBuffer = new RaycastHit[20];

        /// <summary>
        /// Multi-use 2D collider array buffer for use in non-allocating physics methods.
        /// <para>Note: Iterate over the non-alloc result count instead of collider2DBuffer.Length</para>
        /// </summary>
        public static Collider2D[] collider2DBuffer = new Collider2D[20];

        /// <summary>
        /// Multi-use 2D collider array buffer for use in non-allocating physics methods.
        /// <para>Note: Iterate over the non-alloc result count instead of raycastHit2DBuffer.Length</para>
        /// </summary>
        public static RaycastHit2D[] raycastHit2DBuffer = new RaycastHit2D[20];

        /// <summary>
        /// Sets the ignore status between all colliders in a collection.
        /// </summary>
        /// <param name="colliders">Source collider collection.</param>
        /// <param name="ignore">If true all colliders will ignore eachother.</param>
        public static void IgnoreCollision(IList<Collider> colliders, bool ignore = true)
        {
            if (colliders == null)
                throw new ArgumentNullException("colliders");

            int collidersCount = colliders.Count;

            for (int i = 0; i < collidersCount; i++)
            {
                Collider aCol = colliders[i];

                for (int j = 0; j < collidersCount; j++)
                {
                    if (i == j)
                        continue;

                    Collider bCol = colliders[j];

                    Physics.IgnoreCollision(aCol, bCol, ignore);
                }
            }
        }

        /// <summary>
        /// Sets the ignore state value between a single collider and a collection of colliders.
        /// Note: Does not affect ignore status between colliders in the collection.
        /// </summary>
        /// <param name="collider">Source collider component.</param>
        /// <param name="targetColliders">Collection of other target colliders.</param>
        /// <param name="ignore">If true the source collider will ignore all target colliders.</param>
        public static void IgnoreCollision(Collider collider, IList<Collider> targetColliders, bool ignore = true)
        {
            if (collider == null)
                throw new ArgumentNullException("collider");

            if (targetColliders == null)
                throw new ArgumentNullException("targetColliders");

            int targetCollidersCount = targetColliders.Count;

            for (int i = 0; i < targetCollidersCount; i++)
            {
                Collider targetCollider = targetColliders[i];

                Physics.IgnoreCollision(collider, targetCollider, ignore);
            }
        }

        /// <summary>
        /// Sets the ignore state value between two collections of colliders.
        /// Note: Does not affect ignore status between colliders in the same collection.
        /// </summary>
        /// <param name="aColliders">First collection of colliders.</param>
        /// <param name="bColliders">Second collection of colliders</param>
        /// <param name="ignore">If true all colliders in collection A will ignore all colliders in collection B.</param>
        public static void IgnoreCollision(IList<Collider> aColliders, IList<Collider> bColliders, bool ignore = true)
        {
            if (aColliders == null)
                throw new ArgumentNullException("aColliders");

            if (bColliders == null)
                throw new ArgumentNullException("bColliders");

            int aCollidersCount = aColliders.Count;
            int bCollidersCount = bColliders.Count;

            for (int a = 0; a < aCollidersCount; a++)
            {
                Collider aCol = aColliders[a];

                for (int b = 0; b < bCollidersCount; b++)
                {
                    Collider bCol = bColliders[b];

                    Physics.IgnoreCollision(aCol, bCol, ignore);
                }
            }
        }

        /// <summary>
        /// Sets the ignore status between all colliders in a collection.
        /// </summary>
        /// <param name="colliders">Source collider collection.</param>
        /// <param name="ignore">If true all colliders will ignore eachother.</param>
        public static void IgnoreCollision(IList<Collider2D> colliders, bool ignore = true)
        {
            if (colliders == null)
                throw new ArgumentNullException("colliders");

            int collidersCount = colliders.Count;

            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D aCol = colliders[i];

                for (int j = 0; j < collidersCount; j++)
                {
                    if (i == j)
                        continue;

                    Collider2D bCol = colliders[j];

                    Physics2D.IgnoreCollision(aCol, bCol, ignore);
                }
            }
        }

        /// <summary>
        /// Sets the ignore state value between a single collider and a collection of colliders.
        /// Note: Does not affect ignore status between colliders in the collection.
        /// </summary>
        /// <param name="collider">Source collider component.</param>
        /// <param name="targetColliders">Collection of other target colliders.</param>
        /// <param name="ignore">If true the source collider will ignore all target colliders.</param>
        public static void IgnoreCollision(Collider2D collider, IList<Collider2D> targetColliders, bool ignore = true)
        {
            if (collider == null)
                throw new ArgumentNullException("collider");

            if (targetColliders == null)
                throw new ArgumentNullException("targetColliders");

            int targetCollidersCount = targetColliders.Count;

            for (int i = 0; i < targetCollidersCount; i++)
            {
                Collider2D targetCollider = targetColliders[i];

                Physics2D.IgnoreCollision(collider, targetCollider, ignore);
            }
        }

        /// <summary>
        /// Sets the ignore state value between two collections of colliders.
        /// Note: Does not affect ignore status between colliders in the same collection.
        /// </summary>
        /// <param name="aColliders">First collection of colliders.</param>
        /// <param name="bColliders">Second collection of colliders</param>
        /// <param name="ignore">If true all colliders in collection A will ignore all colliders in collection B.</param>
        public static void IgnoreCollision(IList<Collider2D> aColliders, IList<Collider2D> bColliders, bool ignore = true)
        {
            if (aColliders == null)
                throw new ArgumentNullException("aColliders");

            if (bColliders == null)
                throw new ArgumentNullException("bColliders");

            int aCollidersCount = aColliders.Count;
            int bCollidersCount = bColliders.Count;

            for (int a = 0; a < aCollidersCount; a++)
            {
                Collider2D aCol = aColliders[a];

                for (int b = 0; b < bCollidersCount; b++)
                {
                    Collider2D bCol = bColliders[b];

                    Physics2D.IgnoreCollision(aCol, bCol, ignore);
                }
            }
        }
    }
}
