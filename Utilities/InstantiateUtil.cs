using System.Collections.Generic;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of GameObject and Component instantiation related utility funcationality.
    /// </summary>
    public static class InstantiateUtil
    {
        /// <summary>
        /// Instantiates a copy of a game object.
        /// </summary>
        /// <param name="original">Gameobject to copy from.</param>
        /// <param name="name">Name of the new game object to create.</param>
        /// <param name="parent">Parent of the new game object.</param>
        /// <param name="worldPositionStays">If true the world position is the same as the original.</param>
        /// <returns>Returns a new copy of the original game object.</returns>
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
        /// <typeparam name="T">Type of the component reference to copy. (Still copies the entire game object)</typeparam>
        /// <param name="original">Component to copy from.</param>
        /// <param name="name">Name of the new game object to create.</param>
        /// <param name="parent">Parent of the new game object.</param>
        /// <param name="worldPositionStays">If true the world position is the same as the original.</param>
        /// <returns>Returns a new copy of the original game object as a component reference.</returns>
        public static T Instantiate<T>(T original, string name = null, Transform parent = null, bool worldPositionStays = true) where T : Component
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original.gameObject, parent, worldPositionStays);

            if (!string.IsNullOrEmpty(name))
                gameObject.name = name;
            else
                gameObject.name = original.name;

            return gameObject.GetComponent<T>();
        }

        private const string NAME_MARKER = "{NAME}";
        private const string NUMBER_MARKER = "{NUMBER}";
        private const string DEFAULT_NAME = NAME_MARKER + " (" + NUMBER_MARKER + ")";

        /// <summary>
        /// Instantiates multiple copies of a game object.
        /// </summary>
        /// <param name="original">Gameobject to copy from.</param>
        /// <param name="amount">Amount of new objects to create from the original.</param>
        /// <param name="name">Name of the new game object to create.</param>
        /// <param name="parent">Parent of the new game objects.</param>
        /// <param name="worldPositionStays">If true the world position is the same as the original.</param>
        /// <returns>Returns all new copies of the original game object.</returns>
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
        /// <typeparam name="T">Type of the component reference to copy. (Still copies the entire game object)</typeparam>
        /// <param name="original">Component to copy from.</param>
        /// <param name="amount">Amount of new objects to create from the original.</param>
        /// <param name="name">Name of the new game object to create.</param>
        /// <param name="parent">Parent of the new game objects.</param>
        /// <param name="worldPositionStays">If true the world position is the same as the original.</param>
        /// <returns>Returns all new copies of the original game object as component references.</returns>
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
