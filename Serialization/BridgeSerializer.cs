using System.Text;

namespace Expanse.Serialization
{
    /// <summary>
    /// Bridges an IByteSerializer to an IStringSerializer.
    /// </summary>
    public class BridgeSerializer<T> : IStringSerializer where T : IByteSerializer
    {
        private readonly T byteSerializer;
        private Encoding encoding;

        /// <param name="byteSerializer">Byte serializer to wrap around.</param>
        public BridgeSerializer(T byteSerializer)
        {
            this.byteSerializer = byteSerializer;
            this.encoding = Encoding.Default;
        }

        /// <param name="byteSerializer">Byte serializer to wrap around.</param>
        /// <param name="encoding">Encoding to use when serializing and deserializing the byte data.</param>
        public BridgeSerializer(T byteSerializer, Encoding encoding)
        {
            this.byteSerializer = byteSerializer;
            this.encoding = encoding;
        }

        /// <summary>
        /// Serializes a source object.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <returns>Returns serialized data.</returns>
        public string Serialize<TSource>(TSource obj)
        {
            return encoding.GetString(byteSerializer.Serialize(obj));
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(string data)
        {
            return byteSerializer.Deserialize<TTarget>(encoding.GetBytes(data));
        }

        /// <summary>
        /// Encoding to use when serializing and deserializing the byte data
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }
    }
}
