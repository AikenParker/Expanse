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
    /// <summary>
    /// Creator editor utility tool window. Able to create a ScriptableObject of any type using any constructor.
    /// </summary>
    public class Creator : ExpanseWindow
    {
        private const BindingFlags CONSTRUCTOR_BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        private const string SCRIPTOBJ_DEFAULT_SAVELOCATION = @"Assets/";

        private readonly static Type SCRIPTOBJ_TYPE = typeof(ScriptableObject);

        protected override string DisplayName
        {
            get
            {
                return "Creator";
            }
        }
        protected override string Tooltip
        {
            get
            {
                return "Easily create scriptable object assets";
            }
        }

        Type selectedType;
        int selectedConstructorIndex;
        Dictionary<Type, List<ConstructorInfo>> constructorCache = new Dictionary<Type, List<ConstructorInfo>>();
        object[] constructorParams;

        [MenuItem("Expanse/Creator")]
        static void Create()
        {
            Creator window = GetWindow<Creator>();

            window.Initialize();
        }

        protected override void OnDrawContent()
        {
            // Draw type selection field
            {
                EditorGUILayout.BeginHorizontal();

                // Prefix label
                {
                    string typeString = selectedType != null ? selectedType.FullName : TypeUtil.NULL_TYPE_NAME;

                    if (!typeString.Equals(TypeUtil.NULL_TYPE_NAME))
                    {
                        selectedType = TypeUtil.GetTypeFromName(typeString);

                        if (!selectedType.IsAssignableTo(SCRIPTOBJ_TYPE) || selectedType.IsAbstract)
                        {
                            selectedType = null;
                        }
                    }
                    else
                    {
                        selectedType = null;
                    }

                    EditorGUILayout.PrefixLabel(new GUIContent("Type"));
                }

                // Object field
                {
                    string typeDisplayName = string.Format("{0} (Type)", selectedType != null ? selectedType.Name : TypeUtil.NULL_TYPE_NAME);

                    EditorGUILayout.LabelField(typeDisplayName, EditorStyles.objectField);
                }

                EditorGUILayout.EndHorizontal();

                // Thumb texture
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();

                    Rect thumbRect = new Rect(lastRect);

                    int thumbWidth = 18;

                    thumbRect.xMin = lastRect.xMax - thumbWidth;

                    if (GUI.Button(thumbRect, string.Empty, GUIStyle.none))
                    {
                        Action<Type> onSelectedTypeChanged = (newType) =>
                        {
                            if (newType == null || (newType.IsAssignableTo(SCRIPTOBJ_TYPE) && !newType.IsAbstract))
                            {
                                selectedType = newType;

                                this.Repaint();
                            }
                        };

                        TypeFinderWindow.Initialize(selectedType, onSelectedTypeChanged, SCRIPTOBJ_TYPE, true);
                    }
                }
            }

            ConstructorInfo selectedConstructor = null;

            // Draw constructor popup and parameters
            {
                if (selectedType != null)
                {
                    var constructors = GetConstructors(selectedType);

                    if (!constructors.HasIndexValue(selectedConstructorIndex))
                        selectedConstructorIndex = 0;

                    if (constructors.Count > 0)
                        selectedConstructor = constructors[selectedConstructorIndex];

                    if (selectedConstructor != null)
                    {
                        string[] constructorOptions = constructors.Select(x => GetParameterSignatureDisplay(x)).ToArray();

                        int result = EditorGUILayout.Popup("Constructor", selectedConstructorIndex, constructorOptions);

                        if (result != selectedConstructorIndex)
                        {
                            selectedConstructorIndex = result;

                            selectedConstructor = constructors[selectedConstructorIndex];

                            SetConstructorParameterDefaults(selectedConstructor);
                        }

                        ShowConstructorFields(selectedConstructor);
                    }
                    else ShowEmptyConstructorPopup();
                }
                else ShowEmptyConstructorPopup();
            }

            // Draw Create button
            {
                GUI.enabled = selectedType != null && selectedConstructor != null;
                if (GUILayout.Button("Create", GUILayout.MinHeight(30)))
                {
                    OnCreateClicked(selectedType);
                }
                GUI.enabled = true;
            }
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
                if (i == 0) EditorGUISplitter.SplitterLayout();

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

                if (i == constructorParams.Length - 1) EditorGUISplitter.SplitterLayout();
            }
        }

        private void ShowEmptyConstructorPopup()
        {
            EditorUtil.SetGuiEnabled(false);

            EditorGUILayout.Popup("Constructor", 0, new string[] { "-" });
            constructorParams = null;

            EditorUtil.RevertGuiEnabled();
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
                var constructors = type.GetConstructors(CONSTRUCTOR_BINDING_FLAGS)
                    .Where(x => !x.ContainsGenericParameters && !x.IsGenericMethod)
                    .OrderBy(x => x.GetParameters().Length).ToList();
                constructorCache.Add(type, constructors);
                return constructors;
            }
        }

        private void OnCreateClicked(Type type)
        {
            ScriptableObject newAsset;

            if (constructorParams == null || !constructorParams.Any())
                newAsset = CreateInstance(type);
            else
                newAsset = (ScriptableObject)Activator.CreateInstance(type, CONSTRUCTOR_BINDING_FLAGS, Type.DefaultBinder, constructorParams, CultureInfo.CurrentCulture);

            if (newAsset != null)
            {
                string savePath = EditorUtility.SaveFilePanel("Asset Save Location", SCRIPTOBJ_DEFAULT_SAVELOCATION, type.Name, "asset");

                if (string.IsNullOrEmpty(savePath))
                    return;

                savePath = FileUtil.GetProjectRelativePath(savePath);

                AssetDatabase.CreateAsset(newAsset, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Selection.activeObject = newAsset;
            }
            else
            {
                Debug.LogError("Unable to create an instance of " + type.Name);
            }
        }
    }
}