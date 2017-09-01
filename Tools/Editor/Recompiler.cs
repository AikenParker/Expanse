using UnityEditor;

namespace Expanse.Tools
{
    public static class Recompiler
    {
        /// <summary>
        /// Forces a script recompilation by re-importing a script an refreshing the asset database.
        /// </summary>
        [MenuItem("Expanse/Recompile")]
        public static void ForceScriptRecompilation()
        {
            AssetDatabase.StartAssetEditing();
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            
            foreach (string assetPath in allAssetPaths)
            {
                if (!assetPath.StartsWith("Assets/Scripts"))
                    continue;

                MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));

                if (script != null)
                {
                    AssetDatabase.ImportAsset(assetPath);
                    break;
                }
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
