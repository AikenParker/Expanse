using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class TransformExt
    {
        public static void DestroyAllChildren(this Transform source)
        {
            for (int i = 0; i < source.childCount; i++)
            {
                GameObject.Destroy(source.GetChild(i).gameObject);
            }
        }

        public static void Reset(this Transform source)
        {
            source.localPosition = Vector3.zero;
            source.localRotation = Quaternion.identity;
            source.localScale = Vector3.one;
        }
    }
}
