using System;
using System.Runtime.InteropServices;

namespace MetroController.XInputWrapper {

    /// <summary>Contains the imported methods from the xinput dynamic library</summary>
    public static class XInputNativeMethods {
#if WINDOWS7
        /// <summary>
        /// Gets the state of the connected controllers
        /// </summary>
        /// <param name="dwUserIndex">[in] Index of the gamer associated with the device</param>
        /// <param name="pState">[out] Receives the current state</param>
        /// <returns>Returns error code "0" on success</returns>
        [DllImport("xinput9_1_0.dll")]
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
        [DllImport("xinput9_1_0.dll")]
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
        [DllImport("xinput9_1_0.dll")]
        internal static extern int XInputGetCapabilities
        (
            int dwUserIndex,
            int dwFlags,
            ref XInputCapabilities pCapabilities
        );

        /// <summary>
        /// this function is not available prior to Windows 8
        /// </summary>
        /// <param name="dwUserIndex">Index of the gamer associated with the device</param>
        /// <param name="devType">Which device on this user index</param>
        /// <param name="pBatteryInformation">Contains the level and types of batteries</param>
        /// <returns>Returns 0</returns>
        internal static int XInputGetBatteryInformation
        (
            int dwUserIndex,
            byte devType,
            ref XInputBatteryInformation pBatteryInformation
        ) {
           return 0;
        }

        /// <summary>
        /// this function is not available prior to Windows 8
        /// </summary>
        /// <param name="dwUserIndex">Index of the gamer associated with the device</param>
        /// <param name="dwReserved">Reserved for future use</param>
        /// <param name="pKeystroke">Pointer to an XINPUT_KEYSTROKE structure that receives an input event.</param>
        /// <returns>Returns 0</returns>
        internal static int XInputGetKeystroke
        (
            int dwUserIndex,
            int dwReserved,
            ref XInputKeystroke pKeystroke
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
            //,unKnown *pUnKnown    // currently not known what type this is supposed to be, maybe an EventCallback
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
