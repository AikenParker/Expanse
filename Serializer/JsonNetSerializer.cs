#if JSON_NET
using System;
using Newtonsoft.Json;

namespace Expanse
{
    // <summary>
    /// Uses Newtonsoft.Json.NET to serialize data.
    /// </summary>
    public class JsonNetSerializer : IStringSerializer
    {
        private JsonSerializerSettings settings;
        private Formatting formatting;

        public JsonNetSerializer() : this(null, Formatting.None) { }

        public JsonNetSerializer(JsonSerializerSettings settings, Formatting formatting)
        {
            this.settings = settings ?? new JsonSerializerSettings();
            this.formatting = formatting;
        }

        public U Deserialize<U>(string data) where U : new()
        {
            return JsonConvert.DeserializeObject<U>(data, settings);
        }

        public string Serialize<U>(U obj)
        {
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        public JsonSerializerSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public Formatting Formatting
        {
            get { return formatting; }
            set { formatting = value; }
        }
    }
}
#endif
