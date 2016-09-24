using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace Expanse
{
    [CustomEditor(typeof(TriggerZone), true)]
    public class TriggerPlusEditor : Editor
    {
        TriggerZone Target { get; set; }
        Dictionary<string, SerializedProperty> baseProperties = new Dictionary<string, SerializedProperty>();
        TriggerZone.TriggerType triggerType;
        TriggerZone.VolumeType volumeType;
        Collider triggerCollider
        {
            get
            {
                Collider temp = baseProperties["colliderComp"].objectReferenceValue as Collider;
                if (!temp) temp = Target.GetComponent<Collider>();
                return temp;
            }
            set
            {
                baseProperties["colliderComp"].objectReferenceValue = value;
            }
        }
        SerializedProperty property;
        bool showChildren;
        bool initialized;

        void OnEnable()
        {
            Target = (TriggerZone)target;

            // Get base TriggerPlus properties
            baseProperties.Add("triggerType", serializedObject.FindProperty("triggerType"));
            baseProperties.Add("volumeType", serializedObject.FindProperty("volumeType"));
            baseProperties.Add("proximityDistance", serializedObject.FindProperty("proximityDistance"));
            baseProperties.Add("proximityCheckFrequency", serializedObject.FindProperty("proximityCheckFrequency"));
            baseProperties.Add("targetLayer", serializedObject.FindProperty("targetLayer"));
            baseProperties.Add("isMultiTrigger", serializedObject.FindProperty("isMultiTrigger"));
            baseProperties.Add("hasBeenTriggered", serializedObject.FindProperty("hasBeenTriggered"));
            baseProperties.Add("OnTriggeredEvent", serializedObject.FindProperty("OnTriggeredEvent"));
            baseProperties.Add("colliderComp", serializedObject.FindProperty("colliderComp"));

            property = serializedObject.GetIterator();

            // Initializer
            if (!triggerCollider && Target.triggerType != TriggerZone.TriggerType.ProximityBased)
                ApplyTriggerChanges(Target.triggerType, Target.volumeType);
        }

        public override void OnInspectorGUI()
        {
            if (!Target) return;

            // Script fields
            EditorGUI.indentLevel = 0;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUILayout.ObjectField("Editor Script", MonoScript.FromScriptableObject(this), this.GetType(), false);

            // Reset trigger fields (Change flags)
            triggerType = (TriggerZone.TriggerType)baseProperties["triggerType"].enumValueIndex;
            volumeType = (TriggerZone.VolumeType)baseProperties["volumeType"].enumValueIndex;

            // Draw Header
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Trigger Settings", EditorStyles.boldLabel);

            // Draw trigger type enum fields
            EditorGUILayout.PropertyField(baseProperties["triggerType"]);
            if (triggerType != TriggerZone.TriggerType.ProximityBased)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(baseProperties["volumeType"]);
                EditorGUI.indentLevel--;
            }
            if (triggerType == TriggerZone.TriggerType.ProximityBased)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(baseProperties["proximityDistance"]);
                EditorGUILayout.PropertyField(baseProperties["proximityCheckFrequency"]);
                if (Application.isPlaying && Target.proximityTimer != null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.Slider("Timer Value", Target.proximityTimer.Percentage, 0, 1);
                    GUI.enabled = true;
                }
                EditorGUI.indentLevel--;
            }

            // Draw other base properties
            EditorGUILayout.PropertyField(baseProperties["targetLayer"]);
            EditorGUILayout.PropertyField(baseProperties["isMultiTrigger"]);
            EditorGUILayout.PropertyField(baseProperties["hasBeenTriggered"]);
            EditorGUILayout.PropertyField(baseProperties["OnTriggeredEvent"]);

            // Apply type changes
            if ((int)triggerType != baseProperties["triggerType"].enumValueIndex || (int)volumeType != baseProperties["volumeType"].enumValueIndex)
            {
                ApplyTriggerChanges((TriggerZone.TriggerType)baseProperties["triggerType"].enumValueIndex, (TriggerZone.VolumeType)baseProperties["volumeType"].enumValueIndex);
            }

            // Draw child fields
            // Iterate through properties and draw them like it normally would
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(Target.GetType().Name.AddSpaces() + " Settings", EditorStyles.boldLabel);

            property.Reset();
            property.Next(true);
            do
            {
                EditorGUI.indentLevel = property.depth;
                if (property.name.StartsWith("m_") || property.name.EqualsAny(baseProperties.Keys.ToArray())) continue; // Ignore Unity and base properties
                showChildren = EditorGUILayout.PropertyField(property); // Draw property

            }
            while (property.NextVisible(showChildren));
            EditorGUI.indentLevel = 0;

            // Apply changes to the property
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfDirtyOrScript();
        }

        public void ApplyTriggerChanges(TriggerZone.TriggerType newTriggerType, TriggerZone.VolumeType newVolumeType)
        {
            if (triggerCollider)
            {
                DestroyComponent(triggerCollider);
                triggerCollider = null;
            }

            if (newTriggerType != TriggerZone.TriggerType.ProximityBased)
            {
                triggerCollider = AddNewCollider(newVolumeType);
                if (newVolumeType == TriggerZone.VolumeType.Mesh)
                    (triggerCollider as MeshCollider).convex = true;
                triggerCollider.isTrigger = newTriggerType == TriggerZone.TriggerType.TriggerVolume;
            }
        }

        private Collider AddNewCollider(TriggerZone.VolumeType type)
        {
            if (type == TriggerZone.VolumeType.Box)
                return Target.gameObject.AddComponent<BoxCollider>();
            else if (type == TriggerZone.VolumeType.Sphere)
                return Target.gameObject.AddComponent<SphereCollider>();
            else if (type == TriggerZone.VolumeType.Capsule)
                return Target.gameObject.AddComponent<CapsuleCollider>();
            else
                return Target.gameObject.AddComponent<MeshCollider>();
        }

        // Destroying the component immediately causes an error.
        private void DestroyComponent(Collider colliderToDestroy)
        {
            //Destroy all flagged components
            foreach (var comp in Target.GetComponents<Collider>().Where(x => (x.hideFlags | HideFlags.HideInInspector) == x.hideFlags))
                DestroyImmediate(comp, true);

            // Flag to destroy
            if (colliderToDestroy)
            {
                colliderToDestroy.hideFlags = HideFlags.HideInInspector;
                colliderToDestroy.enabled = false;
            }
        }

        // Only needs constant repaint when in play mode.
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying ? true : base.RequiresConstantRepaint();
        }
    }
}