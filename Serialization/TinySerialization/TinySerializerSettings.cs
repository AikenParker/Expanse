#if UNSAFE

namespace Expanse.Serialization.TinySerialization
{
    public struct TinySerializerSettings
    {
        public bool supportCyclicReferences;
        public bool requireTypeSerializableAttribute;
        public bool avoidCustomStructBoxing;

        public bool serializeFields;
        public bool allowPrivateFields;
        public bool ignoreObsoleteFields;
        public bool checkFieldSerializableAttribute;
        public bool checkFieldNonSerializableAttribute;

        public bool serializeProperties;
        public bool allowPrivateProperties;
        public bool ignoreAutoProperties;
        public bool ignoreObsoleteProperties;
        public bool checkPropertySerialzableAttribute;
        public bool checkPropertyNonSerializableAttribute;

        public bool variablePrefixLengthSize;
        public bool compressBoolArray;
        public StringEncodeType defaultStringEncodeType;
        public bool checkStringEncodeTypeOverrideAttribute;

        public static TinySerializerSettings Default
        {
            get
            {
                TinySerializerSettings defaultSettings = new TinySerializerSettings();

                defaultSettings.supportCyclicReferences = false;
                defaultSettings.requireTypeSerializableAttribute = false;
                defaultSettings.avoidCustomStructBoxing = true;

                defaultSettings.serializeFields = true;
                defaultSettings.allowPrivateFields = true;
                defaultSettings.ignoreObsoleteFields = false;
                defaultSettings.checkFieldSerializableAttribute = false;
                defaultSettings.checkFieldNonSerializableAttribute = false;

                defaultSettings.serializeProperties = false;
                defaultSettings.allowPrivateProperties = true;
                defaultSettings.ignoreAutoProperties = true;
                defaultSettings.ignoreObsoleteProperties = false;
                defaultSettings.checkPropertySerialzableAttribute = false;
                defaultSettings.checkPropertyNonSerializableAttribute = false;

                defaultSettings.variablePrefixLengthSize = true;
                defaultSettings.compressBoolArray = true;
                defaultSettings.defaultStringEncodeType = StringEncodeType.Byte;
                defaultSettings.checkStringEncodeTypeOverrideAttribute = false;

                return defaultSettings;
            }
        }
    }
}
#endif
