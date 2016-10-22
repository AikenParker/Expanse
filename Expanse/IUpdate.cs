using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public interface IUpdate : IUnityInterface
    {
        void OnUpdate(float deltaTime);
    }

    public interface IComplexUpdate : IUpdate, IPriority
    {
        /// <summary>
        /// Will this update even if the attached MonoBehaviour is destroyed?
        /// </summary>
        bool UnsafeUpdates { get; }

        /// <summary>
        /// Will this update even if the attached MonoBehaviour is disabled or inactive?
        /// </summary>
        bool AlwaysUpdate { get; }

        /// <summary>
        /// Will the update use unscaled deltaTime?
        /// </summary>
        bool UnscaledDelta { get; }
    }

    public static class UpdateExt
    {
        public enum UpdateResult
        {
            SUCCESS = 0,
            FAIL = 1,
            REMOVE = 2
        }

        public static bool GetIsUnscaled(this IUpdate updateObj)
        {
            if (updateObj == null)
                throw new ArgumentNullException("updateObj");

            IComplexUpdate complexUpdateObj = updateObj as IComplexUpdate;

            if (complexUpdateObj != null)
                return complexUpdateObj.UnscaledDelta;
            else return false;
        }

        public static UpdateResult GetUpdateResult(this IUpdate updateObj)
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
    }
}