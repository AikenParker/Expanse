using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Overrides MeshRenderer inspector to add sorting layer and order options.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MeshRenderer), true)]
    public class MeshRendererInspector : Editor
    {
        void OnEnable()
        {
            SortingOrderUtil.Update();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (this.targets.Length == 1)
            {
                serializedObject.UpdateIfRequiredOrScript();

                MeshRenderer target = (MeshRenderer)this.target;

                int sortingLayerID = target.sortingLayerID;
                int sortingLayerIndex = 0;
                int[] sortingLayerIDs = SortingOrderUtil.SortingOrderIDs;

                for (int i = 0; i < sortingLayerIDs.Length; i++)
                {
                    if (sortingLayerID == sortingLayerIDs[i])
                    {
                        sortingLayerIndex = i;
                        break;
                    }
                }

                int newSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", sortingLayerIndex, SortingOrderUtil.SortingOrderNames);

                if (newSortingLayerIndex != sortingLayerIndex)
                {
                    Undo.RecordObject(target, "Sorting Layer Edit");
                    target.sortingLayerID = sortingLayerIDs[newSortingLayerIndex];
                }

                int sortingOrder = target.sortingOrder;
                int newSortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);

                if (newSortingOrder != sortingOrder)
                {
                    Undo.RecordObject(target, "Sorting Order Edit");
                    target.sortingOrder = newSortingOrder;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    /// <summary>
    /// Overrides SkinnedMeshRenderer inspector to add sorting layer and order options.
    /// </summary>
    [CustomEditor(typeof(SkinnedMeshRenderer), true)]
    [CanEditMultipleObjects]
    public class SkinnedMeshRendererInspector : Editor
    {
        void OnEnable()
        {
            SortingOrderUtil.Update();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (this.targets.Length == 1)
            {
                serializedObject.UpdateIfRequiredOrScript();

                SkinnedMeshRenderer target = (SkinnedMeshRenderer)this.target;

                int sortingLayerID = target.sortingLayerID;
                int sortingLayerIndex = 0;
                int[] sortingLayerIDs = SortingOrderUtil.SortingOrderIDs;

                for (int i = 0; i < sortingLayerIDs.Length; i++)
                {
                    if (sortingLayerID == sortingLayerIDs[i])
                    {
                        sortingLayerIndex = i;
                        break;
                    }
                }

                int newSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", sortingLayerIndex, SortingOrderUtil.SortingOrderNames);

                if (newSortingLayerIndex != sortingLayerIndex)
                {
                    Undo.RecordObject(target, "Sorting Layer Edit");
                    target.sortingLayerID = sortingLayerIDs[newSortingLayerIndex];
                }

                int sortingOrder = target.sortingOrder;
                int newSortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);

                if (newSortingOrder != sortingOrder)
                {
                    Undo.RecordObject(target, "Sorting Order Edit");
                    target.sortingOrder = newSortingOrder;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}