using System;
using System.Collections.Generic;
using System.Diagnostics;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
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
        /// Logs a serialization of a collection object. (Unity Json serializer is default)
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void LogSerializationIterator<Input>(this IEnumerable<Input> source, IStringSerializer serializer = null, LogType logType = LogType.Log)
        {
            LogUtil.LogSerializationIterator(source, serializer, logType);
        }

        /// <summary>
        /// Logs a serialization of an object. (Unity Json serializer is default)
        /// </summary>
        [Conditional(LogUtil.CONDITIONAL)]
        public static void LogSerialization<Input>(this Input source, IStringSerializer serializer = null, LogType logType = LogType.Log)
        {
            LogUtil.LogSerialization(source, serializer, logType);
        }
    }
}
