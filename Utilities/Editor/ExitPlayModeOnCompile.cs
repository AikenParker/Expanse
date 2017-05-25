using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Expanse.Utilities
{
    /// <summary>
    /// Exits play mode automatically when Unity performs code compilation.
    /// </summary>
    [InitializeOnLoad]
    public class ExitPlayModeOnCompile
    {
        private static ExitPlayModeOnCompile instance = null;

        static ExitPlayModeOnCompile()
        {
            instance = new ExitPlayModeOnCompile();
        }

        private ExitPlayModeOnCompile()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        ~ExitPlayModeOnCompile()
        {
            EditorApplication.update -= OnEditorUpdate;

            if (instance == this)
                instance = null;
        }

        private static void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying && EditorApplication.isCompiling)
            {
                EditorApplication.isPlaying = false;
            }
        }
    }
}
