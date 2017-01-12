using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

namespace Expanse
{
    /// <summary>
    /// A collection of transform related utility funtionality.
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
                    newTransform = lastTransform.FindChild(name);
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
    }
}