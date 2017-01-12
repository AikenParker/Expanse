using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// A collection of MemberInfo related extension methods.
    /// </summary>
    public static class MemberInfoExt
    {
        /// <summary>
        /// Returns the custom attribute instance decorating a member.
        /// </summary>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherited = true) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherited).Cast<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns true if a member has a custom attribute.
        /// </summary>
        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherited = true) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherited);
        }

        /// <summary>
        /// Returns true if a member has a custom attribute and outs the result.
        /// </summary>
        public static bool TryGetAttribute<T>(this MemberInfo memberInfo, out T attribute, bool inherited = true) where T : Attribute
        {
            attribute = memberInfo.GetAttribute<T>(inherited);
            return attribute != null;
        }
    }
}
