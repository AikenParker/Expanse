using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Draws a list or array property as a reorderable list.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReorderableAttribute : PropertyAttribute
    {
        public ReorderableAttribute()
        {
            order = AttributeConstants.Order.REORDERABLE;
        }
    }
}
