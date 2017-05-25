using System;
using Expanse.Utilities;

namespace Expanse.TinySerialization
{
    public class DateTimeTypeResolver
    {
        /// <summary>
        /// Gets a DateTime from a byte array at position.
        /// </summary>
        public DateTime GetDateTime(ref byte[] byteData, ref int position)
        {
            long ticks = BitConverter.ToInt64(byteData, position);
            position += sizeof(long);
            return new DateTime(ticks);
        }

        /// <summary>
        /// Sets a DateTime in a byte array at position.
        /// </summary>
        public void SetDateTime(ref byte[] byteData, ref int position, DateTime value)
        {
            position = ByteUtil.GetBytes(value.Ticks, byteData, position);
        }
    }
}
