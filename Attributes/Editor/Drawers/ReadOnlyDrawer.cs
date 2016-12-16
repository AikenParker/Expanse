using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorUtil.ApplyTooltip(fieldInfo, label);

            ApplyReadOnly(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);

            //

            if (fieldInfo.HasAttribute<RangeAttribute>() && property.propertyType == SerializedPropertyType.Float)
            {
                RangeAttribute rangeAttribute = fieldInfo.GetAttribute<RangeAttribute>();

                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, rangeAttribute.min, rangeAttribute.max);
            }
            else if (fieldInfo.HasAttribute<RangeAttribute>() && property.propertyType == SerializedPropertyType.Integer)
            {
                RangeAttribute rangeAttribute = fieldInfo.GetAttribute<RangeAttribute>();

                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, Mathf.FloorToInt(rangeAttribute.min), Mathf.RoundToInt(rangeAttribute.max));
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, property.hasVisibleChildren);
            }

            //

            EditorGUI.EndProperty();

            RevertReadOnly();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public static void ApplyReadOnly(FieldInfo fieldInfo)
        {
            ReadOnlyAttribute readOnlyAttribute;

            bool isReadOnly = false;

            if (fieldInfo.TryGetAttribute(out readOnlyAttribute))
                isReadOnly = readOnlyAttribute.IsReadOnly;

            EditorUtil.SetGUIEnabled(!isReadOnly);
        }

        public static void RevertReadOnly()
        {
            EditorUtil.RevertGUIEnabled();
        }
    }
}