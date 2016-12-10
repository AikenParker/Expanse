using UnityEngine;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEditor;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyDrawer.ApplyReadOnly(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.Enum)
                property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);

            EditorGUI.EndProperty();

            ReadOnlyDrawer.RevertReadOnly();
        }
    }
}