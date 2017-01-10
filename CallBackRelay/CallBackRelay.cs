using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Expanse
{
    /// <summary>
    /// Allows for subscriptions to Unity update functions.
    /// With in-built optimization options.
    /// </summary>
    public partial class CallBackRelay : MonoBehaviour
    {
        private static CallBackRelay globalCBR;
        public static CallBackRelay GlobalCBR
        {
            get
            {
                if (GlobalCBRDestroyed)
                    return null;

                if (globalCBR == null)
                    globalCBR = CreateGlobalCBR();

                return globalCBR;
            }
        }

        public static bool GlobalCBRDestroyed { get; private set; }

        /// <summary>
        /// Creates a new CallBackRelay game object.
        /// </summary>
        private static CallBackRelay CreateGlobalCBR()
        {
            GameObject globalCBRObj = new GameObject("Global CallBackRelay");

            globalCBRObj.hideFlags = HideFlags.HideAndDontSave;

            CallBackRelay globalCBR = globalCBRObj.AddComponent<CallBackRelay>();
            globalCBR.destroyOnLoad = false;

            return globalCBR;
        }

        #region STATIC SUBSCRIPTIONS

        /// <summary>
        /// Subscribe an update object to the Global CBR update list.
        /// </summary>
        public static void SubscribeToGlobalUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.UpdateList.Contains(updateObj, x => x.updateObj))
                instance.UpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unubscribe an update object from the Global CBR update list.
        /// </summary>
        public static bool UnsubscribeFromGlobalUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            return instance.UnsubscribeToUpdate(updateObj);
        }

        /// <summary>
        /// Subscribe an update object to the Global CBR fixed update list.
        /// </summary>
        public static void SubscribeToGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.FixedUpdateList.Contains(updateObj, x => x.updateObj))
                instance.FixedUpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unubscribe an update object from the Global CBR fixed update list.
        /// </summary>
        public static bool UnsubscribeFromGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            return instance.UnsubscribeToFixedUpdate(updateObj);
        }

        /// <summary>
        /// Subscribe an update object to the Global CBR late update list.
        /// </summary>
        public static void SubscribeToGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.LateUpdateList.Contains(updateObj, x => x.updateObj))
                instance.LateUpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unubscribe an update object from the Global CBR late update list.
        /// </summary>
        public static bool UnsubscribeFromGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            return instance.UnsubscribeToLateUpdate(updateObj);
        }

        #endregion

        public List<CallBackRelayUpdateContainer> UpdateList = new List<CallBackRelayUpdateContainer>();
        public List<CallBackRelayUpdateContainer> FixedUpdateList = new List<CallBackRelayUpdateContainer>();
        public List<CallBackRelayUpdateContainer> LateUpdateList = new List<CallBackRelayUpdateContainer>();

        public event Action Destroyed;
        public event Action LevelChanged;

        [ReadOnly(EditableInEditor = true)]
        public bool destroyOnLoad = true;

        public CallBackRelaySettings updateSettings = new CallBackRelaySettings();

        Stopwatch budgetStopwatch = new Stopwatch();

        void Start()
        {
            SceneManager.activeSceneChanged += SceneChanged;

            if (!destroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
        }

        #region INSTANCE SUBSCRIPTIONS

        /// <summary>
        /// Subsribe an update object to the update list.
        /// </summary>
        public void SubscribeToUpdate(IUpdate updateObj)
        {
            if (!UpdateList.Contains(updateObj, x => x.updateObj))
                UpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unsubsribe an update object from the update list.
        /// </summary>
        public bool UnsubscribeToUpdate(IUpdate updateObj)
        {
            return UpdateList.RemoveFirst(x => x.updateObj == updateObj);
        }

        /// <summary>
        /// Subsribe an update object to the fixed update list.
        /// </summary>
        public void SubscribeToFixedUpdate(IUpdate updateObj)
        {
            if (!FixedUpdateList.Contains(updateObj, x => x.updateObj))
                FixedUpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        // <summary>
        /// Unsubsribe an update object from the fixed update list.
        /// </summary>
        public bool UnsubscribeToFixedUpdate(IUpdate updateObj)
        {
            return FixedUpdateList.RemoveFirst(x => x.updateObj == updateObj);
        }

        /// <summary>
        /// Subsribe an update object to the late update list.
        /// </summary>
        public void SubscribeToLateUpdate(IUpdate updateObj)
        {
            if (!LateUpdateList.Contains(updateObj, x => x.updateObj))
                LateUpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        // <summary>
        /// Unsubsribe an update object from the late update list.
        /// </summary>
        public bool UnsubscribeToLateUpdate(IUpdate updateObj)
        {
            return LateUpdateList.RemoveFirst(x => x.updateObj == updateObj);
        }

        #endregion

        void Update()
        {
            this.ExecuteUpdates(UpdateList);
        }

        void FixedUpdate()
        {
            this.ExecuteUpdates(FixedUpdateList);
        }

        void LateUpdate()
        {
            this.ExecuteUpdates(LateUpdateList);
        }

        private void ExecuteUpdates(List<CallBackRelayUpdateContainer> updateList)
        {
            if (updateList.Any())
            {
                switch (updateSettings.updateType)
                {
                    case CallBackRelaySettings.UpdateTypes.ALL:
                        ImplUpdateAll(updateList);
                        break;

                    case CallBackRelaySettings.UpdateTypes.SPREAD:
                        ImplUpdateSpread(updateList);
                        break;

                    case CallBackRelaySettings.UpdateTypes.BUDGET:
                        ImplUpdateBudget(updateList);
                        break;
                }
            }

            updateSettings.frameIndex++;
        }

        /// <summary>
        /// Updates all update objects.
        /// </summary>
        private void ImplUpdateAll(List<CallBackRelayUpdateContainer> updateList)
        {
            CallBackRelayUpdateContainer updateWrapper;
            CallBackRelayUpdateContainer.UpdateResult updateResult;

            updateList.RemoveAll(x => x.GetUpdateResult() == CallBackRelayUpdateContainer.UpdateResult.REMOVE);

            for (int i = 0; i < updateList.Count; i++)
            {
                updateWrapper = updateList[i];

                updateResult = updateWrapper.GetUpdateResult();

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.SUCCESS)
                    updateWrapper.TryUpdate(updateSettings);
            }
        }

        /// <summary>
        /// Updates a set amount of update objects.
        /// </summary>
        private void ImplUpdateSpread(List<CallBackRelayUpdateContainer> updateList)
        {
            CallBackRelayUpdateContainer updateWrapper, firstWrapper = null;
            CallBackRelayUpdateContainer.UpdateResult updateResult;
            int updateCount;

            for (updateCount = 0; updateCount < updateSettings.spreadCount; )
            {
                do
                {
                    updateWrapper = updateList.Dequeue();

                    updateResult = updateWrapper.GetUpdateResult();

                    if (updateResult != CallBackRelayUpdateContainer.UpdateResult.SUCCESS && !updateList.Any())
                        break;
                }
                while (updateResult != CallBackRelayUpdateContainer.UpdateResult.SUCCESS);

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.REMOVE)
                    break;

                if (updateWrapper == firstWrapper)
                {
                    updateList.Enqueue(updateWrapper);
                    break;
                }

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.SUCCESS)
                {
                    if (firstWrapper == null)
                        firstWrapper = updateWrapper;

                    if (updateWrapper.TryUpdate(updateSettings))
                        updateCount++;

                    updateList.Enqueue(updateWrapper);
                }
                else if (updateResult == CallBackRelayUpdateContainer.UpdateResult.FAIL)
                {
                    updateList.Enqueue(updateWrapper);
                }
                else throw new UnexpectedException();
            }
        }

        /// <summary>
        /// Updates objects within a set amount of time.
        /// </summary>
        private void ImplUpdateBudget(List<CallBackRelayUpdateContainer> updateList)
        {
            CallBackRelayUpdateContainer updateWrapper, firstWrapper = null;
            CallBackRelayUpdateContainer.UpdateResult updateResult;

            budgetStopwatch.Reset();
            budgetStopwatch.Start();

            do
            {
                do
                {
                    updateWrapper = updateList.Dequeue();

                    updateResult = updateWrapper.GetUpdateResult();

                    if (updateResult != CallBackRelayUpdateContainer.UpdateResult.SUCCESS && !updateList.Any())
                        break;
                }
                while (updateResult != CallBackRelayUpdateContainer.UpdateResult.SUCCESS);

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.REMOVE)
                    break;

                if (updateWrapper == firstWrapper)
                {
                    updateList.Enqueue(updateWrapper);
                    break;
                }

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.SUCCESS)
                {
                    if (firstWrapper == null)
                        firstWrapper = updateWrapper;

                    updateWrapper.TryUpdate(updateSettings);

                    updateList.Enqueue(updateWrapper);
                }
                else if (updateResult == CallBackRelayUpdateContainer.UpdateResult.FAIL)
                {
                    updateList.Enqueue(updateWrapper);
                }
                else throw new UnexpectedException();
            }
            while (budgetStopwatch.Elapsed.TotalMilliseconds < updateSettings.frameBudget);

            budgetStopwatch.Stop();
        }

        private void SceneChanged(Scene prevScene, Scene newScene)
        {
            if (LevelChanged != null)
                LevelChanged();
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneChanged;

            if (Destroyed != null)
                Destroyed();

            if (this == GlobalCBR)
                GlobalCBRDestroyed = true;
        }
    }
}
