namespace Expanse.Serialization.TinySerialization
{
    public sealed class TinySerializerSettings
    {
        public bool supportCyclicReferences = false;
        public bool requireTypeSerializableAttribute = false;

        public bool serializeFields = true;
        public bool allowPrivateFields = true;
        public bool ignoreObsoleteFields = false;
        public bool checkFieldSerializableAttribute = false;
        public bool checkFieldNonSerializableAttribute = false;

        public bool serializeProperties = false;
        public bool allowPrivateProperties = true;
        public bool ignoreAutoProperties = true;
        public bool ignoreObsoleteProperties = false;
        public bool checkPropertySerialzableAttribute = false;
        public bool checkPropertyNonSerializableAttribute = false;

        public StringEncodeType defaultStringEncodeType = StringEncodeType.Byte;
        public bool checkStringEncodeTypeOverrideAttribute = false;
    }
}
