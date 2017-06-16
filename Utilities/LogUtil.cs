using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Expanse.Serialization;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Debug.Log related utility functionality.
    /// </summary>
    public static class LogUtil
    {
        internal const string CONDITIONAL = "UNITY_EDITOR";

        private static List<string> logCache = new List<string>();

        private static string nullFormatTag = "{null}";
        private static string itemFormatTag = "{item}";
        private static string indexFormatTag = "{index}";
        private static string typeFormatTag = "{type}";
        private static string timeFormatTag = "{time}";
        private static Func<string> timeCallback = () => TimeManager.Time.ToString();
        private static bool cacheLogs = false;

        static LogUtil()
        {
            CombineIteratorLog = true;
            StackTraceEnabled = true;
            IndexPadding = true;
            ZeroBasedIndex = false;
        }

        /// <summary>
        /// Gets or sets a string representation of a null value when logged.
        /// </summary>
        public static string NullFormatTag
        {
            get { return nullFormatTag; }
            set { nullFormatTag = value; }
        }

        /// <summary>
        /// Gets or sets a string tag used to represent item string in the log format.
        /// </summary>
        public static string ItemFormatTag
        {
            get { return itemFormatTag; }
            set { itemFormatTag = value; }
        }

        /// <summary>
        /// Gets or sets a string tag used to represent and item index in the iterator log format.
        /// </summary>
        public static string IndexFormatTag
        {
            get { return indexFormatTag; }
            set { indexFormatTag = value; }
        }

        /// <summary>
        /// Gets or sets a string tag used to represent item type in the log format.
        /// </summary>
        public static string TypeFormatTag
        {
            get { return typeFormatTag; }
            set { typeFormatTag = value; }
        }

        /// <summary>
        /// Gets or sets a string tag used to represent the time of logging.
        /// </summary>
        public static string TimeFormatTag
        {
            get { return timeFormatTag; }
            set { timeFormatTag = value; }
        }

        /// <summary>
        /// Time callback used when a log contains the time format tag.
        /// </summary>
        public static Func<string> TimeCallback
        {
            set { timeCallback = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies if LogIterator should be combined into a single log or one foreach item.
        /// </summary>
        public static bool CombineIteratorLog { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies if the stack trace should be shown. (False overrides stackTraceLogType parameter)
        /// </summary>
        public static bool StackTraceEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies if the index value should show padded out digits in iterator logging.
        /// </summary>
        public static bool IndexPadding { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies if the index value should be zero-based in iterator logging.
        /// </summary>
        public static bool ZeroBasedIndex { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies if we should cache the logs.
        /// </summary>
        public static bool CacheLogs
        {
            get { return cacheLogs; }
            set
            {
                if (cacheLogs != value)
                {
                    cacheLogs = value;

                    if (cacheLogs)
                        Application.logMessageReceived += OnApplicationLogReceived;
                    else
                        Application.logMessageReceived -= OnApplicationLogReceived;
                }
            }
        }

        /// <summary>
        /// Simply displays a string representation of an item.
        /// </summary>
        public static string DefaultLogFormat
        {
            get { return string.Format("{0}\n", itemFormatTag); }
        }

        /// <summary>
        /// Simply displays a string representation of an item and its index.
        /// </summary>
        public static string DefaultIteratorLogFormat
        {
            get { return string.Format("{0}: {1}\n", indexFormatTag, itemFormatTag); }
        }

        /// <summary>
        /// Displays time, item and type.
        /// </summary>
        public static string ExtendedLogFormat
        {
            get { return string.Format("[{0}] {1} ({2})\n", timeFormatTag, itemFormatTag, typeFormatTag); }
        }

        /// <summary>
        /// Displays time, item, index and type.
        /// </summary>
        public static string ExtendedIteratorLogFormat
        {
            get { return string.Format("[{0}] {1}: {2} ({3})\n", timeFormatTag, indexFormatTag, itemFormatTag, typeFormatTag); }
        }

        /// <summary>
        /// Logs a collection of objects.
        /// </summary>
        /// <typeparam name="Input">Type of the objects that gets input.</typeparam>
        /// <typeparam name="Output">Type of the objects that gets logged.</typeparam>
        /// <param name="source">Source objects to be logged.</param>
        /// <param name="logFormat">Describes the formatting of the log.</param>
        /// <param name="logType">Type of log.</param>
        /// <param name="selector">Converts the input object into the output object.</param>
        [Conditional(CONDITIONAL), DebuggerHidden]
        public static void LogIterator<Input, Output>(IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            logFormat = logFormat ?? DefaultIteratorLogFormat;

            int index = ZeroBasedIndex ? 0 : 1;
            int sourceCount = source.Count();
            int indexPaddingCount = IndexPadding ? sourceCount.ToString().Length : 0;
            string indexFormat = string.Format("D{0}", indexPaddingCount);

            bool hasSelector = selector != null;
            bool containsItemFormatTag = logFormat.Contains(itemFormatTag);
            bool containsIndexFormatTag = logFormat.Contains(indexFormatTag);
            bool containsTypeFormatTag = logFormat.Contains(typeFormatTag);
            bool containsTimeFormatTag = logFormat.Contains(timeFormatTag);

            if (CombineIteratorLog)
            {
                StringBuilder logBuilder = new StringBuilder(sourceCount);

                foreach (Input item in source)
                {
                    string message = logFormat;

                    if (containsItemFormatTag)
                    {
                        string sourceStr = item != null ? (hasSelector ? selector(item).ToString() : item.ToString()) : nullFormatTag;

                        message = message.Replace(itemFormatTag, sourceStr);
                    }

                    if (containsIndexFormatTag)
                    {
                        string indexStr = index.ToString(indexFormat);
                        message = message.Replace(indexFormatTag, indexStr);
                    }

                    if (containsTypeFormatTag)
                    {
                        string typeStr = item != null ? item.GetType().FullName : (hasSelector ? typeof(Output).FullName : typeof(Input).FullName);
                        message = message.Replace(typeFormatTag, typeStr);
                    }

                    if (containsTimeFormatTag)
                    {
                        string timeStr = timeCallback();
                        message = message.Replace(timeFormatTag, timeStr);
                    }

                    logBuilder.Append(message);
                    index++;
                }

                LogImpl(logBuilder.ToString(), source as UnityEngine.Object, logType);
            }
            else
            {
                foreach (Input item in source)
                {
                    string message = logFormat;

                    if (containsItemFormatTag)
                    {
                        string sourceStr = item != null ? (hasSelector ? selector(item).ToString() : item.ToString()) : nullFormatTag;

                        message = message.Replace(itemFormatTag, sourceStr);
                    }

                    if (containsIndexFormatTag)
                    {
                        string indexStr = index.ToString(indexFormat);
                        message = message.Replace(indexFormatTag, indexStr);
                    }

                    if (containsTypeFormatTag)
                    {
                        string typeStr = item != null ? item.GetType().FullName : (hasSelector ? typeof(Output).FullName : typeof(Input).FullName);
                        message = message.Replace(typeFormatTag, typeStr);
                    }

                    if (containsTimeFormatTag)
                    {
                        string timeStr = timeCallback();
                        message = message.Replace(timeFormatTag, timeStr);
                    }

                    LogImpl(message, item as UnityEngine.Object, logType);

                    index++;
                }
            }
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        /// <typeparam name="Input">Type of the object that gets input.</typeparam>
        /// <typeparam name="Output">Type of the object that gets logged.</typeparam>
        /// <param name="source">Source object to be logged.</param>
        /// <param name="logFormat">Describes the formatting of the log.</param>
        /// <param name="logType">Type of log.</param>
        /// <param name="selector">Converts the input object into the output object.</param>
        [Conditional(CONDITIONAL), DebuggerHidden]
        public static void Log<Input, Output>(Input source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            logFormat = logFormat ?? DefaultLogFormat;

            string message = logFormat;

            if (message.Contains(itemFormatTag))
            {
                bool hasSelector = selector != null;
                string sourceStr = source != null ? (hasSelector ? selector(source).ToString() : source.ToString()) : nullFormatTag;

                message = message.Replace(itemFormatTag, sourceStr);
            }

            if (message.Contains(typeFormatTag))
            {
                string typeStr = selector != null ? typeof(Output).FullName : typeof(Input).FullName;
                message = message.Replace(typeFormatTag, typeStr);
            }

            if (message.Contains(timeFormatTag))
            {
                string timeStr = timeCallback();
                message = message.Replace(timeFormatTag, timeStr);
            }

            LogImpl(message, source as UnityEngine.Object, logType);
        }

        /// <summary>
        /// Logs a serialization of a collection object. (Unity Json serializer is default)
        /// </summary>
        /// <typeparam name="Input">Type of the objects that gets input.</typeparam>
        /// <param name="source">Source objects to be logged.</param>
        /// <param name="serializer">Serializes the source objects into a string to be logged.</param>
        /// <param name="logType">Type of log.</param>
        [Conditional(CONDITIONAL), DebuggerHidden]
        public static void LogSerializationIterator<Input>(IEnumerable<Input> source, IStringSerializer serializer = null, LogType logType = LogType.Log)
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            if (CombineIteratorLog)
            {
                StringBuilder logBuilder = new StringBuilder(source.Count());

                foreach (Input item in source)
                {
                    string message = serializer.Serialize(item);

                    logBuilder.Append(message);
                }

                LogImpl(logBuilder.ToString(), source as UnityEngine.Object, logType);
            }
            else
            {
                foreach (Input item in source)
                {
                    string message = serializer.Serialize(item);

                    LogImpl(message, item as UnityEngine.Object, logType);
                }
            }
        }

        /// <summary>
        /// Logs a serialization of an object. (Unity Json serializer is default)
        /// </summary>
        /// <typeparam name="Input">Type of the object that gets input.</typeparam>
        /// <param name="source">Source object to be logged.</param>
        /// <param name="serializer">Serializes the source object into a string to be logged.</param>
        /// <param name="logType">Type of log.</param>
        [Conditional(CONDITIONAL), DebuggerHidden]
        public static void LogSerialization<Input>(Input source, IStringSerializer serializer = null, LogType logType = LogType.Log)
        {
            serializer = serializer ?? new UnityJsonUtilitySerializer(true);

            string message = serializer.Serialize(source);

            LogImpl(message, source as UnityEngine.Object, logType);
        }

        // Internal logging implementation
        [Conditional(CONDITIONAL), DebuggerHidden]
        private static void LogImpl(string message, UnityEngine.Object context, LogType logType)
        {
            StackTraceLogType previousStackTraceLogType = Application.GetStackTraceLogType(logType);
            Application.SetStackTraceLogType(logType, StackTraceEnabled ? previousStackTraceLogType : StackTraceLogType.None);

            switch (logType)
            {
                case LogType.Log:
                    UnityEngine.Debug.Log(message, context);
                    break;

                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(message, context);
                    break;

                case LogType.Error:
                    UnityEngine.Debug.LogError(message, context);
                    break;

                case LogType.Exception:
                    UnityEngine.Debug.LogException(new UnityException(message), context);
                    break;

                case LogType.Assert:
                    UnityEngine.Debug.LogAssertion(message, context);
                    break;
            }

            Application.SetStackTraceLogType(logType, previousStackTraceLogType);
        }

        private static void OnApplicationLogReceived(string condition, string stackTrace, LogType type)
        {
            logCache.Add(condition);
        }

        /// <summary>
        /// Clears all stored logs in the cache.
        /// </summary>
        [Conditional(CONDITIONAL)]
        public static void ClearLogCache()
        {
            logCache.Clear();
        }

        /// <summary>
        /// Outputs all logs in the cache to a file.
        /// </summary>
        /// <param name="filePath">Filepath to write the log cache to.</param>
        [Conditional(CONDITIONAL)]
        public static void WriteLogCache(string filePath)
        {
            System.IO.File.WriteAllLines(filePath, logCache.ToArray());
        }
    }
}
