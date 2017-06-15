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
        /// <param name="rigidbody">Source rigidbidy component.</param>
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
        /// <param name="rigidbody">Source rigidbidy component.</param>
        /// <param name="state">Rigidbody state to apply to the rigidbody when unfrozen.</param>
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
        /// <param name="rigidbody">Source rigidbidy component.</param>
        public static void ClearForce(this Rigidbody rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Clears the Rigidbody2D force and sets it to kinematic.
        /// </summary>
        /// <param name="rigidbody">Source rigidbidy component.</param>
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
        /// <param name="rigidbody">Source rigidbidy component.</param>
        /// <param name="state">Rigidbody state to apply to the rigidbody when unfrozen.</param>
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
        /// <param name="rigidbody">Source rigidbidy component.</param>
        public static void ClearForce(this Rigidbody2D rigidbody)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
        }

        /// <summary>
        /// Rigidbody2D equivalent to Rigidbody.AddForce().
        /// </summary>
        /// <param name="rigidbody">Source rigidbidy component.</param>
        /// <param name="force">Amount and direction of force to apply to the rigidbody.</param>
        /// <param name="mode">Mode in which to apply the force to the rigidbody.</param>
        public static void AddForce(this Rigidbody2D rigidbody, Vector2 force, ForceMode mode = ForceMode.Force)
        {
            switch (mode)
            {
                case ForceMode.Force:
                    rigidbody.AddForce(force);
                    break;
                case ForceMode.Impulse:
                    rigidbody.AddForce(force / TimeManager.FixedDeltaTime);
                    break;
                case ForceMode.Acceleration:
                    rigidbody.AddForce(force * rigidbody.mass);
                    break;
                case ForceMode.VelocityChange:
                    rigidbody.AddForce(force * rigidbody.mass / TimeManager.FixedDeltaTime);
                    break;
            }
        }
    }
}
