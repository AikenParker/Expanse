using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Expanse
{
    /// <summary>
    /// Replacer editor utility tool. Able to replace a selection of objects with a prefab.
    /// </summary>
    public class Replacer : ScriptableWizard
    {
        public bool keepNames = true;
        public bool keepTransform = true;
        public bool destroyOrignals = true;
        public bool keepEdgeCollider2DPoints = false;
        public GameObject Prefab;
        public GameObject[] ReplaceObjects;

        [MenuItem("Expanse/Replacer")]
        static void CreateWizard()
        {
            var replaceGameObjects = ScriptableWizard.DisplayWizard<Replacer>("Replace GameObjects", "Replace");
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

                if (keepNames)
                    newObject.name = gameObject.name;
                else
                    newObject.name = Prefab.name;

                newObject.transform.parent = gameObject.transform.parent;

                if (keepTransform)
                {
                    newObject.transform.localPosition = gameObject.transform.localPosition;
                    newObject.transform.localRotation = gameObject.transform.localRotation;
                    newObject.transform.localScale = gameObject.transform.localScale;
                }

                if (keepEdgeCollider2DPoints)
                {
                    KeepEdgeCollider2DPoints(newObject, gameObject);
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replaced" + gameObject.name);

                if (destroyOrignals)
                    Undo.DestroyObjectImmediate(gameObject);
            }

            Undo.CollapseUndoOperations(undoIndex);
        }

        private void KeepEdgeCollider2DPoints(GameObject newGameObject, GameObject originalGameObject)
        {
            EdgeCollider2D newCollider = newGameObject.GetComponent<EdgeCollider2D>();
            EdgeCollider2D originalCollider = originalGameObject.GetComponent<EdgeCollider2D>();

            if (newCollider && originalCollider)
            {
                newCollider.points = originalCollider.points;
            }
        }
    }
}