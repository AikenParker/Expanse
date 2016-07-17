using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Expanse;
using Expanse.Ext;public class ScriptableObjectCreator : EditorWindow{
    const string DEFAULT_SAVE_LOCATION = @"Assets/Scriptable Objects/";

    Type[] allTypes;
    string[] allTypesDisplay;

    int allTypesIndex;

    [MenuItem("Tools/Scriptable Object Creator")]
    static void Init()
    {
        ScriptableObjectCreator window = (ScriptableObjectCreator)EditorWindow.GetWindow(typeof(ScriptableObjectCreator));
        window.titleContent = new GUIContent("Creator");
        window.Show();
    }    void OnEnable()
    {
        allTypes = typeof(FSMPlus).Assembly.GetTypes().Where(t => 
            {
                if (t.BaseType == null || t.IsAbstract)
                    return false;
                else
                    return t.IsSubclassOf(typeof(ScriptableObject));
            }).ToArray();

        allTypesDisplay = allTypes.ConvertValid<Type, string>(t => t.Name.AddSpaces()).ToArray();
    }    void OnGUI()
    {
        allTypesIndex = EditorGUILayout.Popup("Type", allTypesIndex, allTypesDisplay);

        EditorGUILayout.Space();
        if (GUILayout.Button("Create", GUILayout.MinHeight(30)))
            Create();
    }    private void Create()
    {
        ScriptableObject newAsset = ScriptableObject.CreateInstance(allTypes[allTypesIndex]);
        if (newAsset != null)
        {
            string savePath = EditorUtility.SaveFilePanel("Asset Save Location", DEFAULT_SAVE_LOCATION, allTypes[allTypesIndex].Name, "asset");

            if (savePath == "")
                return;

            savePath = FileUtil.GetProjectRelativePath(savePath);

            AssetDatabase.CreateAsset(newAsset, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;
        }
        else
        {
            Debug.LogError("Unable to create an instance of " + allTypes[allTypesIndex].Name);
        }
    }}