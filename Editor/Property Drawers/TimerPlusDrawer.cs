using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace Expanse
{
    [CustomPropertyDrawer(typeof(Timer), true)]
    public class TimerPlusDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Timer target = (Timer)fieldInfo.GetValue(property.serializedObject.targetObject);

            if (Application.isPlaying)
            {
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width - EditorStyles.textField.fixedWidth, position.height), label);
                GUI.enabled = false;
                EditorGUI.Slider(new Rect(position.x + (position.width * .37f), position.y, position.width * .63f, position.height), target.Value, 0, target.Length);
                GUI.enabled = true;

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, false);
            }
        }
    }
}
