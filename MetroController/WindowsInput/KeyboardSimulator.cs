using MetroController.WindowsInput.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MetroController.WindowsInput {

    /// <summary>
    /// Implements the KeyboardSimulator interface by calling the an <see cref="InputMessageDispatcher"/> to simulate Keyboard gestures.
    /// </summary>
    internal class KeyboardSimulator {

        //
        private readonly InputSimulator _inputSimulator;

        /// <summary>
        /// The instance of the <see cref="InputMessageDispatcher"/> to use for dispatching <see cref="Input"/> messages.
        /// </summary>
        private readonly InputMessageDispatcher _messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardSimulator"/> class using an instance of a <see cref="InputMessageDispatcher"/> for dispatching <see cref="Input"/> messages.
        /// </summary>
        /// <param name="inputSimulator">The <see cref="InputSimulator"/> that owns this instance.</param>
        internal KeyboardSimulator(InputSimulator inputSimulator)
        {
            if (inputSimulator == null) throw new ArgumentNullException("inputSimulator");

            _inputSimulator = inputSimulator;
            _messageDispatcher = new InputMessageDispatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardSimulator"/> class using the specified <see cref="InputMessageDispatcher"/> for dispatching <see cref="Input"/> messages.
        /// </summary>
        /// <param name="inputSimulator">The <see cref="InputSimulator"/> that owns this instance.</param>
        /// <param name="messageDispatcher">The <see cref="InputMessageDispatcher"/> to use for dispatching <see cref="Input"/> messages.</param>
        /// <exception cref="InvalidOperationException">If null is passed as the <paramref name="messageDispatcher"/>.</exception>
        internal KeyboardSimulator(InputSimulator inputSimulator, InputMessageDispatcher messageDispatcher)
        {
            if (inputSimulator == null) throw new ArgumentNullException("inputSimulator");

            if (messageDispatcher == null)
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, "The {0} cannot operate with a null {1}. Please provide a valid {1} instance to use for dispatching {2} messages.",
                    typeof(KeyboardSimulator).Name, typeof(InputMessageDispatcher).Name, typeof(Input).Name));

            _inputSimulator = inputSimulator;
            _messageDispatcher = messageDispatcher;
        }

        /// <summary>
        /// Gets the <see cref="MouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="MouseSimulator"/> instance.</value>
        internal MouseSimulator Mouse { get { return _inputSimulator.Mouse; } }

        private static void ModifiersDown(InputBuilder builder, IEnumerable<VirtualKeyCode> modifierKeyCodes)
        {
            if (modifierKeyCodes == null) return;
            foreach (var key in modifierKeyCodes) builder.AddKeyDown(key);
        }

        private static void ModifiersUp(InputBuilder builder, IEnumerable<VirtualKeyCode> modifierKeyCodes)
        {
            if (modifierKeyCodes == null) return;

            // Key up in reverse (I miss LINQ)
            var stack = new Stack<VirtualKeyCode>(modifierKeyCodes);
            while (stack.Count > 0) builder.AddKeyUp(stack.Pop());
        }

        private static void KeysPress(InputBuilder builder, IEnumerable<VirtualKeyCode> keyCodes)
        {
            if (keyCodes == null) return;
            foreach (var key in keyCodes) builder.AddKeyPress(key);
        }

        /// <summary>
        /// Sends the list of <see cref="Input"/> messages using the <see cref="InputMessageDispatcher"/> instance.
        /// </summary>
        /// <param name="inputList">The <see cref="System.Array"/> of <see cref="Input"/> messages to send.</param>
        private void SendSimulatedInput(Input[] inputList)
        {
            _messageDispatcher.DispatchInput(inputList);
        }

        /// <summary>
        /// Calls the Win32 SendInput method to simulate a KeyDown.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to press</param>
        internal KeyboardSimulator KeyDown(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyDown(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method to simulate a KeyUp.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to lift up</param>
        internal KeyboardSimulator KeyUp(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyUp(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method with a KeyDown and KeyUp message in the same input sequence in order to simulate a Key PRESS.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to press</param>
        internal KeyboardSimulator KeyPress(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyPress(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a key press for each of the specified key codes in the order they are specified.
        /// </summary>
        /// <param name="keyCodes"></param>
        internal KeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes)
        {
            var builder = new InputBuilder();
            KeysPress(builder, keyCodes);
            SendSimulatedInput(builder.ToArray());
            return this;
        }

        /// <summary>
        /// Simulates a simple modified keystroke like CTRL-C where CTRL is the modifierKey and C is the key.
        /// The flow is Modifier KeyDown, Key Press, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKeyCode">The modifier key</param>
        /// <param name="keyCode">The key to simulate</param>
        internal KeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode)
        {
            ModifiedKeyStroke(new[] { modifierKeyCode }, new[] { keyCode });
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and one key like CTRL-ALT-C where CTRL and ALT are the modifierKeys and C is the key.
        /// The flow is Modifiers KeyDown in order, Key Press, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCode">The key to simulate</param>
        internal KeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode)
        {
            ModifiedKeyStroke(modifierKeyCodes, new[] { keyCode });
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there is one modifier and multiple keys like CTRL-K-C where CTRL is the modifierKey and K and C are the keys.
        /// The flow is Modifier KeyDown, Keys Press in order, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKey">The modifier key</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        internal KeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes)
        {
            ModifiedKeyStroke(new[] { modifierKey }, keyCodes);
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and multiple keys like CTRL-ALT-K-C where CTRL and ALT are the modifierKeys and K and C are the keys.
        /// The flow is Modifiers KeyDown in order, Keys Press in order, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        internal KeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, IEnumerable<VirtualKeyCode> keyCodes)
        {
            var builder = new InputBuilder();
            var virtualKeyCodes = modifierKeyCodes as VirtualKeyCode[] ?? modifierKeyCodes.ToArray();
            ModifiersDown(builder, virtualKeyCodes);
            KeysPress(builder, keyCodes);
            ModifiersUp(builder, virtualKeyCodes);

            SendSimulatedInput(builder.ToArray());
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method with a stream of KeyDown and KeyUp messages in order to simulate uninterrupted text entry via the keyboard.
        /// </summary>
        /// <param name="text">The text to be simulated.</param>
        internal KeyboardSimulator TextEntry(string text)
        {
            if (text.Length > uint.MaxValue / 2) throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The text parameter is too long. It must be less than {0} characters.", uint.MaxValue / 2), "text");
            var inputList = new InputBuilder().AddCharacters(text).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a single character text entry via the keyboard.
        /// </summary>
        /// <param name="character">The unicode character to be simulated.</param>
        internal KeyboardSimulator TextEntry(char character)
        {
            var inputList = new InputBuilder().AddCharacter(character).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        internal KeyboardSimulator Sleep(int millsecondsTimeout)
        {
            Thread.Sleep(millsecondsTimeout);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="timeout">The time to wait.</param>
        internal KeyboardSimulator Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            return this;
        }
    }
}
