using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Expanse.Serialization
{
    /// <summary>
    /// Serializer using the BinaryFormatter in System.Runtime.Serialization.Formatters.Binary.
    /// </summary>
    public class BinaryFormatterSerializer : IByteSerializer
    {
        private bool unsafeDeserialize;

        private readonly BinaryFormatter binaryFormatter;

        public BinaryFormatterSerializer()
        {
            binaryFormatter = new BinaryFormatter();
        }

        public BinaryFormatterSerializer(ISurrogateSelector selector, StreamingContext context)
        {
            binaryFormatter = new BinaryFormatter(selector, context);
        }

        public T Deserialize<T>(byte[] data) where T : new()
        {
            using (var ms = new MemoryStream(data))
            {
                if (unsafeDeserialize)
                    return (T)binaryFormatter.UnsafeDeserialize(ms, null);
                else
                    return (T)binaryFormatter.Deserialize(ms);
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public bool UnsafeDeserialize
        {
            get { return unsafeDeserialize; }
            set { unsafeDeserialize = value; }
        }
    }
}
