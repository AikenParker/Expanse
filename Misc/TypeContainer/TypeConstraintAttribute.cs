using Expanse;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Allows type constraints to be applied to TypeContainer fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeConstraintAttribute : PropertyAttribute
    {
        public Type BaseType { get; private set; }
        public bool NonAbstractOnly { get; private set; }

        private TypeConstraintAttribute() { }

        public TypeConstraintAttribute(Type baseType, bool nonAbstractOnly = false)
        {
            this.BaseType = baseType;
            this.NonAbstractOnly = nonAbstractOnly;
        }
    }
}
