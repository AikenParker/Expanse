using System;

namespace Expanse.Serialization.TinySerialization
{
    public unsafe abstract class CustomTypeResolver
    {
        public readonly Type type;
        public abstract int GetSize(object obj); // Warning: structs get boxed (Replace with Span)
        public abstract void Deserialize(byte* data, int offset);
        public abstract void Serialize(byte* data, int offset);
    }
}
