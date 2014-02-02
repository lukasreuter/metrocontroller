using MetroController.Properties;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MetroController {

    internal static class Tools {

        #region Debugging

        /// <summary>
        /// A simple method to ouput Debugging string to stdout
        /// </summary>
        /// <param name="str">The string that should be printed</param>
        /// <param name="name">The name of the calling method</param>
        /// <param name="line">The line number of the calling code</param>
        [Conditional("DEBUG")]
        internal static void Dbg(string str, [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Console.WriteLine(str + Resources.Tools_Dbg___in_member__ + name + Resources.Tools_Dbg___ + line);
        }

        /// <summary>
        /// Checks if the given return value has the correct/expected value and prints put a debug message if not
        /// </summary>
        /// <param name="value">The return value as integer</param>
        /// <param name="expected">The expected value as integer</param>
        /// <param name="name">The name of the calling method</param>
        /// <param name="line">The current line number of the calling code</param>
        [Conditional("DEBUG")]
        internal static void TestReturnValue(int value, int expected = 0, [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            if (value != expected) {
                Console.WriteLine(name + Resources.returned_non_zero_error_code + Resources.line_number + line);
            }
        }

        /// <summary>
        /// Checks if the given return value has the correct/expected value and prints put a debug message if not
        /// </summary>
        /// <param name="value">The return value as boolean</param>
        /// <param name="expected">The expected value as boolean</param>
        /// <param name="name">The name of the calling method</param>
        /// <param name="line">The current line number of the calling code</param>
        [Conditional("DEBUG")]
        internal static void TestReturnValue(bool value, bool expected = true, [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            if (value != expected) {
                Console.WriteLine(name + Resources.returned_non_zero_error_code + Resources.line_number + line);
            }
        }

        #endregion Debugging
    }
}
