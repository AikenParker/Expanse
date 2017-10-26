using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Expanse.Tools
{
    public static class PrefabSelection
    {
        /// <summary>
        /// Applies prefab instance changes to its link prefab source of currently selected prefab instances.
        /// </summary>
        [MenuItem("Expanse/Prefab/Apply Selection")]
        public static void ApplyPrefabSelection()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            int appliedPrefabs = 0;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);

                if (prefabType == PrefabType.PrefabInstance)
                {
                    GameObject rootObject = PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObject);
                    Object prefab = PrefabUtility.GetPrefabParent(rootObject);

                    if (prefab != null)
                    {
                        PrefabUtility.ReplacePrefab(rootObject, prefab, ReplacePrefabOptions.Default);
                        appliedPrefabs++;
                    }
                }
            }

            Debug.LogFormat("Applied changes to {0}/{1} prefabs", appliedPrefabs, gameObjects.Length);
        }

        /// <summary>
        /// Reverts prefab instance changes of currenly selected prefab instances.
        /// </summary>
        [MenuItem("Expanse/Prefab/Revert Selection")]
        public static void RevertPrefabSelection()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            int revertedPrefabs = 0;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);

                if (prefabType == PrefabType.PrefabInstance)
                {
                    GameObject rootObject = PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObject);
                    Object prefab = PrefabUtility.GetPrefabParent(rootObject);

                    if (prefab != null)
                    {
                        PrefabUtility.RevertPrefabInstance(rootObject);
                        revertedPrefabs++;
                    }
                }
            }

            Debug.LogFormat("Reverted changes to {0}/{1} prefabs", revertedPrefabs, gameObjects.Length);
        }
    }
}
