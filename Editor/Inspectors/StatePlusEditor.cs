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
    public class StatePlusEditor : Editor
    {
        SerializedProperty Property { get; set; }

        bool showChildren;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            GUIContent label = new GUIContent(target.name.AddSpaces());

            EditorGUILayout.LabelField(label, EditorStyles.largeLabel);
            EditorGUILayout.Separator();

            Property = serializedObject.FindProperty("displayName");
            do
            {
                EditorGUI.indentLevel = Property.depth;
                showChildren = EditorGUILayout.PropertyField(Property);

            }
            while (Property.NextVisible(showChildren));
            EditorGUI.indentLevel = 0;

            // Apply changes to the property
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfDirtyOrScript();

            EditorGUILayout.EndVertical();
        }
    }
}