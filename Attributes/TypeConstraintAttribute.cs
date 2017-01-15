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

        private TypeConstraintAttribute()
        {
            this.order = AttributeConstants.Order.TYPE_CONSTRAINT;
        }

        public TypeConstraintAttribute(Type baseType, bool nonAbstractOnly = false) : this()
        {
            this.BaseType = baseType;
            this.NonAbstractOnly = nonAbstractOnly;
        }
    }
}
