using System;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Screen related utility functionality.
    /// </summary>
    public static class ScreenUtil
    {
        /// <summary>
        /// Returns the half extents size of the screen in world coordinates.
        /// </summary>
        public static Vector2 GetScreenHalfExtents(Camera camera, float depth = 10)
        {
            float width, height;

            if (camera.orthographic == true)
            {
                Vector3 lowerLeftCorner = camera.ViewportToWorldPoint(Vector2.zero);
                Vector3 upperRightCorner = camera.ViewportToWorldPoint(Vector2.one);

                width = upperRightCorner.x - lowerLeftCorner.x;
                height = upperRightCorner.y - lowerLeftCorner.y;
            }
            else
            {
                height = 2.0f * Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad) * depth;
                width = height * Screen.width / Screen.height;
            }

            return new Vector2(width * 0.5f, height * 0.5f);
        }

        /// <summary>
        /// Returns a world position from an anchor.
        /// </summary>
        public static Vector3 GetWorldPositionFromAnchor(Camera camera, Vector2 anchor, float depth = 10)
        {
            float xPos, yPos, zPos = camera.transform.position.z + depth;

            if (camera.orthographic == true)
            {
                Vector3 lowerLeftCorner = camera.ViewportToWorldPoint(Vector2.zero);
                Vector3 upperRightCorner = camera.ViewportToWorldPoint(Vector2.one);

                xPos = Mathf.LerpUnclamped(lowerLeftCorner.x, upperRightCorner.x, anchor.x);
                yPos = Mathf.LerpUnclamped(lowerLeftCorner.y, upperRightCorner.y, anchor.y);
            }
            else
            {
                float height = 2.0f * Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad) * depth;
                float width = height * Screen.width / Screen.height;

                xPos = Mathf.LerpUnclamped(width * -0.5f, width * 0.5f, anchor.x);
                yPos = Mathf.LerpUnclamped(height * -0.5f, height * 0.5f, anchor.y);
            }

            return new Vector3(xPos, yPos, zPos);
        }

        /// <summary>
        /// Returns an anchor from a world position.
        /// </summary>
        public static Vector2 GetAnchorFromWorldPosition(Camera camera, Vector3 worldPosition)
        {
            throw new NotImplementedException();
        }
    }
}
