using System;
using System.Linq;
using System.Reflection;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of MemberInfo related extension methods.
    /// </summary>
    public static class MemberInfoExt
    {
        /// <summary>
        /// Returns the custom attribute instance decorating a member.
        /// </summary>
        /// <typeparam name="T">Type of the attribute to get.</typeparam>
        /// <param name="memberInfo">Member info that is decorated with the attribute.</param>
        /// <param name="inherited">If true it will check for attributes inheriting T as well.</param>
        /// <returns>Returns the instance of the attribute decorating the memberInfo.</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherited = true) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherited).OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns true if a member has a custom attribute.
        /// </summary>
        /// <typeparam name="T">Type of the attribute to check for.</typeparam>
        /// <param name="memberInfo">Member info to check if an attribute is decorating it.</param>
        /// <param name="inherited">If true it will check for attributes inheriting T as well.</param>
        /// <returns>Returns true of an attribute of type T is decorating memberInfo</returns>
        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherited = true) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherited);
        }

        /// <summary>
        /// Trys to get custom attribute decorating a memberInfo and outs the result.
        /// </summary>
        /// <typeparam name="T">Type of the attribute to get.</typeparam>
        /// <param name="memberInfo">Member info that is decorated with the attribute.</param>
        /// <param name="attribute">The instance of the attribute decorating the memberInfo</param>
        /// <param name="inherited">If true it will check for attributes inheriting T as well.</param>
        /// <returns>Returns true if the memberInfo is decorated with the an attribute of type T.</returns>
        public static bool TryGetAttribute<T>(this MemberInfo memberInfo, out T attribute, bool inherited = true) where T : Attribute
        {
            attribute = memberInfo.GetAttribute<T>(inherited);
            return attribute != null;
        }
    }
}
