using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Rigidbody related extension methods.
    /// </summary>
    public static class RigidbodyExt
    {
        /// <summary>
        /// Clears the Rigidbody force and sets it to kinematic.
        /// </summary>
        /// <returns>The velocity state before the freeze</returns>
        public static RigidbodyState Freeze(this Rigidbody rigidbody)
        {
            RigidbodyState rigidbodyState = new RigidbodyState(rigidbody);

            ClearForce(rigidbody);

            rigidbody.isKinematic = true;

            return rigidbodyState;
        }

        /// <summary>
        /// Sets the Rigidbody to not be kinematic and applies a velocity state.
        /// </summary>
        public static void Unfreeze(this Rigidbody rigidbody, RigidbodyState state = default(RigidbodyState))
        {
            rigidbody.isKinematic = false;

            rigidbody.velocity = state.velocity;
            rigidbody.angularVelocity = state.angularVelocity;
        }

        /// <summary>
        /// Contains data describing the current state of a Rigidbody.
        /// </summary>
        public struct RigidbodyState
        {
            public Vector3 velocity;
            public Vector3 angularVelocity;

            public RigidbodyState(Rigidbody rigidbody)
            {
                this.velocity = rigidbody.velocity;
                this.angularVelocity = rigidbody.angularVelocity;
            }
        }

        /// <summary>
        /// Zero the Rigidbody velocity and angular velocity.
        /// </summary>
        public static void ClearForce(this Rigidbody rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Clears the Rigidbody2D force and sets it to kinematic.
        /// </summary>
        /// <returns>The velocity state before the freeze</returns>
        public static RigidbodyState2D Freeze(this Rigidbody2D rigidbody)
        {
            RigidbodyState2D rigidbodyState = new RigidbodyState2D(rigidbody);

            ClearForce(rigidbody);

            rigidbody.isKinematic = true;

            return rigidbodyState;
        }

        /// <summary>
        /// Sets the Rigidbody2D to not be kinematic and applies a velocity state.
        /// </summary>
        public static void Unfreeze(this Rigidbody2D rigidbody, RigidbodyState2D state = default(RigidbodyState2D))
        {
            rigidbody.isKinematic = false;

            rigidbody.velocity = state.velocity;
            rigidbody.angularVelocity = state.angularVelocity;
        }

        /// <summary>
        /// Contains data describing the current state of a Rigidbody2D.
        /// </summary>
        public struct RigidbodyState2D
        {
            public Vector2 velocity;
            public float angularVelocity;

            public RigidbodyState2D(Rigidbody2D rigidbody)
            {
                this.velocity = rigidbody.velocity;
                this.angularVelocity = rigidbody.angularVelocity;
            }
        }

        /// <summary>
        /// Zero the Rigidbody2D velocity and anuglar velocity.
        /// </summary>
        public static void ClearForce(this Rigidbody2D rigidbody2D)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.angularVelocity = 0f;
        }

        /// <summary>
        /// Rigidbody2D equivalent to Rigidbody.AddForce().
        /// </summary>
        public static void AddForce(this Rigidbody2D rigidbody2D, Vector2 force, ForceMode mode = ForceMode.Force)
        {
            switch (mode)
            {
                case ForceMode.Force:
                    rigidbody2D.AddForce(force);
                    break;
                case ForceMode.Impulse:
                    rigidbody2D.AddForce(force / Time.fixedDeltaTime);
                    break;
                case ForceMode.Acceleration:
                    rigidbody2D.AddForce(force * rigidbody2D.mass);
                    break;
                case ForceMode.VelocityChange:
                    rigidbody2D.AddForce(force * rigidbody2D.mass / Time.fixedDeltaTime);
                    break;
            }
        }
    }
}
