using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Base class for anything that gets triggered by another object(s) either coming in contact or by simply getting too close.
    /// </summary>
    public abstract class TriggerPlus : MonoBehaviour
    {
        // NESTED TYPES
        public enum TriggerType { TriggerVolume = 0, CollisionVolume = 1, ProximityBased = 2 };
        public enum VolumeType { Box = 0, Sphere = 1, Capsule = 2, Mesh = 3 };

        // COMPONENTS
        [HideInInspector]
        public Collider colliderComp;
        [HideInInspector]
        public TimerPlus proximityTimer;

        // INSPECTOR
        public TriggerType triggerType;
        public VolumeType volumeType;
        public float proximityDistance = 1;
        [Range(0.1f, 120)]
        public float proximityCheckFrequency = 1;
        public LayerMask targetLayer = 1 << 0;
        public bool isMultiTrigger;
        [ReadOnly]
        public bool hasBeenTriggered;
        public UnityEvent OnTriggeredEvent = new UnityEvent();

        // Subscribe to this through code
        public event System.Action<GameObject> OnTriggered;
        // Override this if you have custom trigger conditions to specify in child class
        protected virtual bool CanBeTriggered { get { return true; } }
        // Determines if this trigger can be triggered
        public bool Triggerable
        {
            get { return enabled && gameObject.activeInHierarchy && CanBeTriggered && (!hasBeenTriggered || isMultiTrigger); }
        }

        void Start()
        {
            // Create a new timer if proximity based
            if (triggerType == TriggerType.ProximityBased)
                CreateProximityTimer();
        }

        /// <summary>
        /// Be sure to call "base.OnCollisionEnter(collision)" for when a collision may activate this trigger.
        /// </summary>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            CallTrigger(collision.collider.gameObject);
        }

        /// <summary>
        /// Be sure to call "base.OnTriggerEnter(other)" for when a collider may activate this trigger.
        /// </summary>
        protected virtual void OnTriggerEnter(Collider other)
        {
            CallTrigger(other.gameObject);
        }

        /// <summary>
        /// Call "base.CheckProximity()" for default overlap sphere behaviour.
        /// </summary>
        protected virtual void CheckProximity()
        {
            List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, proximityDistance, targetLayer).Where(x => x.gameObject != this.gameObject));

            if (colliders != null && colliders.Any() && colliders.Count > 0)
                for (int i = colliders.Count - 1; i >= 0; --i)
                    CallTrigger(colliders[i].gameObject);
        }

        void OnDrawGizmos()
        {
            if (triggerType == TriggerType.ProximityBased)
            {
                Gizmos.color = Triggerable ? Color.green : Color.red;
                Gizmos.DrawSphere(transform.position, proximityDistance);
            }
        }

        // Creates a new proximity timer and starts it up.
        private void CreateProximityTimer()
        {
            if (proximityTimer != null)
                proximityTimer.Dispose();

            proximityTimer = TimerPlus.Create(proximityCheckFrequency, TimerPlus.Presets.Repeater, CheckProximity);
            proximityTimer.OnDisposed += () => proximityTimer = null;
            proximityTimer.Start();
        }

        // Call trigger events if able.
        public void CallTrigger(GameObject triggerObject)
        {
            // Check if this object is even active or that the triggering object is in the correct layer
            if (Triggerable && targetLayer == (targetLayer | (1 << triggerObject.layer)) && IsTriggerObject(triggerObject))
            {
                hasBeenTriggered = true;

                // Dispose proximity timer
                if (proximityTimer != null && !isMultiTrigger)
                    proximityTimer.Dispose();

                // Invoke abstract method 1st
                OnTrigger(triggerObject);

                // Raise Action<> 2nd
                if (OnTriggered != null)
                    OnTriggered(triggerObject);

                // Raise Unity Event 3rd
                if (OnTriggeredEvent != null)
                    OnTriggeredEvent.Invoke();
            }
        }

        // Resets the state of this trigger so that it may be activated again.
        public void Reset()
        {
            hasBeenTriggered = false;

            if (triggerType == TriggerType.ProximityBased && proximityTimer == null)
                CreateProximityTimer();
        }

        /// <summary>
        /// Gets called for every potential triggering object.
        /// Return true if you want the param gameObject to activate this trigger.
        /// </summary>
        protected virtual bool IsTriggerObject(GameObject triggerObject)
        {
            return triggerObject;
        }

        /// <summary>
        /// Gets called when a trigger was successful.
        /// </summary>
        protected abstract void OnTrigger(GameObject triggerObject);

#if (UNITY_EDITOR)

        void Update()
        {
            // Updates changes made to settings only necessary in editor

            // Update trigger type
            if (colliderComp) colliderComp.isTrigger = triggerType == TriggerType.TriggerVolume;

            // Update proximity timer settings
            if (triggerType == TriggerType.ProximityBased && proximityTimer == null)
                CreateProximityTimer();
            else if (triggerType != TriggerType.ProximityBased && proximityTimer != null)
                proximityTimer.Dispose();

            if (proximityTimer != null)
                proximityTimer.ModifyLength(proximityCheckFrequency);
        }

#endif
    }
}