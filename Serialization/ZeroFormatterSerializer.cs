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
        public TTarget Deserialize<TTarget>(byte[] data) where TTarget : new()
        {
            return ZeroFormatter.ZeroFormatterSerializer.Deserialize<TTarget>(data);
        }

        public byte[] Serialize<TSource>(TSource obj)
        {
            return ZeroFormatter.ZeroFormatterSerializer.Serialize(obj);
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int Serialize<TSource>(TSource obj, ref byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
