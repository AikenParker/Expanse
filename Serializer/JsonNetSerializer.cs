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
        public U Deserialize<U>(string data) where U : new()
        {
            return JsonConvert.DeserializeObject<U>(data);
        }

        public string Serialize<U>(U obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
#endif
