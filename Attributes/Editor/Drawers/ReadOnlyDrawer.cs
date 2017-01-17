using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Expanse
{
    /// <summary>
    /// Custom drawer for ReadOnlyAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorUtil.ApplyTooltip(fieldInfo, label);

            ApplyReadOnly(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);

            //

            if (property.propertyType == SerializedPropertyType.Float && fieldInfo.HasAttribute<RangeAttribute>())
            {
                RangeAttribute rangeAttribute = fieldInfo.GetAttribute<RangeAttribute>();

                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, rangeAttribute.min, rangeAttribute.max);
            }
            else if (property.propertyType == SerializedPropertyType.Integer && fieldInfo.HasAttribute<RangeAttribute>())
            {
                RangeAttribute rangeAttribute = fieldInfo.GetAttribute<RangeAttribute>();

                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, Mathf.FloorToInt(rangeAttribute.min), Mathf.RoundToInt(rangeAttribute.max));
            }
            else if (property.propertyType == SerializedPropertyType.Generic && fieldInfo.FieldType == typeof(TypeContainer))
            {
                TypeContainerDrawer typeContainerDrawer = new TypeContainerDrawer();

                typeContainerDrawer.OnGUI(position, property, label);

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

            EditorUtil.SetGuiEnabled(!isReadOnly);
        }

        public static void RevertReadOnly()
        {
            EditorUtil.RevertGuiEnabled();
        }
    }
}