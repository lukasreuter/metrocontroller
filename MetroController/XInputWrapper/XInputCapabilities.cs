using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper {
    /// <summary>Structure holding the capabilities (properties) of the gamepad</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputCapabilities {
        /// <summary>Type of the gamepad</summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public byte Type;
        /// <summary>Subtype of the gamepad <seealso cref="ControllerSubtypes"/></summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte SubType;
        /// <summary>Misc. capability flags <see cref="XInputConstants.CapabilityFlags"/></summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(2)]
        public short Flags;
        /// <summary>Capabilities of the gamepad itself (e.g. how many buttons)</summary>
        [FieldOffset(4)]
        public XInputGamepad Gamepad;
        /// <summary>Capabilities of the vibration motors</summary>
        [FieldOffset(16)]
        public XInputVibration Vibration;
    }
}
