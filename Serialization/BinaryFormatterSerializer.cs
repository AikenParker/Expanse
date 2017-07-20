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

        public TTarget Deserialize<TTarget>(byte[] data, int offset)
        {
            using (var ms = new MemoryStream(data, offset, data.Length - offset, false))
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
            using (var ms = new MemoryStream(buffer, true))
            {
                binaryFormatter.Serialize(ms, obj);
            }

            return buffer.Length;
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer, int offset)
        {
            using (var ms = new MemoryStream(buffer, offset, buffer.Length - offset))
            {
                binaryFormatter.Serialize(ms, obj);
            }

            return buffer.Length;
        }

        public bool UnsafeDeserialize
        {
            get { return unsafeDeserialize; }
            set { unsafeDeserialize = value; }
        }
    }
}
