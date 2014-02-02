using System;

namespace MetroController.XInputWrapper {
#pragma warning disable 1591

    /// <summary>Flags for the different button presses/// </summary>
    [Flags]
    public enum ButtonFlags {
        XINPUT_GAMEPAD_DPAD_UP = 0x0001,
        XINPUT_GAMEPAD_DPAD_DOWN = 0x0002,
        XINPUT_GAMEPAD_DPAD_LEFT = 0x0004,
        XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008,
        XINPUT_GAMEPAD_START = 0x0010,
        XINPUT_GAMEPAD_BACK = 0x0020,
        XINPUT_GAMEPAD_LEFT_THUMB = 0x0040,
        XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080,
        XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100,
        XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200,
        XINPUT_GAMEPAD_GUIDE = 0x0400,
        XINPUT_GAMEPAD_A = 0x1000,
        XINPUT_GAMEPAD_B = 0x2000,
        XINPUT_GAMEPAD_X = 0x4000,
        XINPUT_GAMEPAD_Y = 0x8000,
    };

    /// <summary>Flags for the different subtypes of controllers available</summary>
    public enum ControllerSubtypes {
        XINPUT_DEVSUBTYPE_UNKNOWN = 0x00,
        XINPUT_DEVSUBTYPE_WHEEL = 0x02,
        XINPUT_DEVSUBTYPE_ARCADE_STICK = 0x03,
        XINPUT_DEVSUBTYPE_FLIGHT_STICK = 0x04,
        XINPUT_DEVSUBTYPE_DANCE_PAD = 0x05,
        XINPUT_DEVSUBTYPE_GUITAR = 0x06,
        XINPUT_DEVSUBTYPE_GUITAR_ALTERNATE = 0x07,
        XINPUT_DEVSUBTYPE_DRUM_KIT = 0x08,
        XINPUT_DEVSUBTYPE_GUITAR_BASS = 0x0B,
        XINPUT_DEVSUBTYPE_ARCADE_PAD = 0x13
    };

#pragma warning restore 1591

    /// <summary>Flags for battery status level</summary>
    public enum BatteryTypes {

        /// <summary>This device is not connected</summary>
        BATTERY_TYPE_DISCONNECTED = 0x00,

        /// <summary>Wired device, no battery</summary>
        BATTERY_TYPE_WIRED = 0x01,

        /// <summary>Alkaline battery source</summary>
        BATTERY_TYPE_ALKALINE = 0x02,

        /// <summary>Nickel Metal Hydride battery source</summary>
        BATTERY_TYPE_NIMH = 0x03,

        /// <summary>Cannot determine the battery type</summary>
        BATTERY_TYPE_UNKNOWN = 0xFF,
    };

    /// <summary>
    /// These are only valid for wireless, connected devices, with known battery types
    /// The amount of use time remaining depends on the type of device.
    /// </summary>
    public enum BatteryLevels {

        /// <summary>Batteries are empty</summary>
        BATTERY_LEVEL_EMPTY = 0x00,

        /// <summary>Batteries are low (blinking lights)</summary>
        BATTERY_LEVEL_LOW = 0x01,

        /// <summary>Batteries are half full (2 quarter circles)</summary>
        BATTERY_LEVEL_MEDIUM = 0x02,

        /// <summary>Batteries are completely charged (4 quarter circles)</summary>
        BATTERY_LEVEL_FULL = 0x03
    };

    /// <summary>Types of devices with batteries</summary>
    public enum BatteryDeviceTypes {

        /// <summary>Batteries of the gamepad itself</summary>
        BATTERY_DEVTYPE_GAMEPAD = 0x00,

        /// <summary>batteries of the headset itself</summary>
        BATTERY_DEVTYPE_HEADSET = 0x01,
    };

    /// <summary>Flags for XINPUT_CAPABILITIES</summary>
    [Flags]
    public enum CapabilityFlags {

        /// <summary>Support for attaching an headset</summary>
        XINPUT_CAPS_VOICE_SUPPORTED = 0x0004,

        //For Windows 8 only
        /// <summary>Unknown</summary>
        XINPUT_CAPS_FFB_SUPPORTED = 0x0001,

        /// <summary>Wireless connection possible</summary>
        XINPUT_CAPS_WIRELESS = 0x0002,

        /// <summary>Unknown</summary>
        XINPUT_CAPS_PMD_SUPPORTED = 0x0008,

        /// <summary>Possesses no navigational controls/// </summary>
        XINPUT_CAPS_NO_NAVIGATION = 0x0010,
    };

    /// <summary>Misc. constants for the gamepad input</summary>
    public static class XInputConstants {

        /// <summary>Device type for available in XINPUT_CAPABILITIES</summary>
        public const int XINPUT_DEVTYPE_GAMEPAD = 0x01;

        /// <summary>Device subtypes available in XINPUT_CAPABILITIES</summary>
        public const int XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;

        /// <summary>Left thumb stick deadzone</summary>
        public const int XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE = 7849;

        /// <summary>Right thumb stick deadzone</summary>
        public const int XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE = 8689;

        /// <summary>left and right trigger pressure treshold</summary>
        public const int XINPUT_GAMEPAD_TRIGGER_THRESHOLD = 30;

        /// <summary>Flag to pass to XInputGetCapabilities</summary>
        public const int XINPUT_FLAG_GAMEPAD = 0x00000001;
    }
}
