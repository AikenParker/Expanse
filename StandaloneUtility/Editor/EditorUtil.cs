using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Expanse
{
    public static class EditorUtil
    {
        static bool prevGUIEnabled;
        static Color prevGUIBackgroundColor;

        /// <summary>
        /// Set the enabled state of the GUI.
        /// </summary>
        public static void SetGUIEnabled(bool enabled)
        {
            prevGUIEnabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        /// <summary>
        /// Revert back to the previous state of GUI.
        /// </summary>
        public static void RevertGUIEnabled()
        {
            GUI.enabled = prevGUIEnabled;
        }

        /// <summary>
        /// Set the background color of the GUI.
        /// </summary>
        public static void SetGUIBackgroundColor(Color color)
        {
            prevGUIBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        /// <summary>
        /// Revert back to the previous background color of the GUI.
        /// </summary>
        public static void RevertGUIBackgroundColor()
        {
            GUI.backgroundColor = prevGUIBackgroundColor;
        }

        /// <summary>
        /// If a fieldinfo is decorated with the TooltipAttribute then apply the tooltip value into the GUIContent
        /// </summary>
        public static void ApplyTooltip(FieldInfo fieldInfo, GUIContent label)
        {
            UnityEngine.TooltipAttribute tooltipAttribute = fieldInfo.GetAttribute<UnityEngine.TooltipAttribute>();

            label.tooltip = (tooltipAttribute != null) ? tooltipAttribute.tooltip : string.Empty;
        }

        /// <summary>
        /// Gets a standardized GUIContent of a serialized property.
        /// </summary>
        public static GUIContent GetGUIContent(SerializedProperty source)
        {
            GUIContent guiContent = new GUIContent();

            guiContent.text = source.displayName;

            return guiContent;
        }

        /// <summary>
        /// Draws a default inspector script field to replicate Unity.
        /// </summary>
        public static void DrawInspectorScriptField<T>(T target) where T : MonoBehaviour
        {
            SetGUIEnabled(false);

            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target), typeof(MonoScript), false);

            RevertGUIEnabled();
        }

        /// <summary>
        /// Draws an inspector editor script field.
        /// </summary>
        public static void DrawInspectorEditorScriptField<T>(T target) where T : Editor
        {
            SetGUIEnabled(false);

            EditorGUILayout.ObjectField("Editor Script", MonoScript.FromScriptableObject(target), typeof(MonoScript), false);

            RevertGUIEnabled();
        }
    }
}
