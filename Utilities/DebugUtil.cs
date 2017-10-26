using Conditional = System.Diagnostics.ConditionalAttribute;
using UnityEngine;
using System;

namespace Expanse.Utilities
{
    /// <summary>
    /// Collection of Unity.Engine debug draw utility functionality.
    /// </summary>
    public static class DebugUtil
    {
        private const string CONDITIONAL = "UNITY_EDITOR";
        private const int CIRCLE_PRECISION = 91;
        private const int SPHERE_PRECISION = 37;
        private const float SPHERE_ANGLE = 10f;

        private static bool isEnabled = true;
        private static bool depthTest = true;
        private static Color defaultColor = Color.white;
        private static float duration = 0f;

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">Start position of the line.</param>
        /// <param name="end">End position of the line.</param>
        /// <param name="color">Color of the line.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (!isEnabled)
                return;

            Debug.DrawLine(start, end, color, duration, depthTest);
        }

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="position">Position of the point.</param>
        /// <param name="color">Color of the point.</param>
        /// <param name="scale">Size of the point.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawPoint(Vector3 position, Color color, float scale = 1f)
        {
            if (!isEnabled)
                return;

            Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
            Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
            Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
        }

        /// <summary>
        /// Draws a bounds.
        /// </summary>
        /// <param name="bounds">Bounds to draw.</param>
        /// <param name="rotation">Rotation of the bounds.</param>
        /// <param name="color">Color of the bounds.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBounds(Bounds bounds, Quaternion rotation, Color color)
        {
            if (!isEnabled)
                return;

            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3 ltf = center + rotation * new Vector3(-extents.x, extents.y, extents.z);
            Vector3 rtf = center + rotation * new Vector3(extents.x, extents.y, extents.z);
            Vector3 lbf = center + rotation * new Vector3(-extents.x, -extents.y, extents.z);
            Vector3 rbf = center + rotation * new Vector3(extents.x, -extents.y, extents.z);
            Vector3 ltb = center + rotation * new Vector3(-extents.x, extents.y, -extents.z);
            Vector3 rtb = center + rotation * new Vector3(extents.x, extents.y, -extents.z);
            Vector3 lbb = center + rotation * new Vector3(-extents.x, -extents.y, -extents.z);
            Vector3 rbb = center + rotation * new Vector3(extents.x, -extents.y, -extents.z);

            Debug.DrawLine(ltf, rtf, color, duration, depthTest);
            Debug.DrawLine(rtf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, ltf, color, duration, depthTest);
            Debug.DrawLine(ltb, rtb, color, duration, depthTest);
            Debug.DrawLine(rtb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbb, color, duration, depthTest);
            Debug.DrawLine(lbb, ltb, color, duration, depthTest);
            Debug.DrawLine(ltf, ltb, color, duration, depthTest);
            Debug.DrawLine(rtf, rtb, color, duration, depthTest);
            Debug.DrawLine(rbf, rbb, color, duration, depthTest);
            Debug.DrawLine(lbf, lbb, color, duration, depthTest);
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="position">Center position of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="rotation">Rotation of the box.</param>
        /// <param name="color">Color of the box.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBox(Vector3 position, Vector3 size, Quaternion rotation, Color color)
        {
            if (!isEnabled)
                return;

            Vector3 extents = size * 0.5f;

            Vector3 ltf = position + rotation * new Vector3(-extents.x, extents.y, extents.z);
            Vector3 rtf = position + rotation * new Vector3(extents.x, extents.y, extents.z);
            Vector3 lbf = position + rotation * new Vector3(-extents.x, -extents.y, extents.z);
            Vector3 rbf = position + rotation * new Vector3(extents.x, -extents.y, extents.z);
            Vector3 ltb = position + rotation * new Vector3(-extents.x, extents.y, -extents.z);
            Vector3 rtb = position + rotation * new Vector3(extents.x, extents.y, -extents.z);
            Vector3 lbb = position + rotation * new Vector3(-extents.x, -extents.y, -extents.z);
            Vector3 rbb = position + rotation * new Vector3(extents.x, -extents.y, -extents.z);

            Debug.DrawLine(ltf, rtf, color, duration, depthTest);
            Debug.DrawLine(rtf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, ltf, color, duration, depthTest);
            Debug.DrawLine(ltb, rtb, color, duration, depthTest);
            Debug.DrawLine(rtb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbb, color, duration, depthTest);
            Debug.DrawLine(lbb, ltb, color, duration, depthTest);
            Debug.DrawLine(ltf, ltb, color, duration, depthTest);
            Debug.DrawLine(rtf, rtb, color, duration, depthTest);
            Debug.DrawLine(rbf, rbb, color, duration, depthTest);
            Debug.DrawLine(lbf, lbb, color, duration, depthTest);
        }

        /// <summary>
        /// Draws a sphere.
        /// </summary>
        /// <param name="position">Center postion of the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="color">Color of the sphere.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawSphere(Vector3 position, float radius, Color color)
        {
            if (!isEnabled)
                return;

            Vector3 currentX = new Vector3(position.x, position.y, position.z + radius);
            Vector3 currentY = new Vector3(position.x + radius, position.y, position.z);
            Vector3 currentZ = new Vector3(position.x + radius, position.y, position.z);

            Vector3 newX, newY, newZ;

            for (int i = 1; i < SPHERE_PRECISION; i++)
            {
                newX = new Vector3(position.x, position.y + radius * Mathf.Sin(SPHERE_ANGLE * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(SPHERE_ANGLE * i * Mathf.Deg2Rad));
                newY = new Vector3(position.x + radius * Mathf.Cos(SPHERE_ANGLE * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(SPHERE_ANGLE * i * Mathf.Deg2Rad));
                newZ = new Vector3(position.x + radius * Mathf.Cos(SPHERE_ANGLE * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(SPHERE_ANGLE * i * Mathf.Deg2Rad), position.z);

                Debug.DrawLine(currentX, newX, color, duration, depthTest);
                Debug.DrawLine(currentY, newY, color, duration, depthTest);
                Debug.DrawLine(currentZ, newZ, color, duration, depthTest);

                currentX = newX;
                currentY = newY;
                currentZ = newZ;
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="position">Center position of the circle.</param>
        /// <param name="up">Up vector of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="color">Color of the circle.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCircle(Vector3 position, Vector3 up, float radius, Color color)
        {
            if (!isEnabled)
                return;

            Vector3 top = up.normalized * radius;
            Vector3 front = Vector3.Slerp(top, -top, 0.5f);
            Vector3 size = Vector3.Cross(top, front).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = size.x;
            matrix[1] = size.y;
            matrix[2] = size.z;

            matrix[4] = top.x;
            matrix[5] = top.y;
            matrix[6] = top.z;

            matrix[8] = front.x;
            matrix[9] = front.y;
            matrix[10] = front.z;

            Vector3 lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 nextPoint = Vector3.zero;

            for (int i = 0; i < CIRCLE_PRECISION; i++)
            {
                nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                nextPoint.y = 0;

                nextPoint = position + matrix.MultiplyPoint3x4(nextPoint);

                Debug.DrawLine(lastPoint, nextPoint, color, duration, depthTest);
                lastPoint = nextPoint;
            }
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="position">Center position of rectangle.</param>
        /// <param name="size">Size of rectangle.</param>
        /// <param name="up">Up vector of the rect.</param>
        /// <param name="color">Color of the rect.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRectangle(Vector3 position, Vector2 size, Vector3 up, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a cylinder.
        /// </summary>
        /// <param name="start">Start position of the cylinder.</param>
        /// <param name="end">End position of the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        /// <param name="color">Color of the cylinder.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCylinder(Vector3 start, Vector3 end, float radius, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a cone.
        /// </summary>
        /// <param name="bottom">Base position of the cone.</param>
        /// <param name="point">Point position of the cone.</param>
        /// <param name="radius">Radius of the cone base.</param>
        /// <param name="color">Color of the cone.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCone(Vector3 bottom, Vector3 point, float radius, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a capsule.
        /// </summary>
        /// <param name="start">Start position of the capsule.</param>
        /// <param name="end">End position of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        /// <param name="color">Color of the capsule.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws an arrow.
        /// </summary>
        /// <param name="position">Start position of the arrow.</param>
        /// <param name="direction">Direction and magnitude of the arrow.</param>
        /// <param name="color">Color of the arrow.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a raycast.
        /// </summary>
        /// <param name="origin">Origin position of the raycast.</param>
        /// <param name="direction">Direction of the raycast.</param>
        /// <param name="maxDistance">Max distance of the raycast.</param>
        /// <param name="color">Color of the raycast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycast(Vector3 origin, Vector3 direction, float maxDistance, Color color)
        {
            if (!isEnabled)
                return;

            Debug.DrawRay(origin, direction.normalized * maxDistance, color, duration, depthTest);
        }

        /// <summary>
        /// Draws a spherecast.
        /// </summary>
        /// <param name="origin">Origin position of the spherecast.</param>
        /// <param name="direction">Direction of the spherecast.</param>
        /// <param name="radius">Radius of the spherecast.</param>
        /// <param name="maxDistance">Max distance of the spherecast.</param>
        /// <param name="color">Color of the spherecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawSphereCast(Vector3 origin, Vector3 direction, float radius, float maxDistance, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        /// <param name="rotation">Rotation of the boxcast.</param>
        /// <param name="maxDistance">Max distance of the box cast.</param>
        /// <param name="color">Color of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion rotation, float maxDistance, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a capsulecast.
        /// </summary>
        /// <param name="point1">Top position of the capsulecast.</param>
        /// <param name="point2">Bottom position of the capsulecast.</param>
        /// <param name="radius">Radius of the capsulecast.</param>
        /// <param name="direction">Direction of the capsulecast.</param>
        /// <param name="maxDistance">Max distance of the capsulecast.</param>
        /// <param name="color">Color of the capsulecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, Color color)
        {
            if (!isEnabled)
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a raycast hit.
        /// </summary>
        /// <param name="origin">Origin position of raycast.</param>
        /// <param name="raycastHit">Raycast hit info to draw.</param>
        /// <param name="hitColor">Raycast color if hit.</param>
        /// <param name="noHitColor">Raycast color if no hit.</param>
        /// <param name="normalColor">Color of the raycast hit normal.</param>
        /// <param name="reflectionColor">Color of the raycast hit reflection.</param>
        /// <param name="normalLength">Length of the raycast hit normal.</param>
        /// <param name="reflectionLength">Length of the raycast hit reflection.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycastHit(Vector3 origin, RaycastHit raycastHit, Color hitColor, Color noHitColor, Color normalColor, Color reflectionColor, float normalLength, float reflectionLength)
        {
            if (!isEnabled)
                return;

            bool isHit = raycastHit.collider != null;
            Vector3 hitPoint = raycastHit.point;

            Debug.DrawLine(origin, hitPoint, isHit ? hitColor : noHitColor, duration, depthTest);

            if (isHit)
            {
                if (normalLength > 0)
                    Debug.DrawLine(hitPoint, hitPoint + (raycastHit.normal * normalLength), normalColor, duration, depthTest);

                if (reflectionLength > 0)
                    Debug.DrawLine(hitPoint, hitPoint + (Vector3.Reflect(origin - hitPoint, raycastHit.normal) * normalLength), reflectionColor, duration, depthTest);
            }
        }

        #region OVERLOADS

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">Start position of the line.</param>
        /// <param name="end">End position of the line.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, defaultColor);
        }

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="position">Position of the point.</param>
        /// <param name="scale">Size of the point.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawPoint(Vector3 position, float scale = 1f)
        {
            DrawPoint(position, defaultColor, scale);
        }

        /// <summary>
        /// Draws a bounds.
        /// </summary>
        /// <param name="bounds">Bounds to draw.</param>
        /// <param name="rotation">Rotation of the bounds.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBounds(Bounds bounds, Quaternion rotation)
        {
            DrawBounds(bounds, rotation, defaultColor);
        }

        /// <summary>
        /// Draws a bounds.
        /// </summary>
        /// <param name="bounds">Bounds to draw.</param>
        /// <param name="color">Color of the bounds.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBounds(Bounds bounds, Color color)
        {
            DrawBounds(bounds, Quaternion.identity, color);
        }

        /// <summary>
        /// Draws a bounds.
        /// </summary>
        /// <param name="bounds">Bounds to draw.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBounds(Bounds bounds)
        {
            DrawBounds(bounds, Quaternion.identity, defaultColor);
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="position">Center position of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="color">Color of the box.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBox(Vector3 position, Vector3 size, Color color)
        {
            DrawBox(position, size, Quaternion.identity, color);
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="position">Center position of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="rotation">Rotation of the box.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBox(Vector3 position, Vector3 size, Quaternion rotation)
        {
            DrawBox(position, size, rotation, defaultColor);
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="position">Center position of the box.</param>
        /// <param name="size">Size of the box.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBox(Vector3 position, Vector3 size)
        {
            DrawBox(position, size, Quaternion.identity, defaultColor);
        }

        /// <summary>
        /// Draws a sphere.
        /// </summary>
        /// <param name="position">Center postion of the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawSphere(Vector3 position, float radius)
        {
            DrawSphere(position, radius, defaultColor);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="position">Center position of the circle.</param>
        /// <param name="up">Up vector of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCircle(Vector3 position, Vector3 up, float radius)
        {
            DrawCircle(position, up, radius, defaultColor);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="position">Center position of rectangle.</param>
        /// <param name="size">Size of rectangle.</param>
        /// <param name="up">Up vector of the rect.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRectangle(Vector3 position, Vector2 size, Vector3 up)
        {
            DrawRectangle(position, size, up);
        }

        /// <summary>
        /// Draws a cylinder.
        /// </summary>
        /// <param name="start">Start position of the cylinder.</param>
        /// <param name="end">End position of the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCylinder(Vector3 start, Vector3 end, float radius)
        {
            DrawCylinder(start, end, radius, defaultColor);
        }

        /// <summary>
        /// Draws a cone.
        /// </summary>
        /// <param name="bottom">Base position of the cone.</param>
        /// <param name="point">Point position of the cone.</param>
        /// <param name="radius">Radius of the cone base.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCone(Vector3 bottom, Vector3 point, float radius)
        {
            DrawCone(bottom, point, radius, defaultColor);
        }

        /// <summary>
        /// Draws a cone.
        /// </summary>
        /// <param name="bottom">Base position of the cone.</param>
        /// <param name="point">Point position of the cone.</param>
        /// <param name="slope">Slope angle of the cone in degrees.</param>
        /// <param name="color">Color of the cone.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawConeFromSlope(Vector3 bottom, Vector3 point, float slope, Color color)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a cone.
        /// </summary>
        /// <param name="bottom">Base position of the cone.</param>
        /// <param name="point">Point position of the cone.</param>
        /// <param name="slope">Slope angle of the cone in degrees.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawConeFromSlope(Vector3 bottom, Vector3 point, float slope)
        {
            DrawConeFromSlope(bottom, point, slope, defaultColor);
        }

        /// <summary>
        /// Draws a capsule.
        /// </summary>
        /// <param name="start">Start position of the capsule.</param>
        /// <param name="end">End position of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCapsule(Vector3 start, Vector3 end, float radius)
        {
            DrawCapsule(start, end, radius, defaultColor);
        }

        /// <summary>
        /// Draws an arrow.
        /// </summary>
        /// <param name="position">Start position of the arrow.</param>
        /// <param name="direction">Direction and magnitude of the arrow.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawArrow(Vector3 position, Vector3 direction)
        {
            DrawArrow(position, direction, defaultColor);
        }

        /// <summary>
        /// Draws a raycast.
        /// </summary>
        /// <param name="origin">Origin position of the raycast.</param>
        /// <param name="direction">Direction of the raycast.</param>
        /// <param name="maxDistance">Max distance of the raycast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycast(Vector3 origin, Vector3 direction, float maxDistance)
        {
            DrawRaycast(origin, direction, maxDistance, defaultColor);
        }

        /// <summary>
        /// Draws a raycast.
        /// </summary>
        /// <param name="origin">Origin position of the raycast.</param>
        /// <param name="direction">Direction of the raycast.</param>
        /// <param name="color">Color of the raycast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycast(Vector3 origin, Vector3 direction, Color color)
        {
            DrawRaycast(origin, direction, Mathf.Infinity, color);
        }

        /// <summary>
        /// Draws a raycast.
        /// </summary>
        /// <param name="origin">Origin position of the raycast.</param>
        /// <param name="direction">Direction of the raycast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycast(Vector3 origin, Vector3 direction)
        {
            DrawRaycast(origin, direction, Mathf.Infinity, defaultColor);
        }

        /// <summary>
        /// Draws a spherecast.
        /// </summary>
        /// <param name="origin">Origin position of the spherecast.</param>
        /// <param name="direction">Direction of the spherecast.</param>
        /// <param name="radius">Radius of the spherecast.</param>
        /// <param name="maxDistance">Max distance of the spherecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawSphereCast(Vector3 origin, Vector3 direction, float radius, float maxDistance)
        {
            DrawSphereCast(origin, direction, radius, maxDistance, defaultColor);
        }

        /// <summary>
        /// Draws a spherecast.
        /// </summary>
        /// <param name="origin">Origin position of the spherecast.</param>
        /// <param name="direction">Direction of the spherecast.</param>
        /// <param name="radius">Radius of the spherecast.</param>
        /// <param name="color">Color of the spherecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawSphereCast(Vector3 origin, Vector3 direction, float radius, Color color)
        {
            DrawSphereCast(origin, direction, radius, Mathf.Infinity, color);
        }

        /// <summary>
        /// Draws a spherecast.
        /// </summary>
        /// <param name="origin">Origin position of the spherecast.</param>
        /// <param name="direction">Direction of the spherecast.</param>
        /// <param name="radius">Radius of the spherecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawSphereCast(Vector3 origin, Vector3 direction, float radius)
        {
            DrawSphereCast(origin, direction, radius, Mathf.Infinity, defaultColor);
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        /// <param name="rotation">Rotation of the boxcast.</param>
        /// <param name="color">Color of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion rotation, Color color)
        {
            DrawBoxCast(center, halfExtents, direction, rotation, Mathf.Infinity, color);
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        /// <param name="maxDistance">Max distance of the boxcast.</param>
        /// <param name="color">Color of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, float maxDistance, Color color)
        {
            DrawBoxCast(center, halfExtents, direction, Quaternion.identity, maxDistance, color);
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        /// <param name="color">Color of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Color color)
        {
            DrawBoxCast(center, halfExtents, direction, Quaternion.identity, Mathf.Infinity, color);
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        /// <param name="rotation">Rotation of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion rotation)
        {
            DrawBoxCast(center, halfExtents, direction, rotation, Mathf.Infinity, defaultColor);
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        /// <param name="maxDistance">Max distance of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, float maxDistance)
        {
            DrawBoxCast(center, halfExtents, direction, Quaternion.identity, maxDistance, defaultColor);
        }

        /// <summary>
        /// Draws a boxcast.
        /// </summary>
        /// <param name="center">Center position of the boxcast.</param>
        /// <param name="halfExtents">Half extents of the boxcast.</param>
        /// <param name="direction">Direction of the boxcast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction)
        {
            DrawBoxCast(center, halfExtents, direction, Quaternion.identity, Mathf.Infinity, defaultColor);
        }

        /// <summary>
        /// Draws a capsulecast.
        /// </summary>
        /// <param name="point1">Top position of the capsulecast.</param>
        /// <param name="point2">Bottom position of the capsulecast.</param>
        /// <param name="radius">Radius of the capsulecast.</param>
        /// <param name="direction">Direction of the capsulecast.</param>
        /// <param name="maxDistance">Max distance of the capsulecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
        {
            DrawCapsuleCast(point1, point2, radius, direction, maxDistance, defaultColor);
        }

        /// <summary>
        /// Draws a capsulecast.
        /// </summary>
        /// <param name="point1">Top position of the capsulecast.</param>
        /// <param name="point2">Bottom position of the capsulecast.</param>
        /// <param name="radius">Radius of the capsulecast.</param>
        /// <param name="direction">Direction of the capsulecast.</param>
        /// <param name="color">Color of the capsulecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, Color color)
        {
            DrawCapsuleCast(point1, point2, radius, direction, Mathf.Infinity, color);
        }

        /// <summary>
        /// Draws a capsulecast.
        /// </summary>
        /// <param name="point1">Top position of the capsulecast.</param>
        /// <param name="point2">Bottom position of the capsulecast.</param>
        /// <param name="radius">Radius of the capsulecast.</param>
        /// <param name="direction">Direction of the capsulecast.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
        {
            DrawCapsuleCast(point1, point2, radius, direction, Mathf.Infinity, defaultColor);
        }

        /// <summary>
        /// Draws a raycast hit.
        /// </summary>
        /// <param name="origin">Origin position of raycast.</param>
        /// <param name="raycastHit">Raycast hit info to draw.</param>
        /// <param name="rayColor">Raycast color.</param>
        /// <param name="normalColor">Color of the raycast hit normal.</param>
        /// <param name="reflectionColor">Color of the raycast hit reflection.</param>
        /// <param name="normalLength">Length of the raycast hit normal.</param>
        /// <param name="reflectionLength">Length of the raycast hit reflection.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycastHit(Vector3 origin, RaycastHit raycastHit, Color rayColor, Color normalColor, Color reflectionColor, float normalLength, float reflectionLength)
        {
            DrawRaycastHit(origin, raycastHit, rayColor, rayColor, normalColor, reflectionColor, normalLength, reflectionLength);
        }

        /// <summary>
        /// Draws a raycast hit.
        /// </summary>
        /// <param name="origin">Origin position of raycast.</param>
        /// <param name="raycastHit">Raycast hit info to draw.</param>
        /// <param name="rayColor">Raycast color.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycastHit(Vector3 origin, RaycastHit raycastHit, Color rayColor)
        {
            DrawRaycastHit(origin, raycastHit, rayColor, rayColor, rayColor, rayColor, 0f, 0f);
        }

        /// <summary>
        /// Draws a raycast hit.
        /// </summary>
        /// <param name="origin">Origin position of raycast.</param>
        /// <param name="raycastHit">Raycast hit info to draw.</param>
        [Conditional(CONDITIONAL)]
        public static void DrawRaycastHit(Vector3 origin, RaycastHit raycastHit)
        {
            DrawRaycastHit(origin, raycastHit, defaultColor, defaultColor, defaultColor, defaultColor, 0f, 0f);
        }

        #endregion

        /// <summary>
        /// Whether DebugUtil draws anything or not.
        /// </summary>
        public static bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        /// <summary>
        /// Whether DebugUtil draws use depth tests or not.
        /// </summary>
        public static bool DepthTest
        {
            get { return depthTest; }
            set { depthTest = value; }
        }

        /// <summary>
        /// Color DebugUtil draws in if not explicitly specified.
        /// </summary>
        public static Color DefaultColor
        {
            get { return defaultColor; }
            set { defaultColor = value; }
        }

        /// <summary>
        /// Lifetime of DebugUtil draws.
        /// </summary>
        public static float Duration
        {
            get { return duration; }
            set { duration = value; }
        }
    }
}
