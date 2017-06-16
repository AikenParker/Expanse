using System;
using System.Collections.Generic;
using System.Diagnostics;
using Expanse.Serialization;
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
        /// <typeparam name="Input">Type of the objects that gets input.</typeparam>
        /// <param name="source">Source objects to be logged.</param>
        /// <param name="logFormat">Describes the formatting of the log.</param>
        /// <param name="logType">Type of log.</param>
        [Conditional(LogUtil.CONDITIONAL), DebuggerHidden]
        public static void LogIterator<Input>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log)
        {
            LogUtil.LogIterator<Input, object>(source, logFormat, logType);
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
        [Conditional(LogUtil.CONDITIONAL), DebuggerHidden]
        public static void LogIterator<Input, Output>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.LogIterator<Input, Output>(source, logFormat, logType, selector);
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        /// <typeparam name="Input">Type of the object that gets input.</typeparam>
        /// <param name="source">Source object to be logged.</param>
        /// <param name="logFormat">Describes the formatting of the log.</param>
        /// <param name="logType">Type of log.</param>
        [Conditional(LogUtil.CONDITIONAL), DebuggerHidden]
        public static void Log<Input>(this Input source, string logFormat = null, LogType logType = LogType.Log)
        {
            LogUtil.Log<Input, object>(source, logFormat, logType, null);
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
        [Conditional(LogUtil.CONDITIONAL), DebuggerHidden]
        public static void Log<Input, Output>(this Input source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.Log<Input, Output>(source, logFormat, logType, selector);
        }

        /// <summary>
        /// Logs a serialization of a collection object. (Unity Json serializer is default)
        /// </summary>
        /// <typeparam name="Input">Type of the objects that gets input.</typeparam>
        /// <param name="source">Source objects to be logged.</param>
        /// <param name="serializer">Serializes the source objects into a string to be logged.</param>
        /// <param name="logType">Type of log.</param>
        [Conditional(LogUtil.CONDITIONAL), DebuggerHidden]
        public static void LogSerializationIterator<Input>(this IEnumerable<Input> source, IStringSerializer serializer = null, LogType logType = LogType.Log)
        {
            LogUtil.LogSerializationIterator(source, serializer, logType);
        }

        /// <summary>
        /// Logs a serialization of an object. (Unity Json serializer is default)
        /// </summary>
        /// <typeparam name="Input">Type of the object that gets input.</typeparam>
        /// <param name="source">Source object to be logged.</param>
        /// <param name="serializer">Serializes the source object into a string to be logged.</param>
        /// <param name="logType">Type of log.</param>
        [Conditional(LogUtil.CONDITIONAL), DebuggerHidden]
        public static void LogSerialization<Input>(this Input source, IStringSerializer serializer = null, LogType logType = LogType.Log)
        {
            LogUtil.LogSerialization(source, serializer, logType);
        }
    }
}
