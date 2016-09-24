﻿using UnityEngine;
using UnityEditor;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyDrawer.BeginReadOnlyCheck(fieldInfo);

            if (property.propertyType == SerializedPropertyType.Enum)
                property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);

            ReadOnlyDrawer.EndReadOnlyCheck();
        }
    }
}