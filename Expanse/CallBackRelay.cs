using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Expanse
{
    /// <summary>
    /// Allows for subscriptions to Unity update functions.
    /// With in-built optimization options.
    /// </summary>
    public class CallBackRelay : MonoBehaviour
    {
        private const int INITIAL_UPDATE_CAPACITY = 100;
        private const int INITIAL_FIXEDUPDATE_CAPACITY = 10;
        private const int INITIAL_LATEUPDATE_CAPACITY = 4;

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

        private static CallBackRelay CreateGlobalCBR()
        {
            GameObject globalCBRObj = new GameObject("Global CallBackRelay");

            globalCBRObj.hideFlags = HideFlags.HideAndDontSave;

            CallBackRelay globalCBR = globalCBRObj.AddComponent<CallBackRelay>();
            globalCBR.destroyOnLoad = false;

            return globalCBR;
        }

        #region STATIC SUBSCRIPTIONS

        public static void SubscribeToGlobalUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.UpdateList.Contains(updateObj))
                instance.UpdateList.Add(updateObj);
        }

        public static bool UnsubscribeFromGlobalUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            bool containsObj = instance.UpdateList.Contains(updateObj);

            if (containsObj)
                instance.UpdateList.Remove(updateObj);

            return containsObj;
        }

        public static void SubscribeToGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.FixedUpdateList.Contains(updateObj))
                instance.FixedUpdateList.Add(updateObj);
        }

        public static bool UnsubscribeFromGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            bool containsObj = instance.FixedUpdateList.Contains(updateObj);

            if (containsObj)
                instance.FixedUpdateList.Remove(updateObj);

            return containsObj;
        }

        public static void SubscribeToGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.LateUpdateList.Contains(updateObj))
                instance.LateUpdateList.Add(updateObj);
        }

        public static bool UnsubscribeFromGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            bool containsObj = instance.LateUpdateList.Contains(updateObj);

            if (containsObj)
                instance.LateUpdateList.Remove(updateObj);

            return containsObj;
        }

        #endregion

        // TODO: Make an IUpdate wrapper that stores shit like last update time etc.
        public ExtList<IUpdate> UpdateList = new ExtList<IUpdate>(INITIAL_UPDATE_CAPACITY);
        public ExtList<IUpdate> FixedUpdateList = new ExtList<IUpdate>(INITIAL_FIXEDUPDATE_CAPACITY);
        public ExtList<IUpdate> LateUpdateList = new ExtList<IUpdate>(INITIAL_LATEUPDATE_CAPACITY);

        public event Action Destroyed;
        public event Action LevelLoaded;

        [ReadOnly(EditableInEditor = true)]
        public bool destroyOnLoad = true;

        public UpdateSettings updateSettings = new UpdateSettings();
        public UpdateSettings fixedUpdateSettings = new UpdateSettings();
        public UpdateSettings lateUpdateSettings = new UpdateSettings();

        Stopwatch budgetStopwatch = new Stopwatch();

        void Start()
        {
            if (!destroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
        }

        #region INSTANCE SUBSCRIPTIONS

        public void SubscribeToUpdate(IUpdate updateObj)
        {
            if (!UpdateList.Contains(updateObj))
                UpdateList.Add(updateObj);
        }

        public bool UnsubscribeToUpdate(IUpdate updateObj)
        {
            bool containsObj = UpdateList.Contains(updateObj);

            if (containsObj)
                UpdateList.Remove(updateObj);

            return containsObj;
        }

        public void SubscribeToFixedUpdate(IUpdate updateObj)
        {
            if (!FixedUpdateList.Contains(updateObj))
                FixedUpdateList.Add(updateObj);
        }

        public bool UnsubscribeToFixedUpdate(IUpdate updateObj)
        {
            bool containsObj = FixedUpdateList.Contains(updateObj);

            if (containsObj)
                FixedUpdateList.Remove(updateObj);

            return containsObj;
        }

        public void SubscribeToLateUpdate(IUpdate updateObj)
        {
            if (!LateUpdateList.Contains(updateObj))
                LateUpdateList.Add(updateObj);
        }

        public bool UnsubscribeToLateUpdate(IUpdate updateObj)
        {
            bool containsObj = LateUpdateList.Contains(updateObj);

            if (containsObj)
                LateUpdateList.Remove(updateObj);

            return containsObj;
        }

        #endregion

        void Update()
        {
            this.ExecuteUpdates(UpdateList, updateSettings);
        }

        void FixedUpdate()
        {
            this.ExecuteUpdates(FixedUpdateList, fixedUpdateSettings);
        }

        void LateUpdate()
        {
            this.ExecuteUpdates(LateUpdateList, lateUpdateSettings);
        }

        private void ExecuteUpdates(ExtList<IUpdate> updateList, UpdateSettings settings)
        {
            if (!settings.IsSkipFrame && updateList.HasAny)
            {
                switch (settings.updateType)
                {
                    case UpdateSettings.UpdateTypes.ALL:
                        ImplUpdateAll(updateList, settings);
                        break;

                    case UpdateSettings.UpdateTypes.SPREAD:
                        ImplUpdateSpread(updateList, settings);
                        break;

                    case UpdateSettings.UpdateTypes.BUDGET:
                        ImplUpdateBudget(updateList, settings);
                        break;
                }
            }

            settings.OnEndFrame();
        }

        private void ImplUpdateAll(ExtList<IUpdate> updateList, UpdateSettings settings)
        {
            IUpdate updateObj;
            UpdateExt.UpdateResult updateResult;

            updateList.RemoveAll(x => x.GetUpdateResult() == UpdateExt.UpdateResult.REMOVE);

            for (int i = 0; i < updateList.Count; i++)
            {
                updateObj = updateList[i];

                updateResult = updateObj.GetUpdateResult();

                if (updateResult == UpdateExt.UpdateResult.SUCCESS)
                    updateObj.OnUpdate(updateObj.GetIsUnscaled() ? settings.UnscaledDeltaTime : settings.DeltaTime);
            }
        }

        private void ImplUpdateSpread(ExtList<IUpdate> updateList, UpdateSettings settings)
        {
            IUpdate updateObj, firstObj = null;
            UpdateExt.UpdateResult updateResult;
            int updateCount;

            for (updateCount = 0; updateCount < settings.spreadCount; )
            {
                do
                {
                    updateObj = updateList.Dequeue();

                    updateResult = updateObj.GetUpdateResult();

                    if (updateResult != UpdateExt.UpdateResult.SUCCESS && !updateList.HasAny)
                        break;
                }
                while (updateResult != UpdateExt.UpdateResult.SUCCESS);

                if (updateObj == firstObj)
                    break;

                if (updateResult == UpdateExt.UpdateResult.SUCCESS)
                {
                    if (firstObj == null)
                        firstObj = updateObj;

                    updateObj.OnUpdate(updateObj.GetIsUnscaled() ? settings.UnscaledDeltaTime : settings.DeltaTime);

                    updateList.Enqeue(updateObj);

                    updateCount++;
                }
                else if (updateResult == UpdateExt.UpdateResult.FAIL)
                {
                    updateList.Enqeue(updateObj);
                }
                else break;
            }
        }

        private void ImplUpdateBudget(ExtList<IUpdate> updateList, UpdateSettings settings)
        {
            IUpdate updateObj, firstObj = null;
            UpdateExt.UpdateResult updateResult;

            budgetStopwatch.Reset();
            budgetStopwatch.Start();

            do
            {
                do
                {
                    updateObj = updateList.Dequeue();

                    updateResult = updateObj.GetUpdateResult();

                    if (updateResult != UpdateExt.UpdateResult.SUCCESS && !updateList.HasAny)
                        break;
                }
                while (updateResult != UpdateExt.UpdateResult.SUCCESS);

                if (updateObj == firstObj)
                    break;

                if (updateResult == UpdateExt.UpdateResult.SUCCESS)
                {
                    if (firstObj == null)
                        firstObj = updateObj;

                    updateObj.OnUpdate(updateObj.GetIsUnscaled() ? settings.UnscaledDeltaTime : settings.DeltaTime);

                    updateList.Enqeue(updateObj);
                }
                else if (updateResult == UpdateExt.UpdateResult.FAIL)
                {
                    updateList.Enqeue(updateObj);
                }
                else break;
            }
            while (budgetStopwatch.Elapsed.TotalMilliseconds < settings.frameBudget);

            budgetStopwatch.Stop();
        }

        void OnLevelWasLoaded(int level)
        {
            if (LevelLoaded != null)
                LevelLoaded();
        }

        void OnDestroy()
        {
            if (Destroyed != null)
                Destroyed();

            if (this == GlobalCBR)
                GlobalCBRDestroyed = true;
        }

        [Serializable]
        public class UpdateSettings
        {
            public UpdateTypes updateType = UpdateTypes.ALL;
            public SkipTypes skipType = SkipTypes.NONE;
            [ReadOnly]
            public int frameIndex;

            public int skipFrames;
            public float skipTime;

            public int spreadCount = 1;

            public float frameBudget;

            public enum UpdateTypes
            {
                NONE = 0,
                ALL,
                SPREAD,
                BUDGET
            }

            public enum SkipTypes
            {
                NONE = 0,
                TIME,
                COUNT
            }

            public float LastFrameTime { get; private set; }
            public float LastFrameUnscaledTime { get; private set; }

            public float DeltaTime
            {
                get
                {
                    return Time.time - LastFrameTime;
                }
            }

            public float UnscaledDeltaTime
            {
                get
                {
                    return Time.unscaledTime - LastFrameUnscaledTime;
                }
            }

            public bool IsSkipFrame
            {
                get
                {
                    switch (skipType)
                    {
                        case SkipTypes.COUNT:
                            return (frameIndex % (skipFrames + 1)) != 0;

                        case SkipTypes.TIME:
                            return DeltaTime < skipTime;

                        case SkipTypes.NONE:
                            return false;

                        default:
                            throw new UnexpectedException();
                    }
                }
            }

            public void OnEndFrame()
            {
                if (!IsSkipFrame)
                {
                    LastFrameTime = Time.time;
                    LastFrameUnscaledTime = Time.unscaledTime;
                }

                frameIndex++;
            }
        }
    }
}
