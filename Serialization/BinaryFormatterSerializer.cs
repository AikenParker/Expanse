using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Expanse.Serialization
{
    /// <summary>
    /// Serializer using the BinaryFormatter in System.Runtime.Serialization.Formatters.Binary to serialize and deserialize data.
    /// </summary>
    public class BinaryFormatterSerializer : IByteSerializer
    {
        private bool unsafeDeserialize;

        private readonly BinaryFormatter binaryFormatter;

        public BinaryFormatterSerializer()
        {
            binaryFormatter = new BinaryFormatter();
        }

        /// <param name="selector">Surrogate selector object.</param>
        /// <param name="context">Streaming context object.</param>
        public BinaryFormatterSerializer(ISurrogateSelector selector, StreamingContext context)
        {
            binaryFormatter = new BinaryFormatter(selector, context);
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
                binaryFormatter.Serialize(ms, obj);
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
                binaryFormatter.Serialize(ms, obj);
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
                binaryFormatter.Serialize(ms, obj);
            }

            return buffer.Length;
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(byte[] data) where TTarget : new()
        {
            using (var ms = new MemoryStream(data))
            {
                if (unsafeDeserialize)
                    return (TTarget)binaryFormatter.UnsafeDeserialize(ms, null);
                else
                    return (TTarget)binaryFormatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <param name="offset">Offset at which the target data is.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(byte[] data, int offset) where TTarget : new()
        {
            using (var ms = new MemoryStream(data, offset, data.Length - offset, false))
            {
                if (unsafeDeserialize)
                    return (TTarget)binaryFormatter.UnsafeDeserialize(ms, null);
                else
                    return (TTarget)binaryFormatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// If true unsafe methods of deserialization will be used to improve performance.
        /// </summary>
        public bool UnsafeDeserialize
        {
            get { return unsafeDeserialize; }
            set { unsafeDeserialize = value; }
        }
    }
}
