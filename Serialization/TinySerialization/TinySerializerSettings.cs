#if UNSAFE

namespace Expanse.Serialization.TinySerialization
{
    public struct TinySerializerSettings
    {
        public bool serializeAllFields;
        public bool serializePrivateFields;
        public bool serializeObsoleteFields;
        public StringEncodeType defaultStringEncodeType;

        public static TinySerializerSettings Default
        {
            get
            {
                TinySerializerSettings defaultSettings = new TinySerializerSettings();

                defaultSettings.serializeAllFields = false;
                defaultSettings.serializePrivateFields = true;
                defaultSettings.serializeObsoleteFields = false;
                defaultSettings.defaultStringEncodeType = StringEncodeType.Byte;

                return defaultSettings;
            }
        }
    }
}
#endif
