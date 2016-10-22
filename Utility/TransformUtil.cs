using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Expanse
{
    public static class TransformUtil
    {
        public static Transform GetTransformFromPath(string path, bool doCreate = true)
        {
            string[] directory = path.Split('/', '\\');

            Transform lastTransform = null;

            foreach (string name in directory)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                Transform newTransform = null;

                if (lastTransform)
                    newTransform = lastTransform.FindChild(name);
                else
                {
                    GameObject gameObject = GameObject.Find(name);

                    if (gameObject)
                        newTransform = gameObject.transform;
                }

                if (!newTransform)
                {
                    if (!doCreate)
                        throw new MissingReferenceException(name);
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