using System;
using System.Collections;
using System.IO;
using System.Threading;
using Expanse.Extensions;

namespace Expanse.Utilities
{
    /* TODO
     * Add support for any ISerializer instead of just IStringSerializer
     * Allow encoding specification if using an IStringSerializer
     */

    /// <summary>
    /// A collection of application level related utility functionality.
    /// </summary>
    public static class ApplicationUtil
    {
        // Cached read-only Application values
        // Use these to reduce long Engine calls
        public static readonly string dataPath;
        public static readonly string streamingAssetsPath;
        public static readonly string persistentDataPath;
        public static readonly string temporaryCachePath;

        static ApplicationUtil()
        {
            streamingAssetsPath = UnityEngine.Application.streamingAssetsPath;
            persistentDataPath = UnityEngine.Application.persistentDataPath;
            dataPath = UnityEngine.Application.dataPath;
            temporaryCachePath = UnityEngine.Application.temporaryCachePath;
        }

        /// <summary>
        /// Exits the application or stops playing in editor.
        /// </summary>
        private static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        /// <summary>
        /// Loads an object immediately from a file using a deserializer.
        /// </summary>
        /// <typeparam name="T">Type of object to load file data into.</typeparam>
        /// <param name="filePath">File location of the file to load from.</param>
        /// <param name="serializer">Deserializes the data in the file into an object. (Default = UnityJsonUtilitySerializer)</param>
        /// <returns>Data structure that contains info regarding the results of the load.</returns>
        public static FileLoadInfo<T> LoadFromFile<T>(string filePath, IStringSerializer serializer = null) where T : new()
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileLoadInfo<T> loadInfo = new FileLoadInfo<T>()
            {
                target = default(T),
                filePath = filePath,
                success = false,
                error = "Unhandled error"
            };

            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                string contents = null;

                using (FileStream fileStream = fileInfo.OpenRead())
                {
                    int size = (int)fileStream.Length;

                    byte[] data = new byte[size];

                    fileStream.Read(data, 0, size);

                    contents = System.Text.Encoding.Default.GetString(data);
                }

                if (!string.IsNullOrEmpty(contents))
                {
                    T target = serializer.Deserialize<T>(contents);

                    if (target != null)
                    {
                        loadInfo.target = target;
                        loadInfo.success = true;
                        loadInfo.error = "None";
                        loadInfo.filePath = fileInfo.FullName;

                    }
                    else
                        loadInfo.error = "File at path is unable to be parsed: " + filePath;
                }
                else
                    loadInfo.error = "File at path does not contain any data: " + filePath;
            }
            else
                loadInfo.error = "File at path does not exist: " + filePath;

            return loadInfo;
        }

        /// <summary>
        /// Loads an object in another thread from a file using a deserializer.
        /// </summary>
        /// <typeparam name="T">Type of object to load file data into.</typeparam>
        /// <param name="filePath">File location of the file to load from.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Deserializes the data in the file into an object. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the loading executes on.</param>
        /// <returns>Data structure that contains info regarding the results of the load.</returns>
        public static IEnumerator Co_LoadFromFileThreaded<T>(string filePath, Action<FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, ThreadPriority priority = ThreadPriority.Normal) where T : new()
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileLoadInfo<T> loadInfo = new FileLoadInfo<T>()
            {
                target = default(T),
                filePath = filePath,
                success = false,
                error = "Unhandled error"
            };

            bool hasLoaded = false;

            new Thread(() =>
            {
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists)
                {
                    string contents = null;

                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                        int size = (int)fileStream.Length;

                        byte[] data = new byte[size];

                        fileStream.Read(data, 0, size);

                        contents = System.Text.Encoding.Default.GetString(data);
                    }

                    if (!string.IsNullOrEmpty(contents))
                    {
                        T target = serializer.Deserialize<T>(contents);

                        if (target != null)
                        {
                            loadInfo.target = target;
                            loadInfo.success = true;
                            loadInfo.error = "None";
                            loadInfo.filePath = fileInfo.FullName;

                        }
                        else
                            loadInfo.error = "File at path is unable to be parsed: " + filePath;
                    }
                    else
                        loadInfo.error = "File at path does not contain any data: " + filePath;
                }
                else
                    loadInfo.error = "File at path does not exist: " + filePath;

                hasLoaded = true;

            }).Start(priority);

            while (!hasLoaded)
            {
                yield return null;
            }

            onComplete.SafeInvoke(loadInfo);
        }

        /// <summary>
        /// Saves an object immediately into a file using a serializer.
        /// </summary>
        /// <param name="obj">Object instance to save.</param>
        /// <param name="filePath">File location to save the object into.</param>
        /// <param name="serializer">Serializes the object into a data format that can be saved. (Default = UnityJsonUtilitySerializer)</param>
        /// <returns>Data structure that contains info regarding the results of the save.</returns>
        public static FileSaveInfo SaveToFile(object obj, string filePath, IStringSerializer serializer = null)
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileSaveInfo saveInfo = new FileSaveInfo()
            {
                target = obj,
                filePath = filePath,
                success = false,
                error = "Unhandled error"
            };

            FileInfo fileInfo = new FileInfo(filePath);

            bool fileExists = fileInfo.Exists;

            if (fileExists || fileInfo.Directory.Exists)
            {
                string contents = serializer.Serialize(obj);

                using (FileStream fileStream = fileExists ? fileInfo.OpenWrite() : fileInfo.Create())
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(contents);

                    fileStream.Write(data, 0, data.Length);
                }

                saveInfo.success = true;
                saveInfo.error = "None";
                saveInfo.filePath = fileInfo.FullName;
            }
            else
            {
                saveInfo.error = "Directory at path does not exist: " + filePath;
            }

            return saveInfo;
        }

        /// <summary>
        /// Saves an object in another thread into a file using a serializer.
        /// </summary>
        /// <param name="obj">Object instance to save.</param>
        /// <param name="filePath">File location to save the object into.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Serializes the object into a data format that can be saved. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the saving executes on.</param>
        /// <returns>Data structure that contains info regarding the results of the save.</returns>
        public static IEnumerator Co_SaveToFileThreaded(object obj, string filePath, Action<FileSaveInfo> onComplete = null, IStringSerializer serializer = null, ThreadPriority priority = ThreadPriority.Normal)
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileSaveInfo saveInfo = new FileSaveInfo()
            {
                target = obj,
                filePath = filePath,
                success = false,
                error = "Unhandled error"
            };

            bool hasSaved = false;

            new Thread(() =>
            {
                FileInfo fileInfo = new FileInfo(filePath);

                bool fileExists = fileInfo.Exists;

                if (fileExists || fileInfo.Directory.Exists)
                {
                    string contents = serializer.Serialize(obj);

                    using (FileStream fileStream = fileExists ? fileInfo.OpenWrite() : fileInfo.Create())
                    {
                        byte[] data = System.Text.Encoding.Default.GetBytes(contents);

                        fileStream.Write(data, 0, data.Length);
                    }

                    saveInfo.success = true;
                    saveInfo.error = "None";
                    saveInfo.filePath = fileInfo.FullName;
                }
                else
                {
                    saveInfo.error = "Directory at path does not exist: " + filePath;
                }

                hasSaved = true;

            }).Start(priority);

            while (!hasSaved)
            {
                yield return null;
            }

            onComplete.SafeInvoke(saveInfo);
        }

        /// <summary>
        /// Info data structure that contains the results of a file load action.
        /// </summary>
        public struct FileLoadInfo<T> where T : new()
        {
            /// <summary>
            /// The target object parsed from the file data.
            /// </summary>
            public T target;

            /// <summary>
            /// The full file path where the object was loaded from.
            /// </summary>
            public string filePath;

            /// <summary>
            /// Returns true if Target is valid.
            /// </summary>
            public bool success;

            /// <summary>
            /// Message detailing any errors.
            /// </summary>
            public string error;
        }

        /// <summary>
        /// Info data structure that contains the results of a file save action.
        /// </summary>
        public struct FileSaveInfo
        {
            /// <summary>
            /// The target object saved into a file.
            /// </summary>
            public object target;

            /// <summary>
            /// The full file path where the object was saved.
            /// </summary>
            public string filePath;

            /// <summary>
            /// Is true if Target is valid.
            /// </summary>
            public bool success;

            /// <summary>
            /// Message detailing any errors.
            /// </summary>
            public string error;
        }
    }
}
