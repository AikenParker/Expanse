using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Expanse.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Expanse.Tools
{
    public sealed class ConstantsGenerator : ExpanseWindow
    {
        [MenuItem("Expanse/Constants Generator")]
        public static void Initialize()
        {
            GetWindow<ConstantsGenerator>("Constants", true);
        }

        private const float ANIM_SPEED = 0.06f;

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
            OnNamespaceChanged(persistentData.@namespace);
            OnSceneSettingsEnabledChanged(persistentData.sceneSettings.enabled);
            OnLayerSettingsEnabledChanged(persistentData.layerSettings.enabled);
            OnTagSettingsEnabledChanged(persistentData.tagSettings.enabled);
            OnSortingLayerSettingsEnabledChanged(persistentData.sortingLayerSettings.enabled);

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
                    OnNamespaceChanged(newNamespace);
            }

            EditorGUISplitter.SplitterLayout();
            OnSceneSettingsGUI(persistentData.sceneSettings);
            EditorGUISplitter.SplitterLayout();
            OnLayerSettingsGUI(persistentData.layerSettings);
            EditorGUISplitter.SplitterLayout();
            OnTagSettingsGUI(persistentData.tagSettings);
            EditorGUISplitter.SplitterLayout();
            OnSortingLayerSettingsGUI(persistentData.sortingLayerSettings);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUISplitter.SplitterLayout();

            OnFooterGUI();
        }

        private void OnSceneSettingsGUI(ConstantsGeneratorSceneSettings sceneSettings)
        {
            // Scenes enabled
            {
                bool newEnabled;
                EditorGUIFoldoutToggle.ToggleFoldoutLayout(sessionData.sceneSettingsOpen, sceneSettings.enabled, out sessionData.sceneSettingsOpen, out newEnabled, "Scenes");

                if (newEnabled != sceneSettings.enabled)
                    OnSceneSettingsEnabledChanged(newEnabled);
            }

            bool sceneSettingsOpen = EditorGUILayout.BeginFadeGroup(Mathf.SmoothStep(0, 1, sessionData.sceneSettingsFade));
            if (sceneSettingsOpen)
            {
                EditorGUI.indentLevel++;
                EditorUtil.ApplyGUIEnabled(sceneSettings.enabled);

                // Types enabled
                {
                    bool newTypesEnabled = EditorGUILayout.ToggleLeft("Types", sceneSettings.typesEnabled);

                    if (newTypesEnabled != sceneSettings.typesEnabled)
                        sceneSettings.typesEnabled = newTypesEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(sceneSettings.typesEnabled);

                    // Types enum
                    {
                        string newTypesEnum = EditorGUILayout.DelayedTextField("Enum Name", sceneSettings.typesEnum);

                        if (newTypesEnum != sceneSettings.typesEnum)
                            sceneSettings.typesEnum = GetValidIdentifier(newTypesEnum);
                    }

                    // Type naming convention
                    {
                        NamingConvention newTypesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)sceneSettings.typesNamingConvention, volatileData.namingConventionNames);

                        if (newTypesNamingConvention != sceneSettings.typesNamingConvention)
                            sceneSettings.typesNamingConvention = newTypesNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Names enabled
                {
                    bool newNamesEnabled = EditorGUILayout.ToggleLeft("Names", sceneSettings.namesEnabled);

                    if (newNamesEnabled != sceneSettings.namesEnabled)
                        sceneSettings.namesEnabled = newNamesEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(sceneSettings.namesEnabled);

                    // Names class
                    {
                        string newNamesClass = EditorGUILayout.DelayedTextField("Class Name", sceneSettings.namesClass);

                        if (newNamesClass != sceneSettings.namesClass)
                            sceneSettings.namesClass = GetValidIdentifier(newNamesClass);
                    }

                    // Names naming convention
                    {
                        NamingConvention newNamesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)sceneSettings.namesNamingConvention, volatileData.namingConventionNames);

                        if (newNamesNamingConvention != sceneSettings.namesNamingConvention)
                            sceneSettings.namesNamingConvention = newNamesNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Ids enabled
                {
                    bool newIdsEnabled = EditorGUILayout.ToggleLeft("ID Numbers", sceneSettings.idsEnabled);

                    if (newIdsEnabled != sceneSettings.idsEnabled)
                        sceneSettings.idsEnabled = newIdsEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(sceneSettings.idsEnabled);

                    // Ids class
                    {
                        string newIdesClass = EditorGUILayout.DelayedTextField("Class Name", sceneSettings.idsClass);

                        if (newIdesClass != sceneSettings.idsClass)
                            sceneSettings.idsClass = GetValidIdentifier(newIdesClass);
                    }

                    // Ids naming convention
                    {
                        NamingConvention newIdsNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)sceneSettings.idsNamingConvention, volatileData.namingConventionNames);

                        if (newIdsNamingConvention != sceneSettings.idsNamingConvention)
                            sceneSettings.idsNamingConvention = newIdsNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                EditorUtil.RevertGUIEnabled();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void OnLayerSettingsGUI(ConstantsGeneratorLayerSettings layerSettings)
        {
            // Layers enabled
            {
                bool newEnabled;
                EditorGUIFoldoutToggle.ToggleFoldoutLayout(sessionData.layerSettingsOpen, layerSettings.enabled, out sessionData.layerSettingsOpen, out newEnabled, "Layers");

                if (newEnabled != layerSettings.enabled)
                    OnLayerSettingsEnabledChanged(newEnabled);
            }

            bool layerSettingsOpen = EditorGUILayout.BeginFadeGroup(Mathf.SmoothStep(0, 1, sessionData.layerSettingsFade));
            if (layerSettingsOpen)
            {
                EditorGUI.indentLevel++;
                EditorUtil.ApplyGUIEnabled(layerSettings.enabled);

                // Built-in layers
                {
                    bool newIncludeBuiltIn = EditorGUILayout.Toggle("Include Built-In", layerSettings.includeBuiltIn);

                    if (newIncludeBuiltIn != layerSettings.includeBuiltIn)
                        layerSettings.includeBuiltIn = newIncludeBuiltIn;
                }

                // Types enabled
                {
                    bool newTypesEnabled = EditorGUILayout.ToggleLeft("Types", layerSettings.typesEnabled);

                    if (newTypesEnabled != layerSettings.typesEnabled)
                        layerSettings.typesEnabled = newTypesEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(layerSettings.typesEnabled);

                    // Types enum
                    {
                        string newTypesEnum = EditorGUILayout.DelayedTextField("Enum Name", layerSettings.typesEnum);

                        if (newTypesEnum != layerSettings.typesEnum)
                            layerSettings.typesEnum = GetValidIdentifier(newTypesEnum);
                    }

                    // Type naming convention
                    {
                        NamingConvention newTypesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)layerSettings.typesNamingConvention, volatileData.namingConventionNames);

                        if (newTypesNamingConvention != layerSettings.typesNamingConvention)
                            layerSettings.typesNamingConvention = newTypesNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Names enabled
                {
                    bool newNamesEnabled = EditorGUILayout.ToggleLeft("Names", layerSettings.namesEnabled);

                    if (newNamesEnabled != layerSettings.namesEnabled)
                        layerSettings.namesEnabled = newNamesEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(layerSettings.namesEnabled);

                    // Names class
                    {
                        string newNamesClass = EditorGUILayout.DelayedTextField("Class Name", layerSettings.namesClass);

                        if (newNamesClass != layerSettings.namesClass)
                            layerSettings.namesClass = GetValidIdentifier(newNamesClass);
                    }

                    // Names naming convention
                    {
                        NamingConvention newNamesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)layerSettings.namesNamingConvention, volatileData.namingConventionNames);

                        if (newNamesNamingConvention != layerSettings.namesNamingConvention)
                            layerSettings.namesNamingConvention = newNamesNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Ids enabled
                {
                    bool newIdsEnabled = EditorGUILayout.ToggleLeft("ID Numbers", layerSettings.idsEnabled);

                    if (newIdsEnabled != layerSettings.idsEnabled)
                        layerSettings.idsEnabled = newIdsEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(layerSettings.idsEnabled);

                    // Ids class
                    {
                        string newIdesClass = EditorGUILayout.DelayedTextField("Class Name", layerSettings.idsClass);

                        if (newIdesClass != layerSettings.idsClass)
                            layerSettings.idsClass = GetValidIdentifier(newIdesClass);
                    }

                    // Ids naming convention
                    {
                        NamingConvention newIdsNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)layerSettings.idsNamingConvention, volatileData.namingConventionNames);

                        if (newIdsNamingConvention != layerSettings.idsNamingConvention)
                            layerSettings.idsNamingConvention = newIdsNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Masks enabled
                {
                    bool newMasksEnabled = EditorGUILayout.ToggleLeft("Masks", layerSettings.masksEnabled);

                    if (newMasksEnabled != layerSettings.masksEnabled)
                        layerSettings.masksEnabled = newMasksEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(layerSettings.masksEnabled);

                    // Masks enum
                    {
                        string newMasksClass = EditorGUILayout.DelayedTextField("Class Name", layerSettings.masksClass);

                        if (newMasksClass != layerSettings.masksClass)
                            layerSettings.masksClass = GetValidIdentifier(newMasksClass);
                    }

                    // Masks naming convention
                    {
                        NamingConvention newMasksNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)layerSettings.masksNamingConvention, volatileData.namingConventionNames);

                        if (newMasksNamingConvention != layerSettings.masksNamingConvention)
                            layerSettings.masksNamingConvention = newMasksNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                EditorUtil.RevertGUIEnabled();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void OnTagSettingsGUI(ConstantsGeneratorTagSettings tagSettings)
        {
            // Tags enabled
            {
                bool newEnabled;
                EditorGUIFoldoutToggle.ToggleFoldoutLayout(sessionData.tagSettingsOpen, tagSettings.enabled, out sessionData.tagSettingsOpen, out newEnabled, "Tags");

                if (newEnabled != tagSettings.enabled)
                    OnTagSettingsEnabledChanged(newEnabled);
            }

            bool tagSettingsOpen = EditorGUILayout.BeginFadeGroup(Mathf.SmoothStep(0, 1, sessionData.tagSettingsFade));
            if (tagSettingsOpen)
            {
                EditorGUI.indentLevel++;
                EditorUtil.ApplyGUIEnabled(tagSettings.enabled);

                // Built-in layers
                {
                    bool newIncludeBuiltIn = EditorGUILayout.Toggle("Include Built-In", tagSettings.includeBuiltIn);

                    if (newIncludeBuiltIn != tagSettings.includeBuiltIn)
                        tagSettings.includeBuiltIn = newIncludeBuiltIn;
                }

                // Class
                {
                    string newNamesClass = EditorGUILayout.DelayedTextField("Class Name", tagSettings.@class);

                    if (newNamesClass != tagSettings.@class)
                        tagSettings.@class = GetValidIdentifier(newNamesClass);
                }

                // Naming convention
                {
                    NamingConvention newNamesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)tagSettings.namingConvention, volatileData.namingConventionNames);

                    if (newNamesNamingConvention != tagSettings.namingConvention)
                        tagSettings.namingConvention = newNamesNamingConvention;
                }

                EditorUtil.RevertGUIEnabled();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void OnSortingLayerSettingsGUI(ConstantsGeneratorSortingLayerSettings sortingLayerSettings)
        {
            // Sorting layers enabled
            {
                bool newEnabled;
                EditorGUIFoldoutToggle.ToggleFoldoutLayout(sessionData.sortingLayerSettingOpen, sortingLayerSettings.enabled, out sessionData.sortingLayerSettingOpen, out newEnabled, "Sorting Layers");

                if (newEnabled != sortingLayerSettings.enabled)
                    OnSortingLayerSettingsEnabledChanged(newEnabled);
            }

            bool sortingLayerSettingOpen = EditorGUILayout.BeginFadeGroup(Mathf.SmoothStep(0, 1, sessionData.sortingLayerSettingsFade));
            if (sortingLayerSettingOpen)
            {
                EditorGUI.indentLevel++;
                EditorUtil.ApplyGUIEnabled(sortingLayerSettings.enabled);

                // Built-in layers
                {
                    bool newIncludeBuiltIn = EditorGUILayout.Toggle("Include Built-In", sortingLayerSettings.includeBuiltIn);

                    if (newIncludeBuiltIn != sortingLayerSettings.includeBuiltIn)
                        sortingLayerSettings.includeBuiltIn = newIncludeBuiltIn;
                }

                // Types enabled
                {
                    bool newTypesEnabled = EditorGUILayout.ToggleLeft("Types", sortingLayerSettings.typesEnabled);

                    if (newTypesEnabled != sortingLayerSettings.typesEnabled)
                        sortingLayerSettings.typesEnabled = newTypesEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(sortingLayerSettings.typesEnabled);

                    // Types enum
                    {
                        string newTypesEnum = EditorGUILayout.DelayedTextField("Enum Name", sortingLayerSettings.typesEnum);

                        if (newTypesEnum != sortingLayerSettings.typesEnum)
                            sortingLayerSettings.typesEnum = GetValidIdentifier(newTypesEnum);
                    }

                    // Type naming convention
                    {
                        NamingConvention newTypesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)sortingLayerSettings.typesNamingConvention, volatileData.namingConventionNames);

                        if (newTypesNamingConvention != sortingLayerSettings.typesNamingConvention)
                            sortingLayerSettings.typesNamingConvention = newTypesNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Names enabled
                {
                    bool newNamesEnabled = EditorGUILayout.ToggleLeft("Names", sortingLayerSettings.namesEnabled);

                    if (newNamesEnabled != sortingLayerSettings.namesEnabled)
                        sortingLayerSettings.namesEnabled = newNamesEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(sortingLayerSettings.namesEnabled);

                    // Names class
                    {
                        string newNamesClass = EditorGUILayout.DelayedTextField("Class Name", sortingLayerSettings.namesClass);

                        if (newNamesClass != sortingLayerSettings.namesClass)
                            sortingLayerSettings.namesClass = GetValidIdentifier(newNamesClass);
                    }

                    // Names naming convention
                    {
                        NamingConvention newNamesNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)sortingLayerSettings.namesNamingConvention, volatileData.namingConventionNames);

                        if (newNamesNamingConvention != sortingLayerSettings.namesNamingConvention)
                            sortingLayerSettings.namesNamingConvention = newNamesNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                // Ids enabled
                {
                    bool newIdsEnabled = EditorGUILayout.ToggleLeft("ID Numbers", sortingLayerSettings.idsEnabled);

                    if (newIdsEnabled != sortingLayerSettings.idsEnabled)
                        sortingLayerSettings.idsEnabled = newIdsEnabled;
                }

                {
                    EditorGUI.indentLevel++;
                    EditorUtil.ApplyGUIEnabled(sortingLayerSettings.idsEnabled);

                    // Ids class
                    {
                        string newIdesClass = EditorGUILayout.DelayedTextField("Class Name", sortingLayerSettings.idsClass);

                        if (newIdesClass != sortingLayerSettings.idsClass)
                            sortingLayerSettings.idsClass = GetValidIdentifier(newIdesClass);
                    }

                    // Ids naming convention
                    {
                        NamingConvention newIdsNamingConvention = (NamingConvention)EditorGUILayout.Popup("Naming Convention", (int)sortingLayerSettings.idsNamingConvention, volatileData.namingConventionNames);

                        if (newIdsNamingConvention != sortingLayerSettings.idsNamingConvention)
                            sortingLayerSettings.idsNamingConvention = newIdsNamingConvention;
                    }

                    EditorUtil.RevertGUIEnabled();
                    EditorGUI.indentLevel--;
                }

                EditorUtil.RevertGUIEnabled();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void OnFooterGUI()
        {
            GUILayout.BeginHorizontal();

            // Clear button
            {
                bool clearEnabled = true;

                if (string.IsNullOrEmpty(persistentData.filePath))
                {
                    clearEnabled = false;
                }
                else
                {
                    string fixedFilePath = GetFixedFilePath(persistentData.filePath);
                    FileInfo fileInfo = new FileInfo(fixedFilePath);

                    clearEnabled = fileInfo.Exists;
                }

                EditorUtil.ApplyGUIEnabled(clearEnabled);
                if (GUILayout.Button("Clear", GUILayout.Height(20)))
                {
                    ClearConstants();
                }
                EditorUtil.RevertGUIEnabled();
            }

            // Generate button
            {
                bool generateEnabled = true;

                if (string.IsNullOrEmpty(persistentData.filePath))
                {
                    generateEnabled = false;
                }

                EditorUtil.ApplyGUIEnabled(generateEnabled);
                if (GUILayout.Button("Generate", GUILayout.Height(20)))
                {
                    GenerateConstants();
                }
                EditorUtil.RevertGUIEnabled();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(8);
        }

        void Update()
        {
            bool repaint = false;

            if ((sessionData.sceneSettingsFade != 0f && sessionData.sceneSettingsFade != 1f) ||
                (sessionData.layerSettingsFade != 0f && sessionData.layerSettingsFade != 1f) ||
                (sessionData.tagSettingsFade != 0f && sessionData.tagSettingsFade != 1f) ||
                (sessionData.sortingLayerSettingsFade != 0f && sessionData.sortingLayerSettingsFade != 1f))
                repaint = true;

            sessionData.sceneSettingsFade = Mathf.Clamp01(sessionData.sceneSettingsFade + (ANIM_SPEED * (sessionData.sceneSettingsOpen ? 1 : -1)));
            sessionData.layerSettingsFade = Mathf.Clamp01(sessionData.layerSettingsFade + (ANIM_SPEED * (sessionData.layerSettingsOpen ? 1 : -1)));
            sessionData.tagSettingsFade = Mathf.Clamp01(sessionData.tagSettingsFade + (ANIM_SPEED * (sessionData.tagSettingsOpen ? 1 : -1)));
            sessionData.sortingLayerSettingsFade = Mathf.Clamp01(sessionData.sortingLayerSettingsFade + (ANIM_SPEED * (sessionData.sortingLayerSettingOpen ? 1 : -1)));

            if (repaint)
                Repaint();
        }

        private void ClearConstants()
        {
            string fixedFilePath = GetFixedFilePath(persistentData.filePath);
            FileInfo fileInfo = new FileInfo(fixedFilePath);

            if (EditorUtility.DisplayDialog("Delete File", string.Format("Are you sure you want to delete the file \"{0}\"?", fileInfo.FullName), "Delete", "Cancel"))
            {
                fileInfo.Delete();

                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        private void GenerateConstants()
        {
            string fixedFilePath = GetFixedFilePath(persistentData.filePath);
            FileInfo fileInfo = new FileInfo(fixedFilePath);

            if (persistentData.sceneSettings.enabled)
            {
                int sceneCount = SceneManager.sceneCountInBuildSettings;
                Scene[] scenes = new Scene[sceneCount];

                for (int i = 0; i < sceneCount; i++)
                {
                    scenes[i] = SceneManager.GetSceneByBuildIndex(i);
                }
            }

            if (persistentData.layerSettings.enabled)
            {
                List<int> layerMaskIdList = new List<int>(1 << 5);

                for (int i = 0; i < 1 << 5; i++)
                {
                    string layerName = LayerMask.LayerToName(i);

                    if (string.IsNullOrEmpty(layerName))
                        continue;

                    layerMaskIdList.Add(i);
                }
            }

            if (persistentData.tagSettings.enabled)
            {
                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            }

            if (persistentData.sortingLayerSettings.enabled)
            {
                SortingLayer[] sortingLayers = SortingLayer.layers;
            }
        }

        private void OnFilePathChanged(string newFilePath)
        {
            persistentData.filePath = newFilePath.Trim();

            if (string.IsNullOrEmpty(persistentData.filePath))
            {
                DirectoryInfo assetsDirectoryInfo = new DirectoryInfo(Application.dataPath);

                volatileData.filePathMessageStyle.normal.textColor = ColorLibrary.PurplishRed;
                sessionData.filePathMessage = assetsDirectoryInfo.FullName + "\nError: File Path is missing or invalid";
            }
            else
            {
                string fixedFilePath = GetFixedFilePath(persistentData.filePath);

                sessionData.filePathMessage = fixedFilePath;
                FileInfo fileInfo = new FileInfo(fixedFilePath);

                if (fileInfo.Exists)
                    sessionData.filePathMessage += "\nFile exists and will be overwritten";

                DirectoryInfo directoryInfo = fileInfo.Directory;

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

        private void OnNamespaceChanged(string newNamespace)
        {
            persistentData.@namespace = newNamespace.Trim();

            if (!string.IsNullOrEmpty(persistentData.@namespace))
            {
                List<string> splitNamespace = persistentData.@namespace.Split('.').ToList();
                bool hasChanged = false;

                for (int i = splitNamespace.Count - 1; i >= 0; i--)
                {
                    string name = splitNamespace[i];
                    int nameLength = name.Length;

                    if (nameLength == 0)
                    {
                        hasChanged = true;
                        splitNamespace.RemoveAt(i);
                        continue;
                    }

                    unsafe
                    {
                        bool hitAlpha = false;
                        char* fixedNamePtr = stackalloc char[nameLength];
                        int fixedLength = 0;

                        fixed (char* namePtr = name)
                        {
                            for (int j = 0; j < nameLength; j++)
                            {
                                char c = namePtr[j];

                                bool isAlpha = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');
                                bool isDigit = !isAlpha && (c >= '0' && c <= '9');
                                bool isAt = c == '@';

                                bool isValid = isAlpha || (isDigit && hitAlpha) || (isAt && fixedLength == 0);

                                if (isValid)
                                    fixedNamePtr[fixedLength++] = c;

                                if (isAlpha)
                                    hitAlpha = true;
                            }
                        }

                        if (fixedLength < nameLength)
                        {
                            hasChanged = true;

                            if (fixedLength == 0)
                            {
                                splitNamespace.RemoveAt(i);
                            }
                            else
                            {
                                string fixedName = new string(fixedNamePtr);
                                splitNamespace[i] = fixedName;
                            }
                        }
                    }
                }

                if (hasChanged)
                {
                    persistentData.@namespace = string.Join(".", splitNamespace.ToArray());
                }
            }
        }

        private void OnSceneSettingsEnabledChanged(bool newSceneSettingsEnabled)
        {
            persistentData.sceneSettings.enabled = newSceneSettingsEnabled;

            if (newSceneSettingsEnabled)
                sessionData.sceneSettingsOpen = true;
        }

        private void OnLayerSettingsEnabledChanged(bool newLayerSettingsEnabled)
        {
            persistentData.layerSettings.enabled = newLayerSettingsEnabled;

            if (newLayerSettingsEnabled)
                sessionData.layerSettingsOpen = true;
        }

        private void OnTagSettingsEnabledChanged(bool newTagSettingsEnabled)
        {
            persistentData.tagSettings.enabled = newTagSettingsEnabled;

            if (newTagSettingsEnabled)
                sessionData.tagSettingsOpen = true;
        }

        private void OnSortingLayerSettingsEnabledChanged(bool newSortingLayerSettingsEnabled)
        {
            persistentData.sortingLayerSettings.enabled = newSortingLayerSettingsEnabled;

            if (newSortingLayerSettingsEnabled)
                sessionData.sortingLayerSettingOpen = true;
        }

        protected override void Shutdown()
        {
            SavePersistentEditorData();
        }

        private static string GetFixedFilePath(string rawFilePath)
        {
            if (!rawFilePath.EndsWith(".cs"))
                rawFilePath = rawFilePath + ".cs";

            bool containsColon = rawFilePath.Contains(':');
            string fullFilePath = containsColon ? rawFilePath : Path.Combine(Application.dataPath, rawFilePath);
            FileInfo fullFileInfo = new FileInfo(fullFilePath);

            return fullFileInfo.FullName;
        }

        private unsafe static string GetValidIdentifier(string identifier)
        {
            int identifierLength = string.IsNullOrEmpty(identifier) ? 0 : identifier.Length;

            if (identifierLength == 0)
                return string.Empty;

            bool hitAlpha = false;
            char* validIdentifierPtr = stackalloc char[identifierLength];
            int fixedLength = 0;

            fixed (char* identifierPtr = identifier)
            {
                for (int j = 0; j < identifierLength; j++)
                {
                    char c = identifierPtr[j];

                    bool isAlpha = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');
                    bool isDigit = !isAlpha && (c >= '0' && c <= '9');
                    bool isAt = c == '@';

                    bool isValid = isAlpha || (isDigit && hitAlpha) || (isAt && fixedLength == 0);

                    if (isValid)
                        validIdentifierPtr[fixedLength++] = c;

                    if (isAlpha)
                        hitAlpha = true;
                }
            }

            if (fixedLength < identifierLength)
                return fixedLength == 0 ? string.Empty : new string(validIdentifierPtr);
            else
                return identifier;
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

        public static unsafe char* Ptr { get; private set; }
    }
}
