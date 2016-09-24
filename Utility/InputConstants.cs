using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Expanse
{
    public static class InputConstants
    {
        public const int JOYSTICK_BUTTON_COUNT = 20;

        // controllerIndex 0 = Any
        public static KeyCode ForJoystick(KeyCode key, int controllerIndex)
        {
            return key + (controllerIndex * JOYSTICK_BUTTON_COUNT);
        }

        public static class PS3
        {
            public const KeyCode CROSS = KeyCode.JoystickButton0;
            public const KeyCode CIRCLE = KeyCode.JoystickButton1;
            public const KeyCode SQUARE = KeyCode.JoystickButton2;
            public const KeyCode TRIANGLE = KeyCode.JoystickButton3;
            public const KeyCode L1 = KeyCode.JoystickButton4;
            public const KeyCode R1 = KeyCode.JoystickButton5;
            public const KeyCode SELECT = KeyCode.JoystickButton6;
            public const KeyCode START = KeyCode.JoystickButton7;
            public const KeyCode L3 = KeyCode.JoystickButton8;
            public const KeyCode R3 = KeyCode.JoystickButton9;
        }

        public static class XBOX
        {
            public const KeyCode A = KeyCode.JoystickButton0;
            public const KeyCode B = KeyCode.JoystickButton1;
            public const KeyCode X = KeyCode.JoystickButton2;
            public const KeyCode Y = KeyCode.JoystickButton3;
            public const KeyCode LEFT_BUMPER = KeyCode.JoystickButton4;
            public const KeyCode RIGHT_BUMPER = KeyCode.JoystickButton5;
            public const KeyCode BACK = KeyCode.JoystickButton6;
            public const KeyCode START = KeyCode.JoystickButton7;
            public const KeyCode LEFT_STICK = KeyCode.JoystickButton8;
            public const KeyCode RIGHT_STICK = KeyCode.JoystickButton9;
        }
    }
}