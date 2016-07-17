using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expanse.Ext;

namespace Expanse
{
    /// <summary>
    /// Responsible for updating all TimerPlus instances.
    /// DO NOT create an instance of this yourself.
    /// </summary>
    public class CallBackRelay : MonoBehaviour
    {
        [SerializeField]
        private bool hideGameObject = true;
        private static bool isDestroyed = false;

        private static CallBackRelay instance;
        private static Action OnUpdate, OnFixedUpdate, OnLateUpdate, OnLoad, OnDisposal;

        public static void SubscribeAll(Action onUpdate, Action onFixedUpdate, Action onLateUpdate, Action onLoad, Action onDisposal)
        {
            if (isDestroyed) return;

            // Ensure an instance exists
            if (instance == null)
                CreateSingleton();

            // Subscribe to events
            if (onUpdate != null)
                OnUpdate += onUpdate;
            if (onFixedUpdate != null)
                OnFixedUpdate += onFixedUpdate;
            if (onLateUpdate != null)
                OnLateUpdate += onLateUpdate;
            if (onLoad != null)
                OnLoad += onLoad;
            if (onDisposal != null)
                OnDisposal += onDisposal;
        }

        public static void UnsubscribeAll(Action onUpdate, Action onFixedUpdate, Action onLateUpdate, Action onLoad, Action onDisposal)
        {
            if (isDestroyed) return;

            // Ensure an instance exists
            if (instance == null)
                CreateSingleton();

            // Unsubscribe to events
            if (onUpdate != null)
                OnUpdate -= onUpdate;
            if (onFixedUpdate != null)
                OnFixedUpdate -= onFixedUpdate;
            if (onLateUpdate != null)
                OnLateUpdate += onLateUpdate;
            if (onLoad != null)
                OnLoad -= onLoad;
            if (onDisposal != null)
                OnDisposal -= onDisposal;
        }

        void Awake()
        {
            if (!instance)
                CreateSingleton(this);
            else
                DestroyImmediate(this);
        }

        private static void CreateSingleton()
        {
            CreateSingleton(new GameObject("CallBackRelay", typeof(CallBackRelay)).GetComponent<CallBackRelay>());
        }

        private static void CreateSingleton(CallBackRelay newRelay)
        {
            if (newRelay.hideGameObject)
                newRelay.gameObject.hideFlags = HideFlags.HideInHierarchy;

            instance = newRelay;
            DontDestroyOnLoad(newRelay.gameObject);
        }

        void Update()
        {
            if (OnUpdate != null && instance == this)
                OnUpdate();
        }

        void FixedUpdate()
        {
            if (OnFixedUpdate != null && instance == this)
                OnFixedUpdate();
        }

        void LateUpdate()
        {
            if (OnLateUpdate != null && instance == this)
                OnLateUpdate();
        }

        void OnLevelWasLoaded(int level)
        {
            if (OnLoad != null && instance == this)
                OnLoad();
        }

        void OnDestroy()
        {
            if (OnDisposal != null && instance == this)
            {
                OnDisposal();
                isDestroyed = true;
            }
        }
    }
}