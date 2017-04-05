using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of GameObject related extension methods.
    /// </summary>
    public static class GameObjectExt
    {
        /// <summary>
        /// Using another component as a source add a component to a game object.
        /// </summary>
        public static T AddComponent<T>(this GameObject gameObject, T source) where T : Component
        {
            T newComponent = gameObject.AddComponent<T>();
            newComponent.CopyComponent<T>(source);
            return newComponent;
        }

        /// <summary>
        /// Sets the tag of a game object.
        /// </summary>
        public static void SetTag(this GameObject gameObject, string tag)
        {
            gameObject.tag = tag;
        }

        /// <summary>
        /// Sets the tag of a game object and all children.
        /// </summary>
        public static void SetTagRecursively(this GameObject gameObject, string tag)
        {
            gameObject.tag = tag;

            Transform transform = gameObject.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                child.gameObject.SetTagRecursively(tag);
            }
        }

        /// <summary>
        /// Sets the layer of a game object.
        /// </summary>
        public static void SetLayer(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
        }

        /// <summary>
        /// Sets the layer of a game object and all children.
        /// </summary>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            Transform transform = gameObject.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                child.gameObject.SetLayerRecursively(layer);
            }
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
