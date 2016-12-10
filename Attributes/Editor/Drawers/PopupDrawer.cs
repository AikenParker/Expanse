using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Expanse
{
    [CustomPropertyDrawer(typeof(PopupAttribute), true)]
    public class PopupDrawer : PropertyDrawer
    {
        public string[] DisplayedOptions
        {
            get { return (attribute as PopupAttribute).DisplayedOptions; }
            set { (attribute as PopupAttribute).DisplayedOptions = value; }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyDrawer.ApplyReadOnly(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);

            int selectedIndex = 0;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    property.boolValue = EditorGUI.Popup(position, label.text, property.boolValue ? 0 : 1, DisplayedOptions) == 0;
                    break;

                case SerializedPropertyType.String:
                    selectedIndex = DisplayedOptions.Contains(property.stringValue) ? DisplayedOptions.ToList().IndexOf(property.stringValue) : 0;
                    property.stringValue = DisplayedOptions[EditorGUI.Popup(position, label.text, selectedIndex, DisplayedOptions)];
                    break;

                case SerializedPropertyType.Integer:
                    selectedIndex = DisplayedOptions.Contains(property.intValue.ToString()) ? DisplayedOptions.ToList().IndexOf(property.intValue.ToString()) : 0;
                    property.intValue = System.Convert.ToInt32(DisplayedOptions[EditorGUI.Popup(position, label.text, selectedIndex, DisplayedOptions)]);
                    break;

                default:
                    Debug.LogWarning("Unsupported popup type.");
                    break;
            }

            EditorGUI.EndProperty();

            ReadOnlyDrawer.RevertReadOnly();
        }
    }
}