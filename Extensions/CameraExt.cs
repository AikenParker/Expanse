using System.Collections;
using UnityEngine;
using Expanse.Random;

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
        /// <param name="camera">Camera to switch to.</param>
        public static void SwitchTo(this Camera camera)
        {
            Camera currentCam = Camera.current;

            if (currentCam != null)
            {
                currentCam.enabled = false;

                AudioListener currentAudioListener = currentCam.GetComponent<AudioListener>();

                if (currentAudioListener != null)
                    currentCam.GetComponent<AudioListener>().enabled = false;
            }

            camera.enabled = true;

            AudioListener nextAudioListener = camera.GetComponent<AudioListener>();

            if (nextAudioListener != null)
                camera.GetComponent<AudioListener>().enabled = true;
        }

        /// <summary>
        /// Determines if bounding volume is within the view of a camera.
        /// </summary>
        /// <param name="camera">Source camera component.</param>
        /// <param name="bounds">Bounding box to check.</param>
        /// <returns>Returns true if the bounds is fully or partially within view of a camera.</returns>
        public static bool IsBoundsInView(this Camera camera, Bounds bounds)
        {
            Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            return GeometryUtility.TestPlanesAABB(cameraPlanes, bounds);
        }

        /// <summary>
        /// Shakes the camera in a basic way changing its local position every frame.
        /// </summary>
        /// <param name="camera">Source camera component.</param>
        /// <param name="strength">Amount of shake to apply to the camera.</param>
        /// <param name="duration">Duration of the shaking.</param>
        /// <param name="frequency">Time between each shake.</param>
        /// <param name="rng">Random number generator to use.</param>
        /// <returns>Returns a coroutine that shakes a camera.</returns>
        public static IEnumerator Co_BasicShake(this Camera camera, float strength, float duration, float frequency, RNG rng = null)
        {
            rng = rng ?? Random.RandomUtil.Instance;

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
