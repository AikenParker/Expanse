#if WIRE

using System.IO;
using Wire;

namespace Expanse.Serialization
{
    public sealed class WireSerializer : IByteSerializer
    {
        private Serializer serializer;
        private SerializerSession serializerSession;
        private DeserializerSession deserializerSession;

        public WireSerializer()
        {
            serializer = new Serializer();
            serializerSession = serializer.GetSerializerSession();
            deserializerSession = serializer.GetDeserializerSession();
        }

        public WireSerializer(SerializerOptions options)
        {
            serializer = new Serializer(options);
            serializerSession = serializer.GetSerializerSession();
            deserializerSession = serializer.GetDeserializerSession();
        }

        public TTarget Deserialize<TTarget>(byte[] data) where TTarget : new()
        {
            using (var ms = new MemoryStream(data, false))
            {
                return serializer.Deserialize<TTarget>(ms, deserializerSession);
            }
        }

        public TTarget Deserialize<TTarget>(byte[] data, int offset) where TTarget : new()
        {
            using (var ms = new MemoryStream(data, offset, data.Length - offset, false))
            {
                return serializer.Deserialize<TTarget>(ms, deserializerSession);
            }
        }

        public byte[] Serialize<TSource>(TSource obj)
        {
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(obj, ms, serializerSession);
                return ms.ToArray();
            }
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer, true))
            {
                serializer.Serialize(obj, ms, serializerSession);
            }

            return buffer.Length;
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer, int offset)
        {
            using (var ms = new MemoryStream(buffer, offset, buffer.Length - offset))
            {
                serializer.Serialize(obj, ms, serializerSession);
            }

            return buffer.Length;
        }
    }
}

#endif
