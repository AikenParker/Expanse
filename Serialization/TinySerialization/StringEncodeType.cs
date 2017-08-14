#if UNSAFE

using System;

namespace Expanse.Serialization.TinySerialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class StringEncodeTypeOverrideAttribute : Attribute
    {
        public StringEncodeType stringEncodeType;

        public StringEncodeTypeOverrideAttribute(StringEncodeType stringEncodeType)
        {
            this.stringEncodeType = stringEncodeType;
        }
    }

    public enum StringEncodeType
    {
        Default = 0,
        Char = 1,
        Byte = 2,
        ASCII = 3,
        Unicode = 4,
        UTF7 = 5,
        UTF8 = 6,
        UTF32 = 7,
        BigEndianUnicode = 8
    }
}
#endif
