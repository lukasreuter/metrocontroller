using MetroController.WindowsInput.Native;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MetroController.WindowsInput {

    /// <summary>
    /// A helper class for building a list of <see cref="Input"/> messages ready to be sent to the native Windows API.
    /// </summary>
    internal sealed class InputBuilder : IEnumerable<Input> {

        /// <summary>
        /// The public list of <see cref="Input"/> messages being built by this instance.
        /// </summary>
        private readonly List<Input> _inputList;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBuilder"/> class.
        /// </summary>
        internal InputBuilder()
        {
            _inputList = new List<Input>();
        }

        /// <summary>
        /// Returns the list of <see cref="Input"/> messages as a <see cref="System.Array"/> of <see cref="Input"/> messages.
        /// </summary>
        /// <returns>The <see cref="System.Array"/> of <see cref="Input"/> messages.</returns>
        internal Input[] ToArray()
        {
            return _inputList.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list of <see cref="Input"/> messages.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the list of <see cref="Input"/> messages.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Input> GetEnumerator()
        {
            return _inputList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list of <see cref="Input"/> messages.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the list of <see cref="Input"/> messages.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines if the <see cref="VirtualKeyCode"/> is an ExtendedKey
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>true if the key code is an extended key; otherwise, false.</returns>
        /// <remarks>
        /// The extended keys consist of the ALT and CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP, PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in the numeric keypad.
        ///
        /// See http://msdn.microsoft.com/en-us/library/ms646267(v=vs.85).aspx Section "Extended-Key Flag"
        /// </remarks>
        private static bool IsExtendedKey(VirtualKeyCode keyCode)
        {
            return keyCode == VirtualKeyCode.MENU ||
                   keyCode == VirtualKeyCode.LMENU ||
                   keyCode == VirtualKeyCode.RMENU ||
                   keyCode == VirtualKeyCode.CONTROL ||
                   keyCode == VirtualKeyCode.RCONTROL ||
                   keyCode == VirtualKeyCode.INSERT ||
                   keyCode == VirtualKeyCode.DELETE ||
                   keyCode == VirtualKeyCode.HOME ||
                   keyCode == VirtualKeyCode.END ||
                   keyCode == VirtualKeyCode.PRIOR ||
                   keyCode == VirtualKeyCode.NEXT ||
                   keyCode == VirtualKeyCode.RIGHT ||
                   keyCode == VirtualKeyCode.UP ||
                   keyCode == VirtualKeyCode.LEFT ||
                   keyCode == VirtualKeyCode.DOWN ||
                   keyCode == VirtualKeyCode.NUMLOCK ||
                   keyCode == VirtualKeyCode.CANCEL ||
                   keyCode == VirtualKeyCode.SNAPSHOT ||
                   keyCode == VirtualKeyCode.DIVIDE;
        }

        /// <summary>
        /// Adds a key down to the list of <see cref="Input"/> messages.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddKeyDown(VirtualKeyCode keyCode)
        {
            var down =
                new Input {
                    Type = (uint) InputType.Keyboard,
                    Data = {
                        Keyboard =
                            new Keybdinput {
                                KeyCode = (ushort) keyCode,
                                Scan = 0,
                                Flags = IsExtendedKey(keyCode) ? (uint) KeyboardFlag.ExtendedKey : 0,
                                Time = 0,
                                ExtraInfo = IntPtr.Zero
                            }
                    }
                };

            _inputList.Add(down);
            return this;
        }

        /// <summary>
        /// Adds a key up to the list of <see cref="Input"/> messages.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddKeyUp(VirtualKeyCode keyCode)
        {
            var up =
                new Input {
                    Type = (uint) InputType.Keyboard,
                    Data = {
                        Keyboard =
                            new Keybdinput {
                                KeyCode = (ushort) keyCode,
                                Scan = 0,
                                Flags = (uint) (IsExtendedKey(keyCode)
                                                      ? KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey
                                                      : KeyboardFlag.KeyUp),
                                Time = 0,
                                ExtraInfo = IntPtr.Zero
                            }
                    }
                };

            _inputList.Add(up);
            return this;
        }

        /// <summary>
        /// Adds a key press to the list of <see cref="Input"/> messages which is equivalent to a key down followed by a key up.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddKeyPress(VirtualKeyCode keyCode)
        {
            AddKeyDown(keyCode);
            AddKeyUp(keyCode);
            return this;
        }

        /// <summary>
        /// Moves the mouse relative to its current position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddRelativeMouseMovement(int x, int y)
        {
            var movement = new Input { Type = (uint) InputType.Mouse };
            movement.Data.Mouse.Flags = (uint) MouseFlag.Move;
            movement.Data.Mouse.X = x;
            movement.Data.Mouse.Y = y;

            _inputList.Add(movement);

            return this;
        }

        /// <summary>
        /// Adds a mouse button down for the specified button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddMouseButtonDown(MouseButton button)
        {
            var buttonDown = new Input { Type = (uint) InputType.Mouse };
            buttonDown.Data.Mouse.Flags = (uint) ToMouseButtonDownFlag(button);

            _inputList.Add(buttonDown);

            return this;
        }

        /// <summary>
        /// Adds a mouse button up for the specified button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddMouseButtonUp(MouseButton button)
        {
            var buttonUp = new Input { Type = (uint) InputType.Mouse };
            buttonUp.Data.Mouse.Flags = (uint) ToMouseButtonUpFlag(button);
            _inputList.Add(buttonUp);

            return this;
        }

        /// <summary>
        /// Scroll the vertical mouse wheel by the specified amount.
        /// </summary>
        /// <param name="scrollAmount"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        internal InputBuilder AddMouseVerticalWheelScroll(int scrollAmount)
        {
            var scroll = new Input { Type = (uint) InputType.Mouse };
            scroll.Data.Mouse.Flags = (uint) MouseFlag.VerticalWheel;
            scroll.Data.Mouse.MouseData = (uint) scrollAmount;

            _inputList.Add(scroll);

            return this;
        }

        private static MouseFlag ToMouseButtonDownFlag(MouseButton button)
        {
            switch (button) {
                case MouseButton.LeftButton:
                    return MouseFlag.LeftDown;

                case MouseButton.MiddleButton:
                    return MouseFlag.MiddleDown;

                case MouseButton.RightButton:
                    return MouseFlag.RightDown;

                default:
                    return MouseFlag.LeftDown;
            }
        }

        private static MouseFlag ToMouseButtonUpFlag(MouseButton button)
        {
            switch (button) {
                case MouseButton.LeftButton:
                    return MouseFlag.LeftUp;

                case MouseButton.MiddleButton:
                    return MouseFlag.MiddleUp;

                case MouseButton.RightButton:
                    return MouseFlag.RightUp;

                default:
                    return MouseFlag.LeftUp;
            }
        }
    }
}
