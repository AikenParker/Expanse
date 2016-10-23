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

            if (!instance.UpdateList.Contains(updateObj, x => x.updateObj))
                instance.UpdateList.Add(new UpdateWrapper(updateObj));
        }

        public static bool UnsubscribeFromGlobalUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            bool containsObj = instance.UpdateList.Contains(updateObj, x => x.updateObj);

            if (containsObj)
                instance.UpdateList.RemoveFirst(updateObj, x => x.updateObj);

            return containsObj;
        }

        public static void SubscribeToGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.FixedUpdateList.Contains(updateObj, x => x.updateObj))
                instance.FixedUpdateList.Add(new UpdateWrapper(updateObj));
        }

        public static bool UnsubscribeFromGlobalFixedUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            bool containsObj = instance.FixedUpdateList.Contains(updateObj, x => x.updateObj);

            if (containsObj)
                instance.FixedUpdateList.RemoveFirst(updateObj, x => x.updateObj);

            return containsObj;
        }

        public static void SubscribeToGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return;

            CallBackRelay instance = GlobalCBR;

            if (!instance.LateUpdateList.Contains(updateObj, x => x.updateObj))
                instance.LateUpdateList.Add(new UpdateWrapper(updateObj));
        }

        public static bool UnsubscribeFromGlobalLateUpdate(IUpdate updateObj)
        {
            if (GlobalCBRDestroyed)
                return false;

            CallBackRelay instance = GlobalCBR;

            bool containsObj = instance.LateUpdateList.Contains(updateObj, x => x.updateObj);

            if (containsObj)
                instance.LateUpdateList.RemoveFirst(updateObj, x => x.updateObj);

            return containsObj;
        }

        #endregion

        // TODO: Make an IUpdate wrapper that stores shit like last update time etc.
        public ExtList<UpdateWrapper> UpdateList = new ExtList<UpdateWrapper>(INITIAL_UPDATE_CAPACITY);
        public ExtList<UpdateWrapper> FixedUpdateList = new ExtList<UpdateWrapper>(INITIAL_FIXEDUPDATE_CAPACITY);
        public ExtList<UpdateWrapper> LateUpdateList = new ExtList<UpdateWrapper>(INITIAL_LATEUPDATE_CAPACITY);

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
            if (!UpdateList.Contains(updateObj, x => x.updateObj))
                UpdateList.Add(new UpdateWrapper(updateObj));
        }

        public bool UnsubscribeToUpdate(IUpdate updateObj)
        {
            bool containsObj = UpdateList.Contains(updateObj, x => x.updateObj);

            if (containsObj)
                UpdateList.RemoveFirst(updateObj, x => x.updateObj);

            return containsObj;
        }

        public void SubscribeToFixedUpdate(IUpdate updateObj)
        {
            if (!FixedUpdateList.Contains(updateObj, x => x.updateObj))
                FixedUpdateList.Add(new UpdateWrapper(updateObj));
        }

        public bool UnsubscribeToFixedUpdate(IUpdate updateObj)
        {
            bool containsObj = FixedUpdateList.Contains(updateObj, x => x.updateObj);

            if (containsObj)
                FixedUpdateList.RemoveFirst(updateObj, x => x.updateObj);

            return containsObj;
        }

        public void SubscribeToLateUpdate(IUpdate updateObj)
        {
            if (!LateUpdateList.Contains(updateObj, x => x.updateObj))
                LateUpdateList.Add(new UpdateWrapper(updateObj));
        }

        public bool UnsubscribeToLateUpdate(IUpdate updateObj)
        {
            bool containsObj = LateUpdateList.Contains(updateObj, x => x.updateObj);

            if (containsObj)
                LateUpdateList.RemoveFirst(updateObj, x => x.updateObj);

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

        private void ExecuteUpdates(ExtList<UpdateWrapper> updateList, UpdateSettings settings)
        {
            if (updateList.HasAny)
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

            settings.frameIndex++;
        }

        private void ImplUpdateAll(ExtList<UpdateWrapper> updateList, UpdateSettings settings)
        {
            UpdateWrapper updateWrapper;
            UpdateWrapper.UpdateResult updateResult;

            updateList.RemoveAll(x => x.GetUpdateResult() == UpdateWrapper.UpdateResult.REMOVE);

            for (int i = 0; i < updateList.Count; i++)
            {
                updateWrapper = updateList[i];

                updateResult = updateWrapper.GetUpdateResult();

                if (updateResult == UpdateWrapper.UpdateResult.SUCCESS)
                    updateWrapper.TryUpdate(settings);
            }
        }

        private void ImplUpdateSpread(ExtList<UpdateWrapper> updateList, UpdateSettings settings)
        {
            UpdateWrapper updateWrapper, firstWrapper = null;
            UpdateWrapper.UpdateResult updateResult;
            int updateCount;

            for (updateCount = 0; updateCount < settings.spreadCount; )
            {
                do
                {
                    updateWrapper = updateList.Dequeue();

                    updateResult = updateWrapper.GetUpdateResult();

                    if (updateResult != UpdateWrapper.UpdateResult.SUCCESS && !updateList.HasAny)
                        break;
                }
                while (updateResult != UpdateWrapper.UpdateResult.SUCCESS);

                if (updateResult == UpdateWrapper.UpdateResult.REMOVE)
                    break;

                if (updateWrapper == firstWrapper)
                {
                    updateList.Enqeue(updateWrapper);
                    break;
                }

                if (updateResult == UpdateWrapper.UpdateResult.SUCCESS)
                {
                    if (firstWrapper == null)
                        firstWrapper = updateWrapper;

                    if (updateWrapper.TryUpdate(settings))
                        updateCount++;

                    updateList.Enqeue(updateWrapper);
                }
                else if (updateResult == UpdateWrapper.UpdateResult.FAIL)
                {
                    updateList.Enqeue(updateWrapper);
                }
                else throw new UnexpectedException();
            }
        }

        private void ImplUpdateBudget(ExtList<UpdateWrapper> updateList, UpdateSettings settings)
        {
            UpdateWrapper updateWrapper, firstWrapper = null;
            UpdateWrapper.UpdateResult updateResult;

            budgetStopwatch.Reset();
            budgetStopwatch.Start();

            do
            {
                do
                {
                    updateWrapper = updateList.Dequeue();

                    updateResult = updateWrapper.GetUpdateResult();

                    if (updateResult != UpdateWrapper.UpdateResult.SUCCESS && !updateList.HasAny)
                        break;
                }
                while (updateResult != UpdateWrapper.UpdateResult.SUCCESS);

                if (updateResult == UpdateWrapper.UpdateResult.REMOVE)
                    break;

                if (updateWrapper == firstWrapper)
                {
                    updateList.Enqeue(updateWrapper);
                    break;
                }

                if (updateResult == UpdateWrapper.UpdateResult.SUCCESS)
                {
                    if (firstWrapper == null)
                        firstWrapper = updateWrapper;

                    updateWrapper.TryUpdate(settings);

                    updateList.Enqeue(updateWrapper);
                }
                else if (updateResult == UpdateWrapper.UpdateResult.FAIL)
                {
                    updateList.Enqeue(updateWrapper);
                }
                else throw new UnexpectedException();
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
        }

        [Serializable]
        public class UpdateWrapper
        {
            public readonly IUpdate updateObj;

            public float LastFrameTime { get; private set; }
            public float LastUnscaledFrameTime { get; private set; }

            public UpdateWrapper(IUpdate updateObj)
            {
                this.updateObj = updateObj;
            }

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
                    return Time.unscaledTime - LastUnscaledFrameTime;
                }
            }

            public float TrueDeltaTime
            {
                get
                {
                    return this.GetIsUnscaled() ? this.UnscaledDeltaTime : this.DeltaTime;
                }
            }

            public bool TryUpdate(UpdateSettings settings)
            {
                bool isSkipFrame = GetIsSkipFrame(settings);

                if (!isSkipFrame)
                {
                    updateObj.OnUpdate(TrueDeltaTime);

                    LastFrameTime = Time.time;
                    LastUnscaledFrameTime = Time.unscaledTime;
                }

                return !isSkipFrame;
            }

            private bool GetIsSkipFrame(UpdateSettings settings)
            {
                switch (settings.skipType)
                {
                    case UpdateSettings.SkipTypes.COUNT:
                        return (settings.frameIndex % (settings.skipFrames + 1)) != 0;

                    case UpdateSettings.SkipTypes.TIME:
                        return TrueDeltaTime < settings.skipTime;

                    case UpdateSettings.SkipTypes.NONE:
                        return false;

                    default:
                        throw new UnexpectedException();
                }
            }

            public bool GetIsUnscaled()
            {
                if (updateObj == null)
                    throw new ArgumentNullException("updateObj");

                IComplexUpdate complexUpdateObj = updateObj as IComplexUpdate;

                if (complexUpdateObj != null)
                    return complexUpdateObj.UnscaledDelta;
                else return false;
            }

            public UpdateResult GetUpdateResult()
            {
                if (updateObj == null)
                    return UpdateResult.REMOVE;

                IComplexUpdate complexUpdateObj = updateObj as IComplexUpdate;

                if (complexUpdateObj != null)
                {
                    bool unsafeUpdates = complexUpdateObj.UnsafeUpdates;

                    if (unsafeUpdates)
                        return UpdateResult.SUCCESS;
                }

                if (!updateObj.MonoBehaviour)
                    return UpdateResult.REMOVE;

                bool activeOrEnabled = updateObj.MonoBehaviour.isActiveAndEnabled;

                if (complexUpdateObj != null)
                {
                    if (!complexUpdateObj.AlwaysUpdate && !activeOrEnabled)
                        return UpdateResult.FAIL;
                }
                else
                {
                    if (!activeOrEnabled)
                        return UpdateResult.FAIL;
                }

                return UpdateResult.SUCCESS;
            }

            public enum UpdateResult
            {
                SUCCESS = 0,
                FAIL = 1,
                REMOVE = 2
            }
        }
    }
}
