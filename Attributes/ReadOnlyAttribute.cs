using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Shown in inspector as not editable unless specified otherwise.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public bool editableWhilePlaying { get; set; }
        public bool editableInEditor { get; set; }
    }
}