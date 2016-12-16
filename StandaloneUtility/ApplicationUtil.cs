namespace Expanse
{
    public static class ApplicationUtil
    {
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
