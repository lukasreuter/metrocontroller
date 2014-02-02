using System.Runtime.InteropServices;

namespace MetroController.WindowsInput.Native {
#pragma warning disable 649

    /// <summary>
    /// The combined/overlayed structure that includes Mouse, Keyboard and Hardware Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct Mousekeybdhardwareinput {

        /// <summary>
        /// The <see cref="Mouseinput"/> definition.
        /// </summary>
        [FieldOffset(0)]
        internal Mouseinput Mouse;

        /// <summary>
        /// The <see cref="Keybdinput"/> definition.
        /// </summary>
        [FieldOffset(0)]
        internal Keybdinput Keyboard;

        /// <summary>
        /// The <see cref="Hardwareinput"/> definition.
        /// </summary>
        [FieldOffset(0)]
        internal Hardwareinput Hardware;
    }

#pragma warning restore 649
}
