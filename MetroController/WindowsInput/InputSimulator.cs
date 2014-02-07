namespace MetroController.WindowsInput {

    /// <summary>
    /// Implements the InputSimulator interface to simulate Keyboard and Mouse input and provide the state of those input devices.
    /// </summary>
    internal sealed class InputSimulator {

        //
        private static readonly InputSimulator instance = new InputSimulator();

        /// <summary>
        /// Provides access to the singleton instance of the Inputsimulator class
        /// </summary>
        internal static InputSimulator Instance { get { return instance; } }

        /// <summary>
        /// Gets the <see cref="KeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="KeyboardSimulator"/> instance.</value>
        internal KeyboardSimulator Keyboard { get; private set; }

        /// <summary>
        /// Gets the <see cref="MouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="MouseSimulator"/> instance.</value>
        internal MouseSimulator Mouse { get; private set; }

        /// <summary>
        /// Gets the <see cref="InputDeviceStateAdaptor"/> instance for determining the state of the various input devices.
        /// </summary>
        /// <value>The <see cref="InputDeviceStateAdaptor"/> instance.</value>
        internal InputDeviceStateAdaptor InputDeviceState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class using the default <see cref="KeyboardSimulator"/>
        /// , <see cref="MouseSimulator"/> and <see cref="InputDeviceStateAdaptor"/> instances.
        /// </summary>
        private InputSimulator()
        {
            Keyboard = KeyboardSimulator.Instance;
            Mouse = MouseSimulator.Instance;
            InputDeviceState = InputDeviceStateAdaptor.Instance;
        }

        /*
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
        */
    }
}
