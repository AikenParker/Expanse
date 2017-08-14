#if UNSAFE

using System;

namespace Expanse.Serialization.TinySerialization
{
    /// <summary>
    /// Custom serialization/deserialization for specified types by TinySerializer.
    /// </summary>
    /// <remarks>
    /// Warning: ValueTypes get boxed and unboxed
    /// </remarks>
    public unsafe abstract class CustomTypeResolver
    {
        public readonly Type type;
        public abstract int GetSize(object obj);
        public abstract object Deserialize(byte* data, int offset);
        public abstract void Serialize(object obj, byte* data, int offset);
    }
}
#endif
