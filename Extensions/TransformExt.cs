using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    /// <summary>
    /// A collection of Transform related extension methods.
    /// </summary>
    public static class TransformExt
    {
        /// <summary>
        /// Destroys all game objects parented to a transform.
        /// </summary>
        public static void DestroyAllChildren(this Transform source, bool immediate = false)
        {
            for (int i = source.childCount - 1; i >= 0; i--)
            {
                if (immediate)
                    UnityEngine.Object.DestroyImmediate(source.GetChild(i).gameObject);
                else
                    UnityEngine.Object.Destroy(source.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Detaches all game objects parented to a transform.
        /// </summary>
        public static void DetachAllChildren(this Transform source, Transform newParent = null, bool worldPositionStays = true)
        {
            for (int i = source.childCount - 1; i >= 0; i--)
            {
                source.GetChild(i).SetParent(newParent, worldPositionStays);
            }
        }

        /// <summary>
        /// Resets the position, rotation and scale to default values.
        /// </summary>
        public static void Reset(this Transform source)
        {
            source.localPosition = Vector3.zero;
            source.localRotation = Quaternion.identity;
            source.localScale = Vector3.one;
        }

        /// <summary>
        /// Performs a breadth-first search to find a deep child transform with name.
        /// </summary>
        public static Transform FindDeepChildByBreadth(this Transform parent, string name)
        {
            return TransformUtil.FindDeepChildByBreadth(parent, name);
        }

        /// <summary>
        /// Performs a depth-first search to find a deep child transform with name.
        /// </summary>
        public static Transform FindDeepChildByDepth(this Transform parent, string name)
        {
            return TransformUtil.FindDeepChildByDepth(parent, name);
        }

        /// <summary>
        /// Sets the position and rotation to that of another transform.
        /// </summary>
        public static void CopyFrom(this Transform transform, Transform other, bool copyParent)
        {
            if (copyParent)
                transform.SetParent(other.parent, true);

            transform.position = other.position;
            transform.rotation = other.rotation;
        }

        #region Position

        /// <summary>
        /// Sets the X position of a transform.
        /// </summary>
        public static void SetPosX(this Transform transform, float value)
        {
            Vector3 position = transform.position;
            position.x = value;
            transform.position = position;
        }

        /// <summary>
        /// Adds to the X position of a transform.
        /// </summary>
        public static float AddPosX(this Transform transform, float value)
        {
            Vector3 position = transform.position;
            position.x += value;
            transform.position = position;
            return position.x;
        }

        /// <summary>
        /// Gets the X position of a transform.
        /// </summary>
        public static float GetPosX(this Transform transform)
        {
            return transform.position.x;
        }

        /// <summary>
        /// Sets the Y position of a transform.
        /// </summary>
        public static void SetPosY(this Transform transform, float value)
        {
            Vector3 position = transform.position;
            position.y = value;
            transform.position = position;
        }

        /// <summary>
        /// Adds to the Y position of a transform.
        /// </summary>
        public static float AddPosY(this Transform transform, float value)
        {
            Vector3 position = transform.position;
            position.y += value;
            transform.position = position;
            return position.y;
        }

        /// <summary>
        /// Gets the Y position of a transform.
        /// </summary>
        public static float GetPosY(this Transform transform)
        {
            return transform.position.y;
        }

        /// <summary>
        /// Sets the Z position of a transform.
        /// </summary>
        public static void SetPosZ(this Transform transform, float value)
        {
            Vector3 position = transform.position;
            position.z = value;
            transform.position = position;
        }

        /// <summary>
        /// Adds to the Z position of a transform.
        /// </summary>
        public static float AddPosZ(this Transform transform, float value)
        {
            Vector3 position = transform.position;
            position.z += value;
            transform.position = position;
            return position.z;
        }

        /// <summary>
        /// Gets the Z position of a transform.
        /// </summary>
        public static float GetPosZ(this Transform transform)
        {
            return transform.position.z;
        }

        #endregion

        #region LocalPosition

        /// <summary>
        /// Sets the local X position of a transform.
        /// </summary>
        public static void SetLocalPosX(this Transform transform, float value)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.x = value;
            transform.localPosition = localPosition;
        }

        /// <summary>
        /// Adds to the local X position of a transform.
        /// </summary>
        public static float AddLocalPosX(this Transform transform, float value)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.x += value;
            transform.localPosition = localPosition;
            return localPosition.x;
        }

        /// <summary>
        /// Gets the local X position of a transform.
        /// </summary>
        public static float GetLocalPosX(this Transform transform)
        {
            return transform.localPosition.x;
        }

        /// <summary>
        /// Sets the local Y position of a transform.
        /// </summary>
        public static void SetLocalPosY(this Transform transform, float value)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.y = value;
            transform.localPosition = localPosition;
        }

        /// <summary>
        /// Adds to the local Y position of a transform.
        /// </summary>
        public static float AddLocalPosY(this Transform transform, float value)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.y += value;
            transform.localPosition = localPosition;
            return localPosition.y;
        }

        /// <summary>
        /// Gets the local Y position of a transform.
        /// </summary>
        public static float GetLocalPosY(this Transform transform)
        {
            return transform.localPosition.y;
        }

        /// <summary>
        /// Sets the local Z position of a transform.
        /// </summary>
        public static void SetLocalPosZ(this Transform transform, float value)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.z = value;
            transform.localPosition = localPosition;
        }

        /// <summary>
        /// Adds to the local Z position of a transform.
        /// </summary>
        public static float AddLocalPosZ(this Transform transform, float value)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.z += value;
            transform.localPosition = localPosition;
            return localPosition.z;
        }

        /// <summary>
        /// Gets the local Z position of a transform.
        /// </summary>
        public static float GetLocalPosZ(this Transform transform)
        {
            return transform.localPosition.z;
        }

        #endregion

        #region EulerAngles

        /// <summary>
        /// Sets the X Euler Angle of a transform.
        /// </summary>
        public static void SetEulerX(this Transform transform, float value)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = value;
            transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Adds to the X Euler Angle of a transform.
        /// </summary>
        public static float AddEulerX(this Transform transform, float value)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x += value;
            transform.eulerAngles = eulerAngles;
            return eulerAngles.x;
        }

        /// <summary>
        /// Gets the X Euler Angle of a transform.
        /// </summary>
        public static float GetEulerX(this Transform transform)
        {
            return transform.eulerAngles.x;
        }

        /// <summary>
        /// Sets the Y Euler Angle of a transform.
        /// </summary>
        public static void SetEulerY(this Transform transform, float value)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = value;
            transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Adds to the Y Euler Angle of a transform.
        /// </summary>
        public static float AddEulerY(this Transform transform, float value)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y += value;
            transform.eulerAngles = eulerAngles;
            return eulerAngles.y;
        }

        /// <summary>
        /// Gets the Y Euler Angle of a transform.
        /// </summary>
        public static float GetEulerY(this Transform transform)
        {
            return transform.eulerAngles.y;
        }

        /// <summary>
        /// Sets the Z Euler Angle of a transform.
        /// </summary>
        public static void SetEulerZ(this Transform transform, float value)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z = value;
            transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Adds to the Z Euler Angle of a transform.
        /// </summary>
        public static float AddEulerZ(this Transform transform, float value)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z += value;
            transform.eulerAngles = eulerAngles;
            return eulerAngles.z;
        }

        /// <summary>
        /// Gets the Z Euler Angle of a transform.
        /// </summary>
        public static float GetEulerZ(this Transform transform)
        {
            return transform.eulerAngles.z;
        }

        #endregion

        #region LocalEulerAngles

        /// <summary>
        /// Sets the local X Euler Angle of a transform.
        /// </summary>
        public static void SetLocalEulerX(this Transform transform, float value)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.x = value;
            transform.localEulerAngles = localEulerAngles;
        }

        /// <summary>
        /// Adds to the local X Euler Angle of a transform.
        /// </summary>
        public static float AddLocalEulerX(this Transform transform, float value)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.x += value;
            transform.localEulerAngles = localEulerAngles;
            return localEulerAngles.x;
        }

        /// <summary>
        /// Gets the local X Euler Angle of a transform.
        /// </summary>
        public static float GetLocalEulerX(this Transform transform)
        {
            return transform.localEulerAngles.x;
        }

        /// <summary>
        /// Sets the local Y Euler Angle of a transform.
        /// </summary>
        public static void SetLocalEulerY(this Transform transform, float value)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.y = value;
            transform.localEulerAngles = localEulerAngles;
        }

        /// <summary>
        /// Adds to the local Y Euler Angle of a transform.
        /// </summary>
        public static float AddLocalEulerY(this Transform transform, float value)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.y += value;
            transform.localEulerAngles = localEulerAngles;
            return localEulerAngles.y;
        }

        /// <summary>
        /// Gets the local Y Euler Angle of a transform.
        /// </summary>
        public static float GetLocalEulerY(this Transform transform)
        {
            return transform.localEulerAngles.y;
        }

        /// <summary>
        /// Sets the local Z Euler Angle of a transform.
        /// </summary>
        public static void SetLocalEulerZ(this Transform transform, float value)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.z = value;
            transform.localEulerAngles = localEulerAngles;
        }

        /// <summary>
        /// Adds to the local Z Euler Angle of a transform.
        /// </summary>
        public static float AddLocalEulerZ(this Transform transform, float value)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.z += value;
            transform.localEulerAngles = localEulerAngles;
            return localEulerAngles.z;
        }

        /// <summary>
        /// Gets the local Z Euler Angle of a transform.
        /// </summary>
        public static float GetLocalEulerZ(this Transform transform)
        {
            return transform.localEulerAngles.z;
        }

        #endregion

        #region LocalScale

        /// <summary>
        /// Sets the local X scale of a transform.
        /// </summary>
        public static void SetLocalScaleX(this Transform transform, float value)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = value;
            transform.localScale = localScale;
        }

        /// <summary>
        /// Adds to the local X scale of a transform.
        /// </summary>
        public static float AddLocalScaleX(this Transform transform, float value)
        {
            Vector3 localScale = transform.localScale;
            localScale.x += value;
            transform.localScale = localScale;
            return localScale.x;
        }

        /// <summary>
        /// Gets the local X scale of a transform.
        /// </summary>
        public static float GetLocalScaleX(this Transform transform)
        {
            return transform.localScale.x;
        }

        /// <summary>
        /// Sets the local Y scale of a transform.
        /// </summary>
        public static void SetLocalScaleY(this Transform transform, float value)
        {
            Vector3 localScale = transform.localScale;
            localScale.y = value;
            transform.localScale = localScale;
        }

        /// <summary>
        /// Adds to the local Y scale of a transform.
        /// </summary>
        public static float AddLocalScaleY(this Transform transform, float value)
        {
            Vector3 localScale = transform.localScale;
            localScale.y += value;
            transform.localScale = localScale;
            return localScale.y;
        }

        /// <summary>
        /// Gets the local Y scale of a transform.
        /// </summary>
        public static float GetLocalScaleY(this Transform transform)
        {
            return transform.localScale.y;
        }

        /// <summary>
        /// Sets the local Z scale of a transform.
        /// </summary>
        public static void SetLocalScaleZ(this Transform transform, float value)
        {
            Vector3 localScale = transform.localScale;
            localScale.z = value;
            transform.localScale = localScale;
        }

        /// <summary>
        /// Adds to the local Z scale of a transform.
        /// </summary>
        public static float AddLocalScaleZ(this Transform transform, float value)
        {
            Vector3 localScale = transform.localScale;
            localScale.z += value;
            transform.localScale = localScale;
            return localScale.z;
        }

        /// <summary>
        /// Gets the local Z scale of a transform.
        /// </summary>
        public static float GetLocalScaleZ(this Transform transform)
        {
            return transform.localScale.z;
        }

        #endregion
    }
}
