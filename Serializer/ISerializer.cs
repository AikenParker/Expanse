namespace Expanse
{
    /// <summary>
    /// Interface capable of serializing and deserializing objects.
    /// </summary>
    public interface ISerializer<T>
    {
        T Serialize<U>(U obj);
        U Deserialize<U>(T data) where U : new();
    }

    public interface IStringSerializer : ISerializer<string> { }

    public interface IByteSerializer : ISerializer<byte[]> { }
}
