using System.Globalization;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Provides the data structure for the battery information of the controller itself and its attached headset</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputBatteryInformation {

        /// <summary>
        /// Battery type can be 0x00, 0x01, 0x02, 0x03 or 0xFF.
        /// See XInputConstants for further info
        /// <seealso cref="BatteryTypes"/>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        internal byte BatteryType;

        /// <summary>
        /// Battery Level can be 0x00, 0x01, 0x02 or 0x03.
        /// See XInputConstants for further info
        /// <seealso cref="BatteryLevels"/>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        internal byte BatteryLevel;

        /// <summary>
        /// Prints the Battery type and level
        /// </summary>
        /// <returns>A string containing BatteryType plus BatteryLevel</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0} {1}", (BatteryTypes) BatteryType, (BatteryLevels) BatteryLevel);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            var source = (XInputBatteryInformation) obj;
            return ((BatteryType == source.BatteryType) &&
                    (BatteryLevel == source.BatteryLevel));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            //throw new NotImplementedException();
            return base.GetHashCode();
        }

        public static bool operator ==(XInputBatteryInformation a, XInputBatteryInformation b)
        {
            // If both are null, or both are same instance, return true.
            return Equals(a, b) || a.Equals(b);
        }

        public static bool operator !=(XInputBatteryInformation a, XInputBatteryInformation b)
        {
            return !(a == b);
        }
    }
}
