using UnityEngine;
using UnityEditor;

namespace Expanse
{
    public static class EditorGUIFoldoutToggle
    {
        public static void ToggleFoldoutLayout(bool foldout, bool toggle, out bool newFoldout, out bool newToggle, GUIContent label, GUIStyle foldoutStyle, GUIStyle toggleStyle)
        {
            newToggle = toggle;

            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, 16f, 16f, toggleStyle);

            Rect foldoutRect, toggleRect;
            foldoutRect = toggleRect = position;

            float foldoutWidth = 12f;
            float padding = 2f;

            foldoutRect.xMax = foldoutRect.xMin + foldoutWidth;
            toggleRect.xMin = foldoutRect.xMax + padding;

            newFoldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, false, foldoutStyle);
            newToggle = EditorGUI.ToggleLeft(toggleRect, label, toggle, toggleStyle);
        }

        public static void ToggleFoldoutLayout(bool foldout, bool toggle, out bool newFoldout, out bool newToggle, GUIContent label)
        {
            ToggleFoldoutLayout(foldout, toggle, out newFoldout, out newToggle, label, EditorStyles.foldout, EditorStyles.label);
        }

        public static void ToggleFoldoutLayout(bool foldout, bool toggle, out bool newFoldout, out bool newToggle)
        {
            ToggleFoldoutLayout(foldout, toggle, out newFoldout, out newToggle, GUIContent.none, EditorStyles.foldout, EditorStyles.label);
        }

        public static void ToggleFoldoutLayout(bool foldout, bool toggle, out bool newFoldout, out bool newToggle, string label, GUIStyle foldoutStyle, GUIStyle toggleStyle)
        {
            newToggle = toggle;

            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, 16f, 16f, toggleStyle);

            Rect foldoutRect, toggleRect;
            foldoutRect = toggleRect = position;

            float foldoutWidth = 12f;
            float padding = 2f;

            foldoutRect.xMax = foldoutRect.xMin + foldoutWidth;
            toggleRect.xMin = foldoutRect.xMax + padding;

            newFoldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, false, foldoutStyle);
            newToggle = EditorGUI.ToggleLeft(toggleRect, label, toggle, toggleStyle);
        }

        public static void ToggleFoldoutLayout(bool foldout, bool toggle, out bool newFoldout, out bool newToggle, string label)
        {
            ToggleFoldoutLayout(foldout, toggle, out newFoldout, out newToggle, label, EditorStyles.foldout, EditorStyles.label);
        }
    }
}
