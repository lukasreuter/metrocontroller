using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper {
    /// <summary>Structure that holds the gamepad input data as Virtualkey codes</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputKeystroke {
        /// <summary>The virtualkey associated with the pressed button</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public short VirtualKey;
        /// <summary>The virtualkey code in unicode representation</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(2)]
        public char Unicode;
        /// <summary>Misc input flags</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public short Flags;
        /// <summary>The index of the gamepad</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(5)]
        public byte UserIndex;
        /// <summary>Human Identifier Device code of the gamepad</summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(6)]
        public byte HidCode;
    }
}
