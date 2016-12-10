using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
