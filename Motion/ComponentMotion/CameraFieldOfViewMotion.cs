using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an Camera field of view value towards a target value.
    /// </summary>
    public class CameraFieldOfViewMotion : FloatMotion
    {
        public Camera Camera { get; private set; }

        public CameraFieldOfViewMotion() : base(1, null, null) { }
        public CameraFieldOfViewMotion(float duration) : base(duration, null, null) { }
        public CameraFieldOfViewMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public CameraFieldOfViewMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public CameraFieldOfViewMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Camera camera, float targetFieldOfView)
        {
            Camera = camera;
            SetValues(() => this.Camera.fieldOfView, targetFieldOfView);
        }

        public void SetParameters(Camera camera, float startFieldOfView, float targetFieldOfView)
        {
            Camera = camera;
            SetValues(startFieldOfView, targetFieldOfView);
        }

        protected override void OnValueChanged()
        {
            if (Camera != null)
            {
                Camera.fieldOfView = CurrentValue;
            }
        }
    }
}
