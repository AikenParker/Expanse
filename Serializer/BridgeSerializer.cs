using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// Bridges IByteSerializer to IStringSerializer.
    /// </summary>
    public class BridgeSerializer<T> : IStringSerializer where T : IByteSerializer
    {
        private readonly T byteSerializer;
        private Encoding encoding;

        public BridgeSerializer(T byteSerializer)
        {
            this.byteSerializer = byteSerializer;
            this.encoding = Encoding.Default;
        }

        public BridgeSerializer(T byteSerializer, Encoding encoding)
        {
            this.byteSerializer = byteSerializer;
            this.encoding = encoding;
        }

        public U Deserialize<U>(string data) where U : new()
        {
            return byteSerializer.Deserialize<U>(encoding.GetBytes(data));
        }

        public string Serialize<U>(U obj)
        {
            return encoding.GetString(byteSerializer.Serialize(obj));
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public IByteSerializer ByteSerializer
        {
            get { return byteSerializer; }
        }
    }
}
