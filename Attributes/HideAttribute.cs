using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Not shown in inspector unless specified otherwise.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HideAttribute : PropertyAttribute
    {
        public bool showInPlayMode { get; set; }
        public bool showInEditor { get; set; }
    }
}