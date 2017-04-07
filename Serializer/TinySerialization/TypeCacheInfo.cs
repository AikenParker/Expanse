using System;

namespace Expanse.TinySerialization
{
    public class TypeCacheInfo
    {
        public Type type;
        public FieldCacheInfo[] fields;
        public object constructor;
        public bool isStaticSize;
        public int lastSize;

        private bool hasCalculatedSize;

        public TypeCacheInfo(Type type, FieldCacheInfo[] fields)
        {
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
                        size += fci.GetValue<TSource, string>(obj).Length * sizeof(char) + sizeof(char);
                        this.isStaticSize = false;
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

        public TSource GetDefaultConstructedInstance<TSource>()
        {
            Func<TSource> constructor = this.constructor as Func<TSource>;

            if (constructor == null)
            {
                constructor = EmitUtil.GenerateDefaultConstructor<TSource>();
                this.constructor = constructor;
            }

            return constructor();
        }
    }
}
