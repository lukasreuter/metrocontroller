using System;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Provides a structure for the speed of the left and right vibration motor speeds</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XInputVibration {

        /// <summary>Represents the speed of the left (low frequency) motor</summary>
        [MarshalAs(UnmanagedType.I2)]
        internal ushort LeftMotorSpeed;

        /// <summary>Represents the speed of the right (high frequency) motor</summary>
        [MarshalAs(UnmanagedType.I2)]
        internal ushort RightMotorSpeed;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            var source = (XInputVibration) obj;
            return ((LeftMotorSpeed == source.LeftMotorSpeed) &&
                    (RightMotorSpeed == source.RightMotorSpeed));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            //throw new NotImplementedException();
            return base.GetHashCode();
        }

        public static bool operator ==(XInputVibration a, XInputVibration b)
        {
            // If both are null, or both are same instance, return true.
            return Equals(a, b) || a.Equals(b);
        }

        public static bool operator !=(XInputVibration a, XInputVibration b)
        {
            return !(a == b);
        }
    }
}
