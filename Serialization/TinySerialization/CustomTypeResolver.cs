#if UNSAFE

using System;

namespace Expanse.Serialization.TinySerialization
{
    /// <summary>
    /// Custom serialization/deserialization for specified types by TinySerializer.
    /// </summary>
    public unsafe abstract class CustomTypeResolver
    {
        public readonly Type type;
        public readonly int typeHashCode;

        public CustomTypeResolver(Type type)
        {
            this.type = type;
            this.typeHashCode = type.TypeHandle.Value.ToInt32();
        }

        public abstract int GetSize(TypedReference objRef);
        public abstract int GetSize(object obj);
        public abstract void Serialize(TypedReference objRef, byte* data, int offset);
        public abstract void Serialize(object obj, byte* data, int offset);
        public abstract object Deserialize(byte* data, int offset);
    }

    /// <summary>
    /// Generic custom serialization/deserialization for specified types by TinySerializer.
    /// </summary>
    /// <typeparam name="T">Custom type to provide serialization/deserialization for.</typeparam>
    public unsafe abstract class CustomTypeResolver<T> : CustomTypeResolver
    {
        public CustomTypeResolver() : base(typeof(T)) { }

        public override int GetSize(TypedReference objRef)
        {
            return OnGetSize(__refvalue(objRef, T));
        }

        public override int GetSize(object obj)
        {
            return GetSize(obj);
        }

        public override unsafe void Serialize(TypedReference objRef, byte* data, int offset)
        {
            OnSerialize(__refvalue(objRef, T), data, offset);
        }

        public override unsafe void Serialize(object obj, byte* data, int offset)
        {
            OnSerialize((T)obj, data, offset);
        }

        public override unsafe object Deserialize(byte* data, int offset)
        {
            return OnDeserialize(data, offset);
        }

        public abstract int OnGetSize(T obj);
        public abstract int OnSerialize(T obj, byte* data, int offset);
        public abstract T OnDeserialize(byte* data, int offset);
    }
}
#endif
