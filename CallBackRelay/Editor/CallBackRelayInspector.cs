﻿using UnityEditor;

namespace Expanse
{
    /// <summary>
    /// Custom inspector for CallBackRelay.
    /// </summary>
    [CustomEditor(typeof(CallBackRelay), true)]
    public class CallBackRelayInspector : Editor
    {
        CallBackRelay Target;

        void OnEnable()
        {
            this.Target = target as CallBackRelay;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            Utilities.EditorUtil.DrawInspectorScriptField<CallBackRelay>(Target);
            Utilities.EditorUtil.DrawInspectorEditorScriptField<CallBackRelayInspector>(this);

            //

            SerializedProperty dontDestroyOnLoadProperty = serializedObject.FindProperty(CallBackRelayPropertyNames.DONT_DESTROY_ON_LOAD);
            EditorGUILayout.PropertyField(dontDestroyOnLoadProperty, Utilities.EditorUtil.GetGUIContent(dontDestroyOnLoadProperty));

            SerializedProperty updateSettingsProperty = serializedObject.FindProperty(CallBackRelayPropertyNames.UPDATE_SETTINGS);

            SerializedProperty updateTypeProperty = updateSettingsProperty.FindPropertyRelative(CallBackRelayUpdateSettingsPropertyNames.UPDATE_TYPE);
            EditorGUILayout.PropertyField(updateTypeProperty, Utilities.EditorUtil.GetGUIContent(updateTypeProperty));

            switch ((CallBackRelaySettings.UpdateTypes)updateTypeProperty.enumValueIndex)
            {
                case CallBackRelaySettings.UpdateTypes.Spread:

                    EditorGUI.indentLevel++;
                    SerializedProperty spreadCountProperty = updateSettingsProperty.FindPropertyRelative(CallBackRelayUpdateSettingsPropertyNames.SPREAD_COUNT);
                    EditorGUILayout.PropertyField(spreadCountProperty, Utilities.EditorUtil.GetGUIContent(spreadCountProperty));
                    EditorGUI.indentLevel--;

                    if (spreadCountProperty.intValue < 1)
                        spreadCountProperty.intValue = 1;

                    break;

                case CallBackRelaySettings.UpdateTypes.Budget:

                    EditorGUI.indentLevel++;
                    SerializedProperty frameBudgetProperty = updateSettingsProperty.FindPropertyRelative(CallBackRelayUpdateSettingsPropertyNames.FRAME_BUDGET);
                    EditorGUILayout.PropertyField(frameBudgetProperty, Utilities.EditorUtil.GetGUIContent(frameBudgetProperty));
                    EditorGUI.indentLevel--;

                    if (frameBudgetProperty.floatValue < 0)
                        frameBudgetProperty.floatValue = 0;

                    break;
            }

            SerializedProperty skipTypeProperty = updateSettingsProperty.FindPropertyRelative(CallBackRelayUpdateSettingsPropertyNames.SKIP_TYPE);
            EditorGUILayout.PropertyField(skipTypeProperty, Utilities.EditorUtil.GetGUIContent(skipTypeProperty));

            switch ((CallBackRelaySettings.SkipTypes)skipTypeProperty.enumValueIndex)
            {
                case CallBackRelaySettings.SkipTypes.Count:

                    EditorGUI.indentLevel++;
                    SerializedProperty skipFramesProperty = updateSettingsProperty.FindPropertyRelative(CallBackRelayUpdateSettingsPropertyNames.SKIP_FRAMES);
                    EditorGUILayout.PropertyField(skipFramesProperty, Utilities.EditorUtil.GetGUIContent(skipFramesProperty));
                    EditorGUI.indentLevel--;

                    if (skipFramesProperty.intValue < 0)
                        skipFramesProperty.intValue = 0;

                    break;

                case CallBackRelaySettings.SkipTypes.Time:

                    EditorGUI.indentLevel++;
                    SerializedProperty skipTimeProperty = updateSettingsProperty.FindPropertyRelative(CallBackRelayUpdateSettingsPropertyNames.SKIP_TIME);
                    EditorGUILayout.PropertyField(skipTimeProperty, Utilities.EditorUtil.GetGUIContent(skipTimeProperty));
                    EditorGUI.indentLevel--;

                    if (skipTimeProperty.floatValue < 0)
                        skipTimeProperty.floatValue = 0;

                    break;
            }

            //

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        private static class CallBackRelayPropertyNames
        {
            public const string DONT_DESTROY_ON_LOAD = "destroyOnLoad";
            public const string UPDATE_SETTINGS = "updateSettings";
        }

        private static class CallBackRelayUpdateSettingsPropertyNames
        {
            public const string UPDATE_TYPE = "updateType";
            public const string SKIP_TYPE = "skipType";
            public const string SKIP_FRAMES = "skipFrames";
            public const string SKIP_TIME = "skipTime";
            public const string SPREAD_COUNT = "spreadCount";
            public const string FRAME_BUDGET = "frameBudget";
        }
    }
}
