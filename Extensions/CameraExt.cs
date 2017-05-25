using System.Collections;
using UnityEngine;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Camera related extension methods.
    /// </summary>
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
        public static IEnumerator Co_BasicShake(this Camera camera, float strength, float duration, float frequency, Random rng = null)
        {
            rng = rng ?? RandomUtil.Instance;

            float StartTime = TimeManager.Time;

            while (TimeManager.Time < StartTime + duration)
            {
                Vector3 offset = rng.Vector3(strength);

                camera.transform.localPosition += offset;

                if (frequency > 0f)
                    yield return new WaitForSeconds(frequency);
                else
                    yield return new WaitForEndOfFrame();

                camera.transform.localPosition -= offset;
            }
        }
    }
}
