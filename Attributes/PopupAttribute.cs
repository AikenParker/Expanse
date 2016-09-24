using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


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
            if (options.IsNullOrEmpty())
                throw new NullReferenceException();

            displayedOptions = options;

            order = 15;
        }

        protected PopupAttribute() { }
    }
}
