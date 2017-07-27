using UnityEngine;

namespace Expanse.Serialization
{
    /// <summary>
    /// Uses the in-built Json serializer from UnityEngine.JsonUtility to serialize and deserialize data.
    /// </summary>
    public class UnityJsonSerializer : IStringSerializer
    {
        private bool prettyPrint;

        public UnityJsonSerializer() { }

        /// <param name="prettyPrint">If true the Json serialization will be indented and use newlines</param>
        public UnityJsonSerializer(bool prettyPrint)
        {
            this.prettyPrint = prettyPrint;
        }

        /// <summary>
        /// Serializes a source object.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <returns>Returns serialized data.</returns>
        public string Serialize<TSource>(TSource obj)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }

        /// <summary>
        /// Deserializes data into a target object.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="data">Serialized object data.</param>
        /// <returns>Returns a deserialized instance object.</returns>
        public TTarget Deserialize<TTarget>(string data) where TTarget : new()
        {
            return JsonUtility.FromJson<TTarget>(data);
        }

        /// <summary>
        /// If true the Json serialization will be indented and use newlines.
        /// </summary>
        public bool PrettyPrint
        {
            get { return prettyPrint; }
            set { prettyPrint = value; }
        }
    }
}
