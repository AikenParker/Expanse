using UnityEngine;

namespace Expanse.Serialization
{
    /// <summary>
    /// In-built Json serializer from UnityEngine.JsonUtility.
    /// </summary>
    public class UnityJsonUtilitySerializer : IStringSerializer
    {
        private bool prettyPrint;

        public UnityJsonUtilitySerializer() { }

        public UnityJsonUtilitySerializer(bool prettyPrint)
        {
            this.prettyPrint = prettyPrint;
        }

        public string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }

        public T Deserialize<T>(string data) where T : new()
        {
            return JsonUtility.FromJson<T>(data);
        }

        public bool PrettyPrint
        {
            get { return prettyPrint; }
            set { prettyPrint = value; }
        }
    }
}
