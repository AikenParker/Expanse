#if ZERO_FORMATTER
using System.Text;

namespace Expanse.Serialization
{
    /// <summary>
    /// Uses ZeroFormatter to serialize data.
    /// <see cref="https://github.com/neuecc/ZeroFormatter"/>
    /// </summary>
    public class ZeroFormatterSerializer : IByteSerializer
    {
        public T Deserialize<T>(byte[] data) where T : new()
        {
            return ZeroFormatter.ZeroFormatterSerializer.Deserialize<T>(data);
        }

        public byte[] Serialize<T>(T obj)
        {
            return ZeroFormatter.ZeroFormatterSerializer.Serialize(obj);
        }
    }
}
#endif
