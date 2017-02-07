using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        [Conditional(LogUtil.CONDITIONAL)]
        public static void LogIterator<Input>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log)
        {
            LogUtil.LogIterator<Input, object>(source, logFormat, logType);
        }

        /// <summary>
        /// Logs a collection of objects.
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void LogIterator<Input, Output>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.LogIterator<Input, Output>(source, logFormat, logType, selector);
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void Log<Input>(this Input source, string logFormat = null, LogType logType = LogType.Log)
        {
            LogUtil.Log<Input, object>(source, logFormat, logType, null);
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void Log<Input, Output>(this Input source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.Log<Input, Output>(source, logFormat, logType, selector);
        }

        /// <summary>
        /// Logs a Json serialization of a collection object.
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void LogJsonIterator<Input>(this IEnumerable<Input> source, LogType logType = LogType.Log)
        {
            LogUtil.LogJsonIterator(source, logType);
        }

        /// <summary>
        /// Logs a Json serialization of an object.
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void LogJson<Input>(this Input source, LogType logType = LogType.Log)
        {
            LogUtil.LogJson(source, logType);
        }
    }
}
