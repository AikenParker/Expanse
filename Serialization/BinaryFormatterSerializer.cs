using System;
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

        public byte[] Serialize<TSource>(TSource obj)
        {
            using (var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        public bool UnsafeDeserialize
        {
            get { return unsafeDeserialize; }
            set { unsafeDeserialize = value; }
        }
    }
}
