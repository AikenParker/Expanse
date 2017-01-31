using System;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection Debug.Log related extension methods.
    /// </summary>
    public static class LogExt
    {
        /// <summary>
        /// Logs a collection of objects.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void LogIterator<T>(this IEnumerable<T> source, string logFormat = null, LogType logType = LogType.Log)
        {
            LogUtil.LogIterator<T, object>(source, logFormat, logType, null, Application.GetStackTraceLogType(logType));
        }

        /// <summary>
        /// Logs a collection of objects.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void LogIterator<Input, Output>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.LogIterator<Input, Output>(source, logFormat, logType, selector, Application.GetStackTraceLogType(logType));
        }

        /// <summary>
        /// Logs a collection of objects.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void LogIterator<Input, Output>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null, StackTraceLogType stackTraceLogType = StackTraceLogType.ScriptOnly)
        {
            LogUtil.LogIterator<Input, Output>(source, logFormat, logType, selector, stackTraceLogType);
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void Log<T>(this T source, string logFormat = null, LogType logType = LogType.Log)
        {
            LogUtil.Log<T, object>(source, logFormat, logType, null, Application.GetStackTraceLogType(logType));
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void Log<Input, Output>(this Input source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.Log<Input, Output>(source, logFormat, logType, selector, Application.GetStackTraceLogType(logType));
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void Log<Input, Output>(this Input source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null, StackTraceLogType stackTraceLogType = StackTraceLogType.ScriptOnly)
        {
            LogUtil.Log<Input, Output>(source, logFormat, logType, selector, stackTraceLogType);
        }
    }
}
