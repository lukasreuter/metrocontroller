using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper {
    /// <summary>Contains the imported methods from the xinput dynamic library</summary>
    public static class XInput {
#if WINDOWS7
        [DllImport("xinput9_1_0.dll")]
        public static extern int XInputGetState
        (
            int dwUserIndex,            // [in] Index of the gamer associated with the device
            ref XInputState pState      // [out] Receives the current state
        );

        [DllImport("xinput9_1_0.dll")]
        public static extern int XInputSetState
        (
            int dwUserIndex,                  // [in] Index of the gamer associated with the device
            ref XInputVibration pVibration    // [in, out] The vibration information to send to the controller
        );

        [DllImport("xinput9_1_0.dll")]
        public static extern int XInputGetCapabilities
        (
            int dwUserIndex,   // [in] Index of the gamer associated with the device
            int dwFlags,       // [in] Input flags that identify the device type
            ref XInputCapabilities pCapabilities  // [out] Receives the capabilities
        );


        //this function is not available prior to Windows 8
        public static int XInputGetBatteryInformation
        (
            int dwUserIndex,        // Index of the gamer associated with the device
            byte devType,            // Which device on this user index
            ref XInputBatteryInformation pBatteryInformation // Contains the level and types of batteries
        ) {
           return 0;
        }

        //this function is not available prior to Windows 8
        public static int XInputGetKeystroke
        (
            int dwUserIndex,              // Index of the gamer associated with the device
            int dwReserved,               // Reserved for future use
            ref      XInputKeystroke pKeystroke    // Pointer to an XINPUT_KEYSTROKE structure that receives an input event.
        ) {
            return 0;
        }
#else
        /// <summary>
        /// Gets the state of the connected controllers
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="pState">[out] Receives the current state</param>
        /// <returns>Returns error code "0" on success</returns>
        [DllImport("xinput1_4.dll")]
        internal static extern int XInputGetState
        (
            int dwUserIndex,
            ref XInputState pState
        );

        /// <summary>
        /// Sets the state of a specified controller
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="pVibration">[in, out] The vibration information to send to the controller</param>
        /// <returns>Returns error code "0" on success</returns>
        [DllImport("xinput1_4.dll")]
        internal static extern int XInputSetState
        (
            int dwUserIndex,
            ref XInputVibration pVibration
        );

        /// <summary>
        /// Gets the capabilities of a certain controller
        /// <seealso cref="XInputCapabilities"/>
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="dwFlags">[in] Input flags that identify the device type</param>
        /// <param name="pCapabilities">[out] Receives the capabilities</param>
        /// <returns></returns>
        [DllImport("xinput1_4.dll")]
        internal static extern int XInputGetCapabilities
        (
            int dwUserIndex,
            int dwFlags,
            ref XInputCapabilities pCapabilities
        );

        /// <summary>
        /// Gets the battery info of a headset or gamepad of a specified controller
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="devType">[in] Which device on this user index</param>
        /// <param name="pBatteryInformation">[out] Contains the level and types of batteries</param>
        /// <returns>An error code</returns>
        [DllImport("xinput1_4.dll")]
        internal static extern int XInputGetBatteryInformation
        (
            int dwUserIndex,
            byte devType,
            ref XInputBatteryInformation pBatteryInformation
        );

        /// <summary>
        /// Returns the current state of a controller as Virtual Key Codes
        /// <seealso cref="XInputKeystroke"/>
        /// </summary>
        /// <param name="dwUserIndex">Index of the gamer associated with the device</param>
        /// <param name="dwReserved">Reserved for future use</param>
        /// <param name="pKeystroke">Pointer to an XINPUT_KEYSTROKE structure that receives an input event</param>
        /// <returns>An error code</returns>
        [DllImport("xinput1_4.dll")]
        internal static extern int XInputGetKeystroke
        (
            int dwUserIndex,
            int dwReserved,
            ref XInputKeystroke pKeystroke
        );


        //Hint: The following methods are not part of the officical API and are not documented but still accessible

        /// <summary>
        /// Functionally equivalent to XInputGetState, but exposes the Guide button in wButtons with the bitmask 0x400
        /// <seealso cref="XInputGetState"/>
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="pState">[out] Receives the current state</param>
        /// <returns>Returns error code "0" on success</returns>
        [DllImport("xinput1_4.dll", EntryPoint = "#100")]
        internal static extern int XInputGetStateEx
        (
            int dwUserIndex,
            ref XInputState pState
        );

        /// <summary>
        /// method purpose unknown, listed for completeness
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="dwFlag">[in] Input flags that identify the device type</param>
        /// <returns>An error code</returns>
        [DllImport("xinput1_4.dll", EntryPoint = "#101")]
        internal static extern int XInputWaitForGuideButton
        (
            int dwUserIndex,
            int dwFlag
            //,unKnown *pUnKnown    // currently not known what type this is supposed
                                    // to be, maybe an EventCallback
        );

        /// <summary>
        /// method purpose unknown, listed for completeness
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <returns>An error code</returns>
        [DllImport("xinput1_4.dll", EntryPoint = "#102")]
        internal static extern int XInputCancelGuideButtonWait
        (
            int dwUserIndex
        );

        /// <summary>
        /// Turns off the specified controller/gamepad
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <returns>Returns error code "0" on success</returns>
        [DllImport("xinput1_4.dll", EntryPoint = "#103")]
        internal static extern int XInputControllerOff
        (
            int dwUserIndex
        );
#endif
    }

}
