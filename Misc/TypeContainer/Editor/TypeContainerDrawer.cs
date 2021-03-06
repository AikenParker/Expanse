﻿using System;
using Expanse.Extensions;
using Expanse.Utilities;
using UnityEditor;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Custom drawer for TypeContainer.
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeContainer))]
    public class TypeContainerDrawer : PropertyDrawer
    {
        public Type baseType = null;
        public bool nonAbstractOnly = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            bool isSelfInitialize = fieldInfo == null;

            if (!isSelfInitialize)
            {
                Utilities.EditorUtil.ApplyTooltip(fieldInfo, label);
            }

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty typePathProperty = property.FindPropertyRelative("typeName");

            Type containedType;

            // Draw prefix label
            {
                string typeString = typePathProperty.stringValue;

                if (!typeString.Equals(ReflectionUtil.NULL_TYPE_NAME))
                {
                    containedType = ReflectionUtil.GetTypeFromName(typeString);

                    if ((baseType != null && !containedType.IsAssignableTo(baseType)) ||
                        (nonAbstractOnly && containedType.IsAbstract))
                    {
                        containedType = null;
                    }
                }
                else
                {
                    containedType = null;
                }

                position = EditorGUI.PrefixLabel(position, label);
            }

            // Draw object field
            {
                string typeDisplayName = string.Format("{0} (Type)", containedType != null ? containedType.Name : ReflectionUtil.NULL_TYPE_NAME);

                EditorGUI.LabelField(position, typeDisplayName, EditorStyles.objectField);
            }

            // Draw thumb texture
            {
                int thumbWidth = 18;

                position.xMin = position.xMax - thumbWidth;

                if (GUI.Button(position, string.Empty, GUIStyle.none))
                {
                    Action<Type> onSelectedTypeChanged = (newType) =>
                    {
                        property.serializedObject.Update();

                        typePathProperty.stringValue = newType != null ? newType.FullName : ReflectionUtil.NULL_TYPE_NAME;

                        property.serializedObject.ApplyModifiedProperties();
                    };

                    TypeFinderWindow.Initialize(containedType, onSelectedTypeChanged, baseType, nonAbstractOnly);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
