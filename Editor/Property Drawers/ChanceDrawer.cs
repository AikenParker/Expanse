using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(Chance))]
    public class ChanceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("chance"), label);
        }
    }
}
