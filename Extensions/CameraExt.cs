using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;
using System.Collections;

namespace Expanse
{
    public static class CameraExt
    {
        /// <summary>
        /// Switches active camera and audio listener.
        /// </summary>
        public static void SwitchTo(this Camera camera)
        {
            Camera currentCam = Camera.current;

            if (currentCam != null)
            {
                currentCam.enabled = false;
                currentCam.GetComponent<AudioListener>().enabled = false;
            }

            camera.enabled = true;
            camera.GetComponent<AudioListener>().enabled = true;
        }

        /// <summary>
        /// Determines if bounding volume is within the view of a camera.
        /// </summary>
        public static bool IsBoundsInView(this Camera camera, Bounds bounds)
        {
            Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            return GeometryUtility.TestPlanesAABB(cameraPlanes, bounds);
        }

        /// <summary>
        /// Shakes the camera in a basic way changing its local position every frame.
        /// </summary>
        public static IEnumerator Co_BasicShake(this Camera camera, float strength, float duration, float frequency)
        {
            float StartTime = Time.time;

            while (Time.time < StartTime + duration)
            {
                Vector3 Offset = new Vector3(
                    UnityEngine.Random.Range(-strength, strength),
                    UnityEngine.Random.Range(-strength, strength),
                    UnityEngine.Random.Range(-strength, strength));

                camera.transform.localPosition += Offset;

                if (frequency < Time.deltaTime)
                    yield return new WaitForEndOfFrame();
                else
                    yield return new WaitForSeconds(frequency);

                camera.transform.localPosition -= Offset;
            }
        }
    }
}
