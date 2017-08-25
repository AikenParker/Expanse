namespace Expanse.Serialization
{
    /// <summary>
    /// Interface capable of serializing and deserializing objects.
    /// </summary>
    public interface ISerializer<TData>
    {
        /// <summary>
        /// Serializes a source object.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <returns>Returns serialized data.</returns>
        TData Serialize<TSource>(TSource obj);

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        TTarget Deserialize<TTarget>(TData data);
    }

    /// <summary>
    /// Interface capable of serializing and deserializing objects to and from strings.
    /// </summary>
    public interface IStringSerializer : ISerializer<string> { }

    /// <summary>
    /// Interface capable of serializing and deserializing objects to and from byte arrays.
    /// </summary>
    public interface IByteSerializer : ISerializer<byte[]>
    {
        /// <summary>
        /// Serializes a source object into a buffer.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <param name="buffer">Byte array to serialize into.</param>
        /// <returns>Returns the length of the serialized data.</returns>
        int Serialize<TSource>(TSource obj, ref byte[] buffer);

        /// <summary>
        /// Serializes a source object into a buffer.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <param name="buffer">Byte array to serialize into.</param>
        /// <param name="offset">Offset to write into the buffer.</param>
        /// <returns>Returns the length of the serialized data plus the offset.</returns>
        int Serialize<TSource>(TSource obj, ref byte[] buffer, int offset);

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <param name="offset">Offset at which the target data is.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        TTarget Deserialize<TTarget>(byte[] data, int offset);
    }
}
