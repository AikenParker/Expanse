using UnityEditor;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Custom inspector for SerializedTimerInspector.
    /// </summary>
    [CustomEditor(typeof(SerializedTimer), true)]
    public class SerializedTimerInspector : Editor
    {
        SerializedTimer Target;

        void OnEnable()
        {
            this.Target = target as SerializedTimer;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            Utilities.EditorUtil.DrawInspectorScriptField<SerializedTimer>(Target);
            Utilities.EditorUtil.DrawInspectorEditorScriptField<SerializedTimerInspector>(this);

            // Top-Level

            SerializedProperty useGlobalCBRProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.USE_GLOBAL_CBR);
            EditorGUILayout.PropertyField(useGlobalCBRProperty, Utilities.EditorUtil.GetGUIContent(useGlobalCBRProperty));

            if (useGlobalCBRProperty.boolValue == false)
            {
                EditorGUI.indentLevel++;
                SerializedProperty callBackRelayProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.CALL_BACK_RELAY);
                EditorGUILayout.PropertyField(callBackRelayProperty, Utilities.EditorUtil.GetGUIContent(callBackRelayProperty));
                EditorGUI.indentLevel--;
            }

            SerializedProperty attachedMonoBehaviourProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.ATTACHED_MONOBEHAVIOUR);
            EditorGUILayout.PropertyField(attachedMonoBehaviourProperty, Utilities.EditorUtil.GetGUIContent(attachedMonoBehaviourProperty));

            if (attachedMonoBehaviourProperty.objectReferenceValue == null && !Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Unsafe Timer", MessageType.Warning, true);
            }

            // Settings

            SerializedProperty settingsProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.TIMER_SETTINGS);

            SerializedProperty isRandomizedProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.IS_RANDOMIZED);
            EditorGUILayout.PropertyField(isRandomizedProperty, Utilities.EditorUtil.GetGUIContent(isRandomizedProperty));

            if (isRandomizedProperty.boolValue == false)
            {
                SerializedProperty durationProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.DURATION);
                EditorGUILayout.PropertyField(durationProperty, Utilities.EditorUtil.GetGUIContent(durationProperty));

                if (durationProperty.floatValue < 0)
                    durationProperty.floatValue = 0;
            }
            else
            {
                EditorGUI.indentLevel++;

                SerializedProperty minDurationProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.MIN_DURATION);
                SerializedProperty maxDurationProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.MAX_DURATION);

                EditorGUILayout.PropertyField(minDurationProperty, Utilities.EditorUtil.GetGUIContent(minDurationProperty));
                EditorGUILayout.PropertyField(maxDurationProperty, Utilities.EditorUtil.GetGUIContent(maxDurationProperty));

                if (minDurationProperty.floatValue < 0)
                    minDurationProperty.floatValue = 0;

                if (maxDurationProperty.floatValue < minDurationProperty.floatValue)
                    maxDurationProperty.floatValue = minDurationProperty.floatValue;

                EditorGUI.indentLevel--;
            }

            SerializedProperty autoPlayProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.AUTO_PLAY);
            EditorGUILayout.PropertyField(autoPlayProperty, Utilities.EditorUtil.GetGUIContent(autoPlayProperty));

            SerializedProperty playerBackRateProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.PLAY_BACK_RATE);
            EditorGUILayout.PropertyField(playerBackRateProperty, Utilities.EditorUtil.GetGUIContent(playerBackRateProperty));

            SerializedProperty updateModeProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.UPDATE_MODE);
            EditorGUILayout.PropertyField(updateModeProperty, Utilities.EditorUtil.GetGUIContent(updateModeProperty));

            SerializedProperty completionModeProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.COMPLETION_MODE);
            EditorGUILayout.PropertyField(completionModeProperty, Utilities.EditorUtil.GetGUIContent(completionModeProperty));

            Timer.TimerCompletionModes completionMode = (Timer.TimerCompletionModes)completionModeProperty.enumValueIndex;

            if (completionMode == Timer.TimerCompletionModes.Restart || completionMode == Timer.TimerCompletionModes.Reverse)
            {
                EditorGUI.indentLevel++;

                SerializedProperty repeatsProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.REPEATS);
                EditorGUILayout.PropertyField(repeatsProperty, Utilities.EditorUtil.GetGUIContent(repeatsProperty));

                if (repeatsProperty.intValue < -1)
                    repeatsProperty.intValue = -1;

                if (repeatsProperty.intValue != -1 && !Application.isPlaying)
                    EditorGUILayout.HelpBox("Set to -1 for endless repeats", MessageType.Info, false);

                EditorGUI.indentLevel--;
            }

            SerializedProperty deactivateOnLoadProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.DEACTIVATE_ON_LOAD);
            EditorGUILayout.PropertyField(deactivateOnLoadProperty, Utilities.EditorUtil.GetGUIContent(deactivateOnLoadProperty));

            /*
            SerializedProperty priorityProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.PRIORITY);
            EditorGUILayout.PropertyField(priorityProperty, Utilities.EditorUtil.GetGUIContent(priorityProperty));
            
            SerializedProperty alwaysPlayProperty = settingsProperty.FindPropertyRelative(TimerSettingsPropertyNames.ALWAYS_PLAY);
            EditorGUILayout.PropertyField(alwaysPlayProperty, Utilities.EditorUtil.GetGUIContent(alwaysPlayProperty))
            */

            // Events

            EditorGUILayout.BeginHorizontal();

            SerializedProperty enableCompletedEventProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.ENABLED_COMPLETED_EVENT);
            enableCompletedEventProperty.boolValue = GUILayout.Toggle(enableCompletedEventProperty.boolValue, new GUIContent("Complete"), "Button");

            SerializedProperty enableReturnedProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.ENABLED_RETURNED_EVENT);
            enableReturnedProperty.boolValue = GUILayout.Toggle(enableReturnedProperty.boolValue, new GUIContent("Returned"), "Button");

            SerializedProperty enableCompletedOrReturnedEventProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.ENABLED_COMPLETED_OR_RETURNED_EVENT);
            enableCompletedOrReturnedEventProperty.boolValue = GUILayout.Toggle(enableCompletedOrReturnedEventProperty.boolValue, new GUIContent("Either"), "Button");

            EditorGUILayout.EndHorizontal();

            if (enableCompletedEventProperty.boolValue)
            {
                SerializedProperty completedProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.COMPLETED);
                EditorGUILayout.PropertyField(completedProperty, Utilities.EditorUtil.GetGUIContent(completedProperty));
            }

            if (enableReturnedProperty.boolValue)
            {
                SerializedProperty returnedProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.RETURNED);
                EditorGUILayout.PropertyField(returnedProperty, Utilities.EditorUtil.GetGUIContent(returnedProperty));
            }

            if (enableCompletedOrReturnedEventProperty.boolValue)
            {
                SerializedProperty completedOrReturnedProperty = serializedObject.FindProperty(SerializedTimerPropertyNames.COMPLETED_OR_RETURNED);
                EditorGUILayout.PropertyField(completedOrReturnedProperty, Utilities.EditorUtil.GetGUIContent(completedOrReturnedProperty));
            }

            //

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        private static class SerializedTimerPropertyNames
        {
            public const string USE_GLOBAL_CBR = "useGlobalCBR";
            public const string CALL_BACK_RELAY = "callBackRelay";
            public const string ATTACHED_MONOBEHAVIOUR = "attachedMonoBehaviour";
            public const string TIMER_SETTINGS = "timerSettings";
            public const string COMPLETED = "completed";
            public const string RETURNED = "returned";
            public const string COMPLETED_OR_RETURNED = "completedOrReturned";
            public const string ENABLED_COMPLETED_EVENT = "enableCompletedEvent";
            public const string ENABLED_RETURNED_EVENT = "enableReturnedEvent";
            public const string ENABLED_COMPLETED_OR_RETURNED_EVENT = "enableCompletedOrReturnedEvent";
        }

        private static class TimerSettingsPropertyNames
        {
            public const string DURATION = "duration";
            public const string IS_RANDOMIZED = "isRandomized";
            public const string MIN_DURATION = "minDuration";
            public const string MAX_DURATION = "maxDuration";
            public const string AUTO_PLAY = "autoPlay";
            public const string PLAY_BACK_RATE = "playBackRate";
            public const string COMPLETION_MODE = "completionMode";
            public const string UPDATE_MODE = "updateMode";
            public const string REPEATS = "repeats";
            public const string DEACTIVATE_ON_LOAD = "deactivateOnLoad";
            public const string PRIORITY = "priority";
            public const string ALWAYS_PLAY = "alwaysPlay";
        }
    }
}
