using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper
{
    /// <summary>Provides the data structure for the battery information of the controller itself and its attached headset</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct  XInputBatteryInformation
    {
        /// <summary>
        /// Battery type can be 0x00, 0x01, 0x02, 0x03 or 0xFF.
        /// See XInputConstants for further info
        /// <seealso cref="BatteryTypes"/>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public byte BatteryType;

        /// <summary>
        /// Battery Level can be 0x00, 0x01, 0x02 or 0x03.
        /// See XInputConstants for further info
        /// <seealso cref="BatteryLevels"/>
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte BatteryLevel;

        /// <summary>
        /// Prints the Battery type and level
        /// </summary>
        /// <returns>A string containing BatteryType plus BatteryLevel</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", (BatteryTypes) BatteryType, (BatteryLevels) BatteryLevel);
        }
    }
}
