﻿using MetroController.WindowsInput.Native;
using System;
using System.Runtime.InteropServices;

namespace MetroController.WindowsInput {

    /// <summary>
    /// Implements the InputMessageDispatcher by calling <see cref="Native.NativeMethods.SendInput"/>.
    /// </summary>
    internal static class InputMessageDispatcher {

        /// <summary>
        /// Dispatches the specified list of <see cref="Input"/> messages in their specified order by issuing a single called to <see cref="Native.NativeMethods.SendInput"/>.
        /// </summary>
        /// <param name="inputs">The list of <see cref="Input"/> messages to be dispatched.</param>
        /// <exception cref="ArgumentException">If the <paramref name="inputs"/> array is empty.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="inputs"/> array is null.</exception>
        /// <exception cref="Exception">If the any of the commands in the <paramref name="inputs"/> array could not be sent successfully.</exception>
        internal static void DispatchInput(Input[] inputs)
        {
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (inputs.Length == 0) throw new ArgumentException(@"The input array was empty", "inputs");
            var successful = NativeMethods.SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
            if (successful != inputs.Length) {
                throw new ArgumentException("Some simulated input commands were not sent successfully. " +
                                            "The most common reason for this happening are the security features of Windows including User Interface Privacy Isolation (UIPI). " +
                                            "Your application can only send commands to applications of the same or lower elevation. " +
                                            "Similarly certain commands are restricted to Accessibility/UIAutomation applications. " +
                                            "Refer to the project home page and the code samples for more information.");
            }
        }
    }
}
