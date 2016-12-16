using System;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Wrapper for two axis input listeners.
    /// </summary>
    public class InputListenerDualAxis : IDisposable
    {
        public InputListener InputX { get; private set; }
        public InputListener InputY { get; private set; }

        public InputListenerDualAxis(InputListener AxisX, InputListener AxisY)
        {
            if (AxisX.InputType != InputListener.InputTypes.AXIS)
                Debug.LogWarning("Input Axis X is not an Axis Input Listener");

            if (AxisY.InputType != InputListener.InputTypes.AXIS)
                Debug.LogWarning("Input Axis Y is not an Axis Input Listener");

            this.InputX = AxisX;
            this.InputY = AxisY;
        }

        public Vector2 AxisRaw
        {
            get
            {
                return new Vector2(InputX.Axis, InputY.Axis);
            }
        }

        public Vector2 Axis
        {
            get
            {
                Vector2 rawAxis = AxisRaw;
                return rawAxis != Vector2.zero ? rawAxis.normalized : Vector2.zero;
            }
        }

        public float AxisX
        {
            get
            {
                return InputX.Axis;
            }
        }

        public float AxisY
        {
            get
            {
                return InputY.Axis;
            }
        }

        public void Dispose()
        {
            InputX.Dispose();
            InputY.Dispose();
        }
    }
}
