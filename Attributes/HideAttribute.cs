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
        public bool ShowInPlayMode { get; set; }
        public bool ShowInEditor { get; set; }

        public HideAttribute()
        {
            order = 20;
        }
    }
}