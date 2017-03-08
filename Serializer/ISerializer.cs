namespace Expanse
{
    /// <summary>
    /// Interface capable of serializing and deserializing objects.
    /// </summary>
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string data);
    }
}
