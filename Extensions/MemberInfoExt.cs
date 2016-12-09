using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Expanse
{
    public static class MemberInfoExt
    {
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherited = true) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherited).Cast<T>().FirstOrDefault();
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherited = true) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherited);
        }

        public static bool TryGetAttribute<T>(this MemberInfo memberInfo, out T attribute, bool inherited = true) where T : Attribute
        {
            attribute = memberInfo.GetAttribute<T>(inherited);
            return attribute != null;
        }
    }
}
