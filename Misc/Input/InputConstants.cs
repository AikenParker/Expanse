using UnityEngine;

namespace Expanse.Misc
{
    /// <summary>
    /// Collection of keycode constants for various platform specific controllers.
    /// </summary>
    public static class InputConstants
    {
        /// <summary>
        /// Maximum amount of joysticks buttons.
        /// </summary>
        public const int JOYSTICK_BUTTON_COUNT = 20;

        /// <summary>
        /// Gets a specfic controller keycode.
        /// </summary>
        /// <param name="key">Base keycode to get. E.g. InputConstants.PlayStation.CROSS</param>
        /// <param name="controllerIndex">Controller index for specific keycode. (0 = Any)</param>
        /// <returns>Returns the keycode for a specific controller.</returns>
        public static KeyCode ForJoystick(KeyCode key, int controllerIndex)
        {
            return key + (controllerIndex * JOYSTICK_BUTTON_COUNT);
        }

        /// <summary>
        /// Keycode constants for a standard DualShock PlayStation controller gamepad.
        /// </summary>
        public static class PlayStation
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

        /// <summary>
        /// Keycode constants for a standard Xbox controller gamepad.
        /// </summary>
        public static class Xbox
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

        /// <summary>
        /// Keycode constants for standard Android buttons.
        /// </summary>
        public static class Android
        {
            public const KeyCode HOME = KeyCode.Home;
            public const KeyCode MENU = KeyCode.Menu;
            public const KeyCode BACK = KeyCode.Escape;
        }
    }
}