using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Expanse.Extensions;
using Expanse.Utilities;

namespace Expanse.Misc
{
    /// <summary>
    /// Searchable popup window that lists all available types and provides a callback on selection.
    /// </summary>
    public class TypeFinderWindow : EditorWindow
    {
        private Type selectedType;
        private Action<Type> selectedTypeChanged;

        private string searchString;
        private Vector2 scrollPosition;

        private int resultsLimit = 200;

        private Type[] allTypes;

        public static void Initialize(Type currentType, Action<Type> selectedTypeChanged, Type baseType = null, bool nonAbstractOnly = false)
        {
            TypeFinderWindow popupWindow = ScriptableObject.CreateInstance<TypeFinderWindow>();

            Vector2 windowSize = new Vector2(250, 300);
            Vector2 windowPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

            windowPosition.x -= windowSize.x * 0.5f;
            windowPosition.y += 10;

            Rect windowRect = new Rect(windowPosition, windowSize);

            popupWindow.position = windowRect;
            popupWindow.searchString = currentType != null ? currentType.Name : string.Empty;
            popupWindow.titleContent = new GUIContent("Select Type");

            popupWindow.selectedType = currentType;
            popupWindow.selectedTypeChanged = selectedTypeChanged;

            popupWindow.allTypes = ReflectionUtil.Types;

            if (baseType != null)
            {
                popupWindow.allTypes = popupWindow.allTypes.Where(x => x == null || x.IsAssignableTo(baseType)).ToArray();
            }

            if (nonAbstractOnly)
            {
                popupWindow.allTypes = popupWindow.allTypes.Where(x => x == null || !x.IsAbstract).ToArray();
            }

            popupWindow.ShowUtility();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            // Draw search bar
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.ExpandWidth(true));
                if (GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                {
                    searchString = string.Empty;
                    GUI.FocusControl(null);
                }
                GUILayout.EndHorizontal();
            }

            // Draw search results
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                GUILayout.BeginVertical();
                GUILayout.Space(5);

                string searchStringLower = searchString.ToLower();

                int resultsShown = 0;

                for (int i = 0; i < allTypes.Length && resultsShown < resultsLimit; i++)
                {
                    Type type = allTypes[i];

                    string typeName = type != null ? type.FullName : ReflectionUtil.NULL_TYPE_NAME;
                    string typeNameLower = typeName.ToLower();

                    bool isSelected = selectedType == type;

                    if (string.IsNullOrEmpty(searchString) || typeNameLower.Contains(searchStringLower) || type == null)
                    {
                        resultsShown++;

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(isSelected ? string.Empty : typeName, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        GUILayout.EndHorizontal();

                        Rect areaRect = GUILayoutUtility.GetLastRect();

                        if (isSelected)
                        {
                            Rect textureRect = areaRect;
                            textureRect.xMin -= 4;
                            textureRect.xMax += 4;

                            Texture2D areaTexture = CreateTexture(textureRect, new Color32(70, 129, 229, 255));

                            GUI.DrawTexture(textureRect, areaTexture);

                            EditorGUI.LabelField(areaRect, typeName, EditorStyles.whiteLabel);
                        }
                        else
                        {
                            if (GUI.Button(areaRect, GUIContent.none, GUIStyle.none))
                            {
                                selectedType = type;

                                if (selectedTypeChanged != null)
                                    selectedTypeChanged.Invoke(selectedType);
                            }
                        }
                    }
                }

                GUILayout.FlexibleSpace();

                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }

        void OnLostFocus()
        {
            this.Close();
        }

        private Texture2D CreateTexture(Rect area, Color color)
        {
            int width = Mathf.RoundToInt(area.width);
            int height = Mathf.RoundToInt(area.height);

            Color[] pixColors = new Color[width * height];

            for (int i = 0; i < pixColors.Length; i++)
            {
                pixColors[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixColors);

            return texture;
        }
    }
}
