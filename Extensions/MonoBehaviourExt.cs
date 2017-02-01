using System;
using System.Collections;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of UnityEngine.MonoBehaviour related extension methods.
    /// </summary>
    public static class MonoBehaviourExt
    {
        /// <summary>
        /// Waits until the next frame before invoking the action.
        /// </summary>
        public static Coroutine WaitForEndOfFrame(this MonoBehaviour monoBehaviour, Action action)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForEndOfFrame(action));
        }

        /// <summary>
        /// Waits a given number of scaled seconds before invoking the action.
        /// </summary>
        public static Coroutine WaitForSeconds(this MonoBehaviour monoBehaviour, Action action, float delay)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForSeconds(action, delay));
        }

        /// <summary>
        /// Waits until the next fixed update step before invoking the action.
        /// </summary>
        public static Coroutine WaitForFixedUpdate(this MonoBehaviour monoBehaviour, Action action)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForFixedUpdate(action));
        }

        /// <summary>
        /// Waits a given number of unscaled seconds before invoking the action.
        /// </summary>
        public static Coroutine WaitForSecondsRealtime(this MonoBehaviour monoBehaviour, Action action, float delay)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForSecondsRealtime(action, delay));
        }

        /// <summary>
        /// Waits until the predicate is true before invoking the action.
        /// </summary>
        public static Coroutine WaitUntil(this MonoBehaviour monoBehaviour, Action action, Func<bool> predicate)
        {
            return monoBehaviour.StartCoroutine(Co_WaitUntil(action, predicate));
        }

        /// <summary>
        /// Waits until the predicate is false before invoking the action.
        /// </summary>
        public static Coroutine WaitWhile(this MonoBehaviour monoBehaviour, Action action, Func<bool> predicate)
        {
            return monoBehaviour.StartCoroutine(Co_WaitWhile(action, predicate));
        }

        /// <summary>
        /// Waits for the custom yeild instruction before invoking the action.
        /// </summary>
        public static Coroutine WaitCustom(this MonoBehaviour monoBehaviour, Action action, CustomYieldInstruction customYieldInstruction)
        {
            return monoBehaviour.StartCoroutine(Co_WaitCustom(action, customYieldInstruction));
        }

        private static IEnumerator Co_WaitForEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();

            if (action != null)
                action.Invoke();
        }

        private static IEnumerator Co_WaitForSeconds(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (action != null)
                action.Invoke();
        }

        private static IEnumerator Co_WaitForFixedUpdate(Action action)
        {
            yield return new WaitForFixedUpdate();

            if (action != null)
                action.Invoke();
        }

        private static IEnumerator Co_WaitForSecondsRealtime(Action action, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            if (action != null)
                action.Invoke();
        }

        private static IEnumerator Co_WaitUntil(Action action, Func<bool> predicate)
        {
            yield return new WaitUntil(predicate);

            if (action != null)
                action.Invoke();
        }

        private static IEnumerator Co_WaitWhile(Action action, Func<bool> predicate)
        {
            yield return new WaitWhile(predicate);

            if (action != null)
                action.Invoke();
        }

        private static IEnumerator Co_WaitCustom(Action action, CustomYieldInstruction customYieldInstruction)
        {
            yield return customYieldInstruction;

            if (action != null)
                action.Invoke();
        }
    }
}
