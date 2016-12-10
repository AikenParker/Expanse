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
        public bool EditableInPlayMode { get; set; }
        public bool EditableInEditor { get; set; }

        public ReadOnlyAttribute()
        {
            order = AttributeConstants.Order.READ_ONLY;
        }

        public bool IsReadOnly
        {
            get
            {
                return (Application.isPlaying && !EditableInPlayMode) ||
                  (!Application.isPlaying && !EditableInEditor);
            }
        }
    }
}