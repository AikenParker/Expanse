using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Component related extension methods.
    /// </summary>
    public static class ComponentExt
    {
        /// <summary>
        /// Copies over field and property values from one component to another.
        /// </summary>
        /// <param name="copyTo">Copied values are applied to this component.</param>
        /// <param name="copyFrom">Values are copied from this component.</param>
        /// <param name="useCache">If true the data required to copy the values are cached.</param>
        /// <returns>The source object with newly changed properties and fields.</returns>
        public static void CopyComponent<T>(this T copyTo, T copyFrom, bool useCache = true) where T : Component
        {
            ComponentUtil.CopyComponent(copyTo, copyFrom, useCache);
        }
    }
}
