using UnityEngine;
using UnityEditor;

namespace Expanse.Tools
{
    /// <summary>
    /// Replacer editor utility tool. Able to replace a selection of objects with a prefab.
    /// </summary>
    public class Replacer : ScriptableWizard
    {
        public bool keepName = true;
        public bool keepPosition = true;
        public bool keepRotation = true;
        public bool keepScale = false;
        public bool destroyOrignals = true;

        public GameObject Prefab;
        public GameObject[] ReplaceObjects;

        [MenuItem("Expanse/Replacer")]
        static void CreateWizard()
        {
            Replacer replaceGameObjects = DisplayWizard<Replacer>("Replace GameObjects", "Replace");
            replaceGameObjects.ReplaceObjects = Selection.gameObjects;
        }

        void OnWizardCreate()
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("ReplacedGameObjects");
            int undoIndex = Undo.GetCurrentGroup();

            foreach (GameObject gameObject in ReplaceObjects)
            {
                GameObject newObject;
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);

                if (keepName)
                    newObject.name = gameObject.name;
                else
                    newObject.name = Prefab.name;

                newObject.transform.parent = gameObject.transform.parent;

                if (keepPosition)
                    newObject.transform.localPosition = gameObject.transform.localPosition;

                if (keepRotation)
                    newObject.transform.localRotation = gameObject.transform.localRotation;

                if (keepScale)
                    newObject.transform.localScale = gameObject.transform.localScale;

                Undo.RegisterCreatedObjectUndo(newObject, "Replaced" + gameObject.name);

                if (destroyOrignals)
                    Undo.DestroyObjectImmediate(gameObject);
            }

            Undo.CollapseUndoOperations(undoIndex);
        }
    }
}