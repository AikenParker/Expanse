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
    public class TimerPlusManager : MonoBehaviour
    {
        public static TimerPlusManager instance { get; private set; }
        private Action TimerUpdate, TimerFixedUpdate, TimerDisposeOnLoad, TimerDisposeAll;

        public void Initialize(Action update, Action fixedUpdate, Action disposeOnLoad, Action disposeAll)
        {
            if (instance == this)
            {
                // Set singleton instance. Don't destroy on load and hide.
                instance = this;
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;

                // Set static TimerPlus methods.
                TimerUpdate = update;
                TimerFixedUpdate = fixedUpdate;
                TimerDisposeOnLoad = disposeOnLoad;
                TimerDisposeAll = disposeOnLoad;
            }
        }

        void Awake()
        {
            if (!instance)
            {
                // Set singleton instance. Don't destroy on load and hide.
                instance = this;
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        void Update()
        {
            TimerUpdate();
        }

        void FixedUpdate()
        {
            TimerFixedUpdate();
        }

        void OnLevelWasLoaded(int level)
        {
            if (instance == this)
                TimerDisposeOnLoad();
        }

        void OnDestroy()
        {
            if (instance == this)
                TimerDisposeAll();
        }
    }
}