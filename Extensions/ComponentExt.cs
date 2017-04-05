using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of Component related extension methods.
    /// </summary>
    public static class ComponentExt
    {
        /// <summary>
        /// Copies over field and property values from one component to another.
        /// </summary>
        /// <returns>The source object with newly changed properties and fields.</returns>
        public static void CopyComponent<T>(this T source, T other, bool useCache = true) where T : Component
        {
            ComponentUtil.CopyComponent(source, other, useCache);
        }
    }
}
