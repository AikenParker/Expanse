using System;
using System.Collections;
using System.IO;
using Expanse.Serialization;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of UnityEngine.MonoBehaviour related extension methods.
    /// </summary>
    public static class MonoBehaviourExt
    {
        /// <summary>
        /// Waits one frame before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitForEndOfFrame(this MonoBehaviour monoBehaviour, Action action)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForEndOfFrame(action));
        }

        /// <summary>
        /// Waits a given number of scaled seconds before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <param name="delay">Time in scaled seconds to wait before invoking the action.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitForSeconds(this MonoBehaviour monoBehaviour, Action action, float delay)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForSeconds(action, delay));
        }

        /// <summary>
        /// Waits until the next fixed update step before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitForFixedUpdate(this MonoBehaviour monoBehaviour, Action action)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForFixedUpdate(action));
        }

        /// <summary>
        /// Waits a given number of unscaled seconds before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <param name="delay">Time in real seconds to wait before invoking the action.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitForSecondsRealtime(this MonoBehaviour monoBehaviour, Action action, float delay)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForSecondsRealtime(action, delay));
        }

        /// <summary>
        /// Waits until a predicate returns true before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <param name="predicate">Predicate to be checked before invoking the action.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitUntil(this MonoBehaviour monoBehaviour, Action action, Func<bool> predicate)
        {
            return monoBehaviour.StartCoroutine(Co_WaitUntil(action, predicate));
        }

        /// <summary>
        /// Waits until a predicate returns false before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <param name="predicate">Predicate to be checked before invoking the action.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitWhile(this MonoBehaviour monoBehaviour, Action action, Func<bool> predicate)
        {
            return monoBehaviour.StartCoroutine(Co_WaitWhile(action, predicate));
        }

        /// <summary>
        /// Waits for the custom yeild instruction before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <param name="customYieldInstruction">Custom yield instruction to wait on before invoking the action.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitCustom(this MonoBehaviour monoBehaviour, Action action, CustomYieldInstruction customYieldInstruction)
        {
            return monoBehaviour.StartCoroutine(Co_WaitCustom(action, customYieldInstruction));
        }

        /// <summary>
        /// Waits for a threaded task to complete before invoking an action.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine WaitForThreadedTask(this MonoBehaviour monoBehaviour, Action action, Action threadedTask, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            return monoBehaviour.StartCoroutine(Co_WaitForThreadedTask(threadedTask, action, priority));
        }

        /// <summary>
        /// Loads an object in another thread from a file using a deserializer.
        /// </summary>
        /// <typeparam name="T">Type of object to load file data into.</typeparam>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="filePath">File location of the file to load from.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Deserializes the data in the file into an object. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the loading executes on.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine LoadFromFileThreaded<T>(this MonoBehaviour monoBehaviour, string filePath, Action<Utilities.ApplicationUtil.FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            return monoBehaviour.StartCoroutine(Utilities.ApplicationUtil.Co_LoadFromFileThreaded(filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Loads an object in another thread from a file in the StreamingAssets folder using a deserializer.
        /// </summary>
        /// <typeparam name="T">Type of object to load file data into.</typeparam>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="filePath">File location of the file in the SteamingAssets folder to load from.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Deserializes the data in the file into an object. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the loading executes on.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine LoadFromStreamingAssetFileThreaded<T>(this MonoBehaviour monoBehaviour, string filePath, Action<Utilities.ApplicationUtil.FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            filePath = Path.Combine(Utilities.ApplicationUtil.streamingAssetsPath, filePath);

            return monoBehaviour.StartCoroutine(Utilities.ApplicationUtil.Co_LoadFromFileThreaded(filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Loads an object in another thread from a file in the PersistentData folder using a deserializer.
        /// </summary>
        /// <typeparam name="T">Type of object to load file data into.</typeparam>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="filePath">File location of the file in the PersistentDataFile folder to load from.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Deserializes the data in the file into an object. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the loading executes on.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine LoadFromPersistentDataFileThreaded<T>(this MonoBehaviour monoBehaviour, string filePath, Action<Utilities.ApplicationUtil.FileLoadInfo<T>> onComplete, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) where T : new()
        {
            filePath = Path.Combine(Utilities.ApplicationUtil.persistentDataPath, filePath);

            return monoBehaviour.StartCoroutine(Utilities.ApplicationUtil.Co_LoadFromFileThreaded(filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Saves an object in another thread into a file using a serializer.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="obj">Object instance to save.</param>
        /// <param name="filePath">File location to save the object into.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Serializes the object into a data format that can be saved. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the saving executes on.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine SaveToFileThreaded(this MonoBehaviour monoBehaviour, object obj, string filePath, Action<Utilities.ApplicationUtil.FileSaveInfo> onComplete = null, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            return monoBehaviour.StartCoroutine(Utilities.ApplicationUtil.Co_SaveToFileThreaded(obj, filePath, onComplete, serializer, priority));
        }

        /// <summary>
        /// Saves an object in another thread into a file in the PersistentData folder using a serializer.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehavior to attach the coroutine to.</param>
        /// <param name="obj">Object instance to save.</param>
        /// <param name="filePath">File location in the PersistentDataFile folder to save the object into.</param>
        /// <param name="onComplete">Callback used when this operation is complete.</param>
        /// <param name="serializer">Serializes the object into a data format that can be saved. (Default = UnityJsonUtilitySerializer)</param>
        /// <param name="priority">Priority of the thread the saving executes on.</param>
        /// <returns>Coroutine container object.</returns>
        public static Coroutine SaveToPersistentDataFileThreaded(this MonoBehaviour monoBehaviour, object obj, string filePath, Action<Utilities.ApplicationUtil.FileSaveInfo> onComplete = null, IStringSerializer serializer = null, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            filePath = Path.Combine(Utilities.ApplicationUtil.persistentDataPath, filePath);

            return monoBehaviour.StartCoroutine(Utilities.ApplicationUtil.Co_SaveToFileThreaded(obj, filePath, onComplete, serializer, priority));
        }

        #region INTERNAL_COROUTINES

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

        #endregion
    }
}
