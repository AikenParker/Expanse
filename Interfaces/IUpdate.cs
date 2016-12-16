using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public interface IUpdate : IUnity
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
}