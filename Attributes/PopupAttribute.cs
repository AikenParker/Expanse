using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expanse.Ext;

namespace Expanse
{
    /// <summary>
    /// Enables a popup drop down selection with specified options. (Supports bool, int and string)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PopupAttribute : PropertyAttribute
    {
        public string[] displayedOptions { get; set; }

        public PopupAttribute(string[] options)
        {
            if (displayedOptions.IsNullOrEmpty())
                throw new NullReferenceException();

            displayedOptions = options;
        }

        public PopupAttribute()
        {
            order = 15;
        }
    }
}