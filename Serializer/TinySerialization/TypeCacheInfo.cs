#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif

using System;
using Expanse.Utilities;

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
        private Delegate constructor;
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
                    case SupportedFieldType.Int:
                        size += sizeof(int);
                        continue;
                    case SupportedFieldType.Bool:
                        size += sizeof(bool);
                        continue;
                    case SupportedFieldType.Float:
                        size += sizeof(float);
                        continue;
                    case SupportedFieldType.Double:
                        size += sizeof(double);
                        continue;
                    case SupportedFieldType.Char:
                        size += sizeof(char);
                        continue;
                    case SupportedFieldType.String:
                        {
                            switch (serializer.SerializationInfo.StringResolutionType)
                            {
                                case StringTypeResolver.StringResolutionType.NullTerminated:
                                    size += (fci.GetValue<TSource, string>(obj).Length * sizeof(char)) + sizeof(char);
                                    break;

                                case StringTypeResolver.StringResolutionType.PreDefinedLength:
                                    size += sizeof(int) + (fci.GetValue<TSource, string>(obj).Length * sizeof(char));
                                    break;

                                default:
                                    throw new UnsupportedException("stringResolutionType");
                            }

                            this.isStaticSize = false;
                        }
                        continue;
                    case SupportedFieldType.DateTime:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.Long:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.Short:
                        size += sizeof(short);
                        continue;
                    case SupportedFieldType.UInt:
                        size += sizeof(uint);
                        continue;
                    case SupportedFieldType.UShort:
                        size += sizeof(ushort);
                        continue;
                    case SupportedFieldType.ULong:
                        size += sizeof(ulong);
                        continue;
                    case SupportedFieldType.Byte:
                        size += sizeof(byte);
                        continue;
                    case SupportedFieldType.SByte:
                        size += sizeof(sbyte);
                        continue;
                    case SupportedFieldType.Decimal:
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
                    case SupportedFieldType.Int:
                        size += sizeof(int);
                        continue;
                    case SupportedFieldType.Bool:
                        size += sizeof(bool);
                        continue;
                    case SupportedFieldType.Float:
                        size += sizeof(float);
                        continue;
                    case SupportedFieldType.Double:
                        size += sizeof(double);
                        continue;
                    case SupportedFieldType.Char:
                        size += sizeof(char);
                        continue;
                    case SupportedFieldType.String:
                        this.isStaticSize = false;
                        return false;
                    case SupportedFieldType.DateTime:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.Long:
                        size += sizeof(long);
                        continue;
                    case SupportedFieldType.Short:
                        size += sizeof(short);
                        continue;
                    case SupportedFieldType.UInt:
                        size += sizeof(uint);
                        continue;
                    case SupportedFieldType.UShort:
                        size += sizeof(ushort);
                        continue;
                    case SupportedFieldType.ULong:
                        size += sizeof(ulong);
                        continue;
                    case SupportedFieldType.Byte:
                        size += sizeof(byte);
                        continue;
                    case SupportedFieldType.SByte:
                        size += sizeof(sbyte);
                        continue;
                    case SupportedFieldType.Decimal:
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
                    this.constructor = constructor = EmitUtil.GenerateDefaultConstructorDelegate<TSource>();
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
                this.constructor = EmitUtil.GenerateDefaultConstructorDelegate<TSource>();
            }
#endif
        }
    }
}
