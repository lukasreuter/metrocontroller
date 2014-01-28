using System;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Structure holding the capabilities (properties) of the gamepad</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputCapabilities {

        /// <summary>Type of the gamepad</summary>
        [MarshalAs(UnmanagedType.I1)]
        public byte Type;

        /// <summary>Subtype of the gamepad <seealso cref="ControllerSubtypes"/></summary>
        [MarshalAs(UnmanagedType.I1)]
        public byte SubType;

        /// <summary>Misc. capability flags <see cref="XInputConstants.CapabilityFlags"/></summary>
        [MarshalAs(UnmanagedType.I2)]
        public short Flags;

        /// <summary>Capabilities of the gamepad itself (e.g. how many buttons)</summary>
        public XInputGamepad Gamepad;

        /// <summary>Capabilities of the vibration motors</summary>
        public XInputVibration Vibration;
    }
}
