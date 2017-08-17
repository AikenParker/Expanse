#if UNSAFE
namespace Expanse.Serialization.TinySerialization
{
    public static class TinySerializerConstants
    {
        public const sbyte HAS_VALUE = 0; // AKA SByte Length
        public const sbyte NULL_VALUE = -1;
        public const sbyte USHORT_LENGTH = -2;
        public const sbyte UINT_LENGTH = -3;
        public const sbyte ULONG_LENGTH = -4;
    }
}
#endif
