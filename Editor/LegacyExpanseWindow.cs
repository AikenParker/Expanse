using UnityEngine;
using System;
using UnityEditor;
using Expanse.Extensions;

namespace Expanse
{
    /// <summary>
    /// Base class for any Expanse editor windows.
    /// </summary>
    [Obsolete("Use ExpanseWindow instead")]
    public abstract class LegacyExpanseWindow : EditorWindow
    {
        protected virtual string DisplayName
        {
            get
            {
                return GetType().Name.AddSpaces();
            }
        }
        protected virtual string Tooltip
        {
            get
            {
                return null;
            }
        }
        protected virtual string TextureIconPath
        {
            get
            {
                return null;
            }
        }
        protected virtual float ContentMargin
        {
            get
            {
                return 10f;
            }
        }
        protected virtual bool ContentScrollEnabled
        {
            get { return true; }
        }
        protected virtual bool HeaderEnabled
        {
            get { return false; }
        }
        protected virtual bool FooterEnabled
        {
            get { return false; }
        }
        protected virtual bool LeftPanelEnabled
        {
            get { return false; }
        }
        protected virtual bool RightPanelEnabled
        {
            get { return false; }
        }

        protected Texture TextureIcon { get; set; }

        private bool sidePanelEnabled;
        private bool compileFlag;

        /// <summary>
        /// 1. Create static method
        /// 2. Add attribute: [MenuItem("Tools/DisplayName")]
        /// 3. Call Initialize()
        /// </summary>
        protected virtual void Initialize()
        {
            if (TextureIconPath != null)
                TextureIcon = AssetDatabase.LoadAssetAtPath<Texture>(TextureIconPath);

            GUIContent titleContent = new GUIContent(DisplayName);

            if (Tooltip != null)
                titleContent.tooltip = Tooltip;

            if (TextureIcon != null)
                titleContent.image = TextureIcon;

            this.titleContent = titleContent;
        }

        void OnEnable()
        {
            OnEnabled();
        }

        void OnDisable()
        {
            OnDisabled();
        }

        void OnGUI()
        {
            HandleHeaderGUI();

            HandleContentGUI();

            HandleFooterGUI();
        }

        private void HandleHeaderGUI()
        {
            if (HeaderEnabled)
            {
                GUILayout.BeginVertical();
                try
                {
                    OnDrawHeader();
                }
                catch (Exception e)
                {
                    Debug.Log("Exception in Header");
                    Debug.LogException(e, this);
                }
                GUILayout.EndVertical();
            }
        }

        private void HandleContentGUI()
        {
            if (LeftPanelEnabled || RightPanelEnabled)
            {
                sidePanelEnabled = true;
                GUILayout.BeginHorizontal();
            }
            else sidePanelEnabled = false;

            HandleLeftPanelGUI();

            GUILayout.BeginVertical();
            if (ContentScrollEnabled)
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);

            GUILayout.BeginVertical();
            GUILayout.Space(ContentMargin);
            GUILayout.BeginHorizontal();
            GUILayout.Space(ContentMargin);
            GUILayout.BeginVertical();
            try
            {
                OnDrawContent();
            }
            catch (Exception e)
            {
                Debug.Log("Exception in Content");
                Debug.LogException(e, this);
            }
            finally
            {
                GUILayout.EndVertical();
                GUILayout.Space(ContentMargin);
                GUILayout.EndHorizontal();
                GUILayout.Space(ContentMargin);
                GUILayout.EndVertical();

                if (ContentScrollEnabled)
                    GUILayout.EndScrollView();

                GUILayout.EndVertical();
            }

            HandleRightPanelGUI();

            if (sidePanelEnabled)
                GUILayout.EndHorizontal();
        }

        private void HandleLeftPanelGUI()
        {
            if (LeftPanelEnabled)
            {
                GUILayout.BeginVertical();
                try
                {
                    OnDrawLeftPanel();
                }
                catch (Exception e)
                {
                    Debug.Log("Exception in Left Panel");
                    Debug.LogException(e, this);
                }
                GUILayout.EndVertical();
            }
        }

        private void HandleRightPanelGUI()
        {
            if (RightPanelEnabled)
            {
                GUILayout.BeginVertical();
                try
                {
                    OnDrawRightPanel();
                }
                catch (Exception e)
                {
                    Debug.Log("Exception in Right Panel");
                    Debug.LogException(e, this);
                }
                GUILayout.EndVertical();
            }
        }

        private void HandleFooterGUI()
        {
            if (FooterEnabled)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                try
                {
                    OnDrawFooter();
                }
                catch (Exception e)
                {
                    Debug.Log("Exception in Footer");
                    Debug.LogException(e, this);
                }
                GUILayout.EndVertical();
            }
        }

        void OnDestroy()
        {
            OnDestroyed();
        }

        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }
        protected virtual void OnDrawHeader() { }
        protected virtual void OnDrawLeftPanel() { }
        protected virtual void OnDrawContent() { }
        protected virtual void OnDrawRightPanel() { }
        protected virtual void OnDrawFooter() { }
        protected virtual void OnDestroyed() { }

        public Vector2 ScrollPosition { get; set; }

        public Vector2 Position
        {
            get { return position.position; }
            set { position = new Rect(value, position.size); }
        }

        public Vector2 Size
        {
            get { return position.size; }
            set { position = new Rect(position.position, value); }
        }
    }
}