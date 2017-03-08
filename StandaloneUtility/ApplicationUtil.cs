using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace Expanse
{
    /// <summary>
    /// A collection of application level related utility functionality.
    /// </summary>
    public static class ApplicationUtil
    {
        private static readonly string streamingAssetsPath;
        private static readonly string persistentDataPath;

        /// <summary>
        /// Contains the path to the streaming assets folder.
        /// </summary>
        public static string StreamingAssestsPath
        {
            get { return streamingAssetsPath; }
        }

        /// <summary>
        /// Contains the path to the persistent data folder.
        /// </summary>
        public static string PersistentDataPath
        {
            get { return persistentDataPath; }
        }

        static ApplicationUtil()
        {
            streamingAssetsPath = UnityEngine.Application.streamingAssetsPath;
            persistentDataPath = UnityEngine.Application.persistentDataPath;
        }

        /// <summary>
        /// Exits the application or stops playing in editor.
        /// </summary>
        private static void Exit()
        {
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        /// <summary>
        /// Loads an object from a file.
        /// </summary>
        public static FileLoadInfo<T> LoadFromFile<T>(string filePath, ISerializer serializer = null) where T : new()
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileLoadInfo<T> loadInfo = new FileLoadInfo<T>()
            {
                Target = default(T),
                FilePath = filePath,
                Success = false,
                Error = "Unhandled error"
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
                        loadInfo.Target = target;
                        loadInfo.Success = true;
                        loadInfo.Error = "None";
                        loadInfo.FilePath = fileInfo.FullName;

                    }
                    else
                        loadInfo.Error = "File at path is unable to be parsed: " + filePath;
                }
                else
                    loadInfo.Error = "File at path does not contain any data: " + filePath;
            }
            else
                loadInfo.Error = "File at path does not exist: " + filePath;

            return loadInfo;
        }

        /// <summary>
        /// Loads an object from a file using another thread.
        /// </summary>
        public static IEnumerator Co_LoadFromFileThreaded<T>(string filePath, Action<FileLoadInfo<T>> onComplete, ISerializer serializer = null, ThreadPriority priority = ThreadPriority.Normal) where T : new()
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileLoadInfo<T> loadInfo = new FileLoadInfo<T>()
            {
                Target = default(T),
                FilePath = filePath,
                Success = false,
                Error = "Unhandled error"
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
                            loadInfo.Target = target;
                            loadInfo.Success = true;
                            loadInfo.Error = "None";
                            loadInfo.FilePath = fileInfo.FullName;

                        }
                        else
                            loadInfo.Error = "File at path is unable to be parsed: " + filePath;
                    }
                    else
                        loadInfo.Error = "File at path does not contain any data: " + filePath;
                }
                else
                    loadInfo.Error = "File at path does not exist: " + filePath;

                hasLoaded = true;

            }).Start(priority);

            while (!hasLoaded)
            {
                yield return null;
            }

            onComplete.SafeInvoke(loadInfo);
        }

        /// <summary>
        /// Saves an object to a file.
        /// </summary>
        public static FileSaveInfo SaveToFile(object obj, string filePath, ISerializer serializer = null)
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileSaveInfo saveInfo = new FileSaveInfo()
            {
                Target = obj,
                FilePath = filePath,
                Success = false,
                Error = "Unhandled error"
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

                saveInfo.Success = true;
                saveInfo.Error = "None";
                saveInfo.FilePath = fileInfo.FullName;
            }
            else
            {
                saveInfo.Error = "Directory at path does not exist: " + filePath;
            }

            return saveInfo;
        }

        /// <summary>
        /// Saves an object to a file using another thread.
        /// </summary>
        public static IEnumerator Co_SaveToFileThreaded(object obj, string filePath, Action<FileSaveInfo> onComplete = null, ISerializer serializer = null, ThreadPriority priority = ThreadPriority.Normal)
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            FileSaveInfo saveInfo = new FileSaveInfo()
            {
                Target = obj,
                FilePath = filePath,
                Success = false,
                Error = "Unhandled error"
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

                    saveInfo.Success = true;
                    saveInfo.Error = "None";
                    saveInfo.FilePath = fileInfo.FullName;
                }
                else
                {
                    saveInfo.Error = "Directory at path does not exist: " + filePath;
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
        /// Info class that contains the results of a file load action.
        /// </summary>
        public struct FileLoadInfo<T> where T : new()
        {
            private T target;
            private string filePath;
            private bool success;
            private string error;

            /// <summary>
            /// The target object parsed from the file data.
            /// </summary>
            public T Target
            {
                get { return target; }
                internal set { target = value; }
            }

            /// <summary>
            /// The full file path where the object was loaded from.
            /// </summary>
            public string FilePath
            {
                get { return filePath; }
                internal set { filePath = value; }
            }

            /// <summary>
            /// Returns true if Target is valid.
            /// </summary>
            public bool Success
            {
                get { return success; }
                internal set { success = value; }
            }

            /// <summary>
            /// Message detailing any errors.
            /// </summary>
            public string Error
            {
                get { return error; }
                internal set { error = value; }
            }
        }

        /// <summary>
        /// Info class that contains the results of a file save action.
        /// </summary>
        public struct FileSaveInfo
        {
            private object target;
            private string filePath;
            private bool success;
            private string error;

            /// <summary>
            /// The target object saved into a file.
            /// </summary>
            public object Target
            {
                get { return target; }
                internal set { target = value; }
            }

            /// <summary>
            /// The full file path where the object was saved.
            /// </summary>
            public string FilePath
            {
                get { return filePath; }
                internal set { filePath = value; }
            }

            /// <summary>
            /// Returns true if Target is valid.
            /// </summary>
            public bool Success
            {
                get { return success; }
                internal set { success = value; }
            }

            /// <summary>
            /// Message detailing any errors.
            /// </summary>
            public string Error
            {
                get { return error; }
                internal set { error = value; }
            }
        }
    }
}
