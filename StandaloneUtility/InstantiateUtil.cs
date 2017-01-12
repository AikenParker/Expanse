using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of game object and component instantiation related utility funcationality.
    /// </summary>
    public static class InstantiateUtil
    {
        /// <summary>
        /// Instantiates a copy of a game object.
        /// </summary>
        public static GameObject Instantiate(GameObject original, string name = null, Transform parent = null, bool worldPositionStays = true)
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, parent, worldPositionStays);

            if (!string.IsNullOrEmpty(name))
                gameObject.name = name;
            else
                gameObject.name = original.name;

            return gameObject;
        }

        /// <summary>
        /// Instantiates a copy of a game object from a component.
        /// </summary>
        public static T Instantiate<T>(T original, string name = null, Transform parent = null, bool worldPositionStays = true) where T : Component
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original.gameObject, parent, worldPositionStays);

            if (!string.IsNullOrEmpty(name))
                gameObject.name = name;
            else
                gameObject.name = original.name;

            return gameObject.GetComponent<T>();
        }

        const string NAME_MARKER = "{NAME}";
        const string NUMBER_MARKER = "{NUMBER}";
        const string DEFAULT_NAME = NAME_MARKER + " (" + NUMBER_MARKER + ")";

        /// <summary>
        /// Instantiates multiple copies of a game object.
        /// </summary>
        public static IEnumerable<GameObject> InstantiateMany(GameObject original, int amount, string name = DEFAULT_NAME, Transform parent = null, bool worldPositionStays = true)
        {
            name = name ?? DEFAULT_NAME;

            bool hasNameMarker = name.Contains(NAME_MARKER);
            bool hasNumberMarker = name.Contains(NUMBER_MARKER);

            if (hasNameMarker)
                name = name.Replace(NAME_MARKER, original.name);

            for (int i = 0; i < amount; i++)
            {
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, parent, worldPositionStays);

                if (hasNumberMarker)
                    gameObject.name = name.Replace(NUMBER_MARKER, (i + 1).ToString());
                else
                    gameObject.name = name;

                yield return gameObject;
            }
        }

        /// <summary>
        /// Instantiates multiple copies of a game object from a component.
        /// </summary>
        public static IEnumerable<T> InstantiateMany<T>(T original, int amount, string name = DEFAULT_NAME, Transform parent = null, bool worldPositionStays = true) where T : Component
        {
            name = name ?? DEFAULT_NAME;

            bool hasNameMarker = name.Contains(NAME_MARKER);
            bool hasNumberMarker = name.Contains(NUMBER_MARKER);

            if (hasNameMarker)
                name = name.Replace(NAME_MARKER, original.name);

            for (int i = 0; i < amount; i++)
            {
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original.gameObject, parent, worldPositionStays);

                if (hasNumberMarker)
                    gameObject.name = name.Replace(NUMBER_MARKER, (i + 1).ToString());
                else
                    gameObject.name = name;

                yield return gameObject.GetComponent<T>();
            }
        }
    }
}
