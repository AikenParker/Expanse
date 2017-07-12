using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Expanse.Extensions;
using Expanse.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Expanse
{
    /// <summary>
    /// Allows for subscriptions to Unity update functions.
    /// With in-built optimization options.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrderConstants.CALL_BACK_RELAY)]
    public class CallBackRelay : MonoBehaviour
    {
        private static CallBackRelay globalCBR;
        public static CallBackRelay GlobalCBR
        {
            get
            {
                if (GlobalCBRDestroyed)
                    return null;

                return globalCBR ?? (globalCBR = CreateGlobalCBR());
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

            if (!UpdateListContains(instance.UpdateList, updateObj))
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

            return instance.UnsubscribeFromUpdate(updateObj);
        }

        /// <summary>
        /// Subscribe an update object to the Global CBR fixed update list.
        /// </summary>
        public static void SubscribeToGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!UpdateListContains(instance.FixedUpdateList, updateObj))
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

            return instance.UnsubscribeFromFixedUpdate(updateObj);
        }

        /// <summary>
        /// Subscribe an update object to the Global CBR late update list.
        /// </summary>
        public static void SubscribeToGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!UpdateListContains(instance.LateUpdateList, updateObj))
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

            return instance.UnsubscribeFromLateUpdate(updateObj);
        }

        #endregion

        public List<CallBackRelayUpdateContainer> UpdateList = new List<CallBackRelayUpdateContainer>(10);
        public List<CallBackRelayUpdateContainer> FixedUpdateList = new List<CallBackRelayUpdateContainer>();
        public List<CallBackRelayUpdateContainer> LateUpdateList = new List<CallBackRelayUpdateContainer>();

        public event Action Destroyed;
        public event Action LevelChanged;

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
            if (!UpdateListContains(UpdateList, updateObj))
                UpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unsubsribe an update object from the update list.
        /// </summary>
        public bool UnsubscribeFromUpdate(IUpdate updateObj)
        {
            return UpdateList.RemoveFirst(x => x.updateObj == updateObj);
        }

        /// <summary>
        /// Subsribe an update object to the fixed update list.
        /// </summary>
        public void SubscribeToFixedUpdate(IUpdate updateObj)
        {
            if (!UpdateListContains(FixedUpdateList, updateObj))
                FixedUpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unsubsribe an update object from the fixed update list.
        /// </summary>
        public bool UnsubscribeFromFixedUpdate(IUpdate updateObj)
        {
            return FixedUpdateList.RemoveFirst(x => x.updateObj == updateObj);
        }

        /// <summary>
        /// Subsribe an update object to the late update list.
        /// </summary>
        public void SubscribeToLateUpdate(IUpdate updateObj)
        {
            if (!UpdateListContains(LateUpdateList, updateObj))
                LateUpdateList.Add(new CallBackRelayUpdateContainer(updateObj));
        }

        /// <summary>
        /// Unsubsribe an update object from the late update list.
        /// </summary>
        public bool UnsubscribeFromLateUpdate(IUpdate updateObj)
        {
            return LateUpdateList.RemoveFirst(x => x.updateObj == updateObj);
        }

        /// <summary>
        /// Unsubscribe an update object from any update list.
        /// </summary>
        /// <param name="updateObj">Update object instance to remove from any update list.</param>
        /// <returns>Returns true if an object was removed from any update list.</returns>
        public bool Unsubscribe(IUpdate updateObj)
        {
            return UnsubscribeFromUpdate(updateObj) ||
                UnsubscribeFromFixedUpdate(updateObj) ||
                UnsubscribeFromLateUpdate(updateObj);
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
            if (updateList.Count > 0)
            {
                switch (updateSettings.updateType)
                {
                    case CallBackRelaySettings.UpdateTypes.All:
                        ImplUpdateAll(updateList);
                        break;

                    case CallBackRelaySettings.UpdateTypes.Spread:
                        ImplUpdateSpread(updateList);
                        break;

                    case CallBackRelaySettings.UpdateTypes.Budget:
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
            int updateListCount = updateList.Count;

            // Remove all invalid update containers first

            for (int i = updateListCount - 1; i >= 0; i--)
            {
                CallBackRelayUpdateContainer updateContainer = updateList[i];

                CallBackRelayUpdateContainer.UpdateResult updateResult = updateContainer.GetUpdateResult();

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Remove)
                    updateList.RemoveAt(i);
            }

            // Then update valid update containers in order

            for (int i = 0; i < updateList.Count; i++)
            {
                CallBackRelayUpdateContainer updateContainer = updateList[i];

                CallBackRelayUpdateContainer.UpdateResult updateResult = updateContainer.GetUpdateResult();

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Success)
                    updateContainer.TryUpdate(updateSettings);
            }
        }

        /// <summary>
        /// Updates a set amount of update objects.
        /// </summary>
        private void ImplUpdateSpread(List<CallBackRelayUpdateContainer> updateList)
        {
            CallBackRelayUpdateContainer updateContainer, firstUpdateContainer = null;
            CallBackRelayUpdateContainer.UpdateResult updateResult;
            int updateCount;

            for (updateCount = 0; updateCount < updateSettings.spreadCount;)
            {
                do
                {
                    updateContainer = updateList.Dequeue();

                    updateResult = updateContainer.GetUpdateResult();

                    if (updateResult != CallBackRelayUpdateContainer.UpdateResult.Success && updateList.Count == 0)
                        break;
                }
                while (updateResult != CallBackRelayUpdateContainer.UpdateResult.Success);

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Remove)
                    break;

                if (updateContainer == firstUpdateContainer)
                {
                    updateList.Enqueue(updateContainer);
                    break;
                }

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Success)
                {
                    if (firstUpdateContainer == null)
                        firstUpdateContainer = updateContainer;

                    if (updateContainer.TryUpdate(updateSettings))
                        updateCount++;

                    updateList.Enqueue(updateContainer);
                }
                else if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Fail)
                {
                    updateList.Enqueue(updateContainer);
                }
                else throw new UnexpectedException();
            }
        }

        /// <summary>
        /// Updates objects within a set amount of time.
        /// </summary>
        private void ImplUpdateBudget(List<CallBackRelayUpdateContainer> updateList)
        {
            CallBackRelayUpdateContainer updateContainer, firstUpdateContainer = null;
            CallBackRelayUpdateContainer.UpdateResult updateResult;

            budgetStopwatch.Reset();
            budgetStopwatch.Start();

            do
            {
                do
                {
                    updateContainer = updateList.Dequeue();

                    updateResult = updateContainer.GetUpdateResult();

                    if (updateResult != CallBackRelayUpdateContainer.UpdateResult.Success && updateList.Count == 0)
                        break;
                }
                while (updateResult != CallBackRelayUpdateContainer.UpdateResult.Success);

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Remove)
                    break;

                if (updateContainer == firstUpdateContainer)
                {
                    updateList.Enqueue(updateContainer);
                    break;
                }

                if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Success)
                {
                    if (firstUpdateContainer == null)
                        firstUpdateContainer = updateContainer;

                    updateContainer.TryUpdate(updateSettings);

                    updateList.Enqueue(updateContainer);
                }
                else if (updateResult == CallBackRelayUpdateContainer.UpdateResult.Fail)
                {
                    updateList.Enqueue(updateContainer);
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

        private static bool UpdateListContains(List<CallBackRelayUpdateContainer> updateList, IUpdate updateObj)
        {
            for (int i = 0; i < updateList.Count; i++)
            {
                if (updateList[i].updateObj == updateObj)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the total number of subscribed updates objects on this callback relay.
        /// </summary>
        public int SubscriberCount
        {
            get { return UpdateList.Count + FixedUpdateList.Count + LateUpdateList.Count; }
        }
    }
}
