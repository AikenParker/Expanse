using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    /// <summary>
    /// Automatically managed input listener.
    /// </summary>
    public class InputListener : IDisposable, IComplexUpdate
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

        public InputListener(string inputName, InputTypes type = InputTypes.BUTTON)
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
            if (!Active || InputType == InputTypes.NONE)
            {
                IsReleased = IsHeld = IsPressed = false;
                UnscaledHoldDuration = ScaledHoldDuration = Axis = 0f;
                return;
            }

            switch (InputType)
            {
                case InputTypes.BUTTON:
                    ButtonUpdate();
                    break;
                case InputTypes.AXIS:
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

        bool IComplexUpdate.UnsafeUpdates
        {
            get
            {
                // TODO: Make attachable
                return true;
            }
        }

        bool IComplexUpdate.AlwaysUpdate
        {
            get
            {
                return false;
            }
        }

        bool IComplexUpdate.UnscaledDelta
        {
            get
            {
                return false;
            }
        }

        int IPriority.Priority
        {
            get
            {
                return 1;
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
            NONE = 0,
            BUTTON = 1,
            AXIS = 2
        }
    }
}
