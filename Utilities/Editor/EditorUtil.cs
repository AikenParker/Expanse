using System.Reflection;
using Expanse.Extensions;
using UnityEditor;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Editor related utility functionality.
    /// </summary>
    public static class EditorUtil
    {
        private static bool prevGUIEnabled;
        private static Color prevGUIColor;
        private static Color prevGUIContentColor;
        private static Color prevGUIBackgroundColor;

        /// <summary>
        /// Sets the enabled state of the GUI if its current state is enabled.
        /// </summary>
        /// <param name="enabled">State value to set the GUI in.</param>
        public static void ApplyGUIEnabled(bool enabled)
        {
            prevGUIEnabled = GUI.enabled;
            if (prevGUIEnabled)
                GUI.enabled = enabled;
        }

        /// <summary>
        /// Set the enabled state of the GUI.
        /// </summary>
        /// <param name="enabled">State value to set the GUI in.</param>
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
        /// Set the color of the GUI.
        /// </summary>
        /// <param name="color">Color value to set the GUI in.</param>
        public static void SetGUIColor(Color color)
        {
            prevGUIColor = GUI.color;
            GUI.color = color;
        }

        /// <summary>
        /// Revert back to the previous color of the GUI.
        /// </summary>
        public static void RevertGUIColor()
        {
            GUI.color = prevGUIColor;
        }

        /// <summary>
        /// Set the content color of the GUI.
        /// </summary>
        /// <param name="color">Color value to set the GUIContent in.</param>
        public static void SetGUIContentColor(Color color)
        {
            prevGUIContentColor = GUI.contentColor;
            GUI.contentColor = color;
        }

        /// <summary>
        /// Revert back to the previous content color of the GUI.
        /// </summary>
        public static void RevertGUIContentColor()
        {
            GUI.contentColor = prevGUIContentColor;
        }

        /// <summary>
        /// Set the background color of the GUI.
        /// </summary>
        /// <param name="color">Color value to the GUIBackground in.</param>
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
        /// <param name="source">Serialized property to get a GUIContent.</param>
        /// <returns>Returns a new GUIContent to be used with the serialized property.</returns>
        public static GUIContent GetGUIContent(SerializedProperty source)
        {
            GUIContent guiContent = new GUIContent();

            guiContent.text = source.displayName;

            return guiContent;
        }

        /// <summary>
        /// Draws a default inspector script field to replicate Unity.
        /// </summary>
        /// <typeparam name="T">Type of the MonoBehaviour script.</typeparam>
        /// <param name="target">Target object instance.</param>
        public static void DrawInspectorScriptField<T>(T target) where T : MonoBehaviour
        {
            SetGUIEnabled(false);

            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target), typeof(MonoScript), false);

            RevertGUIEnabled();
        }

        /// <summary>
        /// Draws an inspector editor script field.
        /// </summary>
        /// <typeparam name="T">Type of the Editor script.</typeparam>
        /// <param name="target">Target object instance.</param>
        public static void DrawInspectorEditorScriptField<T>(T target) where T : Editor
        {
            SetGUIEnabled(false);

            EditorGUILayout.ObjectField("Editor Script", MonoScript.FromScriptableObject(target), typeof(MonoScript), false);

            RevertGUIEnabled();
        }
    }
}
