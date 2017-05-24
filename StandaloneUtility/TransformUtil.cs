using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

namespace Expanse
{
    /// <summary>
    /// A collection of Transform related utility funtionality.
    /// </summary>
    public static class TransformUtil
    {
        /// <summary>
        /// Gets or creates a transform at a specific heirachical path. Parentage seperated via a slash character.
        /// </summary>
        public static Transform GetTransformFromPath(string path, bool create = true)
        {
            Transform lastTransform = null;

            foreach (string name in path.Split('/', '\\'))
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                Transform newTransform = null;

                if (lastTransform)
                {
                    newTransform = lastTransform.Find(name);
                }
                else
                {
                    List<GameObject> rootObjects = new List<GameObject>();

                    SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);

                    GameObject gameObject = rootObjects.Find(x => x.name.Equals(name));

                    if (gameObject)
                        newTransform = gameObject.transform;
                }

                if (!newTransform)
                {
                    if (!create)
                    {
                        throw new MissingReferenceException(name);
                    }
                    else
                    {
                        newTransform = new GameObject(name).transform;
                        newTransform.SetParent(lastTransform);
                    }
                }

                lastTransform = newTransform;
            }

            return lastTransform;
        }

        /// <summary>
        /// Performs a breadth-first search to find a deep child transform with name.
        /// </summary>
        public static Transform FindDeepChildByBreadth(Transform parent, string name)
        {
            Transform result = parent.Find(name);

            if (result != null)
                return result;

            foreach (Transform child in parent)
            {
                result = FindDeepChildByBreadth(child, name);

                if (result != null)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Performs a depth-first search to find a deep child transform with name.
        /// </summary>
        public static Transform FindDeepChildByDepth(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform result = child.FindDeepChildByDepth(name);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}