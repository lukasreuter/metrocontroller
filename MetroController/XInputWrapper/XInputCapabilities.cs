using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Structure holding the capabilities (properties) of the gamepad</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputCapabilities {

        /// <summary>Type of the gamepad</summary>
        [MarshalAs(UnmanagedType.I1)]
        internal byte Type;

        /// <summary>Subtype of the gamepad <seealso cref="ControllerSubtypes"/></summary>
        [MarshalAs(UnmanagedType.I1)]
        internal byte SubType;

        /// <summary>Misc. capability flags <see cref="CapabilityFlags"/></summary>
        [MarshalAs(UnmanagedType.I2)]
        internal short Flags;

        /// <summary>Capabilities of the gamepad itself (e.g. how many buttons)</summary>
        internal XInputGamepad Gamepad;

        /// <summary>Capabilities of the vibration motors</summary>
        internal XInputVibration Vibration;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            var source = (XInputCapabilities) obj;
            return ((Type == source.Type) &&
                    (SubType == source.SubType) &&
                    (Flags == source.Flags) &&
                    (Gamepad == source.Gamepad) &&
                    (Vibration == source.Vibration));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return (base.GetHashCode() + Type + SubType + Flags);
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public static bool operator ==(XInputCapabilities a, XInputCapabilities b)
        {
            // If both are null, or both are same instance, return true.
            return Equals(a, b) || a.Equals(b);
        }

        public static bool operator !=(XInputCapabilities a, XInputCapabilities b)
        {
            return !(a == b);
        }
    }
}
