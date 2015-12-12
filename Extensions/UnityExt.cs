using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Expanse
{
    namespace Ext
    {
        /// <summary>
        /// Unity specific extension methods.
        /// </summary>
        public static class UnityExt
        {
            /// <summary>
            /// Copies over field and property values from one component to another (WARNING: uses reflection)
            /// </summary>
            /// <returns>The source object with newly changed properties and fields.</returns>
            public static T GetCopyof<T>(this Component source, T other) where T : Component
            {
                if (source.IsNullOrEmpty() || other.IsNullOrEmpty())
                    throw new ArgumentNullException();

                Type type = source.GetType();

                // Make sure each type is the same
                if (type != other.GetType())
                    throw new ArgumentException("Other must be the same type as the source.");

                // Using reflection get the type's properties and fields
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
                PropertyInfo[] propertyMembers = type.GetProperties(flags);
                FieldInfo[] fieldMembers = type.GetFields(flags);

                // Set the property values
                foreach (var member in propertyMembers)
                {
                    // Check if the property is writable and not obsolete
                    if (member.CanWrite && !member.IsDefined(typeof(ObsoleteAttribute), true))
                    {
                        try { member.SetValue(source, member.GetValue(other, null), null); }
                        catch { } // Just in case of an exception throw
                    }
                }

                // Set the field values
                foreach (var member in fieldMembers)
                {
                    // Again make sure the field is not obsolete
                    if (!member.IsDefined(typeof(ObsoleteAttribute), true))
                        member.SetValue(source, member.GetValue(other));
                }

                return source as T;
            }

            /// <summary>
            /// Clones an existing component and adds it onto target game object.
            /// </summary>
            public static T AddComponent<T>(this GameObject source, T compToAdd) where T : Component
            {
                return source.AddComponent<T>().GetCopyof<T>(compToAdd) as T;
            }

            /// <summary>
            /// Plays an audio clip on the specified audio source. Handles null-checking.
            /// </summary>
            /// <returns>True if the clip was successfully played on the source.</returns>
            public static void PlayClipAttached(this AudioSource source, AudioClip clip)
            {
                // Null checking
                if (source == null || clip == null) return;

                source.clip = clip;
                source.time = 0;
                source.Play();
            }

            /// <summary>
            /// Plays an audio clip on a clone of the specified audio source. Handles null-checking.
            /// </summary>
            public static AudioSource PlayClipDetached(this AudioSource source, AudioClip clip, Transform target = null)
            {
                // Null checking
                if (source == null || clip == null) return null;

                // Create a new game object, copy over the audio source component, play the clip on new audio source, then destroy object.
                // Create and name new game object
                GameObject NewObject = new GameObject();
                NewObject.transform.SetParent(target);
                NewObject.hideFlags = HideFlags.HideInHierarchy;
                NewObject.name = "Sound Object (" + source.gameObject.name + " - " + clip.name + ")";

                // Add a new audio source component to the new game object and copy over values from the source audio component.
                AudioSource NewSource;
                try { NewSource = NewObject.AddComponent<AudioSource>(source); }
                catch { return null; }

                // Destroy the new game object after the clip has finished playing
                GameObject.Destroy(NewObject, clip.length + 0.1f);

                // Play the clip on the new audio source
                NewSource.PlayClipAttached(clip);
                return NewSource;
            }

            /// <summary>
            /// Returns the length of the animation curve in seconds.
            /// </summary>
            public static float Duration(this AnimationCurve source)
            {
                if (source.IsNullOrEmpty() || source.length <= 0)
                    throw new NullReferenceException();

                return source[source.length - 1].time;
            }

            /// <summary>
            /// Returns a component from an array of ray cast hit.
            /// </summary>
            public static T GetComponent<T>(this RaycastHit[] source) where T : Component
            {
                foreach (var item in source)
                {
                    if (item.collider.GetComponent<T>())
                        return item.collider.GetComponent<T>();
                }
                return default(T);
            }

            /// <summary>
            /// Instantiates a new instance of a component and can parent it to another object.
            /// </summary>
            public static T Instantiate<T>(this T source, Transform parent = null) where T : Component
            {
                if (source.IsNullOrEmpty()) throw new NullReferenceException();

                T NewComponent = (T)GameObject.Instantiate(source);

                if (parent) NewComponent.transform.parent = parent;

                return NewComponent;
            }

            /// <summary>
            /// Instantiates a new instance of a component, can specify its name and can parent it to another object.
            /// </summary>
            public static T Instantiate<T>(this T original, Transform parent, string name) where T : Component
            {
                if (original.IsNullOrEmpty()) throw new NullReferenceException();

                T NewComponent = (T)GameObject.Instantiate(original);

                if (parent) NewComponent.transform.parent = parent;

                NewComponent.name = name;

                return NewComponent;
            }

            /// <summary>
            /// Iterates through both game object colliders and makes them ignore each other.
            /// </summary>
            public static void IgnoreCollision(this GameObject source, GameObject other, bool ignore = true)
            {
                if (!source || !other) return;

                foreach (var sCol in source.GetComponentsInChildren<Collider>())
                {
                    foreach (var oCol in other.GetComponentsInChildren<Collider>())
                    {
                        Physics.IgnoreCollision(sCol, oCol, ignore);
                    }
                }
            }

            /// <summary>
            /// Easily allows for the modification of a color's alpha value.
            /// </summary>
            public static Color SetAlpha(this Color source, float alpha)
            {
                return new Color(source.r, source.g, source.b, alpha);
            }

            /// <summary>
            /// Performs an action the next frame if used like a coroutine
            /// </summary>
            public static IEnumerator NextFrame(Action action)
            {
                yield return new WaitForEndOfFrame();
                action();
            }

            /// <summary>
            /// Shakes the camera in a basic way changing its local position every frame.
            /// </summary>
            public static IEnumerator Shake(this Camera source, float strength, float duration, float frequency)
            {
                float StartTime = Time.time;

                while (Time.time < StartTime + duration)
                {
                    Vector3 Offset = new Vector3(
                        UnityEngine.Random.Range(-strength, strength),
                        UnityEngine.Random.Range(-strength, strength),
                        UnityEngine.Random.Range(-strength, strength));

                    source.transform.localPosition += Offset;

                    if (frequency < Time.deltaTime)
                        yield return new WaitForEndOfFrame();
                    else
                        yield return new WaitForSeconds(frequency);

                    source.transform.localPosition -= Offset;
                }
            }
        }
    }
}
