using UnityEngine;
using UnityEditor;using System.Collections;using System.Collections.Generic;using System.Linq;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent displayPrefix = label;
            string[] displayedOptions = (attribute as PopupAttribute).displayedOptions;

            EditorGUI.BeginProperty(position, label, property);
            int selectedIndex = 0;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    property.boolValue = EditorGUI.Popup(position, displayPrefix.text, property.boolValue ? 0 : 1, displayedOptions) == 0;
                    break;
                case SerializedPropertyType.String:
                    selectedIndex = displayedOptions.Contains(property.stringValue) ? displayedOptions.ToList().IndexOf(property.stringValue) : 0;
                    property.stringValue = displayedOptions[EditorGUI.Popup(position, displayPrefix.text, selectedIndex, displayedOptions)];
                    break;
                case SerializedPropertyType.Integer:
                    selectedIndex = displayedOptions.Contains(property.intValue.ToString()) ? displayedOptions.ToList().IndexOf(property.intValue.ToString()) : 0;
                    property.intValue = System.Convert.ToInt32(displayedOptions[EditorGUI.Popup(position, displayPrefix.text, selectedIndex, displayedOptions)]);
                    break;
                default:
                    break;
            }
            EditorGUI.EndProperty();
        }
    }
}