#if JSON_NET
using System;
using Newtonsoft.Json;

namespace Expanse.Serialization
{
    // <summary>
    /// Uses Newtonsoft.Json.NET to serialize and deserialize data.
    /// </summary>
    public class NewtonsoftJsonNetSerializer : IStringSerializer
    {
        private JsonSerializerSettings settings;
        private Formatting formatting;

        public NewtonsoftJsonNetSerializer() : this(null, Formatting.None) { }

        /// <param name="settings">Settings for the serializer and deserializer to use.</param>
        /// <param name="formatting">Formatting rules to use when serializing data.</param>
        public NewtonsoftJsonNetSerializer(JsonSerializerSettings settings, Formatting formatting)
        {
            this.settings = settings ?? new JsonSerializerSettings();
            this.formatting = formatting;
        }

        /// <summary>
        /// Serializes a source object.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <returns>Returns serialized data.</returns>
        public string Serialize<TSource>(TSource obj)
        {
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(string data) where TTarget : new()
        {
            return JsonConvert.DeserializeObject<TTarget>(data, settings);
        }

        /// <summary>
        /// Settings for the serializer and deserializer to use.
        /// </summary>
        public JsonSerializerSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        /// <summary>
        /// Formatting rules to use when serializing data.
        /// </summary>
        public Formatting Formatting
        {
            get { return formatting; }
            set { formatting = value; }
        }
    }
}
#endif
