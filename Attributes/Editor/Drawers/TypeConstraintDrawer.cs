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
    /// Custom drawer for TypeConstraintAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeConstraintAttribute))]
    public class TypeConstraintDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType != typeof(TypeContainer))
            {
                Debug.LogWarning("TypeConstraint attribute can only be applied to TypeContainer fields");
                return;
            }

            EditorUtil.ApplyTooltip(fieldInfo, label);

            TypeConstraintAttribute typeConstraintAttribute = (TypeConstraintAttribute)attribute;

            //

            TypeContainerDrawer typeContainerDrawer = new TypeContainerDrawer()
            {
                baseType = typeConstraintAttribute.BaseType,
                nonAbstractOnly = typeConstraintAttribute.NonAbstractOnly
            };

            typeContainerDrawer.OnGUI(position, property, label);
        }
    }
}