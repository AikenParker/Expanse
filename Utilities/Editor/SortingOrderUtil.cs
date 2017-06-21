using System;
using System.Reflection;
using UnityEditorInternal;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of 2D Sorting Order related utility functionality.
    /// </summary>
    public static class SortingOrderUtil
    {
        /// <summary>
        /// Collection that contains all specified sorting order names.
        /// </summary>
        public static string[] SortingOrderNames { get; private set; }
        /// <summary>
        /// Collection that contains all specified sorting order IDs.
        /// </summary>
        public static int[] SortingOrderIDs { get; private set; }

        private static readonly PropertyInfo sortingOrderNamesProperty, sortingOrderIDsProperty;
        private static object[] emptyParams = new object[0];

        static SortingOrderUtil()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);

            sortingOrderNamesProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            sortingOrderIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);

            Update();
        }

        /// <summary>
        /// Updates the SortingOrderNames and SortingOrderIDs properties.
        /// </summary>
        public static void Update()
        {
            SortingOrderNames = (string[])sortingOrderNamesProperty.GetValue(null, emptyParams);
            SortingOrderIDs = (int[])sortingOrderIDsProperty.GetValue(null, emptyParams);
        }
    }
}
