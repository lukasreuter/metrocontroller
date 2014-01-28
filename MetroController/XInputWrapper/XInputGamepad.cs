using System;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Structure holding all controller input specific informations</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputGamepad {

        /// <summary>Contains which buttons are currently pressed <seealso cref="IsButtonPressed"/></summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public short wButtons;

        /// <summary>Amount of pressure applied onto the left trigger [0, 255]</summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(2)]
        public byte bLeftTrigger;

        /// <summary>Amount of pressure applied onto the right trigger [0, 255]</summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(3)]
        public byte bRightTrigger;

        /// <summary>Current position of the left thumb stick on the x axis</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public short sThumbLX;

        /// <summary>Current position of the left thumb stick on the y axis</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(6)]
        public short sThumbLY;

        /// <summary>Current position of the right thumb stick on the x axis</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(8)]
        public short sThumbRX;

        /// <summary>Current position of the right thumb stick on the y axis</summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(10)]
        public short sThumbRY;

        /// <summary>
        /// Checks if a certain button is pressed
        /// </summary>
        /// <param name="buttonFlags">The specific flag to test for <see cref="ButtonFlags"/></param>
        /// <returns>True if the specified button is pressed</returns>
        public bool IsButtonPressed(int buttonFlags)
        {
            return (wButtons & buttonFlags) == buttonFlags;
        }

        /// <summary>
        /// Checks if a certain button exists on the gamepad
        /// </summary>
        /// <param name="buttonFlags">The specific flag to test for <see cref="ButtonFlags"/></param>
        /// <returns>True if the specified button exists</returns>
        public bool IsButtonPresent(int buttonFlags)
        {
            //TODO: check the capability of the controller and return the result based on that
            return IsButtonPressed(buttonFlags);
        }

        /// <summary>
        /// Copies an <see cref="XInputGamepad"/> object over the current instance
        /// </summary>
        /// <param name="source">The object to copy</param>
        public void Copy(XInputGamepad source)
        {
            sThumbLX = source.sThumbLX;
            sThumbLY = source.sThumbLY;
            sThumbRX = source.sThumbRX;
            sThumbRY = source.sThumbRY;
            bLeftTrigger = source.bLeftTrigger;
            bRightTrigger = source.bRightTrigger;
            wButtons = source.wButtons;
        }

        /// <summary>
        /// Checks if this instance is equal to an object
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if the instance is equal to the object</returns>
        public override bool Equals(object obj)
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

        /// <summary>
        /// Checks if two <see cref="XInputGamepad"/> references are equal
        /// </summary>
        /// <param name="a">First reference</param>
        /// <param name="b">Second reference</param>
        /// <returns>True if both references are equal</returns>
        public static bool operator ==(XInputGamepad a, XInputGamepad b)
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

        /// <summary>
        /// Checks if two <see cref="XInputGamepad"/> references are not equal
        /// </summary>
        /// <param name="a">First reference</param>
        /// <param name="b">Second reference</param>
        /// <returns>True if the references are not equal</returns>
        public static bool operator !=(XInputGamepad a, XInputGamepad b)
        {
            return !(a == b);
        }

        //TODO: implement this
        /// <summary>
        /// Returns an unique hash code based upon the current button presses
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            return (base.GetHashCode() + this.wButtons);
        }

        //TODO: implement this
        /// <summary>
        /// Returns the current instance variables as a printable string
        /// </summary>
        /// <returns>A string describing the instance</returns>
        public override string ToString()
        {
            string str;
            str = wButtons.ToString();
            str += bLeftTrigger.ToString();

            return str;
        }
    }
}
