using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of application level related utility functionality.
    /// </summary>
    public static class ApplicationUtil
    {
        /// <summary>
        /// Exits the application or stops playing in editor.
        /// </summary>
        private static void Exit()
        {
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Loads an object from a Json file using another thread.
        /// </summary>
        public static IEnumerator Co_LoadJsonFile<T>(string filePath, Action<JsonLoadInfo<T>> onComplete, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            JsonLoadInfo<T> loadInfo = new JsonLoadInfo<T>();

            bool hasLoaded = false;

            new Thread(() =>
            {
                if (File.Exists(filePath))
                {
#if UNITY_ANDROID
                    WWW fileRequest = new WWW(filePath);
                    while (!fileRequest.isDone) { }
                    string json = fileRequest.text;
#else
                    string json = File.ReadAllText(filePath);
#endif
                    if (!string.IsNullOrEmpty(json))
                    {
                        T target = JsonUtility.FromJson<T>(json);

                        if (target != null)
                            loadInfo = new JsonLoadInfo<T>(target, true, "Json file at path successfully loaded: " + filePath);
                        else
                            loadInfo = new JsonLoadInfo<T>(default(T), false, "Json file at path is unable to be parsed: " + filePath);
                    }
                    else
                        loadInfo = new JsonLoadInfo<T>(default(T), false, "Json file at path does not contain any data: " + filePath);
                }
                else
                    loadInfo = new JsonLoadInfo<T>(default(T), false, "Json file at path does not exist: " + filePath);

                hasLoaded = true;

            }).Start(priority);

            while (!hasLoaded)
            {
                yield return null;
            }

            onComplete.SafeInvoke(loadInfo);
        }

        /// <summary>
        /// Loads an object from a Json file in the StreamingAssets folder using another thread.
        /// </summary>
        public static IEnumerator Co_LoadStreamingAssetJsonFile<T>(string filePath, Action<JsonLoadInfo<T>> onComplete, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            filePath = Path.Combine(Application.streamingAssetsPath, filePath);

            yield return Co_LoadJsonFile(filePath, onComplete, priority);
        }

        /// <summary>
        /// Info class that contains the results of a Json load action.
        /// </summary>
        public class JsonLoadInfo<T> where T : new()
        {
            private readonly T target;
            private readonly bool success;
            private readonly string error;

            public JsonLoadInfo()
            {
                this.target = default(T);
                this.success = false;
                this.error = "Unhandled error";
            }

            public JsonLoadInfo(T target, bool success, string error)
            {
                this.target = target;
                this.success = success;
                this.error = error;
            }

            /// <summary>
            /// The target object parsed from the json data.
            /// </summary>
            public T Target
            {
                get { return target; }
            }

            /// <summary>
            /// Returns true if Target is valid.
            /// </summary>
            public bool Success
            {
                get { return success; }
            }

            /// <summary>
            /// Message detailing any errors.
            /// </summary>
            public string Error
            {
                get { return error; }
            }
        }
    }
}
