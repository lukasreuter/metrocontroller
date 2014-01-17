using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace J2i.Net.XInputWrapper
{
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputState
    {
        [FieldOffset(0)]
        public int PacketNumber;

        [FieldOffset(4)]
        public XInputGamepad Gamepad;

        public
        void Copy(XInputState source)
        {
            PacketNumber = source.PacketNumber;
            Gamepad.Copy(source.Gamepad);
        }

        public override
        bool Equals(object obj)
        {
            if ((obj == null) || (!(obj is XInputState)))
                return false;

            XInputState source = (XInputState) obj;
            return ((PacketNumber == source.PacketNumber) &&
                    (Gamepad.Equals(source.Gamepad)));
        }

        public static
        bool operator ==(XInputState a, XInputState b)
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
            return ((a.PacketNumber == b.PacketNumber) &&
                    (a.Gamepad == b.Gamepad));
        }

        public static
        bool operator !=(XInputState a, XInputState b) 
        {
            return !(a == b);
        }

        public override
        int GetHashCode()
        {
            return (base.GetHashCode() + this.Gamepad.GetHashCode());
        }
    }
}
