using System;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Action related extension methods.
    /// </summary>
    public static class ActionExt
    {
        /// <summary>
        /// Performs an null-check before invoking the action.
        /// </summary>
        /// <param name="action">Action to safely invoke.</param>
        public static void SafeInvoke(this Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Performs an null-check before invoking the action.
        /// </summary>
        /// <param name="action">Action to safely invoke.</param>
        /// <param name="arg1">First argument of the action.</param>
        public static void SafeInvoke<T>(this Action<T> action, T arg1)
        {
            if (action != null)
            {
                action.Invoke(arg1);
            }
        }

        /// <summary>
        /// Performs an null-check before invoking the action.
        /// </summary>
        /// <param name="action">Action to safely invoke.</param>
        /// <param name="arg1">First argument of the action.</param>
        /// <param name="arg2">Second argument of the action.</param>
        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                action.Invoke(arg1, arg2);
            }
        }

        /// <summary>
        /// Performs an null-check before invoking the action.
        /// </summary>
        /// <param name="action">Action to safely invoke.</param>
        /// <param name="arg1">First argument of the action.</param>
        /// <param name="arg2">Second argument of the action.</param>
        /// <param name="arg3">Third argument of the action.</param>
        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                action.Invoke(arg1, arg2, arg3);
            }
        }

        /// <summary>
        /// Performs an null-check before invoking the action.
        /// </summary>
        /// <param name="action">Action to safely invoke.</param>
        /// <param name="arg1">First argument of the action.</param>
        /// <param name="arg2">Second argument of the action.</param>
        /// <param name="arg3">Third argument of the action.</param>
        /// <param name="arg4">Fourth argument of the action.</param>
        public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null)
            {
                action.Invoke(arg1, arg2, arg3, arg4);
            }
        }
    }
}
