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
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PopupAttribute popupAttribute = attribute as PopupAttribute;
            string[] displayOptions = popupAttribute.DisplayedOptions;

            EditorUtil.ApplyTooltip(fieldInfo, label);

            ReadOnlyDrawer.ApplyReadOnly(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    {
                        displayOptions = new string[] { displayOptions[0], displayOptions[1] };
                        property.boolValue = EditorGUI.Popup(position, label.text, property.boolValue ? 1 : 0, displayOptions) == 1;
                        break;
                    }

                case SerializedPropertyType.String:
                    {
                        int selectedIndex = displayOptions.Contains(property.stringValue) ? displayOptions.IndexOf(property.stringValue) : 0;
                        int popupIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayOptions);
                        property.stringValue = displayOptions[popupIndex];
                        break;
                    }

                case SerializedPropertyType.Integer:
                    {
                        int selectedIndex = property.intValue;
                        int popupIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayOptions);
                        property.intValue = popupIndex;
                        break;
                    }

                default:
                    Debug.LogWarningFormat("Unsupported popup type: {0}", property.propertyType);
                    break;
            }

            EditorGUI.EndProperty();

            ReadOnlyDrawer.RevertReadOnly();
        }
    }
}