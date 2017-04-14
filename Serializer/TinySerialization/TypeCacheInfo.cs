using System;

namespace Expanse.TinySerialization
{
    public class TypeCacheInfo
    {
        private TinySerializer serializer;

        public Type type;
        public FieldCacheInfo[] fields;
        public bool isStaticSize;
        public int lastSize;

        private bool hasCalculatedSize;

#if !AOT_ONLY
        private object constructor;
#endif

        public TypeCacheInfo(TinySerializer serializer, Type type, FieldCacheInfo[] fields)
        {
            this.serializer = serializer;
            this.type = type;
            this.fields = fields;
        }

        public int CalculateSize<TSource>(TSource obj)
        {
            if (this.isStaticSize && this.hasCalculatedSize)
                return this.lastSize;

            int size = 0;
            this.isStaticSize = true;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldCacheInfo fci = fields[i];

                // --SUPPORTED-TYPE-SWITCH--
                switch (fci.type)
                {
                    case SupportedFieldType.INT:
                        size += sizeof(int);
                        continue;
                    case SupportedFieldType.BOOL:
                        size += sizeof(bool);
                        continue;
                    case SupportedFieldType.FLOAT:
                        size += sizeof(float);
                        continue;
                    case SupportedFieldType.DOUBLE:
                        size += sizeof(double);
                        continue;
                    case SupportedFieldType.CHAR:
                        size += sizeof(char);
                        continue;
                    case SupportedFieldType.STRING:
                        {
                            switch (serializer.SerializationInfo.StringResolutionType)
                            {
                                case StringTypeResolver.StringResolutionType.NULL_TERMINATED:
                                    size += (fci.GetValue<TSource, string>(obj).Length * sizeof(char)) + sizeof(char);
                                    break;

                                case StringTypeResolver.StringResolutionType.PREDEFINED_LENGTH:
                                    size += sizeof(int) + (fci.GetValue<TSource, string>(obj).Length * sizeof(char));
                                    break;

                                default:
                                    throw new UnsupportedException("stringResolutionType");
                            }

                            this.isStaticSize = false;
                        }
                        continue;
                    case SupportedFieldType.DATE_TIME:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.LONG:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.SHORT:
                        size += sizeof(short);
                        continue;
                    case SupportedFieldType.UINT:
                        size += sizeof(uint);
                        continue;
                    case SupportedFieldType.USHORT:
                        size += sizeof(ushort);
                        continue;
                    case SupportedFieldType.ULONG:
                        size += sizeof(ulong);
                        continue;
                    case SupportedFieldType.BYTE:
                        size += sizeof(byte);
                        continue;
                    case SupportedFieldType.SBYTE:
                        size += sizeof(sbyte);
                        continue;
                    case SupportedFieldType.DECIMAL:
                        size += sizeof(decimal);
                        continue;
                }
            }

            this.hasCalculatedSize = true;
            this.lastSize = size;

            return size;
        }

        public bool TryCalculateStaticSize<TSource>(out int size)
        {
            if (this.isStaticSize && this.hasCalculatedSize)
            {
                size = this.lastSize;
                return true;
            }

            size = 0;
            this.isStaticSize = true;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldCacheInfo fci = fields[i];

                // --SUPPORTED-TYPE-SWITCH--
                switch (fci.type)
                {
                    case SupportedFieldType.INT:
                        size += sizeof(int);
                        continue;
                    case SupportedFieldType.BOOL:
                        size += sizeof(bool);
                        continue;
                    case SupportedFieldType.FLOAT:
                        size += sizeof(float);
                        continue;
                    case SupportedFieldType.DOUBLE:
                        size += sizeof(double);
                        continue;
                    case SupportedFieldType.CHAR:
                        size += sizeof(char);
                        continue;
                    case SupportedFieldType.STRING:
                        this.isStaticSize = false;
                        return false;
                    case SupportedFieldType.DATE_TIME:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.LONG:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.SHORT:
                        size += sizeof(short);
                        continue;
                    case SupportedFieldType.UINT:
                        size += sizeof(uint);
                        continue;
                    case SupportedFieldType.USHORT:
                        size += sizeof(ushort);
                        continue;
                    case SupportedFieldType.ULONG:
                        size += sizeof(ulong);
                        continue;
                    case SupportedFieldType.BYTE:
                        size += sizeof(byte);
                        continue;
                    case SupportedFieldType.SBYTE:
                        size += sizeof(sbyte);
                        continue;
                    case SupportedFieldType.DECIMAL:
                        size += sizeof(decimal);
                        continue;
                }
            }

            this.hasCalculatedSize = true;
            this.lastSize = size;

            return true;
        }

        public TSource GetDefaultConstructedInstance<TSource>() where TSource : new()
        {
#if AOT_ONLY
            return new TSource();
#else

            Func<TSource> constructor = this.constructor as Func<TSource>;

            if (constructor == null)
            {
                if (serializer.SerializationInfo.EmitReflection)
                {
                    this.constructor = constructor = EmitUtil.GenerateDefaultConstructor<TSource>();
                }
                else
                {
                    return new TSource();
                }
            }

            return constructor();
#endif
        }

        public void SetupDefaultConstructor<TSource>() where TSource : new()
        {
#if !AOT_ONLY
            if (serializer.SerializationInfo.EmitReflection)
            {
                this.constructor = EmitUtil.GenerateDefaultConstructor<TSource>();
            }
#endif
        }
    }
}
