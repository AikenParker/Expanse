using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(ReorderableAttribute))]
    public class ReorderableDrawer : PropertyDrawer
    {
        static Rect firstRect;
        static float height;
        static int selectedIndex = -1;

        ReorderableList reorderableList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.isArray)
            {
                bool isFirstProperty = this.GetIsFirstProperty(property);
                bool isLastProperty = this.GetIsLastProperty(property);

                if (isFirstProperty)
                {
                    firstRect = position;
                }

                if (isLastProperty)
                {
                    SerializedObject serializedObject = property.serializedObject;

                    SerializedProperty arrayProperty = serializedObject.FindProperty(fieldInfo.Name);

                    reorderableList = new ReorderableList(serializedObject, arrayProperty, true, true, true, true);
                    reorderableList.index = selectedIndex;

                    reorderableList.drawHeaderCallback = OnDrawListHeader;
                    reorderableList.drawElementCallback = OnDrawListElement;
                    reorderableList.onSelectCallback = OnSelectListElement;

                    Rect listRect = RectUtil.MaxRect(firstRect, position);

                    serializedObject.Update();
                    reorderableList.DoList(listRect);
                    serializedObject.ApplyModifiedProperties();

                    height = reorderableList.GetHeight();
                }
            }
        }

        private void OnSelectListElement(ReorderableList list)
        {
            selectedIndex = list.index;
        }

        private void OnDrawListHeader(Rect rect)
        {
            GUIContent headerContent = new GUIContent();

            headerContent.text = fieldInfo.Name.AddSpaces().ToTitleCase();

            EditorUtil.ApplyTooltip(fieldInfo, headerContent);

            rect = rect.AddPosition(-16f, 0f);

            EditorGUI.LabelField(rect, headerContent);
        }

        private void OnDrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            GUIContent elementContent = new GUIContent();

            elementContent.text = property.displayName;

            rect = rect.AddSize(0f, -4f).AddPosition(0f, 2f);

            EditorGUI.PropertyField(rect, property, elementContent, property.hasVisibleChildren);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isArray && GetIsLastProperty(property))
            {
                return height;
            }

            return 0;
        }

        bool GetIsLastProperty(SerializedProperty property)
        {
            SerializedObject serializedObject = property.serializedObject;

            string sizePropertyPath = string.Format("{0}.Array.size", fieldInfo.Name);

            SerializedProperty arraySizeProperty = serializedObject.FindProperty(sizePropertyPath);

            int arraySize = arraySizeProperty.intValue;

            string lastPropertyPath = string.Format("{0}.Array.data[{1}]", fieldInfo.Name, arraySize - 1);

            return lastPropertyPath.Equals(property.propertyPath);
        }

        bool GetIsFirstProperty(SerializedProperty property)
        {
            string firstPropertyPath = string.Format("{0}.Array.data[0]", fieldInfo.Name);

            return firstPropertyPath.Equals(property.propertyPath);
        }
    }
}
