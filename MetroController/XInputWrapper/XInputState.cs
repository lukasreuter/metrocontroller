using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper {
    /// <summary>Contains the data for the Packtenumber and the <see cref="XInputGamepad"/></summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputState {
        /// <summary>The consecutive number of sent packets</summary>
        public int PacketNumber;
        /// <summary>The <see cref="XInputGamepad"/> structure holding the input information</summary>
        public XInputGamepad Gamepad;

        /// <summary>
        /// Copies an XInputState from a another XInputState struct
        /// </summary>
        /// <param name="source">The object which should be copied <seealso cref="XInputState"/></param>
        public void Copy(XInputState source)
        {
            PacketNumber = source.PacketNumber;
            Gamepad.Copy(source.Gamepad);
        }

        /// <summary>
        /// Checks if the struct instance is equal to an object
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if both instances are equal</returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || (!(obj is XInputState)))
                return false;

            XInputState source = (XInputState) obj;
            return ((PacketNumber == source.PacketNumber) &&
                    (Gamepad.Equals(source.Gamepad)));
        }

        /// <summary>
        /// Compares two XInputstates for equality
        /// </summary>
        /// <param name="a">A <see cref="XInputState"/></param>
        /// <param name="b">A <see cref="XInputState"/></param>
        /// <returns>True if both objects are equal</returns>
        public static bool operator ==(XInputState a, XInputState b)
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

        /// <summary>
        /// Compares two XInputStates for inequality
        /// </summary>
        /// <param name="a">A <see cref="XInputState"/></param>
        /// <param name="b">A <see cref="XInputState"/></param>
        /// <returns>True if the objects are not equal</returns>
        public static bool operator !=(XInputState a, XInputState b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Generates a gamepad unique hashcode
        /// </summary>
        /// <returns>A hashcode</returns>
        public override int GetHashCode()
        {
            return (base.GetHashCode() + this.Gamepad.GetHashCode());
        }
    }
}
