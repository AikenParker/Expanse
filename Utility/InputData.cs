using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Easy to use input wrapper.
    /// </summary>
    public class InputData
    {
        public readonly string input;
        public readonly KeyCode[] inputKeys;
        public readonly bool isButton, isAxis, isKey;
        public readonly float axisThreshold;

        public bool Down { get; private set; }
        public bool Hold { get; private set; }
        public bool Up { get; private set; }
        public float Axis { get; private set; }
        public float HoldTime { get; private set; }

        private bool downFlag, holdFlag, upFlag, axisFlag = true;

        private InputData() {}

        #region CONSTRUCTORS

        /// <summary>
        /// Standard input button.
        /// </summary>
        public InputData(string inputVal)
        {
            input = inputVal;
            isButton = true;
        }

        /// <summary>
        /// Input axis. (Optional button)
        /// </summary>
        public InputData(string inputVal, float axisThresholdVal, bool checkButton)
        {
            input = inputVal;
            isAxis = true;
            axisThreshold = axisThresholdVal;
            isButton = checkButton;
        }

        /// <summary>
        /// Single input key code.
        /// </summary>
        public InputData(KeyCode inputKeyCode)
        {
            inputKeys = new KeyCode[] { inputKeyCode };
            isKey = true;
        }

        /// <summary>
        /// Multiple key codes.
        /// </summary>
        public InputData(KeyCode[] inputKeyCodes)
        {
            inputKeys = inputKeyCodes;
            isKey = true;
        }

        #endregion

        public void Update()
        {
            if (isKey && inputKeys.Any())
            {
                // Check for key input
                Down = inputKeys.Any(key => Input.GetKeyDown(key));
                Hold = inputKeys.Any(key => Input.GetKey(key));
                Up = inputKeys.Any(key => Input.GetKeyUp(key));
            }
            else if (isAxis)
            {
                // Check for axis input (Maybe button as well)
                Axis = Input.GetAxis(input);

                if ((Axis >= axisThreshold && axisFlag) || (isButton && Input.GetButtonDown(input)))
                {
                    Down = true;
                    axisFlag = false;
                }
                else
                {
                    Down = false;
                }

                Hold = (Axis >= axisThreshold) || (isButton && Input.GetButton(input));

                if ((Axis < axisThreshold && !axisFlag) || (isButton && Input.GetButton(input)))
                {
                    Up = true;
                    axisFlag = true;
                }
                else
                {
                    Up = false;
                }
            }
            else if (isButton)
            {
                // Check for button input
                Down = Input.GetButtonDown(input);
                Hold = Input.GetButton(input);
                Up = Input.GetButtonUp(input);
            }

            // Raise appropriate flags
            if (Down) downFlag = true;
            if (Hold) holdFlag = true;
            if (Up) upFlag = true;

            // Add to hold time
            if (Hold) HoldTime += Time.unscaledDeltaTime;
            else HoldTime = 0;
        }

        // Retrieve flag and lower it.

        public bool GetDown()
        {
            bool flagVal = downFlag;
            downFlag = false;
            return flagVal;
        }

        public bool GetHold()
        {
            bool flagVal = holdFlag;
            holdFlag = false;
            return flagVal;
        }

        public bool GetUp()
        {
            bool flagVal = upFlag;
            upFlag = false;
            return flagVal;
        }

        // Retrieve flag but do NOT lower it.

        public bool PeekDown()
        {
            return downFlag;
        }

        public bool PeekHold()
        {
            return holdFlag;
        }

        public bool PeekUp()
        {
            return upFlag;
        }
    }
}