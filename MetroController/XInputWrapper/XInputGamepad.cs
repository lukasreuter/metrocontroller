using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper
{
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputGamepad
    {
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public short wButtons;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(2)]
        public byte bLeftTrigger;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(3)]
        public byte bRightTrigger;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public short sThumbLX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(6)]
        public short sThumbLY;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(8)]
        public short sThumbRX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(10)]
        public short sThumbRY;

        public
        bool IsButtonPressed(int buttonFlags)
        {
            return (wButtons & buttonFlags) == buttonFlags;
        }

        public
        bool IsButtonPresent(int buttonFlags)
        {
            return IsButtonPressed(buttonFlags);
        }

        public
        void Copy(XInputGamepad source)
        {
            sThumbLX = source.sThumbLX;
            sThumbLY = source.sThumbLY;
            sThumbRX = source.sThumbRX;
            sThumbRY = source.sThumbRY;
            bLeftTrigger = source.bLeftTrigger;
            bRightTrigger = source.bRightTrigger;
            wButtons = source.wButtons;
        }

        public override
        bool Equals(object obj)
        {
            if (!(obj is XInputGamepad)) return false;
            XInputGamepad source = (XInputGamepad) obj;
            return ((sThumbLX == source.sThumbLX) &&
                    (sThumbLY == source.sThumbLY) &&
                    (sThumbRX == source.sThumbRX) &&
                    (sThumbRY == source.sThumbRY) &&
                    (bLeftTrigger == source.bLeftTrigger) &&
                    (bRightTrigger == source.bRightTrigger) &&
                    (wButtons == source.wButtons));
        }

        public static
        bool operator ==(XInputGamepad a, XInputGamepad b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null)) {
                return false;
            }

            // Return true if the fields match:
            return ((a.wButtons == b.wButtons) &&
                    (a.bLeftTrigger == b.bLeftTrigger) &&
                    (a.bRightTrigger == b.bRightTrigger) &&
                    (a.sThumbLX == b.sThumbLX) &&
                    (a.sThumbLY == b.sThumbLY) &&
                    (a.sThumbRX == b.sThumbRX) &&
                    (a.sThumbRY == b.sThumbRY));
        }

        public static
        bool operator !=(XInputGamepad a, XInputGamepad b)
        {
            return !(a == b);
        }

        //TODO: implement this
        public override
        int GetHashCode()
        {
            return (base.GetHashCode() + this.wButtons);
        }

        //TODO: implement this
        public override
        string ToString()
        {
            string str;
            str  = wButtons.ToString();
            str += bLeftTrigger.ToString();

            return str;
        }
    }
}
