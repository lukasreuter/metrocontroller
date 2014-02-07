using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace MetroController.XInputWrapper {

    /// <summary>Provides access to the XInput methods</summary>
    public class XboxController {
        private static bool _keepRunning;
        private static int _waitTime;
        private static bool _isRunning;
        private static readonly object SyncLock;
        private static Thread _pollingThread;
        private static readonly XboxController[] Controllers;

        private readonly int _playerIndex;
        private bool _stopMotorTimerActive;
        private DateTime _stopMotorTime;

        private XInputState _gamepadStatePrev = new XInputState();
        private XInputState _gamepadStateCurrent;

        /// <summary>Contains the method that is called when a controller has changed its state</summary>
        public event EventHandler<XboxControllerStateChangedEventArgs> StateChanged = null;

        /// <summary>The frequency (updates per second) with which updates for the controllers are fetched</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static int UpdateFrequency
        {
            get { return _updateFrequency; }
            set
            {
                _updateFrequency = value;
                _waitTime = 1000 / Math.Max(1, Math.Min(1000, _updateFrequency));
            }
        }

        private static int _updateFrequency;

        /// <summary>Indicates if the controller is powered on and connected</summary>
        public bool IsConnected { get; private set; }

        /// <summary>General Information about the controller battery (type and charge level)</summary>
        internal XInputBatteryInformation BatteryInformationGamepad { get; set; }

        /// <summary>General information about the controller-connected Headset battery (type and charge level)</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        internal XInputBatteryInformation BatteryInformationHeadset { get; set; }

        /// <summary>Maximum number of controllers that are supported</summary>
        private const int MAX_CONTROLLER_COUNT = 4;

        /// <summary>First index of the Controllers[] array</summary>
        public const int FIRST_CONTROLLER_INDEX = 0;

        /// <summary>Last index of the Controller[] array</summary>
        public const int LAST_CONTROLLER_INDEX = MAX_CONTROLLER_COUNT - 1;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static XboxController()
        {
            Controllers = new XboxController[MAX_CONTROLLER_COUNT];
            SyncLock = new object();
            for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i) {
                Controllers[i] = new XboxController(i);
            }
            UpdateFrequency = 25;
        }

        private XboxController(int playerIndex)
        {
            _playerIndex = playerIndex;
            _gamepadStateCurrent = new XInputState();
            _gamepadStatePrev.Copy(_gamepadStateCurrent);
            // Update here to get the battery information for the controller
            UpdateBatteryState();
        }

        /// <summary>
        /// Retrieves a reference to the Controller instance with the given index from Controllers[]
        /// </summary>
        /// <param name="index">The index of the controller from 0 to 3</param>
        /// <returns>A reference to the XboxController object</returns>
        public static XboxController RetrieveController(int index)
        {
            return Controllers[index];
        }

        /// <summary>
        /// Called from UpdateState when a controller has differing packetnumbers
        /// </summary>
        protected void OnStateChanged()
        {
            if (StateChanged != null) {
                StateChanged(this, new XboxControllerStateChangedEventArgs { CurrentInputState = _gamepadStateCurrent, PreviousInputState = _gamepadStatePrev });
            }
        }

        /// <summary>
        /// Retrieves the capabilities of a controller (e.g. which controller type is it, can it vibrate etc.)
        /// <seealso cref="XInputCapabilities"/>
        /// </summary>
        /// <returns>A reference to an XInputCapabilities object</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public XInputCapabilities GetCapabilities()
        {
            var capabilities = new XInputCapabilities();
            var ret = XInputNativeMethods.XInputGetCapabilities(_playerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, ref capabilities);
            Tools.TestReturnValue(ret);

            return capabilities;
        }

        #region Digital Button States

        /// <summary>Checks if Up is pressed</summary>
        public bool IsDPadUpPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_UP); } }

        /// <summary>Checks if Down is pressed</summary>
        public bool IsDPadDownPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN); } }

        /// <summary>Checks if Left is pressed</summary>
        public bool IsDPadLeftPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT); } }

        /// <summary>Checks if right is pressed</summary>
        public bool IsDPadRightPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT); } }

        /// <summary>Checks if A is pressed</summary>
        public bool IsAPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_A); } }

        /// <summary>Checks if B is pressed</summary>
        public bool IsBPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_B); } }

        /// <summary>Checks if X is pressed</summary>
        public bool IsXPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_X); } }

        /// <summary>Checks if Y is pressed</summary>
        public bool IsYPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_Y); } }

        /// <summary>Checks if Back is pressed</summary>
        public bool IsBackPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_BACK); } }

        /// <summary>Checks if Start is pressed</summary>
        public bool IsStartPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_START); } }

        /// <summary>Checks if Guide is pressed</summary>
        public bool IsGuidePressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_GUIDE); } }

        /// <summary>Checks if Left shoulder button is pressed</summary>
        public bool IsLeftShoulderPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER); } }

        /// <summary>Checks if Right shoulder button is pressed</summary>
        public bool IsRightShoulderPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER); } }

        /// <summary>Checks if Left Stick is pressed</summary>
        public bool IsLeftStickPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB); } }

        /// <summary>Checks if Right Stick is pressed</summary>
        public bool IsRightStickPressed { get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB); } }

        #endregion Digital Button States

        #region Analogue Input States

        /// <summary>
        /// Returns the state of the left trigger as integer
        /// Minimum: 0  Maximum: 255
        /// </summary>
        public int LeftTrigger { get { return _gamepadStateCurrent.Gamepad.bLeftTrigger; } }

        /// <summary>
        /// Returns the state of the right trigger as integer
        /// Minimum: 0  Maximum: 255
        /// </summary>
        public int RightTrigger { get { return _gamepadStateCurrent.Gamepad.bRightTrigger; } }

        /// <summary>
        /// Returns the current Coordinates of the left thumb stick as a Point.
        /// Minimum: -32768  Maximum: 32767
        /// </summary>
        public Point LeftThumbStick
        {
            get
            {
                return new Point {
                    X = _gamepadStateCurrent.Gamepad.sThumbLX,
                    Y = _gamepadStateCurrent.Gamepad.sThumbLY
                };
            }
        }

        /// <summary>
        /// Returns the current Coordinates of the right thumb stick as a Point
        /// Minimum: -32768  Maximum: 32767
        /// </summary>
        public Point RightThumbStick
        {
            get
            {
                return new Point {
                    X = _gamepadStateCurrent.Gamepad.sThumbRX,
                    Y = _gamepadStateCurrent.Gamepad.sThumbRY
                };
            }
        }

        #endregion Analogue Input States

        #region Polling

        /// <summary>
        /// Starts polling the controller state every <see cref="_waitTime"/>
        /// </summary>
        public static void StartPolling()
        {
            if (_isRunning) return;
            lock (SyncLock) {
                if (_isRunning) return;
                _pollingThread = new Thread(PollerLoop);
                _pollingThread.Start();
            }
        }

        /// <summary>
        /// Stops the continous polling and updating of the controller states
        /// </summary>
        public static void StopPolling()
        {
            if (_isRunning) _keepRunning = false;
        }

        private static void PollerLoop()
        {
            lock (SyncLock) {
                if (_isRunning) return;
                _isRunning = true;
            }
            _keepRunning = true;
            while (_keepRunning) {
                for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; i++) {
                    Controllers[i].UpdateState();
                }
                Thread.Sleep(_waitTime);
            }
            lock (SyncLock) {
                _isRunning = false;
            }
        }

        /// <summary>
        /// Gets the current input state of the controller instance through <see cref="XInputNativeMethods.XInputGetStateEx"/> and
        /// updates the state depending on differing packetnumbers
        /// </summary>
        /// <returns>A reference of the current <see cref="XboxController"/> </returns>
        public XboxController UpdateState()
        {
            var result = XInputNativeMethods.XInputGetStateEx(_playerIndex, ref _gamepadStateCurrent);
            IsConnected = (result == 0);

            //TODO: maybe only check for battery updates every x cycles to save some performance
            if (BatteryInformationGamepad.BatteryType > (byte) BatteryTypes.BATTERY_TYPE_WIRED) {
                UpdateBatteryState();
            }

            if (_gamepadStateCurrent.PacketNumber != _gamepadStatePrev.PacketNumber) {
                OnStateChanged();
            }
            _gamepadStatePrev.Copy(_gamepadStateCurrent);

            if (!_stopMotorTimerActive || (DateTime.Now < _stopMotorTime)) return this;
            var stopStrength = new XInputVibration { LeftMotorSpeed = 0, RightMotorSpeed = 0 };
            var ret = XInputNativeMethods.XInputSetState(_playerIndex, ref stopStrength);
            Tools.TestReturnValue(ret);

            //we return a reference of this, so we can chain another method call
            return this;
        }

        /// <summary>
        /// Updates the state of the controller and headset battery
        /// </summary>
        public void UpdateBatteryState()
        {
            var headset = new XInputBatteryInformation();
            var gamepad = new XInputBatteryInformation();

            var ret = XInputNativeMethods.XInputGetBatteryInformation(_playerIndex, (byte) BatteryDeviceTypes.BATTERY_DEVTYPE_GAMEPAD, ref gamepad);
            Tools.TestReturnValue(ret);
            ret = XInputNativeMethods.XInputGetBatteryInformation(_playerIndex, (byte) BatteryDeviceTypes.BATTERY_DEVTYPE_HEADSET, ref headset);
            Tools.TestReturnValue(ret);

            BatteryInformationHeadset = headset;
            BatteryInformationGamepad = gamepad;
        }

        #endregion Polling

        #region Motor Functions

        /// <summary>
        /// Initiates a controller vibration with the shortest amount of time possible.
        /// Needs enabled polling to work correctly
        /// </summary>
        /// <param name="leftMotor">Strength of the left (low frequency) motor</param>
        /// <param name="rightMotor">Strength of the right (high frequency) motor</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void Vibrate(double leftMotor, double rightMotor)
        {
            Vibrate(leftMotor, rightMotor, TimeSpan.MinValue);
        }

        /// <summary>
        /// Initiates a controller vibration for the given timespan.
        /// Needs enabled polling to work correctly
        /// </summary>
        /// <param name="leftMotor">Strength of the left (low frequency) motor</param>
        /// <param name="rightMotor">Strength of the right (high frequency) motor</param>
        /// <param name="length">a <see cref="TimeSpan"/></param>
        internal void Vibrate(double leftMotor, double rightMotor, TimeSpan length)
        {
            leftMotor = Math.Max(0d, Math.Min(1d, leftMotor));
            rightMotor = Math.Max(0d, Math.Min(1d, rightMotor));

            var vibration = new XInputVibration { LeftMotorSpeed = (ushort) (65535d * leftMotor), RightMotorSpeed = (ushort) (65535d * rightMotor) };
            Vibrate(vibration, length);
        }

        /// <summary>
        /// Initiates a controller vibration for the given timespan with the given strength.
        /// Needs enabled polling to work correctly
        /// </summary>
        /// <param name="strength">A struct of <see cref="XInputVibration"/></param>
        /// <param name="length">A Reference to an <see cref="TimeSpan"/> object</param>
        private void Vibrate(XInputVibration strength, TimeSpan length)
        {
            _stopMotorTimerActive = false;
            var ret = XInputNativeMethods.XInputSetState(_playerIndex, ref strength);
            Tools.TestReturnValue(ret);

            if (length == TimeSpan.MinValue) return;
            _stopMotorTime = DateTime.Now.Add(length);
            _stopMotorTimerActive = true;
        }

        #endregion Motor Functions

        /// <summary>
        /// Returns a string with the controller number and the current input state
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return ("Controller: " + _playerIndex + " " + _gamepadStateCurrent);
        }
    }
}
