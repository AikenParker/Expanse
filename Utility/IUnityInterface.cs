using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// Base class for Unity interfaces. Allows for conversion between interface types and Unity types.
    /// </summary>
    public interface IUnityInterface
    {
        /// <summary>
        /// The game object this component is attached to. A component is always attached to a game object.
        /// </summary>
        GameObject gameObject { get; }
    }
}