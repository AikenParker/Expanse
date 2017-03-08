using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// In-built Json serializer from UnityEngine.JsonUtility.
    /// </summary>
    public class UnityJsonUtilitySerializer : ISerializer
    {
        private bool prettyPrint;

        public UnityJsonUtilitySerializer() { }

        public UnityJsonUtilitySerializer(bool prettyPrint)
        {
            this.prettyPrint = prettyPrint;
        }

        string ISerializer.Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }

        T ISerializer.Deserialize<T>(string data)
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
