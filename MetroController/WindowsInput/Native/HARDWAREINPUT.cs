﻿using System;

namespace WindowsInput.Native {
#pragma warning disable 649

    /// <summary>
    /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse. (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    internal struct HARDWAREINPUT {

        /// <summary>
        /// Value specifying the message generated by the input hardware.
        /// </summary>
        internal uint Msg;

        /// <summary>
        /// Specifies the low-order word of the lParam parameter for uMsg.
        /// </summary>
        internal ushort ParamL;

        /// <summary>
        /// Specifies the high-order word of the lParam parameter for uMsg.
        /// </summary>
        internal ushort ParamH;
    }

#pragma warning restore 649
}
