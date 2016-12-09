using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class IEnumerableExt
    {
        public static void Log<Input, Output>(this IEnumerable<Input> source, string prefix = null, Func<Input, Output> selector = null, LogType logType = LogType.Log)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int index = 0;

            bool hasSelector = selector != null;

            foreach (Input elem in source)
            {
                string elemName = hasSelector ? selector(elem).ToString() : elem.ToString();

                string message;

                if (!string.IsNullOrEmpty(prefix))
                    message = string.Format("{0} | {1} {2}", ++index, prefix, elemName);
                else
                    message = string.Format("{0} | {1}", ++index, elemName);

                switch (logType)
                {
                    case LogType.Log:
                        Debug.Log(message, elem as UnityEngine.Object);
                        break;

                    case LogType.Warning:
                        Debug.LogWarning(message, elem as UnityEngine.Object);
                        break;

                    case LogType.Error:
                        Debug.LogError(message, elem as UnityEngine.Object);
                        break;

                    case LogType.Exception:
                        Debug.LogException(new UnityException(message), elem as UnityEngine.Object);
                        break;

                    case LogType.Assert:
                        Debug.LogAssertion(message, elem as UnityEngine.Object);
                        break;
                }
            }
        }

        /// <summary>
        /// Determines if all objects in souce are not equal to one another.
        /// </summary>
        public static bool IsUnique<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Distinct().Count() != source.Count();
        }

        /// <summary>
        /// Returns true if all items do not equal null.
        /// </summary>
        public static bool All<T>(this IEnumerable<T> source)
        {
            return All(source, (x) => x != null);
        }

        /// <summary>
        /// Returns true if all items meet a condition.
        /// </summary>
        public static bool All<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (T elem in source)
            {
                if (!predicate(elem))
                    return false;
            }

            return true;
        }
    }
}
