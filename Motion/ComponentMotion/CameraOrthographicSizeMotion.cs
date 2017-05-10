using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an Camera orthographic size value towards a target value.
    /// </summary>
    public class CameraOrthographicSizeMotion : FloatMotion
    {
        public Camera Camera { get; private set; }

        public CameraOrthographicSizeMotion() : base(1, null, null) { }
        public CameraOrthographicSizeMotion(float duration) : base(duration, null, null) { }
        public CameraOrthographicSizeMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public CameraOrthographicSizeMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public CameraOrthographicSizeMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Camera camera, float targetOrthographicSize)
        {
            Camera = camera;
            SetValues(() => this.Camera.orthographicSize, targetOrthographicSize);
        }

        public void SetParameters(Camera camera, float startOrthographicSize, float targetOrthographicSize)
        {
            Camera = camera;
            SetValues(startOrthographicSize, targetOrthographicSize);
        }

        protected override void OnValueChanged()
        {
            if (Camera != null)
            {
                Camera.orthographicSize = CurrentValue;
            }
        }
    }
}
