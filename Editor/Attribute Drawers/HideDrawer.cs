using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(HideAttribute))]
    public class HideDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying && (attribute as HideAttribute).showInPlayMode ||
                !Application.isPlaying && (attribute as HideAttribute).showInEditor)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUILayout.PropertyField(property, label, property.hasVisibleChildren);
                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}