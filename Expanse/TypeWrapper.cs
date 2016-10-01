using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    [System.Serializable]
    public class TypeWrapper
    {
        public Type type;

        public TypeWrapper(Type typeVal)
        {
            type = typeVal;
        }

        public static implicit operator Type(TypeWrapper typeWrapperVal)
        {
            return typeWrapperVal.type;
        }

        public static implicit operator TypeWrapper(Type typeVal)
        {
            return new TypeWrapper(typeVal);
        }
    }
}