using System;

namespace WindowsInput {

    /// <summary>
    /// Implements the InputSimulator interface to simulate Keyboard and Mouse input and provide the state of those input devices.
    /// </summary>
    internal class InputSimulator {

        /// <summary>
        /// The <see cref="KeyboardSimulator"/> instance to use for simulating keyboard input.
        /// </summary>
        private readonly KeyboardSimulator _keyboardSimulator;

        /// <summary>
        /// The <see cref="MouseSimulator"/> instance to use for simulating mouse input.
        /// </summary>
        private readonly MouseSimulator _mouseSimulator;

        /// <summary>
        /// The <see cref="InputDeviceStateAdaptor"/> instance to use for interpreting the state of the input devices.
        /// </summary>
        private readonly InputDeviceStateAdaptor _inputDeviceState;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class using the specified <see cref="KeyboardSimulator"/>
        /// , <see cref="MouseSimulator"/> and <see cref="InputDeviceStateAdaptor"/> instances.
        /// </summary>
        /// <param name="keyboardSimulator">The <see cref="KeyboardSimulator"/> instance to use for simulating keyboard input.</param>
        /// <param name="mouseSimulator">The <see cref="MouseSimulator"/> instance to use for simulating mouse input.</param>
        /// <param name="inputDeviceStateAdaptor">The <see cref="InputDeviceStateAdaptor"/> instance to use for interpreting the state of input devices.</param>
        internal InputSimulator(KeyboardSimulator keyboardSimulator,
                                MouseSimulator mouseSimulator,
                                InputDeviceStateAdaptor inputDeviceStateAdaptor)
        {
            _keyboardSimulator = keyboardSimulator;
            _mouseSimulator = mouseSimulator;
            _inputDeviceState = inputDeviceStateAdaptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class using the default <see cref="KeyboardSimulator"/>
        /// , <see cref="MouseSimulator"/> and <see cref="InputDeviceStateAdaptor"/> instances.
        /// </summary>
        internal InputSimulator()
        {
            _keyboardSimulator = new KeyboardSimulator(this);
            _mouseSimulator = new MouseSimulator(this);
            _inputDeviceState = new InputDeviceStateAdaptor();
        }

        /// <summary>
        /// Gets the <see cref="KeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="KeyboardSimulator"/> instance.</value>
        internal KeyboardSimulator Keyboard { get { return _keyboardSimulator; } }

        /// <summary>
        /// Gets the <see cref="MouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="MouseSimulator"/> instance.</value>
        internal MouseSimulator Mouse { get { return _mouseSimulator; } }

        /// <summary>
        /// Gets the <see cref="InputDeviceStateAdaptor"/> instance for determining the state of the various input devices.
        /// </summary>
        /// <value>The <see cref="InputDeviceStateAdaptor"/> instance.</value>
        internal InputDeviceStateAdaptor InputDeviceState { get { return _inputDeviceState; } }
    }
}