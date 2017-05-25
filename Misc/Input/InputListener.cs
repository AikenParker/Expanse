using UnityEngine;
using System;

namespace Expanse.Misc
{
    /// <summary>
    /// Automatically managed input listener.
    /// </summary>
    public class InputListener : IDisposable, IUpdate
    {
        public InputTypes InputType { get; private set; }
        public string InputName { get; private set; }
        public float AxisThreshold { get; private set; }
        private float AxisMin { get; set; }
        private float AxisMax { get; set; }

        public bool Active { get; set; }
        public bool IsPressed { get; private set; }
        public bool IsHeld { get; private set; }
        public bool IsReleased { get; private set; }

        public float Axis { get; private set; }
        public float ScaledHoldDuration { get; private set; }
        public float UnscaledHoldDuration { get; private set; }

        public InputListener(string inputName, InputTypes type = InputTypes.Button)
        {
            this.InputName = inputName;
            this.InputType = type;
            this.AxisThreshold = 0.05f;
            this.AxisMin = -1f;
            this.AxisMax = 1f;

            CallBackRelay.SubscribeToGlobalUpdate(this);
            CallBackRelay.GlobalCBR.Destroyed += Dispose;

            this.Active = true;
        }

        void IUpdate.OnUpdate(float deltaTime)
        {
            if (!Active || InputType == InputTypes.None)
            {
                IsReleased = IsHeld = IsPressed = false;
                UnscaledHoldDuration = ScaledHoldDuration = Axis = 0f;
                return;
            }

            switch (InputType)
            {
                case InputTypes.Button:
                    ButtonUpdate();
                    break;
                case InputTypes.Axis:
                    AxisUpdate();
                    break;
            }
        }

        private void ButtonUpdate()
        {
            IsPressed = Input.GetButtonDown(InputName);
            IsHeld = Input.GetButton(InputName);
            IsReleased = Input.GetButtonUp(InputName);

            if (IsHeld)
            {
                ScaledHoldDuration += Time.deltaTime;
                UnscaledHoldDuration += Time.unscaledDeltaTime;
            }
            else
            {
                UnscaledHoldDuration = ScaledHoldDuration = 0f;
            }
        }

        private void AxisUpdate()
        {
            Axis = Input.GetAxis(InputName);

            IsPressed = Axis >= (AxisMax - AxisThreshold) && !IsHeld;
            IsReleased = Axis < (AxisMax - AxisThreshold) && IsHeld;
            IsHeld = Axis >= (AxisMax - AxisThreshold);

            if (IsHeld)
            {
                ScaledHoldDuration += Time.deltaTime;
                UnscaledHoldDuration += Time.unscaledDeltaTime;
            }
            else
            {
                UnscaledHoldDuration = ScaledHoldDuration = 0f;
            }
        }

        public void Dispose()
        {
            CallBackRelay.UnsubscribeFromGlobalUpdate(this);
        }

        bool IUpdate.UnsafeUpdates
        {
            get
            {
                // TODO: Make attachable
                return true;
            }
        }

        bool IUpdate.AlwaysUpdate
        {
            get
            {
                return false;
            }
        }

        bool IUpdate.UnscaledDelta
        {
            get
            {
                return false;
            }
        }

        GameObject IUnity.gameObject
        {
            get
            {
                return null;
            }
        }

        MonoBehaviour IUnity.MonoBehaviour
        {
            get
            {
                return null;
            }
        }

        public enum InputTypes
        {
            None = 0,
            Button = 1,
            Axis = 2
        }
    }
}
