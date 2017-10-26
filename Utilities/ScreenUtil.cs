using System;
using UnityEngine;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Screen related utility functionality.
    /// </summary>
    public static class ScreenUtil
    {
        public const float INCHES_TO_CM = 2.54f;

        public static readonly Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
        public static readonly float ScreenRatio = (float)Screen.width / Screen.height;
        public static readonly Vector2 PhysicalScreenSize = ScreenSize / Screen.dpi * INCHES_TO_CM;

        /// <summary>
        /// Returns the half extents size of the screen in world coordinates.
        /// </summary>
        /// <param name="camera">Target camera component.</param>
        /// <param name="depth">The depth from teh camera at which to calculate the screen extents from.</param>
        /// <returns>Returns the width and height of half the screen extents from the camera in world coordinates.</returns>
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
        /// <param name="camera">Target camera component.</param>
        /// <param name="anchor">Normalized screen space position.</param>
        /// <param name="depth">The depth from teh camera at which to calculate the screen extents from.</param>
        /// <returns>Returns a world position at depth.</returns>
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
        /// <param name="camera">Target camera component.</param>
        /// <param name="worldPosition">World position to calculate the anchor position from.</param>
        /// <returns>Returns an normalized screen-space anchor position.</returns>
        public static Vector2 GetAnchorFromWorldPosition(Camera camera, Vector3 worldPosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates and sets the vertical FOV / orthographic size of a camera to match a horizontal FOV at specified pixels per unit.
        /// </summary>
        /// <param name="camera">Target camera component.</param>
        /// <param name="horizontalFOV">Horizontal field of view for the camera.</param>
        /// <param name="pixelToUnits">Pixels per unit.</param>
        public static void CalculateCameraFieldOfView(Camera camera, float horizontalFOV, float pixelToUnits)
        {
            if (camera.orthographic)
            {
                camera.orthographicSize = horizontalFOV / camera.aspect / pixelToUnits;
            }
            else
            {
                float vereticalFOV = 2f * Mathf.Atan(Mathf.Tan(horizontalFOV * Mathf.Deg2Rad * 0.5f) / camera.aspect);

                camera.fieldOfView = vereticalFOV * Mathf.Rad2Deg;
            }
        }

        /// <summary>
        /// Calculates the physical length in centimeters from pixel length.
        /// </summary>
        /// <param name="pixelLength">Length in pixels.</param>
        /// <returns>Returns length in centimenters.</returns>
        public static float CalculatePhysicalLength(float pixelLength)
        {
            return pixelLength / Screen.dpi * INCHES_TO_CM;
        }
    }
}
