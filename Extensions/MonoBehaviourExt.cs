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

        /// <summary>
        /// Waits for a threaded task to complete before invoking the action.
        /// </summary>
        public static Coroutine WaitForThreadedTask(this MonoBehaviour monoBehaviour, Action action, Action threadedTask, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForThreadedTask(threadedTask, action, priority));
        }

        /// <summary>
        /// Loads an object from a Json file using another thread.
        /// </summary>
        public static Coroutine LoadJsonFile<T>(this MonoBehaviour monoBehaviour, string filePath, Action<ApplicationUtil.JsonLoadInfo<T>> onComplete, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_LoadJsonFile(filePath, onComplete, priority));
        }

        /// <summary>
        /// Loads an object from a Json file in the StreamingAssets folder using another thread.
        /// </summary>
        public static Coroutine LoadStreamingAssetJsonFile<T>(this MonoBehaviour monoBehaviour, string filePath, Action<ApplicationUtil.JsonLoadInfo<T>> onComplete, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_LoadStreamingAssetJsonFile(filePath, onComplete, priority));
        }

        private static IEnumerator Co_WaitForEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitForSeconds(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitForFixedUpdate(Action action)
        {
            yield return new WaitForFixedUpdate();

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitForSecondsRealtime(Action action, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitUntil(Action action, Func<bool> predicate)
        {
            yield return new WaitUntil(predicate);

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitWhile(Action action, Func<bool> predicate)
        {
            yield return new WaitWhile(predicate);

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitCustom(Action action, CustomYieldInstruction customYieldInstruction)
        {
            yield return customYieldInstruction;

            action.SafeInvoke();
        }

        private static IEnumerator Co_WaitForThreadedTask(Action action, Action threadedTask, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            yield return new WaitForThreadedTask(threadedTask, priority);

            action.SafeInvoke();
        }
    }
}
