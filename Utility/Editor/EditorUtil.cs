using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Expanse
{
    public static class EditorUtil
    {
        static bool prevGUIEnabled;

        public static void SetGUIEnabled(bool enabled)
        {
            prevGUIEnabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        public static void RevertGUIEnabled()
        {
            GUI.enabled = prevGUIEnabled;
        }

        public static void ApplyTooltip(FieldInfo fieldInfo, GUIContent label)
        {
            TooltipAttribute tooltipAttribute = fieldInfo.GetAttribute<TooltipAttribute>();

            label.tooltip = (tooltipAttribute != null) ? tooltipAttribute.tooltip : string.Empty;
        }
    }
}
