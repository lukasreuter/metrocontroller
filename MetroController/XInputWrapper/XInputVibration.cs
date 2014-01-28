using System;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Provides a structure for the speed of the left and right vibration motor speeds</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputVibration {

        /// <summary>Represents the speed of the left (low frequency) motor</summary>
        [MarshalAs(UnmanagedType.I2)]
        public ushort LeftMotorSpeed;

        /// <summary>Represents the speed of the right (high frequency) motor</summary>
        [MarshalAs(UnmanagedType.I2)]
        public ushort RightMotorSpeed;
    }
}
