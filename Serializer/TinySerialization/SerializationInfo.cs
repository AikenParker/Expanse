using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Expanse.TinySerialization
{
    public struct SerializationInfo
    {
        private BindingFlags bindingFlags;
        private StringTypeResolver.StringResolutionType stringResolutionType;
        private DecimalTypeResolver.DecimalResolutionType decimalResolutionType;
        private bool emitReflection;

        public StringTypeResolver.StringResolutionType StringResolutionType
        {
            get { return stringResolutionType; }
            set { stringResolutionType = value; }
        }

        public DecimalTypeResolver.DecimalResolutionType DecimalResolutionType
        {
            get { return decimalResolutionType; }
            set { decimalResolutionType = value; }
        }

        public BindingFlags BindingFlags
        {
            get { return bindingFlags; }
            set { bindingFlags = value; }
        }

        public bool EmitReflection
        {
            get { return emitReflection; }
            set { emitReflection = value; }
        }

        public static SerializationInfo Default
        {
            get
            {
                return new SerializationInfo()
                {
                    BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    StringResolutionType = StringTypeResolver.StringResolutionType.PREDEFINED_LENGTH,
                    DecimalResolutionType = DecimalTypeResolver.DecimalResolutionType.UNION,
                    EmitReflection = false
                };
            }
        }
    }
}
