using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse.TinySerialization
{
    public class StringTypeResolver
    {
        private StringResolutionType resolutionType;
        private char[] charCache;

        public StringTypeResolver(StringResolutionType resolutionType)
        {
            this.resolutionType = resolutionType;
        }

        /// <summary>
        /// Gets a string from a byte array at position.
        /// </summary>
        public string GetString(ref byte[] byteData, ref int position)
        {
            switch (resolutionType)
            {
                case StringResolutionType.NULL_TERMINATED:
                    {
                        int stringLength = 0;
                        {
                            int stringPosition = sizeof(char) - 1;
                            while (position + stringPosition < byteData.Length)
                            {
                                if (byteData[position + stringPosition] == 0 && byteData[position + stringPosition - 1] == 0)
                                    break;

                                stringPosition += sizeof(char);
                                stringLength++;
                            }
                        }

                        if (charCache == null || charCache.Length < stringLength)
                            charCache = new char[stringLength];

                        for (int j = 0; j < stringLength; j++)
                        {
                            byte byte1 = byteData[position + (j * sizeof(char))];
                            byte byte2 = byteData[position + (j * sizeof(char) + 1)];
                            charCache[j] = (char)(byte1 + byte2);
                        }

                        position += stringLength * sizeof(char) + sizeof(char);
                        return new string(charCache, 0, stringLength);
                    }

                case StringResolutionType.PREDEFINED_LENGTH:
                    {
                        int stringLength = BitConverter.ToInt32(byteData, position);
                        position += sizeof(int);

                        if (charCache == null || charCache.Length < stringLength)
                            charCache = new char[stringLength];

                        for (int j = 0; j < stringLength; j++)
                        {
                            byte byte1 = byteData[position + (j * sizeof(char))];
                            byte byte2 = byteData[position + (j * sizeof(char) + 1)];
                            charCache[j] = (char)(byte1 + byte2);
                        }

                        position += stringLength * sizeof(char);
                        return new string(charCache, 0, stringLength);
                    }

                default:
                    throw new UnsupportedException("resolutionType");
            }
        }

        /// <summary>
        /// Sets a string from a byte array at position.
        /// </summary>
        public void SetString(ref byte[] byteData, ref int position, string value)
        {
            switch (resolutionType)
            {
                case StringResolutionType.NULL_TERMINATED:
                    {
                        int stringLength = value.Length;
                        for (int j = 0; j < stringLength; j++)
                        {
                            position = ByteUtil.GetBytes(value[j], byteData, position);
                        }
                        for (int j = 0; j < sizeof(char); j++)
                        {
                            byteData[position++] = 0x000000;
                        }
                    }
                    break;

                case StringResolutionType.PREDEFINED_LENGTH:
                    {
                        int stringLength = value.Length;
                        position = ByteUtil.GetBytes(stringLength, byteData, position);
                        for (int j = 0; j < stringLength; j++)
                        {
                            position = ByteUtil.GetBytes(value[j], byteData, position);
                        }
                    }
                    break;
            }
        }

        public enum StringResolutionType : byte
        {
            NULL_TERMINATED = 0,
            PREDEFINED_LENGTH = 1
        }
    }
}
