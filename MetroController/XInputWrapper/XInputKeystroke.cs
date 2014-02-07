using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Structure that holds the gamepad input data as Virtualkey codes</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputKeystroke {

        /// <summary>The virtualkey associated with the pressed button</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        internal short VirtualKey;

        /// <summary>The virtualkey code in unicode representation</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(2)]
        internal char Unicode;

        /// <summary>Misc input flags</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        internal short Flags;

        /// <summary>The index of the gamepad</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(5)]
        internal byte UserIndex;

        /// <summary>Human Identifier Device code of the gamepad</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(6)]
        internal byte HidCode;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            var source = (XInputKeystroke) obj;
            return ((VirtualKey == source.VirtualKey) &&
                    (Unicode == source.Unicode) &&
                    (Flags == source.Flags) &&
                    (UserIndex == source.UserIndex) &&
                    (HidCode == source.HidCode));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return (base.GetHashCode() + UserIndex + HidCode);
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public static bool operator ==(XInputKeystroke a, XInputKeystroke b)
        {
            // If both are null, or both are same instance, return true.
            return Equals(a, b) || a.Equals(b);
        }

        public static bool operator !=(XInputKeystroke a, XInputKeystroke b)
        {
            return !(a == b);
        }
    }
}
