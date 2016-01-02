using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Expanse.Ext;

namespace Expanse
{
    [CustomEditor(typeof(StatePlus), true)]
    public class StatePlusLoadedEditor : Editor
    {
        private string displayName;
        private bool enableToolbar = true;
        private SerializedProperty targetProp;

        SerializedProperty Property { get; set; }

        bool showProperty, showChildren;

        public override void OnInspectorGUI()
        {
            if (targetProp == null)
                throw new Exception("This StatePlus editor must be initialized. Use the other one.");

            EditorGUILayout.BeginVertical();
            GUIContent label = new GUIContent(displayName);
            EditorGUI.indentLevel = 0;

            Rect foldoutPosition = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(foldoutPosition, label, targetProp);
            EditorGUILayout.PropertyField(targetProp, label, false);
            EditorGUI.EndProperty();
            EditorGUILayout.EndHorizontal();

            foldoutPosition.width = 10;
            showProperty = EditorGUI.Foldout(foldoutPosition, showProperty, "", true);

            if (showProperty)
            {
                bool prevEnabled = GUI.enabled;
                GUI.enabled = enableToolbar;
                int toolbarIndex = GUILayout.Toolbar(-1, new string[] { "Clear", "Find", "Clone" });
                GUI.enabled = prevEnabled;

                if (toolbarIndex != -1)
                    ToolbarAction(toolbarIndex);

                Property = serializedObject.FindProperty("displayName");
                do
                {
                    EditorGUI.indentLevel = Property.depth + 1;
                    showChildren = EditorGUILayout.PropertyField(Property);

                }
                while (Property.NextVisible(showChildren));
                EditorGUI.indentLevel = 0;

                // Apply changes to the property
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfDirtyOrScript();
            }

            EditorGUI.indentLevel = 0;
            EditorGUILayout.EndVertical();
        }

        public void EditorCreation(string displayNameVal, bool enableToolbarVal, SerializedProperty targetPropVal)
        {
            displayName = displayNameVal;
            enableToolbar = enableToolbarVal;
            targetProp = targetPropVal;
        }

        private void ToolbarAction(int index)
        {
            if (index == 0) // Clear
            {
                targetProp.objectReferenceValue = null;
            }
            else if (index == 1) // Find
            {
                Selection.activeObject = targetProp.objectReferenceValue;
            }
            else if (index == 2) // Clone
            {
                StatePlus newState = (StatePlus)ScriptableObject.CreateInstance(targetProp.objectReferenceValue.GetType());
                newState.Load((StatePlus)targetProp.objectReferenceValue);

                if (newState != null)
                {
                    string savePath = EditorUtility.SaveFilePanel("Asset Save Location", targetProp.propertyPath, "NewState", "asset");

                    if (savePath == "")
                        return;

                    savePath = FileUtil.GetProjectRelativePath(savePath);

                    AssetDatabase.CreateAsset(newState, savePath);
                    AssetDatabase.SaveAssets();

                    targetProp.objectReferenceValue = newState;
                }
                else
                {
                    Debug.LogError("Unable to create a copy of " + targetProp.name);
                }
            }
            else
            {
                throw new Exception("Invalid toolbar action index.");
            }
        }
    }
}