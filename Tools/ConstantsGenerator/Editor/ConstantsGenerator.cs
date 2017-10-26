using System;
using System.IO;
using System.Text;
using Expanse.Utilities;
using UnityEditor;
using UnityEngine;

namespace Expanse.Tools
{
    public sealed class ConstantsGenerator : ExpanseWindow
    {
        [MenuItem("Expanse/Constants Generator")]
        public static void Initialize()
        {
            GetWindow<ConstantsGenerator>("Constants", true);
        }

        [SerializeField]
        private ConstantsGeneratorPersistentData persistentData;
        [SerializeField]
        private ConstantsGeneratorSessionData sessionData;
        [NonSerialized]
        private ConstantsGeneratorVolatileData volatileData;

        private bool preGUIReloadFlag;

        protected override void Startup()
        {
            persistentData = new ConstantsGeneratorPersistentData();
            sessionData = new ConstantsGeneratorSessionData();
            volatileData = new ConstantsGeneratorVolatileData();

            LoadPersistentEditorData();

            preGUIReloadFlag = true;
        }

        protected override void OnBeforeAssemblyReload()
        {
            SavePersistentEditorData();
        }

        protected override void OnAfterAssemblyReload()
        {
            LoadPersistentEditorData();
            volatileData = new ConstantsGeneratorVolatileData();
            preGUIReloadFlag = true;
        }

        private void SavePersistentEditorData()
        {
            string dataJson = EditorJsonUtility.ToJson(persistentData, true);
            byte[] dataBytes = Encoding.UTF8.GetBytes(dataJson);
            FileInfo dataFileInfo = new FileInfo(PersistentDataPath);

            using (FileStream serializationStream = dataFileInfo.Exists ? dataFileInfo.OpenWrite() : dataFileInfo.Create())
            {
                serializationStream.SetLength(dataBytes.Length);
                serializationStream.Write(dataBytes, 0, dataBytes.Length);
            }
        }

        private void LoadPersistentEditorData()
        {
            FileInfo editorDataFileInfo = new FileInfo(PersistentDataPath);

            if (!editorDataFileInfo.Exists)
                return;

            byte[] editorDataBytes;

            using (FileStream deserializationStream = editorDataFileInfo.OpenRead())
            {
                editorDataBytes = new byte[deserializationStream.Length];
                deserializationStream.Read(editorDataBytes, 0, editorDataBytes.Length);
            }

            string editorDataJson = Encoding.UTF8.GetString(editorDataBytes);

            try
            {
                EditorJsonUtility.FromJsonOverwrite(editorDataJson, persistentData);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                persistentData = new ConstantsGeneratorPersistentData();
            }
        }

        private void OnPreGUIReload()
        {
            volatileData.filePathMessageStyle = new GUIStyle(EditorStyles.helpBox);
            volatileData.filePathMessageStyle.wordWrap = true;

            OnFilePathChanged(persistentData.filePath);

            preGUIReloadFlag = false;
        }

        protected override void OnGUI()
        {
            if (preGUIReloadFlag)
                OnPreGUIReload();

            // Header
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            sessionData.scrollPosition = EditorGUILayout.BeginScrollView(sessionData.scrollPosition);

            // File Path
            {
                string newFilePath = EditorGUILayout.DelayedTextField("File Path", persistentData.filePath);

                if (newFilePath != persistentData.filePath)
                    OnFilePathChanged(newFilePath);

                EditorGUILayout.LabelField(sessionData.filePathMessage, volatileData.filePathMessageStyle);
            }

            // Namespace
            {
                string newNamespace = EditorGUILayout.DelayedTextField("Namespace", persistentData.@namespace);

                if (newNamespace != persistentData.@namespace)
                {
                    persistentData.@namespace = newNamespace;
                }
            }

            // Generate
            {
                if (GUILayout.Button("Generate"))
                {
                    GenerateConstants();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void GenerateConstants()
        {
            Debug.Log("OnGenerateConstants");
        }

        private void OnFilePathChanged(string newFilePath)
        {
            persistentData.filePath = newFilePath.Trim();

            if (string.IsNullOrEmpty(persistentData.filePath))
            {
                DirectoryInfo assetsDirectoryInfo = new DirectoryInfo(Application.dataPath);

                volatileData.filePathMessageStyle.normal.textColor = ColorLibrary.PurplishRed;
                sessionData.filePathMessage = $"{assetsDirectoryInfo.FullName}\nError: File Path is missing or invalid";
            }
            else
            {
                string fixedFilePath = persistentData.filePath;

                if (!fixedFilePath.EndsWith(".cs"))
                    fixedFilePath = fixedFilePath + ".cs";

                string fullFilePath = Path.Combine(Application.dataPath, fixedFilePath);
                FileInfo fullFileInfo = new FileInfo(fullFilePath);

                sessionData.filePathMessage = $"{fullFileInfo.FullName}";

                if (fullFileInfo.Exists)
                    sessionData.filePathMessage += "\nFile exists and will be overwritten";

                DirectoryInfo directoryInfo = fullFileInfo.Directory;

                if (directoryInfo.Exists)
                {
                    volatileData.filePathMessageStyle.normal.textColor = ColorLibrary.Black;
                }
                else
                {
                    volatileData.filePathMessageStyle.normal.textColor = ColorLibrary.GoldenBrown;
                    sessionData.filePathMessage += "\nWarning: Directory does not exist and will be created";
                }
            }
        }

        protected override void Shutdown()
        {
            SavePersistentEditorData();
        }

        private string PersistentDataPath
        {
            get
            {
                MonoScript script = MonoScript.FromScriptableObject(this);
                string assetScriptPath = AssetDatabase.GetAssetPath(script);
                int firstSlashIndex = assetScriptPath.IndexOfAny(new char[] { '/', '\\' });
                assetScriptPath = assetScriptPath.Substring(firstSlashIndex + 1, assetScriptPath.Length - firstSlashIndex - 1);
                string fullScriptPath = Path.Combine(Application.dataPath, assetScriptPath);
                FileInfo fullScriptFileInfo = new FileInfo(fullScriptPath);

                DirectoryInfo scriptDirectoryInfo = fullScriptFileInfo.Directory;
                return Path.Combine(scriptDirectoryInfo.FullName, "ConstantsGenerator.dat");
            }
        }
    }
}
