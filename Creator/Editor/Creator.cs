using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Expanse
{
    public class Creator : ExpanseWindow
    {
        const string DISPLAY_NAME = "Creator";
        const string NONE_TYPE = "[NONE]";
        const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        const string SCRIPTOBJ_DEFAULT_SAVELOCATION = @"Assets/";
        static Type SCRIPTOBJ_TYPE = typeof(ScriptableObject);

        protected override string DisplayName
        {
            get
            {
                return DISPLAY_NAME;
            }
        }
        protected override string Tooltip
        {
            get
            {
                return "Easily create scriptable object assets";
            }
        }

        ScriptObjCreatorData ScriptObjData { get; set; }
        Vector2 scriptObjFilterScrollPosition;

        Dictionary<Type, List<ConstructorInfo>> constructorCache;
        object[] constructorParams;

        [MenuItem("Window/" + DISPLAY_NAME)]
        static void Create()
        {
            Creator window = GetWindow<Creator>();

            window.Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();

            ScriptObjData = ScriptObjCreatorData.LoadData();
            constructorCache = new Dictionary<Type, List<ConstructorInfo>>();

            RefreshTypes();
        }

        protected override void OnEnabled()
        {
            if (ScriptObjData == null)
            {
                ScriptObjData = ScriptObjCreatorData.LoadData();

                RefreshTypes();
            }
        }

        protected override void OnDrawContent()
        {
            if (ScriptObjData.IsDirty)
                ScriptObjData.InitializeLists();

            DrawHeading("FILTER");

            GUILayout.BeginVertical();
            ScriptObjData.showModules = EditorGUILayout.Foldout(ScriptObjData.showModules, "Module Filter", EditorStyles.foldout);
            if (ScriptObjData.showModules)
                ScriptObjData.modulesList.Draw();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            ScriptObjData.showNamespaces = EditorGUILayout.Foldout(ScriptObjData.showNamespaces, "Namespace Filter", EditorStyles.foldout);
            if (ScriptObjData.showNamespaces)
                ScriptObjData.namespacesList.Draw();
            GUILayout.EndVertical();

            var parentOptions = ScriptObjData.GetParentOptions();
            int parentIndex = EditorGUILayout.Popup("Parent Filter", ScriptObjData.GetSelectedParentIndex(parentOptions), parentOptions);
            ScriptObjData.SetSelectedParentIndex(parentOptions, parentIndex);

            DrawHeading("CREATE");

            var typeOptions = ScriptObjData.GetTypeOptions();
            int typeIndex = EditorGUILayout.Popup("Type", ScriptObjData.GetSelectedTypeIndex(typeOptions), typeOptions);
            ScriptObjData.SetSelectedTypeIndex(typeOptions, typeIndex);

            Type selectedType = ScriptObjData.GetSelectedType();
            ConstructorInfo selectedConstructor = null;

            if (selectedType != null)
            {
                var constructors = GetConstructors(selectedType);

                if (!constructors.HasIndexValue(ScriptObjData.selectedConstructorIndex))
                    ScriptObjData.selectedConstructorIndex = 0;

                selectedConstructor = ListExt.SafeGet(constructors, ScriptObjData.selectedConstructorIndex);

                if (selectedConstructor != null)
                {
                    string[] constructorOptions = constructors.Select(x => GetParameterSignatureDisplay(x)).ToArray();
                    int result = EditorGUILayout.Popup("Constructor", ScriptObjData.selectedConstructorIndex, constructorOptions);

                    if (result != ScriptObjData.selectedConstructorIndex)
                    {
                        ScriptObjData.selectedConstructorIndex = result;

                        selectedConstructor = ListExt.SafeGet(constructors, ScriptObjData.selectedConstructorIndex);

                        SetConstructorParameterDefaults(selectedConstructor);
                    }

                    ShowConstructorFields(selectedConstructor);
                }
                else ShowEmptyConstructorPopup();
            }
            else ShowEmptyConstructorPopup();

            GUI.enabled = selectedType != null && selectedConstructor != null;
            if (GUILayout.Button("Create", GUILayout.MinHeight(30)))
            {
                OnCreateClicked(selectedType);
            }
            GUI.enabled = true;
        }

        private void SetConstructorParameterDefaults(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            constructorParams = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i].IsOptional)
                    constructorParams[i] = parameters[i].DefaultValue;
                else
                    constructorParams[i] = GetDefaultTypeValue(parameters[i].ParameterType);
        }

        private object GetDefaultTypeValue(Type type)
        {
            if (type == typeof(Color))
                return Color.white;
            else if (type.IsValueType)
                return Activator.CreateInstance(type);
            else if (type == typeof(AnimationCurve))
                return AnimationCurve.Linear(0, 0, 1, 1);
            return null;
        }

        private bool AreParametersInvalid(ParameterInfo[] parameters)
        {
            if (constructorParams == null)
                return true;
            else if (constructorParams.Length != parameters.Length)
                return true;
            else
            {
                for (int i = 0; i < constructorParams.Length; i++)
                {
                    if (constructorParams[i] == null)
                        if (parameters[i].ParameterType.IsValueType)
                            return true;
                        else continue;
                    else if (!parameters[i].ParameterType.IsAssignableFrom(constructorParams[i].GetType()))
                        return true;
                }
                return false;
            }
        }

        private void ShowConstructorFields(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();

            if (AreParametersInvalid(parameters))
                SetConstructorParameterDefaults(constructorInfo);

            for (int i = 0; i < constructorParams.Length; i++)
            {
                if (i == 0) CustomGUI.SplitterLayout();

                var parameter = parameters[i];

                if (parameter.ParameterType == typeof(int))
                    constructorParams[i] = EditorGUILayout.IntField(parameter.Name, (int)constructorParams[i]);
                else if (parameter.ParameterType == typeof(float))
                    constructorParams[i] = EditorGUILayout.FloatField(parameter.Name, (float)constructorParams[i]);
                else if (parameter.ParameterType == typeof(bool))
                    constructorParams[i] = EditorGUILayout.Toggle(parameter.Name, (bool)constructorParams[i]);
                else if (parameter.ParameterType == typeof(string))
                    constructorParams[i] = EditorGUILayout.TextField(parameter.Name, (string)constructorParams[i]);
                else if (typeof(UnityEngine.Object).IsAssignableFrom(parameter.ParameterType))
                {
                    try { constructorParams[i] = EditorGUILayout.ObjectField(parameter.Name, (UnityEngine.Object)constructorParams[i], parameter.ParameterType, false); }
                    catch (ExitGUIException e) { e.ToString(); }
                }
                else if (parameter.ParameterType == typeof(double))
                    constructorParams[i] = EditorGUILayout.DoubleField(parameter.Name, (double)constructorParams[i]);
                else if (parameter.ParameterType == typeof(long))
                    constructorParams[i] = EditorGUILayout.LongField(parameter.Name, (long)constructorParams[i]);
                else if (parameter.ParameterType == typeof(short))
                    constructorParams[i] = (short)EditorGUILayout.IntField(parameter.Name, (int)((short)constructorParams[i]));
                else if (parameter.ParameterType == typeof(Vector2))
                    constructorParams[i] = EditorGUILayout.Vector2Field(parameter.Name, (Vector2)constructorParams[i]);
                else if (parameter.ParameterType == typeof(Vector3))
                    constructorParams[i] = EditorGUILayout.Vector3Field(parameter.Name, (Vector3)constructorParams[i]);
                else if (parameter.ParameterType == typeof(Vector4))
                    constructorParams[i] = EditorGUILayout.Vector4Field(parameter.Name, (Vector4)constructorParams[i]);
                else if (parameter.ParameterType == typeof(Color))
                {
                    try { constructorParams[i] = EditorGUILayout.ColorField(parameter.Name, (Color)constructorParams[i]); }
                    catch (ExitGUIException e) { e.ToString(); }
                }
                else if (parameter.ParameterType == typeof(Rect))
                    constructorParams[i] = EditorGUILayout.RectField(parameter.Name, (Rect)constructorParams[i]);
                else if (parameter.ParameterType == typeof(Bounds))
                    constructorParams[i] = EditorGUILayout.BoundsField(parameter.Name, (Bounds)constructorParams[i]);
                else if (parameter.ParameterType == typeof(LayerMask))
                    constructorParams[i] = EditorGUILayout.LayerField(parameter.Name, (LayerMask)constructorParams[i]);
                else if (parameter.ParameterType == typeof(AnimationCurve))
                {
                    try { constructorParams[i] = EditorGUILayout.CurveField(parameter.Name, (AnimationCurve)constructorParams[i]); }
                    catch (ExitGUIException e) { e.ToString(); }
                }
                else if (parameter.ParameterType.IsEnum)
                    constructorParams[i] = EditorGUILayout.EnumPopup(parameter.Name, (Enum)constructorParams[i]);
                else if (parameter.ParameterType == typeof(short))
                    constructorParams[i] = (short)EditorGUILayout.IntField(parameter.Name, (int)((short)constructorParams[i]));
                else if (parameter.ParameterType == typeof(uint))
                    constructorParams[i] = (uint)EditorGUILayout.LongField(parameter.Name, (long)((uint)constructorParams[i]));
                else if (parameter.ParameterType == typeof(ushort))
                    constructorParams[i] = (ushort)EditorGUILayout.IntField(parameter.Name, (int)((ushort)constructorParams[i]));
                else if (parameter.ParameterType == typeof(ulong))
                    constructorParams[i] = (ulong)EditorGUILayout.LongField(parameter.Name, (long)((ulong)constructorParams[i]));
                else
                {
                    GUI.enabled = false;
                    string displayName = constructorParams[i] == null ? "None" : constructorParams[i].ToString();
                    EditorGUILayout.TextField(parameter.Name, string.Format("{0} ({1})", displayName, parameter.ParameterType.Name.AddSpaces()));
                    GUI.enabled = true;
                }

                if (i == constructorParams.Length - 1) CustomGUI.SplitterLayout();
            }
        }

        private void ShowEmptyConstructorPopup()
        {
            EditorGUILayout.Popup("Constructor", 0, new string[] { NONE_TYPE });
            constructorParams = null;
        }

        private string GetParameterSignatureDisplay(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();

            if (!parameters.Any())
                return "Default";
            else
            {
                StringBuilder sb = new StringBuilder(parameters.Length + 2);
                sb.Append("(");
                bool first = true;

                foreach (var parameter in parameters)
                {
                    if (first)
                    {
                        sb.Append(string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name));
                        first = false;
                    }
                    else
                    {
                        sb.Append(string.Format(", {0} {1}", parameter.ParameterType.Name, parameter.Name));
                    }
                }

                sb.Append(")");
                return sb.ToString();
            }
        }

        private List<ConstructorInfo> GetConstructors(Type type)
        {
            if (constructorCache.ContainsKey(type))
            {
                return constructorCache[type];
            }
            else
            {
                var constructors = type.GetConstructors(BINDING_FLAGS)
                    .Where(x => !x.ContainsGenericParameters && !x.IsGenericMethod)
                    .OrderBy(x => x.GetParameters().Length).ToList();
                constructorCache.Add(type, constructors);
                return constructors;
            }
        }

        private void DrawHeading(string text)
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label(text, EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
        }

        protected override void OnDestroyed()
        {
            ScriptObjData.SaveData();
        }

        private void RefreshTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(x => x.GetTypes())
                .Where(t => t.IsSubclassOf(SCRIPTOBJ_TYPE));

            ScriptObjData.allTypes = types.Where(t => !t.IsAbstract).ToList();
            ScriptObjData.allAbstractTypes = types.Where(t => t.IsAbstract)
                .OrderBy(t => t.Name).ToList();
            ScriptObjData.allAbstractTypes.Insert(0, SCRIPTOBJ_TYPE);
            ScriptObjData.allModules = types.Select(x => x.Module.Name).Distinct().ToList();
            ScriptObjData.allModules.Sort();
            ScriptObjData.allNamespaces = types.Where(x => !string.IsNullOrEmpty(x.Namespace))
                .Select(x => x.Namespace.Trim()).Distinct().ToList();
            ScriptObjData.allNamespaces.Sort();
            ScriptObjData.allNamespaces.Insert(0, NONE_TYPE);

            ScriptObjData.InitializeLists();
        }

        private void OnCreateClicked(Type type)
        {
            ScriptableObject newAsset;

            if (constructorParams == null || !constructorParams.Any())
                newAsset = CreateInstance(type);
            else
                newAsset = (ScriptableObject)Activator.CreateInstance(type, BINDING_FLAGS, Type.DefaultBinder, constructorParams, CultureInfo.CurrentCulture);

            if (newAsset != null)
            {
                string savePath = EditorUtility.SaveFilePanel("Asset Save Location", SCRIPTOBJ_DEFAULT_SAVELOCATION, type.Name, "asset");

                if (string.IsNullOrEmpty(savePath))
                    return;

                savePath = FileUtil.GetProjectRelativePath(savePath);

                AssetDatabase.CreateAsset(newAsset, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                //EditorUtility.FocusProjectWindow();
                Selection.activeObject = newAsset;
            }
            else
            {
                Debug.LogError("Unable to create an instance of " + type.Name);
            }
        }

        [Serializable]
        public class ScriptObjCreatorData
        {
            const string DATA_FILE = "/Editor/Expanse/ScriptObjCreator.json";

            const float CHECK_BOX_WIDTH = 10;
            const float ELEMENT_HEIGHT_OFFSET = 2f;
            const float ELEMENT_SPACING = 5f;

            string[] filterPresetOptions = new string[] { "All", "None" };

            [NonSerialized]
            public List<Type> allTypes;
            [NonSerialized]
            public List<Type> allAbstractTypes;
            [NonSerialized]
            public List<string> allModules;
            [NonSerialized]
            public List<string> allNamespaces;

            [NonSerialized]
            public List<string> activeTypes;

            public List<string> inactiveModules, inactiveNamespaces;
            [NonSerialized]
            public ReorderableList<string> modulesList, namespacesList;

            public string selectedParentType;
            public string selectedType;
            public int selectedConstructorIndex;

            public bool showModules, showNamespaces;

            public bool IsDirty { get; private set; }

            public static ScriptObjCreatorData LoadData()
            {
                ScriptObjCreatorData dataObj;

                string fullpath = StreamingAssetsUtil.GetFullStreamingAssetFilepath(DATA_FILE);
                FileInfo dataFileInfo = new FileInfo(fullpath);
                if (dataFileInfo.Exists)
                {
                    string dataFileContents = File.ReadAllText(dataFileInfo.FullName);
                    dataObj = JsonUtility.FromJson<ScriptObjCreatorData>(dataFileContents);
                }
                else
                {
                    dataObj = new ScriptObjCreatorData();
                    dataObj.SetupNew();
                }

                dataObj.SetupDefaults();
                return dataObj;
            }

            private void SetupNew()
            {
                selectedParentType = SCRIPTOBJ_TYPE.Name;
                selectedType = NONE_TYPE;

                inactiveModules = new List<string>();
                inactiveNamespaces = new List<string>();
            }

            private void SetupDefaults()
            {
                if (inactiveModules == null)
                    inactiveModules = new List<string>();

                if (inactiveNamespaces == null)
                    inactiveNamespaces = new List<string>();
            }

            public void InitializeLists()
            {
                IsDirty = false;

                modulesList = new ReorderableList<string>(allModules, "Modules", false, false, false);
                modulesList.drawElementCallback = OnDrawModuleElement;
                modulesList.drawHeaderCallback = OnDrawModuleHeader;
                namespacesList = new ReorderableList<string>(allNamespaces, "Namespaces", false, false, false);
                namespacesList.drawElementCallback = OnDrawNamespaceElement;
                namespacesList.drawHeaderCallback = OnDrawNamespaceHeader;

                activeTypes = allTypes.Where((t) =>
                {
                    if (!t.IsSubclassOf(GetTypeFromName(allAbstractTypes, selectedParentType)))
                        return false;
                    else if (IsModuleInactive(t) || IsNamespaceInactive(t))
                        return false;
                    else return true;
                }).Select(t => t.Name).ToList();
                activeTypes.Insert(0, NONE_TYPE);

                ValidateSelection();
            }

            private void OnDrawModuleHeader(Rect rect)
            {
                GUI.Label(rect, modulesList.DisplayName);
                int inactiveCount = inactiveModules.Count;
                int filterOption = inactiveCount == allModules.Count ? 1 : (inactiveCount == 0 ? 0 : -1);
                int filterResult = GUI.Toolbar(rect.SplitWidth(2, 2).AddPosition(4, 0), filterOption, filterPresetOptions, EditorStyles.toolbarButton);

                if (filterOption != filterResult)
                {
                    switch (filterResult)
                    {
                        case 0:
                            inactiveModules.Clear();
                            break;
                        case 1:
                            inactiveModules.AddRange(allModules);
                            break;
                        default: break;
                    }
                    IsDirty = true;
                }
            }

            private void OnDrawNamespaceHeader(Rect rect)
            {
                GUI.Label(rect, namespacesList.DisplayName);
                int inactiveCount = inactiveNamespaces.Count;
                int filterOption = inactiveCount == allNamespaces.Count ? 1 : (inactiveCount == 0 ? 0 : -1);
                int filterResult = GUI.Toolbar(rect.SplitWidth(2, 2).AddPosition(4, 0), filterOption, filterPresetOptions, EditorStyles.toolbarButton);

                if (filterOption != filterResult)
                {
                    switch (filterResult)
                    {
                        case 0:
                            inactiveNamespaces.Clear();
                            break;
                        case 1:
                            inactiveNamespaces.AddRange(allNamespaces);
                            break;
                        default: break;
                    }
                    IsDirty = true;
                }
            }

            private void OnDrawModuleElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                string value = modulesList[index];
                bool isChecked = !IsModuleInactive(value);

                Rect sourceRect = new Rect(rect);
                sourceRect = sourceRect.AddPosition(0, ELEMENT_HEIGHT_OFFSET);
                Rect checkBoxRect = new Rect(sourceRect);
                checkBoxRect.width = CHECK_BOX_WIDTH;
                Rect labelRect = new Rect(sourceRect);
                labelRect.x += CHECK_BOX_WIDTH + ELEMENT_SPACING;
                labelRect.width -= CHECK_BOX_WIDTH + ELEMENT_SPACING;

                bool result = GUI.Toggle(checkBoxRect, isChecked, "");
                GUI.Label(labelRect, value);

                if (result != isChecked)
                {
                    if (result)
                        inactiveModules.Remove(value);
                    else
                        inactiveModules.Add(value);

                    IsDirty = true;
                }
            }

            private void OnDrawNamespaceElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                string value = namespacesList[index];
                bool isChecked = !IsNamespaceInactive(value);

                Rect sourceRect = new Rect(rect);
                sourceRect = sourceRect.AddPosition(0, ELEMENT_HEIGHT_OFFSET);
                Rect checkBoxRect = new Rect(sourceRect);
                checkBoxRect.width = CHECK_BOX_WIDTH;
                Rect labelRect = new Rect(sourceRect);
                labelRect.x += CHECK_BOX_WIDTH + ELEMENT_SPACING;
                labelRect.width -= CHECK_BOX_WIDTH + ELEMENT_SPACING;

                bool result = GUI.Toggle(checkBoxRect, isChecked, "");
                GUI.Label(labelRect, value);

                if (result != isChecked)
                {
                    if (result)
                        inactiveNamespaces.Remove(value);
                    else
                        inactiveNamespaces.Add(value);

                    IsDirty = true;
                }
            }

            private void ValidateSelection()
            {
                if (GetSelectedType() == null && !selectedType.Equals(NONE_TYPE))
                    selectedType = NONE_TYPE;

                if (GetSelectedParentType() == null && !selectedParentType.Equals(SCRIPTOBJ_TYPE.Name))
                    selectedParentType = SCRIPTOBJ_TYPE.Name;
            }

            public Type GetTypeFromName(List<Type> typeList, string typeName)
            {
                return typeList.Find(x => x.Name.Equals(typeName));
            }

            public bool IsModuleInactive(Type type)
            {
                return inactiveModules.Contains(type.Module.Name);
            }

            public bool IsModuleInactive(string type)
            {
                return inactiveModules.Contains(type);
            }

            public bool IsNamespaceInactive(Type type)
            {
                if (string.IsNullOrEmpty(type.Namespace))
                    return inactiveNamespaces.Contains(NONE_TYPE);
                else return inactiveNamespaces.Contains(type.Namespace);
            }

            public bool IsNamespaceInactive(string type)
            {
                return inactiveNamespaces.Contains(type);
            }

            public void SaveData()
            {
                string fullpath = StreamingAssetsUtil.GetFullStreamingAssetFilepath(DATA_FILE);
                string dataFileContents = JsonUtility.ToJson(this, true);
                File.WriteAllText(fullpath, dataFileContents);
            }

            public Type GetSelectedType()
            {
                if (!selectedType.Equals(NONE_TYPE))
                {
                    Type type = GetTypeFromName(allTypes, selectedType);
                    return activeTypes.Contains(type.Name) ? type : null;
                }
                else return null;
            }

            public Type GetSelectedParentType()
            {
                return GetTypeFromName(allAbstractTypes, selectedParentType);
            }

            public string[] GetParentOptions()
            {
                return allAbstractTypes.Select(t => t.Name).ToArray();
            }

            public int GetSelectedParentIndex(string[] options)
            {
                return options.IndexOf(selectedParentType);
            }

            public void SetSelectedParentIndex(string[] options, int index)
            {
                selectedParentType = options[index];

                IsDirty = true;
            }

            public string[] GetTypeOptions()
            {
                return activeTypes.ToArray();
            }

            public int GetSelectedTypeIndex(string[] options)
            {
                return options.IndexOf(selectedType);
            }

            public void SetSelectedTypeIndex(string[] options, int index)
            {
                selectedType = options[index];
            }
        }
    }
}