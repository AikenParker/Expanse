using UnityEngine;
using UnityEditor;using System.Collections;using System.Collections.Generic;using System.Linq;
using System.Reflection;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                if (!(attribute as ReadOnlyAttribute).editableWhilePlaying) GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
            else
            {
                if (!(attribute as ReadOnlyAttribute).editableInEditor) GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }
    }
}