using UnityEngine;
using UnityEditor;using System.Collections;using System.Collections.Generic;using System.Linq;
using Expanse.Ext;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(PopupAttribute), true)]
    public class PopupDrawer : PropertyDrawer
    {
        public string[] displayedOptions
        {
            get { return (attribute as PopupAttribute).displayedOptions; }
            set { (attribute as PopupAttribute).displayedOptions = value; }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyDrawer.BeginReadOnlyCheck(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);
            int selectedIndex = 0;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    property.boolValue = EditorGUI.Popup(position, label.text, property.boolValue ? 0 : 1, displayedOptions) == 0;
                    break;
                case SerializedPropertyType.String:
                    if (displayedOptions[0] == PopupAttribute.STATEPLUS_TYPE_KEY)
                        displayedOptions = typeof(FSMPlus).Assembly.GetTypes().Where(t => (t.IsSubclassOf(typeof(StatePlus)) || t == typeof(StatePlus)) && t.IsAbstract).ConvertValid(t => t.AssemblyQualifiedName).ToArray();
                    selectedIndex = displayedOptions.Contains(property.stringValue) ? displayedOptions.ToList().IndexOf(property.stringValue) : 0;
                    property.stringValue = displayedOptions[EditorGUI.Popup(position, label.text, selectedIndex, displayedOptions)];
                    break;
                case SerializedPropertyType.Integer:
                    selectedIndex = displayedOptions.Contains(property.intValue.ToString()) ? displayedOptions.ToList().IndexOf(property.intValue.ToString()) : 0;
                    property.intValue = System.Convert.ToInt32(displayedOptions[EditorGUI.Popup(position, label.text, selectedIndex, displayedOptions)]);
                    break;
                default:
                    Debug.LogWarning("Unsupported popup type.");
                    break;
            }
            EditorGUI.EndProperty();

            ReadOnlyDrawer.EndReadOnlyCheck();
        }
    }
}