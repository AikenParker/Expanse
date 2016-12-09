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
            ReadOnlyDrawer.BeginReadOnlyCheck(fieldInfo);

            if (IsHidden() && Event.current.type != EventType.Repaint)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUILayout.PropertyField(property, label, property.hasVisibleChildren);
                EditorGUI.EndProperty();
            }

            ReadOnlyDrawer.EndReadOnlyCheck();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsHidden() ? 0 : base.GetPropertyHeight(property, label);
        }

        public bool IsHidden()
        {
            return (!Application.isPlaying && (attribute as HideAttribute).ShowInPlayMode) ||
                (Application.isPlaying && (attribute as HideAttribute).ShowInEditor);
        }
    }
}