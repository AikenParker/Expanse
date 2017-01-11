using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    /* TODO
     * Add suffix identifiers (e.g. 'a' = alpha, 'b' = beta)
     * Add operator overloads (==, <, >)
     */

    /// <summary>
    /// Contains basic version information.
    /// </summary>
    public struct Version
    {
        public readonly ushort majorVersion;
        public readonly ushort minorVersion;
        public readonly ushort buildNumber;

        public Version(ushort majorVersion, ushort minorVersion, ushort buildNumber)
        {
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
            this.buildNumber = buildNumber;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", majorVersion, minorVersion, buildNumber);
        }
    }
}
