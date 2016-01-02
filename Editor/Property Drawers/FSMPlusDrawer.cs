using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Expanse.Ext;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(FSMPlus), true)]
    public class FSMPlusDrawer : PropertyDrawer
    {
        Editor currentStateEditor, defaultStateEditor;
        SerializedProperty BaseStateTypeProp { get; set; }
        SerializedProperty CurrentStateProp { get; set; }
        SerializedProperty DefaultStateProp { get; set; }
        SerializedProperty IteratorProps { get; set; }
        bool showOverrideState, showChildren;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BaseStateTypeProp = property.FindPropertyRelative("_baseStateType");
            CurrentStateProp = property.FindPropertyRelative("_currentState");
            DefaultStateProp = property.FindPropertyRelative("_defaultState");
            IteratorProps = property.FindPropertyRelative("stateOverrides");

            Rect boxRect = EditorGUILayout.BeginVertical();
            RectOffset boxOffset = new RectOffset(15, 7, 8, 6);
            GUI.Box(boxOffset.Add(boxRect), "");
            EditorGUILayout.LabelField(label, EditorStyles.largeLabel, GUILayout.MinHeight(19));

            // Base State Type
            EditorGUILayout.PropertyField(BaseStateTypeProp, new GUIContent(BaseStateTypeProp.displayName));

            // Current State
            Editor.CreateCachedEditor(CurrentStateProp.objectReferenceValue as StatePlus, typeof(StatePlusLoadedEditor), ref currentStateEditor);
            if (currentStateEditor != null && CurrentStateProp.objectReferenceValue != null)
            {
                ((StatePlusLoadedEditor)currentStateEditor).EditorCreation(string.Format("{0}", CurrentStateProp.displayName), !Application.isPlaying, CurrentStateProp);
                currentStateEditor.OnInspectorGUI();
            }
            else
            {
                if (currentStateEditor != null)
                    UnityEngine.Object.DestroyImmediate(currentStateEditor);
                EditorGUILayout.PropertyField(CurrentStateProp, new GUIContent(CurrentStateProp.displayName));
            }

            // Default State
            Editor.CreateCachedEditor(DefaultStateProp.objectReferenceValue as StatePlus, typeof(StatePlusLoadedEditor), ref defaultStateEditor);
            if (defaultStateEditor != null && DefaultStateProp.objectReferenceValue != null)
            {
                ((StatePlusLoadedEditor)defaultStateEditor).EditorCreation(string.Format("{0} ({1})", DefaultStateProp.displayName, DefaultStateProp.objectReferenceValue.name), !Application.isPlaying, DefaultStateProp);
                defaultStateEditor.OnInspectorGUI();
            }
            else
            {
                if (defaultStateEditor != null)
                    UnityEngine.Object.DestroyImmediate(defaultStateEditor);
                EditorGUILayout.PropertyField(DefaultStateProp, new GUIContent(DefaultStateProp.displayName));
            }
            
            // State overrides
            showOverrideState = EditorGUILayout.PropertyField(IteratorProps, new GUIContent(IteratorProps.displayName));
            if (showOverrideState)
            {
                IteratorProps.NextVisible(true);
                do
                {
                    EditorGUI.indentLevel = IteratorProps.depth;
                    showChildren = EditorGUILayout.PropertyField(IteratorProps);

                }
                while (IteratorProps.NextVisible(showChildren));
                EditorGUI.indentLevel = 0;
            }

            // Apply changes to the property
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.UpdateIfDirtyOrScript();

            EditorGUILayout.EndVertical();
        }
    }
}