using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper
{
    public static class XInput
    {
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
        [DllImport("xinput1_4.dll")]
        public static extern int XInputGetState
        (
            int dwUserIndex,            // [in]  Index of the gamer associated with the device
            ref XInputState pState      // [out] Receives the current state
        );

        [DllImport("xinput1_4.dll")]
        public static extern int XInputSetState
        (
            int dwUserIndex,                  // [in] Index of the gamer associated with the device
            ref XInputVibration pVibration    // [in, out] The vibration information to send to the controller
        );

        [DllImport("xinput1_4.dll")]
        public static extern int XInputGetCapabilities
        (
            int dwUserIndex,   // [in] Index of the gamer associated with the device
            int dwFlags,       // [in] Input flags that identify the device type
            ref XInputCapabilities pCapabilities  // [out] Receives the capabilities
        );

        [DllImport("xinput1_4.dll")]
        public static extern int XInputGetBatteryInformation
        (
            int dwUserIndex,        // Index of the gamer associated with the device
            byte devType,           // Which device on this user index
            ref XInputBatteryInformation pBatteryInformation // Contains the level and types of batteries
        );

        [DllImport("xinput1_4.dll")]
        public static extern int XInputGetKeystroke
        (
            int dwUserIndex,                  // Index of the gamer associated with the device
            int dwReserved,                   // Reserved for future use
            ref XInputKeystroke pKeystroke    // Pointer to an XINPUT_KEYSTROKE structure that receives an input event.
        );

        // The following methods are not officically documented in the API but still accessible

        // functionally equivalent to XInputGetState, but exposes the Guide button in wButtons with the bitmask 0x400.
        [DllImport("xinput1_4.dll", EntryPoint = "#100")]
        public static extern int XInputGetStateEx
        (
            int dwUserIndex,            // [in]  Index of the gamer associated with the device
            ref XInputState pState      // [out] Receives the current state
        );

        // function purpose unknown, listed for completness
        [DllImport("xinput1_4.dll", EntryPoint = "#101")]
        public static extern int XInputWaitForGuideButton
        (
            int dwUserIndex,        // [in] Index of the gamer associated with the device
            int dwFlag//,           // [in] Input flags that identify the device type
            //unKnown *pUnKnown //currently not known what type this is supposed to be, maybe an EventCallback
        );

        // function purpose unknown, listed for completness
        [DllImport("xinput1_4.dll", EntryPoint = "#102")]
        public static extern int XInputCancelGuideButtonWait
        (
            int dwUserIndex         // [in] Index of the gamer associated with the device
        );
        
        // special hack for xinput1_4.dll - @103 is a function to turn off a controller
        [DllImport("xinput1_4.dll", EntryPoint = "#103")]
        public static extern int XInputControllerOff
        (
            int dwUserIndex         // [in] Index of the gamer associated with the device
        );
#endif
    }

}
