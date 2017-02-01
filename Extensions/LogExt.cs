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
        public static void LogIterator<Input, Output>(this IEnumerable<Input> source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.LogIterator<Input, Output>(source, logFormat, logType, selector);
        }

        /// <summary>
        /// Logs an object.
        /// </summary>
        [System.Diagnostics.Conditional(LogUtil.CONDITIONAL)]
        public static void Log<Input, Output>(this Input source, string logFormat = null, LogType logType = LogType.Log, Func<Input, Output> selector = null)
        {
            LogUtil.Log<Input, Output>(source, logFormat, logType, selector);
        }
    }
}
