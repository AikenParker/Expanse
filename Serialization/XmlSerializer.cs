using System;
using System.IO;
using System.Text;

namespace Expanse.Serialization
{
    /// <summary>
    /// Uses XmlSerialize to serialize and deserialize data.
    /// </summary>
    public sealed class XmlSerializer : IStringSerializer
    {
        private System.Xml.Serialization.XmlSerializer serializer;
        private StringBuilder serializationStringBuilder;

        /// <param name="type">Known type to serialize/deserialize.</param>
        public XmlSerializer(Type type)
        {
            serializer = new System.Xml.Serialization.XmlSerializer(type);
            serializationStringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Serializes a source object.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <returns>Returns serialized data.</returns>
        public string Serialize<TSource>(TSource obj)
        {
            using (var sw = new StringWriter(serializationStringBuilder))
            {
                serializer.Serialize(sw, obj);
            }

            return serializationStringBuilder.ToString();
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(string data) where TTarget : new()
        {
            using (var sr = new StringReader(data))
            {
                return (TTarget)serializer.Deserialize(sr);
            }
        }
    }
}
