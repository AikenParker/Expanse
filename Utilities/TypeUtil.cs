using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Type related utility functionality.
    /// </summary>
    public static class TypeUtil
    {
        public const string NULL_TYPE_NAME = "None";

        public static Type[] AllTypes { get; private set; }

        static TypeUtil()
        {
            AssemblySorter assemblySorter = new AssemblySorter();

            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .OrderBy(x => x, assemblySorter)
                .SelectMany(x => x.GetTypes())
                .Distinct().ToArray();

            AllTypes = new Type[types.Length + 1];
            types.CopyTo(AllTypes, 1);
            AllTypes[0] = null;
        }

        /// <summary>
        /// Returns a Type from a full type name.
        /// </summary>
        public static Type GetTypeFromName(string typeName)
        {
            if (typeName.Equals(TypeUtil.NULL_TYPE_NAME))
                return null;

            for (int i = 1; i < AllTypes.Length; i++)
            {
                if (AllTypes[i].FullName.Equals(typeName))
                    return AllTypes[i];
            }

            return null;
        }

        private class AssemblySorter : IComparer<Assembly>
        {
            private int GetNameSortValue(string name)
            {
                switch (name)
                {
                    case "Assembly-CSharp":
                        return 3;
                    case "UnityEngine":
                        return 2;
                    case "mscorlib":
                        return 1;
                    default:
                        return 0;
                }
            }

            int IComparer<Assembly>.Compare(Assembly x, Assembly y)
            {
                string xName = x.GetName().Name;
                string yName = y.GetName().Name;

                int xSortValue = GetNameSortValue(xName);
                int ySortValue = GetNameSortValue(yName);

                return ySortValue - xSortValue;
            }
        }
    }
}
