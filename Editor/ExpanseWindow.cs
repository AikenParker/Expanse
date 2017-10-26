using UnityEngine;
using UnityEditor;

namespace Expanse
{
    /// <summary>
    /// Base class for any Expanse editor windows.
    /// </summary>
	public abstract class ExpanseWindow : EditorWindow
    {
        void Awake()
        {
            Startup();
        }

        void OnDestroy()
        {
            Shutdown();
        }

        protected abstract void Startup();
        protected abstract void OnGUI();
        protected abstract void Shutdown();

        protected virtual void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        protected virtual void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        protected virtual void OnAfterAssemblyReload() { }

        protected virtual void OnBeforeAssemblyReload() { }

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
