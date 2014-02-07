using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Contains the data for the Packtenumber and the <see cref="XInputGamepad"/></summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputState {

        /// <summary>The consecutive number of sent packets</summary>
        internal int PacketNumber;

        /// <summary>The <see cref="XInputGamepad"/> structure holding the input information</summary>
        internal XInputGamepad Gamepad;

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
            if (!(obj is XInputState)) return false;

            var source = (XInputState) obj;
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
            return Equals(a, b) || a.Equals(b);
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
            // ReSharper disable once NonReadonlyFieldInGetHashCode
            return (base.GetHashCode() + Gamepad.GetHashCode());
        }
    }
}
