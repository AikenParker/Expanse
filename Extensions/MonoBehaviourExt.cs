using System;
using System.Collections;
using System.IO;
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
        /// Loads an object from a file using another thread.
        /// </summary>
        public static Coroutine LoadFromFileThreaded<T>(this MonoBehaviour monoBehaviour, string filePath, Action<ApplicationUtil.FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_LoadFromFileThreaded(filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Loads an object from a file in the StreamingAssets folder using another thread.
        /// </summary>
        public static Coroutine LoadFromStreamingAssetFileThreaded<T>(this MonoBehaviour monoBehaviour, string filePath, Action<ApplicationUtil.FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            filePath = Path.Combine(ApplicationUtil.StreamingAssestsPath, filePath);

            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_LoadFromFileThreaded(filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Loads an object from a file in the PersistentData folder using another thread.
        /// </summary>
        public static Coroutine LoadFromPersistentDataFileThreaded<T>(this MonoBehaviour monoBehaviour, string filePath, Action<ApplicationUtil.FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            filePath = Path.Combine(ApplicationUtil.PersistentDataPath, filePath);

            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_LoadFromFileThreaded(filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Saves an object to a file using another thread.
        /// </summary>
        public static Coroutine SaveToFileThreaded(this MonoBehaviour monoBehaviour, object obj, string filePath, Action<ApplicationUtil.FileSaveInfo> onComplete = null, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_SaveToFileThreaded(obj, filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Saves an object to a file in the PersistentData folder using another thread.
        /// </summary>
        public static Coroutine SaveToPersistentDataFileThreaded(this MonoBehaviour monoBehaviour, object obj, string filePath, Action<ApplicationUtil.FileSaveInfo> onComplete = null, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            filePath = Path.Combine(ApplicationUtil.PersistentDataPath, filePath);

            return monoBehaviour.StartCoroutine(ApplicationUtil.Co_SaveToFileThreaded(obj, filePath, onComplete, serializer, priority));
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
