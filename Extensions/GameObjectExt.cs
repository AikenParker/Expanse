using System;
using System.Collections.Generic;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of GameObject related extension methods.
    /// </summary>
    public static class GameObjectExt
    {
        /// <summary>
        /// Using another component as a source add a component to a game object.
        /// </summary>
        /// <param name="gameObject">Source game object.</param>
        /// <param name="source">Source component to add.</param>
        /// <returns>Returns a new component added onto the game object with vlaues copied from source.</returns>
        public static T AddComponent<T>(this GameObject gameObject, T source) where T : Component
        {
            return GameObjectUtil.AddComponent<T>(gameObject, source);
        }

        /// <summary>
        /// Sets the tag of a game object.
        /// </summary>
        /// <param name="gameObject">Source game obejct.</param>
        /// <param name="tag">Tag value to apply onto the source game object.</param>
        public static void SetTag(this GameObject gameObject, string tag)
        {
            GameObjectUtil.SetTag(gameObject, tag);
        }

        /// <summary>
        /// Sets the tag of a game object and all children.
        /// </summary>
        /// <param name="gameObject">Root source game object.</param>
        /// <param name="tag">Tag value to apply to the source root game object and its children.</param>
        public static void SetTagRecursively(this GameObject gameObject, string tag)
        {
            GameObjectUtil.SetTagRecursively(gameObject, tag);
        }

        /// <summary>
        /// Sets the layer of a game object.
        /// </summary>
        /// <param name="gameObject">Source game object.</param>
        /// <param name="layer">Layer value to apply to the source game object.</param>
        public static void SetLayer(this GameObject gameObject, int layer)
        {
            GameObjectUtil.SetLayer(gameObject, layer);
        }

        /// <summary>
        /// Sets the layer of a game object and all children.
        /// </summary>
        /// <param name="gameObject">Source root game object.</param>
        /// <param name="layer">Layer value to apply to the source game object and its children.</param>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            GameObjectUtil.SetLayerRecursively(gameObject, layer);
        }

        /// <summary>
        /// Destroys all game objects in a list and then clears it.
        /// </summary>
        /// <param name="source">List of all game objects to destroy.</param>
        /// <param name="immediate">If true use DestroyImmediate() instead of Destroy().</param>
        public static void DestroyGameObjects(this IList<GameObject> source, bool immediate = false)
        {
            GameObjectUtil.DestroyGameObjects(source, immediate);
        }

        /// <summary>
        /// Destroys all game objects in a list and then clears it.
        /// </summary>
        /// <param name="source">List of all components with gameobjects to destroy.</param>
        /// <param name="immediate">If true use DestroyImmediate() instead of Destroy().</param>
        public static void DestroyGameObjects<T>(this IList<T> source, bool immediate = false) where T : Component
        {
            GameObjectUtil.DestroyGameObjects<T>(source, immediate);
        }

        /// <summary>
        /// Destroys all components in a list and then clears it.
        /// </summary>
        /// <param name="source">List of all ocmponents to destroy.</param>
        /// <param name="immediate">If true use DestroyImmediate() instead of Destroy().</param>
        public static void DestroyComponents<T>(this IList<T> source, bool immediate = false) where T : Component
        {
            GameObjectUtil.DestroyComponents<T>(source, immediate);
        }
    }
}
