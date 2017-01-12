using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of application level related utility functionality.
    /// </summary>
    public static class ApplicationUtil
    {
        /// <summary>
        /// Exits the application or stops playing in editor.
        /// </summary>
        private static void Exit()
        {
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
