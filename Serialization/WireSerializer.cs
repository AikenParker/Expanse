#if WIRE
using System.IO;
using Wire;

namespace Expanse.Serialization
{
    /// <summary>
    /// Uses WireSerializtion to serialize data.
    /// <see cref="https://github.com/rogeralsing/Wire"/>
    /// </summary>
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

        /// <param name="options">Options to set the serializer to.</param>
        public WireSerializer(SerializerOptions options)
        {
            serializer = new Serializer(options);
            serializerSession = serializer.GetSerializerSession();
            deserializerSession = serializer.GetDeserializerSession();
        }

        /// <summary>
        /// Serializes a source object.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <returns>Returns serialized data.</returns>
        public byte[] Serialize<TSource>(TSource obj)
        {
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(obj, ms, serializerSession);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serializes a source object into a buffer.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <param name="buffer">Byte array to serialize into.</param>
        /// <returns>Returns the length of the serialized data.</returns>
        public int Serialize<TSource>(TSource obj, ref byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer, true))
            {
                serializer.Serialize(obj, ms, serializerSession);
            }

            return buffer.Length;
        }

        /// <summary>
        /// Serializes a source object into a buffer.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <param name="buffer">Byte array to serialize into.</param>
        /// <param name="offset">Offset to write into the buffer.</param>
        /// <returns>Returns the length of the serialized data plus the offset.</returns>
        public int Serialize<TSource>(TSource obj, ref byte[] buffer, int offset)
        {
            using (var ms = new MemoryStream(buffer, offset, buffer.Length - offset))
            {
                serializer.Serialize(obj, ms, serializerSession);
            }

            return buffer.Length;
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(byte[] data)
        {
            using (var ms = new MemoryStream(data, false))
            {
                return serializer.Deserialize<TTarget>(ms, deserializerSession);
            }
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <param name="offset">Offset at which the target data is.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(byte[] data, int offset)
        {
            using (var ms = new MemoryStream(data, offset, data.Length - offset, false))
            {
                return serializer.Deserialize<TTarget>(ms, deserializerSession);
            }
        }
    }
}

#endif
