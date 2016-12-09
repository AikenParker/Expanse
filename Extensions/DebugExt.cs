using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

/// <summary>
/// Debug Extension
/// 	- Static class that extends Unity's debugging functionallity.
/// 	- Attempts to mimic Unity's existing debugging behaviour for ease-of-use.
/// 	- Includes gizmo drawing methods for less memory-intensive debug visualization.
/// </summary>

namespace Expanse
{
    public static class DebugExt
    {
        #region DebugDrawFunctions

        /// <summary>
        /// 	- Debugs a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to debug.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the point.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the point.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the point.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not this point should be faded when behind other objects.
        /// </param>
        public static void DebugPoint(Vector3 position, Color color, float scale = 1.0f, float duration = 0, bool depthTest = true)
        {
            color = (color == default(Color)) ? Color.white : color;

            Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
            Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
            Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to debug.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the point.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the point.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not this point should be faded when behind other objects.
        /// </param>
        public static void DebugPoint(Vector3 position, float scale = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugPoint(position, Color.white, scale, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to debug.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the bounds.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the bounds.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the bounds should be faded when behind other objects.
        /// </param>
        public static void DebugBounds(Bounds bounds, Color color, float duration = 0, bool depthTest = true)
        {
            Vector3 center = bounds.center;

            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            Vector3 ruf = center + new Vector3(x, y, z);
            Vector3 rub = center + new Vector3(x, y, -z);
            Vector3 luf = center + new Vector3(-x, y, z);
            Vector3 lub = center + new Vector3(-x, y, -z);

            Vector3 rdf = center + new Vector3(x, -y, z);
            Vector3 rdb = center + new Vector3(x, -y, -z);
            Vector3 lfd = center + new Vector3(-x, -y, z);
            Vector3 lbd = center + new Vector3(-x, -y, -z);

            Debug.DrawLine(ruf, luf, color, duration, depthTest);
            Debug.DrawLine(ruf, rub, color, duration, depthTest);
            Debug.DrawLine(luf, lub, color, duration, depthTest);
            Debug.DrawLine(rub, lub, color, duration, depthTest);

            Debug.DrawLine(ruf, rdf, color, duration, depthTest);
            Debug.DrawLine(rub, rdb, color, duration, depthTest);
            Debug.DrawLine(luf, lfd, color, duration, depthTest);
            Debug.DrawLine(lub, lbd, color, duration, depthTest);

            Debug.DrawLine(rdf, lfd, color, duration, depthTest);
            Debug.DrawLine(rdf, rdb, color, duration, depthTest);
            Debug.DrawLine(lfd, lbd, color, duration, depthTest);
            Debug.DrawLine(lbd, rdb, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to debug.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the bounds.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the bounds should be faded when behind other objects.
        /// </param>
        public static void DebugBounds(Bounds bounds, float duration = 0, bool depthTest = true)
        {
            DebugBounds(bounds, Color.white, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform that the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The size of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- Color of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            Vector3 lbb = transform.TransformPoint(center + ((-size) * 0.5f));
            Vector3 rbb = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = transform.TransformPoint(center + ((size) * 0.5f));
            Vector3 ruf = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Debug.DrawLine(lbb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbb, color, duration, depthTest);

            Debug.DrawLine(lub, rub, color, duration, depthTest);
            Debug.DrawLine(rub, luf, color, duration, depthTest);
            Debug.DrawLine(luf, ruf, color, duration, depthTest);
            Debug.DrawLine(ruf, lub, color, duration, depthTest);

            Debug.DrawLine(lbb, lub, color, duration, depthTest);
            Debug.DrawLine(rbb, rub, color, duration, depthTest);
            Debug.DrawLine(lbf, luf, color, duration, depthTest);
            Debug.DrawLine(rbf, ruf, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform that the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            DebugLocalCube(transform, size, Color.white, center, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        ///		- The size of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- Color of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            color = (color == default(Color)) ? Color.white : color;

            Vector3 lbb = space.MultiplyPoint3x4(center + ((-size) * 0.5f));
            Vector3 rbb = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = space.MultiplyPoint3x4(center + ((size) * 0.5f));
            Vector3 ruf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Debug.DrawLine(lbb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbb, color, duration, depthTest);

            Debug.DrawLine(lub, rub, color, duration, depthTest);
            Debug.DrawLine(rub, luf, color, duration, depthTest);
            Debug.DrawLine(luf, ruf, color, duration, depthTest);
            Debug.DrawLine(ruf, lub, color, duration, depthTest);

            Debug.DrawLine(lbb, lub, color, duration, depthTest);
            Debug.DrawLine(rbb, rub, color, duration, depthTest);
            Debug.DrawLine(lbf, luf, color, duration, depthTest);
            Debug.DrawLine(rbf, ruf, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        ///		- The size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            DebugLocalCube(space, size, Color.white, center, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            Vector3 _up = up.normalized * radius;
            Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
            Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = _right.x;
            matrix[1] = _right.y;
            matrix[2] = _right.z;

            matrix[4] = _up.x;
            matrix[5] = _up.y;
            matrix[6] = _up.z;

            matrix[8] = _forward.x;
            matrix[9] = _forward.y;
            matrix[10] = _forward.z;

            Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 _nextPoint = Vector3.zero;

            color = (color == default(Color)) ? Color.white : color;

            for (var i = 0; i < 91; i++)
            {
                _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                _nextPoint.y = 0;

                _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

                Debug.DrawLine(_lastPoint, _nextPoint, color, duration, depthTest);
                _lastPoint = _nextPoint;
            }
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, Vector3.up, color, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, Vector3 up, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, up, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, Vector3.up, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a wire sphere.
        /// </summary>
        /// <param name='position'>
        /// 	- The position of the center of the sphere.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the sphere.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the sphere.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the sphere.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the sphere should be faded when behind other objects.
        /// </param>
        public static void DebugWireSphere(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            float angle = 10.0f;

            Vector3 x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
            Vector3 y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
            Vector3 z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);

            Vector3 new_x;
            Vector3 new_y;
            Vector3 new_z;

            for (int i = 1; i < 37; i++)
            {

                new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
                new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
                new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

                Debug.DrawLine(x, new_x, color, duration, depthTest);
                Debug.DrawLine(y, new_y, color, duration, depthTest);
                Debug.DrawLine(z, new_z, color, duration, depthTest);

                x = new_x;
                y = new_y;
                z = new_z;
            }
        }

        /// <summary>
        /// 	- Debugs a wire sphere.
        /// </summary>
        /// <param name='position'>
        /// 	- The position of the center of the sphere.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the sphere.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the sphere.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the sphere should be faded when behind other objects.
        /// </param>
        public static void DebugWireSphere(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugWireSphere(position, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cylinder.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cylinder should be faded when behind other objects.
        /// </param>
        public static void DebugCylinder(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            //Radial circles
            DebugExt.DebugCircle(start, up, color, radius, duration, depthTest);
            DebugExt.DebugCircle(end, -up, color, radius, duration, depthTest);
            DebugExt.DebugCircle((start + end) * 0.5f, up, color, radius, duration, depthTest);

            //Side lines
            Debug.DrawLine(start + right, end + right, color, duration, depthTest);
            Debug.DrawLine(start - right, end - right, color, duration, depthTest);

            Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
            Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

            //Start endcap
            Debug.DrawLine(start - right, start + right, color, duration, depthTest);
            Debug.DrawLine(start - forward, start + forward, color, duration, depthTest);

            //End endcap
            Debug.DrawLine(end - right, end + right, color, duration, depthTest);
            Debug.DrawLine(end - forward, end + forward, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cylinder.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cylinder should be faded when behind other objects.
        /// </param>
        public static void DebugCylinder(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true)
        {
            DebugCylinder(start, end, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone gets wider in.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, Vector3 direction, Color color, float angle = 45, float duration = 0, bool depthTest = true)
        {
            float length = direction.magnitude;

            Vector3 _forward = direction;
            Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
            Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;

            direction = direction.normalized;

            Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);

            float dist;
            var farPlane = new Plane(-direction, position + _forward);
            var distRay = new Ray(position, slerpedVector);

            farPlane.Raycast(distRay, out dist);

            Debug.DrawRay(position, slerpedVector.normalized * dist, color);
            Debug.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist, color, duration, depthTest);
            Debug.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist, color, duration, depthTest);
            Debug.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist, color, duration, depthTest);

            DebugExt.DebugCircle(position + _forward, direction, color, (_forward - (slerpedVector.normalized * dist)).magnitude, duration, depthTest);
            DebugExt.DebugCircle(position + (_forward * 0.5f), direction, color, ((_forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone gets wider in.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, Vector3 direction, float angle = 45, float duration = 0, bool depthTest = true)
        {
            DebugCone(position, direction, Color.white, angle, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, Color color, float angle = 45, float duration = 0, bool depthTest = true)
        {
            DebugCone(position, Vector3.up, color, angle, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, float angle = 45, float duration = 0, bool depthTest = true)
        {
            DebugCone(position, Vector3.up, Color.white, angle, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the arrow.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the arrow.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the arrow should be faded when behind other objects. 
        /// </param>
        public static void DebugArrow(Vector3 position, Vector3 direction, Color color, float duration = 0, bool depthTest = true)
        {
            Debug.DrawRay(position, direction, color, duration, depthTest);
            DebugExt.DebugCone(position + direction, -direction * 0.333f, color, 15, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the arrow.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the arrow should be faded when behind other objects. 
        /// </param>
        public static void DebugArrow(Vector3 position, Vector3 direction, float duration = 0, bool depthTest = true)
        {
            DebugArrow(position, direction, Color.white, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the capsule.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the capsule should be faded when behind other objects.
        /// </param>
        public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            float height = (start - end).magnitude;
            float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
            Vector3 middle = (end + start) * 0.5f;

            start = middle + ((start - middle).normalized * sideLength);
            end = middle + ((end - middle).normalized * sideLength);

            //Radial circles
            DebugExt.DebugCircle(start, up, color, radius, duration, depthTest);
            DebugExt.DebugCircle(end, -up, color, radius, duration, depthTest);

            //Side lines
            Debug.DrawLine(start + right, end + right, color, duration, depthTest);
            Debug.DrawLine(start - right, end - right, color, duration, depthTest);

            Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
            Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

            for (int i = 1; i < 26; i++)
            {

                //Start endcap
                Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);

                //End endcap
                Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            }
        }

        /// <summary>
        /// 	- Debugs a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the capsule.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the capsule should be faded when behind other objects.
        /// </param>
        public static void DebugCapsule(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true)
        {
            DebugCapsule(start, end, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        ///     - Debugs a box.
        /// </summary>
        /// <param name="origin">
        ///     - The center position of the box.
        /// </param>
        /// <param name="halfExtents">
        ///     - Half the volume dimensions of the box.
        /// </param>
        /// <param name="orientation">
        ///     - The rotation of the box.
        /// </param>
        /// <param name="color">
        ///     - The color of the box.
        /// </param>
        /// <param name="duration">
        ///     - How long to draw the box.
        /// </param>
        /// <param name="depthTest">
        ///     - Whether or not the box should be faded when behind other objects.
        /// </param>
        public static void DebugBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color, float duration = 0, bool depthTest = true)
        {
            DebugBox(new Box(origin, halfExtents, orientation), color, duration, depthTest);
        }

        /// <summary>
        ///     - Debugs a box cast.
        /// </summary>
        /// <param name="origin">
        ///     - The center position of the start box.
        /// </param>
        /// <param name="halfExtents">
        ///     - Half the volume dimensions of both boxes.
        /// </param>
        /// <param name="orientation">
        ///     - The rotation of both boxes.
        /// </param>
        /// <param name="direction">
        ///     - The direction of the box cast.
        /// </param>
        /// <param name="distance">
        ///     - The distance of the box cast.
        /// </param>
        /// <param name="color">
        ///     - The color of the box cast.
        /// </param>
        /// <param name="duration">
        ///     - How long to draw the box cast.
        /// </param>
        /// <param name="depthTest">
        ///     - Whether or not the box cast should be faded when behind other objects.
        /// </param>
        public static void DebugBoxCast(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color, float duration = 0, bool depthTest = true)
        {
            direction.Normalize();
            Box bottomBox = new Box(origin, halfExtents, orientation);
            Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

            Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
            Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
            Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
            Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

            DebugBox(bottomBox, color, duration, depthTest);
            DebugBox(topBox, color, duration, depthTest);
        }


        #endregion

        #region GizmoDrawFunctions

        /// <summary>
        /// 	- Draws a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to draw.
        /// </param>
        ///  <param name='color'>
        /// 	- The color of the drawn point.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the drawn point.
        /// </param>
        public static void DrawPoint(Vector3 position, Color color, float scale = 1.0f)
        {
            Color oldColor = Gizmos.color;

            Gizmos.color = color;
            Gizmos.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale);
            Gizmos.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale);
            Gizmos.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to draw.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the drawn point.
        /// </param>
        public static void DrawPoint(Vector3 position, float scale = 1.0f)
        {
            DrawPoint(position, Color.white, scale);
        }

        /// <summary>
        /// 	- Draws an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to draw.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the bounds.
        /// </param>
        public static void DrawBounds(Bounds bounds, Color color)
        {
            Vector3 center = bounds.center;

            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            Vector3 ruf = center + new Vector3(x, y, z);
            Vector3 rub = center + new Vector3(x, y, -z);
            Vector3 luf = center + new Vector3(-x, y, z);
            Vector3 lub = center + new Vector3(-x, y, -z);

            Vector3 rdf = center + new Vector3(x, -y, z);
            Vector3 rdb = center + new Vector3(x, -y, -z);
            Vector3 lfd = center + new Vector3(-x, -y, z);
            Vector3 lbd = center + new Vector3(-x, -y, -z);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawLine(ruf, luf);
            Gizmos.DrawLine(ruf, rub);
            Gizmos.DrawLine(luf, lub);
            Gizmos.DrawLine(rub, lub);

            Gizmos.DrawLine(ruf, rdf);
            Gizmos.DrawLine(rub, rdb);
            Gizmos.DrawLine(luf, lfd);
            Gizmos.DrawLine(lub, lbd);

            Gizmos.DrawLine(rdf, lfd);
            Gizmos.DrawLine(rdf, rdb);
            Gizmos.DrawLine(lfd, lbd);
            Gizmos.DrawLine(lbd, rdb);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to draw.
        /// </param>
        public static void DrawBounds(Bounds bounds)
        {
            DrawBounds(bounds, Color.white);
        }

        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        ///		- The local position of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cube.
        /// </param>
        public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3))
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Vector3 lbb = transform.TransformPoint(center + ((-size) * 0.5f));
            Vector3 rbb = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = transform.TransformPoint(center + ((size) * 0.5f));
            Vector3 ruf = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Gizmos.DrawLine(lbb, rbb);
            Gizmos.DrawLine(rbb, lbf);
            Gizmos.DrawLine(lbf, rbf);
            Gizmos.DrawLine(rbf, lbb);

            Gizmos.DrawLine(lub, rub);
            Gizmos.DrawLine(rub, luf);
            Gizmos.DrawLine(luf, ruf);
            Gizmos.DrawLine(ruf, lub);

            Gizmos.DrawLine(lbb, lub);
            Gizmos.DrawLine(rbb, rub);
            Gizmos.DrawLine(lbf, luf);
            Gizmos.DrawLine(rbf, ruf);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        ///		- The local position of the cube.
        /// </param>	
        public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3))
        {
            DrawLocalCube(transform, size, Color.white, center);
        }

        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The local position of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cube.
        /// </param>
        public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3))
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Vector3 lbb = space.MultiplyPoint3x4(center + ((-size) * 0.5f));
            Vector3 rbb = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = space.MultiplyPoint3x4(center + ((size) * 0.5f));
            Vector3 ruf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Gizmos.DrawLine(lbb, rbb);
            Gizmos.DrawLine(rbb, lbf);
            Gizmos.DrawLine(lbf, rbf);
            Gizmos.DrawLine(rbf, lbb);

            Gizmos.DrawLine(lub, rub);
            Gizmos.DrawLine(rub, luf);
            Gizmos.DrawLine(luf, ruf);
            Gizmos.DrawLine(ruf, lub);

            Gizmos.DrawLine(lbb, lub);
            Gizmos.DrawLine(rbb, rub);
            Gizmos.DrawLine(lbf, luf);
            Gizmos.DrawLine(rbf, ruf);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The local position of the cube.
        /// </param>
        public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3))
        {
            DrawLocalCube(space, size, Color.white, center);
        }

        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f)
        {
            up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
            Vector3 _forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 _right = Vector3.Cross(up, _forward).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = _right.x;
            matrix[1] = _right.y;
            matrix[2] = _right.z;

            matrix[4] = up.x;
            matrix[5] = up.y;
            matrix[6] = up.z;

            matrix[8] = _forward.x;
            matrix[9] = _forward.y;
            matrix[10] = _forward.z;

            Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 _nextPoint = Vector3.zero;

            Color oldColor = Gizmos.color;
            Gizmos.color = (color == default(Color)) ? Color.white : color;

            for (var i = 0; i < 91; i++)
            {
                _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                _nextPoint.y = 0;

                _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

                Gizmos.DrawLine(_lastPoint, _nextPoint);
                _lastPoint = _nextPoint;
            }

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, Color color, float radius = 1.0f)
        {
            DrawCircle(position, Vector3.up, color, radius);
        }

        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1.0f)
        {
            DrawCircle(position, position, Color.white, radius);
        }

        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, float radius = 1.0f)
        {
            DrawCircle(position, Vector3.up, Color.white, radius);
        }

        //Wiresphere already exists

        /// <summary>
        /// 	- Draws a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        public static void DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1.0f)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            //Radial circles
            DebugExt.DrawCircle(start, up, color, radius);
            DebugExt.DrawCircle(end, -up, color, radius);
            DebugExt.DrawCircle((start + end) * 0.5f, up, color, radius);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            //Side lines
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);

            Gizmos.DrawLine(start + forward, end + forward);
            Gizmos.DrawLine(start - forward, end - forward);

            //Start endcap
            Gizmos.DrawLine(start - right, start + right);
            Gizmos.DrawLine(start - forward, start + forward);

            //End endcap
            Gizmos.DrawLine(end - right, end + right);
            Gizmos.DrawLine(end - forward, end + forward);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        public static void DrawCylinder(Vector3 start, Vector3 end, float radius = 1.0f)
        {
            DrawCylinder(start, end, Color.white, radius);
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone to get wider in.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45)
        {
            float length = direction.magnitude;

            Vector3 _forward = direction;
            Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
            Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;

            direction = direction.normalized;

            Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);

            float dist;
            var farPlane = new Plane(-direction, position + _forward);
            var distRay = new Ray(position, slerpedVector);

            farPlane.Raycast(distRay, out dist);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawRay(position, slerpedVector.normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist);

            DebugExt.DrawCircle(position + _forward, direction, color, (_forward - (slerpedVector.normalized * dist)).magnitude);
            DebugExt.DrawCircle(position + (_forward * 0.5f), direction, color, ((_forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone to get wider in.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, Vector3 direction, float angle = 45)
        {
            DrawCone(position, direction, Color.white, angle);
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, Color color, float angle = 45)
        {
            DrawCone(position, Vector3.up, color, angle);
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, float angle = 45)
        {
            DrawCone(position, Vector3.up, Color.white, angle);
        }

        /// <summary>
        /// 	- Draws an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the arrow.
        /// </param>
        public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawRay(position, direction);
            DebugExt.DrawCone(position + direction, -direction * 0.333f, color, 15);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        public static void DrawArrow(Vector3 position, Vector3 direction)
        {
            DrawArrow(position, direction, Color.white);
        }

        /// <summary>
        /// 	- Draws a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            float height = (start - end).magnitude;
            float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
            Vector3 middle = (end + start) * 0.5f;

            start = middle + ((start - middle).normalized * sideLength);
            end = middle + ((end - middle).normalized * sideLength);

            //Radial circles
            DebugExt.DrawCircle(start, up, color, radius);
            DebugExt.DrawCircle(end, -up, color, radius);

            //Side lines
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);

            Gizmos.DrawLine(start + forward, end + forward);
            Gizmos.DrawLine(start - forward, end - forward);

            for (int i = 1; i < 26; i++)
            {

                //Start endcap
                Gizmos.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start);
                Gizmos.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start);
                Gizmos.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start);
                Gizmos.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start);

                //End endcap
                Gizmos.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end);
                Gizmos.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end);
                Gizmos.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end);
                Gizmos.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end);
            }

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        public static void DrawCapsule(Vector3 start, Vector3 end, float radius = 1)
        {
            DrawCapsule(start, end, Color.white, radius);
        }

        #endregion

        #region InternalHelpers

        private static void DebugBox(Box box, Color color, float duration, bool depthTest)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color, duration, depthTest);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color, duration, depthTest);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color, duration, depthTest);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color, duration, depthTest);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color, duration, depthTest);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color, duration, depthTest);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color, duration, depthTest);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color, duration, depthTest);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color, duration, depthTest);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color, duration, depthTest);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color, duration, depthTest);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color, duration, depthTest);
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }

        private struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
            public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
            public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
            public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

            public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
            public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
            public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
            public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
            public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
            public Vector3 backTopRight { get { return localBackTopRight + origin; } }
            public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
            public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

            public Vector3 origin { get; private set; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }

            public Box(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }

            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        #endregion
    }

    public static class DebugExt2D
    {
        /// <summary>
        ///     - Debugs a 2D rectangle.
        /// </summary>
        /// <param name="position">
        ///     - The position of the center of the rectangle.
        /// </param>
        /// <param name="size">
        ///     - The length and width of the rectangle.
        /// </param>
        /// <param name="color">
        ///     - The color of the rectangle.
        /// </param>
        /// <param name="angle">
        ///     - The angle of the rectangle.
        /// </param>
        /// <param name="duration">
        ///     - How long to draw the rectangle.
        /// </param>
        /// <param name="depthTest">
        ///     - Whether or not the rectangle should be faded when behind other objects.
        /// </param>
        public static void DebugRect(Vector2 position, Vector2 size, Color color, float angle = 0, float duration = 0, bool depthTest = true)
        {
            Vector2 halfSize = size * 0.5f;

            Vector2 tl = (position + new Vector2(-halfSize.x, -halfSize.y)).RotateAroundCenter(position, angle);
            Vector2 tr = (position + new Vector2(halfSize.x, -halfSize.y)).RotateAroundCenter(position, angle);
            Vector2 bl = (position + new Vector2(-halfSize.x, halfSize.y)).RotateAroundCenter(position, angle);
            Vector2 br = (position + new Vector2(halfSize.x, halfSize.y)).RotateAroundCenter(position, angle);

            Debug.DrawLine(tl, tr, color, duration, depthTest);
            Debug.DrawLine(tr, br, color, duration, depthTest);
            Debug.DrawLine(br, bl, color, duration, depthTest);
            Debug.DrawLine(bl, tl, color, duration, depthTest);
        }

        /// <summary>
        ///     - Debugs a 2D rectangle.
        /// </summary>
        /// <param name="position">
        ///     - The position of the center of the rectangle.
        /// </param>
        /// <param name="size">
        ///     - The length and width of the rectangle.
        /// </param>
        /// <param name="angle">
        ///     - The angle of the rectangle.
        /// </param>
        /// <param name="duration">
        ///     - How long to draw the rectangle.
        /// </param>
        /// <param name="depthTest">
        ///     - Whether or not the rectangle should be faded when behind other objects.
        /// </param>
        public static void DebugRect(Vector2 position, Vector2 size, float angle = 0, float duration = 0, bool depthTest = true)
        {
            DebugExt2D.DebugRect(position, size, Color.white, angle, duration, depthTest);
        }

        /// <summary>
        ///     - Debugs a 2D circle.
        /// </summary>
        /// <param name="position">
        ///     - The position of the center of the circle.
        /// </param>
        /// <param name="radius">
        ///     - The radius of the circle.
        /// </param>
        /// <param name="color">
        ///     - The color of the circle.
        /// </param>
        /// <param name="duration">
        ///     - How long to draw the circle.
        /// </param>
        /// <param name="depthTest">
        ///     - Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector2 position, float radius, Color color, float duration = 0, bool depthTest = true)
        {
            Vector3 _position = position.ToVector3();

            Vector3 _up = Vector3.forward * radius;
            Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
            Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = _right.x;
            matrix[1] = _right.y;
            matrix[2] = _right.z;

            matrix[4] = _up.x;
            matrix[5] = _up.y;
            matrix[6] = _up.z;

            matrix[8] = _forward.x;
            matrix[9] = _forward.y;
            matrix[10] = _forward.z;

            Vector3 _lastPoint = _position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 _nextPoint = Vector3.zero;

            color = (color == default(Color)) ? Color.white : color;

            for (var i = 0; i < 91; i++)
            {
                _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                _nextPoint.y = 0;

                _nextPoint = _position + matrix.MultiplyPoint3x4(_nextPoint);

                Debug.DrawLine(_lastPoint, _nextPoint, color, duration, depthTest);
                _lastPoint = _nextPoint;
            }
        }

        /// <summary>
        ///     - Debugs a 2D circle.
        /// </summary>
        /// <param name="position">
        ///     - The position of the center of the circle.
        /// </param>
        /// <param name="radius">
        ///     - The radius of the circle.
        /// </param>
        /// <param name="duration">
        ///     - How long to draw the circle.
        /// </param>
        /// <param name="depthTest">
        ///     - Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector2 position, float radius, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, radius, Color.white, duration, depthTest);
        }
    }
}
