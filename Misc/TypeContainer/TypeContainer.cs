﻿using System;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Serailizable wrapper for System.Type.
    /// </summary>
    [Serializable]
    public class TypeContainer : ISerializationCallbackReceiver
    {
        private Type type;

        public Type Type
        {
            get { return type; }
            set
            {
                type = value;
                typeName = type != null ? type.FullName : ReflectionUtil.NULL_TYPE_NAME;
            }
        }

        [SerializeField, HideInInspector]
        public string typeName = ReflectionUtil.NULL_TYPE_NAME;

        public TypeContainer() { }

        public TypeContainer(Type type)
        {
            this.Type = type;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (type != null)
            {
                typeName = type.FullName;
            }
            else
            {
                typeName = ReflectionUtil.NULL_TYPE_NAME;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!typeName.Equals(ReflectionUtil.NULL_TYPE_NAME))
            {
                type = ReflectionUtil.GetTypeFromName(typeName);
            }
            else
            {
                type = null;
            }
        }

        public static implicit operator Type(TypeContainer typeContainer)
        {
            return typeContainer.Type;
        }

        public static implicit operator TypeContainer(Type type)
        {
            return new TypeContainer(type);
        }
    }
}
