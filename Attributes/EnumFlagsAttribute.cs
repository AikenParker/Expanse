using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Enables multiple flags to be raised in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute()
        {
            order = AttributeConstants.Order.ENUM_FLAGS;
        }
    }
}