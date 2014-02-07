using MetroController.XInputWrapper;
using System;
using System.Windows.Data;

namespace MetroController.Converters {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class BatteryTypeValueToBatteryTypeString : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = (byte) value;
            switch ((BatteryTypes) v) {
                case BatteryTypes.BATTERY_TYPE_ALKALINE:
                    return "alkaline";

                case BatteryTypes.BATTERY_TYPE_DISCONNECTED:
                    return "disconnected";

                case BatteryTypes.BATTERY_TYPE_NIMH:
                    return "NiMH";

                case BatteryTypes.BATTERY_TYPE_UNKNOWN:
                    return "unknown";

                case BatteryTypes.BATTERY_TYPE_WIRED:
                    return "wired";

                default:
                    return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = (string) value;
            switch (v) {
                case "alkaline":
                    return BatteryTypes.BATTERY_TYPE_ALKALINE;

                case "disconnected":
                    return BatteryTypes.BATTERY_TYPE_DISCONNECTED;

                case "NiMH":
                    return BatteryTypes.BATTERY_TYPE_NIMH;

                case "unknown":
                    return BatteryTypes.BATTERY_TYPE_UNKNOWN;

                case "wired":
                    return BatteryTypes.BATTERY_TYPE_WIRED;

                default:
                    return 0;
            }
        }
    }
}
