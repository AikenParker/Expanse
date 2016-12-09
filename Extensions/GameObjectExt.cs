using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public static class GameObjectExt
    {
        /// <summary>
        /// Using another component as a source add a component to a game object.
        /// </summary>
        public static T AddComponent<T>(this GameObject gameObject, T source) where T : Component
        {
            return gameObject.AddComponent<T>().CopyComponent<T>(source) as T;
        }

        /// <summary>
        /// Destroys all game objects in a list and then clears it.
        /// </summary>
        public static void DestroyGameObjects(this IList<GameObject> source, bool immediate = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < source.Count; i++)
            {
                if (immediate)
                    GameObject.DestroyImmediate(source[i]);
                else
                    GameObject.Destroy(source[i]);
            }

            source.Clear();
        }

        /// <summary>
        /// Destroys all game objects in a list and then clears it.
        /// </summary>
        public static void DestroyGameObjects<T>(this IList<T> source, bool immediate = false) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < source.Count; i++)
            {
                if (immediate)
                    GameObject.DestroyImmediate(source[i].gameObject);
                else
                    GameObject.Destroy(source[i].gameObject);
            }

            source.Clear();
        }

        /// <summary>
        /// Destroys all components in a list and then clears it.
        /// </summary>
        public static void DestroyComponents<T>(this IList<T> source, bool immediate = false) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException("source");

            for (int i = 0; i < source.Count; i++)
            {
                if (immediate)
                    GameObject.DestroyImmediate(source[i]);
                else
                    GameObject.Destroy(source[i]);
            }

            source.Clear();
        }
    }
}
